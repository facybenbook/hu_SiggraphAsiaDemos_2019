using ChartAndGraph.Axis;
using ChartAndGraph.DataSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ChartAndGraph
{
    /// <summary>
    /// this is a base class for all chart types
    /// </summary>
    [Serializable]
    public abstract class AnyChart : MonoBehaviour, IInternalUse, ISerializationCallbackReceiver
    {
        private Vector2 mLastSetSize = Vector2.zero;
        private bool mGenerating = false;
        private Dictionary<int, string> mHorizontalValueToStringMap = new Dictionary<int, string>();
        private Dictionary<int, string> mVerticalValueToStringMap = new Dictionary<int, string>();
        protected GameObject mFixPosition = null;

        public Dictionary<int, string> VerticalValueToStringMap
        {
            get { return mVerticalValueToStringMap; }
        }

        public Dictionary<int, string> HorizontalValueToStringMap
        {
            get { return mHorizontalValueToStringMap; }
        }

        protected virtual Camera TextCameraLink
        {
            get { return null; }
        }

        protected virtual float TextIdleDistanceLink
        {
            get { return 20f; }
        }

        public UnityEvent OnRedraw = new UnityEvent();

        [SerializeField]
        private bool keepOrthoSize = false;

        public bool KeepOrthoSize
        {
            get { return keepOrthoSize; }
            set
            {
                KeepOrthoSize = value;
                GenerateChart();
            }
        }

        [SerializeField]
        private bool vRSpaceText = false;

        public bool VRSpaceText
        {
            get { return vRSpaceText; }
            set
            {
                vRSpaceText = value;
                GenerateChart();
            }
        }

        [SerializeField]
        private float vRSpaceScale = 0.1f;

        public float VRSpaceScale
        {
            get { return vRSpaceScale; }
            set
            {
                vRSpaceScale = value;
                GenerateChart();
            }
        }

        HashSet<object> mHovered = new HashSet<object>();
        protected bool IsUnderCanvas { get; private set; }
        protected bool CanvasChanged { get; private set; }
        protected ItemLabels mItemLabels;
        protected VerticalAxis mVerticalAxis;
        protected HorizontalAxis mHorizontalAxis;
        protected CategoryLabels mCategoryLabels;
        protected GroupLabels mGroupLabels;
        protected GameObject VerticalMainDevisions;
        protected GameObject VerticalSubDevisions;
        protected GameObject HorizontalMainDevisions;
        protected GameObject HorizontalSubDevisions;

        bool mGenerateOnNextUpdate = false;
        bool mRealtimeOnNextUpdate = false;

        private void AxisChanged(object sender, EventArgs e)
        {
            GenerateChart();
        }

        /// <summary>
        /// This method is called every time a property of the chart has changed. This would ussually trigger data validation and chart rebuilding
        /// </summary>
        protected virtual void OnPropertyUpdated()
        {
            ValidateProperties();
        }

        private void Labels_OnDataChanged(object sender, EventArgs e)
        {
            OnLabelSettingsSet();
        }

        private void Labels_OnDataUpdate(object sender, EventArgs e)
        {
            OnLabelSettingChanged();
        }

        protected virtual void OnLabelSettingChanged()
        {

        }

        /// <summary>
        /// override this in a dervied class to set the total depth of the chart, this is used by 3d axis components to determine automatic size
        /// </summary>
        protected abstract float TotalDepthLink
        {
            get;
        }

        /// <summary>
        /// override this in a dervied class to set the total height of the chart, this is used by axis components and by size fitting for canvas
        /// </summary>
        protected abstract float TotalHeightLink
        {
            get;
        }

        /// <summary>
        /// override this in a dervied class to set the total width of the chart, this is used by axis components and by size fitting for canvas
        /// </summary>
        protected abstract float TotalWidthLink
        {
            get;
        }

        protected abstract IChartData DataLink
        {
            get;
        }

        /// <summary>
        /// true if the class type is meant for use with canvas
        /// </summary>
        public abstract bool IsCanvas
        {
            get;
        }

        public float TotalWidth
        {
            get { return TotalWidthLink; }
        }

        public float TotalHeight
        {
            get { return TotalHeightLink; }
        }

        public float TotalDepth
        {
            get { return TotalDepthLink; }
        }
        protected virtual double GetScrollOffset(int axis)
        {
            return 0.0;
        }

        protected bool hideHierarchy = true;

        /// <summary>
        /// Keeps all the chart elements hidden from the editor and the inspector 
        /// </summary>
        bool IInternalUse.HideHierarchy { get { return hideHierarchy; } }

        // the axis generated for the chart
        List<IAxisGenerator> mAxis = new List<IAxisGenerator>();

        protected void FixAxisLabels()
        {
            for (int i = 0; i < mAxis.Count; ++i)
                mAxis[i].FixLabels(this);
        }

        protected virtual void OnAxisValuesChanged()
        {

        }

        protected virtual void OnLabelSettingsSet()
        {

        }

        protected virtual void Start()
        {
            if (gameObject.activeInHierarchy == false)
                return;
            DoCanvas(true);
            EnsureTextController();
        }

        /// <summary>
        /// this method will detect changes in the parent canvas , if the canvas changed the chart will be regenerated
        /// </summary>
        /// <param name="start"></param>
        void DoCanvas(bool start)
        {
            Canvas parentCanvas = GetComponentInParent<Canvas>();
            bool prev = IsUnderCanvas;
            IsUnderCanvas = parentCanvas != null;
            if (IsUnderCanvas == false)
                return;
            if (start == false)
            {
                if (IsUnderCanvas != prev)
                {
                    CanvasChanged = true;
                    GenerateChart();
                    CanvasChanged = false;
                }
                return;
            }
        }

        /// <summary>
        /// generates a text controller object for this chart
        /// </summary>
        void CreateTextController()
        {
            GameObject obj = new GameObject("textController", typeof(RectTransform)); ;
            ChartCommon.HideObject(obj, hideHierarchy);
            obj.transform.SetParent(transform);
            obj.transform.localScale = new Vector3(1f, 1f, 1f);
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localPosition = Vector3.zero;
            TextController = obj.AddComponent<TextController>();
            TextController.mParent = this;
        }

        /// <summary>
        /// This method ensures that the chart has a TextController object under it's hirarchy , The TextController object is responsible for text placing and billboarding
        /// </summary>
        protected void EnsureTextController()
        {
            if (TextController != null)
                return;
            CreateTextController();
        }
        protected bool Invalidating
        {
            get { return mGenerateOnNextUpdate; }
        }

        /// <summary>
        /// call this to invalidate a chart in reatime , this will update only some parts of the chart depending on implementation.
        /// This method will only work if SupportRealtimeGeneration returns true
        /// </summary>
        protected virtual void InvalidateRealtime()
        {
            if (mGenerating)
                return;
            mRealtimeOnNextUpdate = true;
        }

        /// <summary>
        /// call this to invalidate the chart and make the chart rebuild itself completly in the next update call
        /// </summary>
        protected virtual void Invalidate()
        {
            if (mGenerating)
                return;
            mGenerateOnNextUpdate = true;
        }

        protected virtual void Update()
        {
            if (gameObject.activeInHierarchy == false)
                return;

            DoCanvas(false);

            if (mGenerateOnNextUpdate == true)  // complete redraw preceeds realtime
                GenerateChart();
            else if (mRealtimeOnNextUpdate)
            {
                GenerateRealtime();
                InvokeOnRedraw();
            }

            mGenerateOnNextUpdate = false;  // graph is invalidated set this back to false
            mRealtimeOnNextUpdate = false;
            DataLink.Update();

            if (IsCanvas)
            {
                RectTransform trans = GetComponent<RectTransform>();
                if (trans != null && trans.hasChanged)
                {
                    if (mLastSetSize != trans.rect.size)
                    {
                        Invalidate();
                    }
                }
            }
        }

        protected virtual void LateUpdate()
        {

        }

        /// <summary>
        /// used internally, do not call this method
        /// </summary>
        protected virtual void OnValidate()
        {
            if (gameObject.activeInHierarchy == false)
                return;
            ValidateProperties();
            DoCanvas(true);
          //  EnsureTextController();
        }


        protected TextController TextController { get; private set; }

        /// <summary>
        /// This method returns information used for legened creation
        /// </summary>
        protected abstract LegenedData LegendInfo { get; }

        /// <summary>
        /// Given an axis component , this method would return true if that axis is supported for this chart type.
        /// for example: if the chart supports only horizontal axis , this method should return true for horizontal axis and false otherwise
        /// This method is used by the axis drawing method to determine if this chart type provides data for an axis. (if not then the axis is drawn without values
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        protected abstract bool HasValues(AxisBase axis);

        /// <summary>
        /// returns the maximum value for the specified axis , or 0 if HasValues(axis) returns false.
        /// the maximum value is usually determined by the chart data source , and can be mannualy overriden by the user
        /// </summary>
        protected abstract double MaxValue(AxisBase axis);

        /// <summary>
        /// returns the minimum value for the specified axis , or 0 if HasValues(axis) returns false.
        /// the minimum value is usually determined by the chart data source , and can be mannualy overriden by the user
        /// </summary>
        protected abstract double MinValue(AxisBase axis);


        /// <summary>
        /// when enabling a chart , all chartItem components must be activated. Every unity gameobject create by graph and chart is marked as a ChartItem
        /// </summary>
        protected virtual void OnEnable()
        {
            ChartItem[] children = GetComponentsInChildren<ChartItem>(true);
            for (int i = 0; i < children.Length; ++i)
            {
                if (children[i] != null)
                {
                    if (children[i].gameObject != gameObject)
                        children[i].gameObject.SetActive(true);
                }
            }
        }

        /// <summary>
        /// when disbaling a chart , all chartItem components must be disabled. Every unity gameobject create by graph and chart is marked as a ChartItem
        /// </summary>
        protected virtual void OnDisable()
        {
            ChartItem[] children = GetComponentsInChildren<ChartItem>();
            for (int i = 0; i < children.Length; ++i)
            {
                if (children[i] != null)
                {
                    if (children[i].gameObject != gameObject)
                        children[i].gameObject.SetActive(false);
                }
            }
        }

        public abstract bool SupportRealtimeGeneration { get; }


        

        /// <summary>
        /// call this in a dervied class in order to invoke the OnRedraw event. The OnRedraw event should be called any time a marker position is about to change.
        /// </summary>
        protected void InvokeOnRedraw()
        {
            if (OnRedraw != null)
                OnRedraw.Invoke();
        }

        public virtual void GenerateRealtime()
        {
            
        }

        /// <summary>
        /// This method generates a chart by calling InternalGenerateChart
        /// </summary>
        public void GenerateChart()
        {
            if (mGenerating == true) // avoid nested calls to generate chart
                return;
            mGenerating = true;
            InternalGenerateChart();    // call derivative class implementation
            Transform[] children = gameObject.GetComponentsInChildren<Transform>();

            // setup the layer for all child object so it matches the parent
            for(int i=0; i< children.Length; i++)
            {
                Transform t = children[i];
                if (t == null || t.gameObject == null)
                    continue;
                t.gameObject.layer = gameObject.layer;
            }

            TextController.transform.SetAsLastSibling();
            if (IsCanvas)
                FitCanvas();
            InvokeOnRedraw();
            mGenerating = false;

        }
        /// <summary>
        /// override this method in a derived class to generate a chart type.
        /// </summary>
        public virtual void InternalGenerateChart()
        {
            if (gameObject.activeInHierarchy == false)
                return;
            mGenerateOnNextUpdate = false;
            if (ChartGenerated != null)
                ChartGenerated();
        }
        /// <summary>
        /// override this method in a dervied class to add custom clearing for any chart type. default implementation deletes all chartItem components from this transform
        /// </summary>
        protected virtual void ClearChart()
        {
            mHovered.Clear();

            if (TextController != null) // destroy all child text object
            {
                TextController.DestroyAll();
                TextController.transform.SetParent(transform, false); // the text controller my not be a direct child of this gameobject , make it so that it is.
            }


            // destroy all child ChartItem objects
            ChartItem[] children = GetComponentsInChildren<ChartItem>();
            for (int i = 0; i < children.Length; ++i)
            {
                if (children[i] != null)
                {
                    RectMask2D mask = children[i].GetComponent<RectMask2D>();
                    if(mask != null)
                    {
                        Debug.Log(mask.gameObject);
                    }
                    if (TextController != null && children[i].gameObject == TextController.gameObject)
                        continue;
                    if (children[i].gameObject != gameObject)
                        ChartCommon.SafeDestroy(children[i].gameObject);
                }
            }
            // ensure the text controller has been created ( it is generated only when the graph chart is first created)
            EnsureTextController();


            //destroy all axis components in this chart
            for (int i = 0; i < mAxis.Count; i++)
            {
                if (mAxis[i] != null && mAxis[i].This() != null)
                {
                    ChartCommon.SafeDestroy(mAxis[i].GetGameObject());
                }
            }
            mAxis.Clear();

        }

        protected GameObject FixPosition
        {
            get { return mFixPosition; }
        }
        protected virtual Vector3 CanvasFitOffset { get { return new Vector3(0.5f, 0.5f,0f); } }
        protected virtual bool FitAspectCanvas { get { return false; } }

        void FitCanvas()
        {
            RectTransform trans = GetComponent<RectTransform>();
            mLastSetSize = trans.rect.size;
            if (FitAspectCanvas == false)
                return;
            GameObject fixPosition = new GameObject();
            mFixPosition = fixPosition;
            ChartCommon.HideObject(fixPosition, hideHierarchy);
            // fixPosition.AddComponent<ChartItem>();
            // var fixPositionRect = fixPosition.GetComponent<RectTransform>();
            //            fixPositionRect.anchorMin = new Vector2(1f, 1f);
            //            fixPositionRect.anchorMax = new Vector2(1f, 1f);
            fixPosition.transform.position = transform.position;
            while (gameObject.transform.childCount > 0)
                transform.GetChild(0).SetParent(fixPosition.transform, false);
            fixPosition.transform.SetParent(transform, false);
            fixPosition.transform.localScale = new Vector3(1f, 1f, 1f);
            float widthScale = trans.rect.size.x / TotalWidthLink;
            float heightScale = trans.rect.size.y / TotalHeightLink;
            float uniformScale = Math.Min(widthScale, heightScale);
            fixPosition.transform.localScale = new Vector3(uniformScale, uniformScale, uniformScale);
            Vector3 offs = CanvasFitOffset;
            fixPosition.transform.localPosition = new Vector3(-TotalWidthLink * uniformScale * offs.x, -TotalHeightLink * uniformScale * offs.y, 0f);
        }

        /// <summary>
        /// called by a derived class to indicate that the mouse has left any selectable object in the chart.
        /// </summary>
        protected virtual void OnNonHoverted()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userData"></param>
        protected virtual void OnItemLeave(object userData)
        {
            if (mHovered.Count == 0)
                return;
            mHovered.Remove(userData);
            if (mHovered.Count == 0)
                OnNonHoverted();
        }

        protected virtual void OnItemSelected(object userData)
        {

        }

        protected virtual void OnItemHoverted(object userData)
        {
            mHovered.Add(userData);
        }

        internal protected virtual IAxisGenerator InternalUpdateAxis(ref GameObject axisObject,AxisBase axisBase, ChartOrientation axisOrientation, bool isSubDiv,bool forceRecreate,double scrollOffset)
        {
            IAxisGenerator res = null;
            if (axisObject == null || forceRecreate || CanvasChanged)
            {
                ChartCommon.SafeDestroy(axisObject);
                GameObject axis = null;
                if (IsUnderCanvas)
                    axis = ChartCommon.CreateCanvasChartItem();
                else
                    axis = ChartCommon.CreateChartItem();
                axis.transform.SetParent(transform, false);
                axis.transform.localScale = new Vector3(1f, 1f, 1f);
                axis.transform.localRotation = Quaternion.identity;
                axis.transform.localPosition = new Vector3();
                axis.layer = gameObject.layer; // put the axis on the same layer as the chart
                ChartCommon.HideObject(axis, hideHierarchy);
                axisObject = axis;
                if (IsUnderCanvas)
                    res = axis.AddComponent<CanvasAxisGenerator>();
                else
                    res = axis.AddComponent<AxisGenerator>();
            }
            else
            {
                if (IsUnderCanvas)
                    res = axisObject.GetComponent<CanvasAxisGenerator>();
                else
                    res = axisObject.GetComponent<AxisGenerator>();
            }
            res.SetAxis(scrollOffset,this, axisBase, axisOrientation, isSubDiv);
            
      //      axisObject.transform.localScale = new Vector3(1f, 1f, 1f);
     //       axisObject.transform.localRotation = Quaternion.identity;
     //       axisObject.transform.localPosition = new Vector3();
            return res;
        }

        protected virtual void ValidateProperties()
        {
            if(mItemLabels!=null)
                mItemLabels.ValidateProperties();
            if (mCategoryLabels != null)
                mCategoryLabels.ValidateProperties();
            if (mGroupLabels != null)
                mGroupLabels.ValidateProperties();
            if (mHorizontalAxis != null)
                mHorizontalAxis.ValidateProperties();
            if (mVerticalAxis != null)
                mVerticalAxis.ValidateProperties();
        }

        protected void GenerateAxis(bool force)
        {
            mAxis.Clear();
            if (mVerticalAxis)
            {
                double scroll = GetScrollOffset(1);
                IAxisGenerator main = InternalUpdateAxis(ref VerticalMainDevisions, mVerticalAxis, ChartOrientation.Vertical, false, force, scroll);
                IAxisGenerator sub = InternalUpdateAxis(ref VerticalSubDevisions, mVerticalAxis, ChartOrientation.Vertical, true, force, scroll);
                if (main != null)
                    mAxis.Add(main);
                if (sub != null)
                    mAxis.Add(sub);
            }
            if (mHorizontalAxis)
            {
                double scroll = GetScrollOffset(0);
                IAxisGenerator main = InternalUpdateAxis(ref HorizontalMainDevisions, mHorizontalAxis, ChartOrientation.Horizontal, false, force,scroll);
                IAxisGenerator sub = InternalUpdateAxis(ref HorizontalSubDevisions, mHorizontalAxis, ChartOrientation.Horizontal, true, force,scroll);
                if (main != null)
                    mAxis.Add(main);
                if (sub != null)
                    mAxis.Add(sub);
            }
        }

        private event Action ChartGenerated;

        #region Internal Use
        event Action IInternalUse.Generated
        {
            add
            {
                ChartGenerated += value;
            }
            remove
            {
                ChartGenerated -= value;
            }
        }

        void IInternalUse.InternalItemSelected(object userData)
        {
            OnItemSelected(userData);
        }

        void IInternalUse.InternalItemLeave(object userData)
        {
            OnItemLeave(userData);
        }

        void IInternalUse.InternalItemHovered(object userData)
        {
            OnItemHoverted(userData);
        }

        void IInternalUse.CallOnValidate()
        {
            OnValidate();
        }

        /// <summary>
        /// Label settings for the chart items
        /// </summary>
        ItemLabels IInternalUse.ItemLabels
        {
            get { return mItemLabels; }
            set
            {
                if (mItemLabels != null)
                {

                    ((IInternalSettings)mItemLabels).InternalOnDataUpdate -= Labels_OnDataUpdate;
                    ((IInternalSettings)mItemLabels).InternalOnDataChanged -= Labels_OnDataChanged;
                }
                mItemLabels = value;
                if (mItemLabels != null)
                {
                    ((IInternalSettings)mItemLabels).InternalOnDataUpdate += Labels_OnDataUpdate;
                    ((IInternalSettings)mItemLabels).InternalOnDataChanged += Labels_OnDataChanged;
                }
                OnLabelSettingsSet();
            }
        }

        VerticalAxis IInternalUse.VerticalAxis
        {
            get
            {
                return mVerticalAxis;
            }
            set
            {
                if (mVerticalAxis != null)
                {
                    ((IInternalSettings)mVerticalAxis).InternalOnDataChanged -= AxisChanged;
                    ((IInternalSettings)mVerticalAxis).InternalOnDataUpdate -= AxisChanged;
                }
                mVerticalAxis = value;
                if (mVerticalAxis != null)
                {
                    ((IInternalSettings)mVerticalAxis).InternalOnDataChanged += AxisChanged;
                    ((IInternalSettings)mVerticalAxis).InternalOnDataUpdate += AxisChanged;
                }
                OnAxisValuesChanged();
            }
        }


        HorizontalAxis IInternalUse.HorizontalAxis
        {
            get
            {
                return mHorizontalAxis;
            }
            set
            {
                if (mHorizontalAxis != null)
                {

                    ((IInternalSettings)mHorizontalAxis).InternalOnDataChanged -= AxisChanged;
                    ((IInternalSettings)mHorizontalAxis).InternalOnDataUpdate -= AxisChanged;
                }
                mHorizontalAxis = value;
                if (mHorizontalAxis != null)
                {
                    ((IInternalSettings)mHorizontalAxis).InternalOnDataChanged += AxisChanged;
                    ((IInternalSettings)mHorizontalAxis).InternalOnDataUpdate += AxisChanged;
                }
                OnAxisValuesChanged();
            }
        }

        /// <summary>
        /// Label settings for the chart categories
        /// </summary>
        CategoryLabels IInternalUse.CategoryLabels
        {
            get { return mCategoryLabels; }
            set
            {
                if (mCategoryLabels != null)
                {

                    ((IInternalSettings)mCategoryLabels).InternalOnDataUpdate -= Labels_OnDataUpdate;
                    ((IInternalSettings)mCategoryLabels).InternalOnDataChanged -= Labels_OnDataChanged;
                }
                mCategoryLabels = value;
                if (mCategoryLabels != null)
                {
                    ((IInternalSettings)mCategoryLabels).InternalOnDataUpdate += Labels_OnDataUpdate;
                    ((IInternalSettings)mCategoryLabels).InternalOnDataChanged += Labels_OnDataChanged;
                }
                OnLabelSettingsSet();
            }
        }


        /// <summary>
        /// Label settings for group labels
        /// </summary>
        GroupLabels IInternalUse.GroupLabels
        {
            get { return mGroupLabels; }
            set
            {
                if (mGroupLabels != null)
                {
                    ((IInternalSettings)mGroupLabels).InternalOnDataUpdate -= Labels_OnDataUpdate;
                    ((IInternalSettings)mGroupLabels).InternalOnDataChanged -= Labels_OnDataChanged;
                }
                mGroupLabels = value;
                if (mGroupLabels != null)
                {
                    ((IInternalSettings)mGroupLabels).InternalOnDataUpdate += Labels_OnDataUpdate;
                    ((IInternalSettings)mGroupLabels).InternalOnDataChanged += Labels_OnDataChanged;
                }
                OnLabelSettingsSet();
            }
        }

        TextController IInternalUse.InternalTextController { get { return TextController; } }
        LegenedData IInternalUse.InternalLegendInfo { get { return LegendInfo; } }

        Camera IInternalUse.InternalTextCamera
        {
            get
            {
                return TextCameraLink;
            }
        }

        float IInternalUse.InternalTextIdleDistance
        {
            get
            {
                return TextIdleDistanceLink;
            }
        }

        bool IInternalUse.InternalHasValues(AxisBase axis)
        {
            return HasValues(axis);
        }
        double IInternalUse.InternalMaxValue(AxisBase axis)
        {
            return MaxValue(axis);
        }
        double IInternalUse.InternalMinValue(AxisBase axis)
        {
            return MinValue(axis);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            DataLink.OnBeforeSerialize();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            DataLink.OnAfterDeserialize();
        }

        float IInternalUse.InternalTotalDepth { get { return TotalDepthLink; } }
        float IInternalUse.InternalTotalWidth { get { return TotalWidthLink; } }
        float IInternalUse.InternalTotalHeight { get { return TotalHeightLink; } }

        protected abstract bool SupportsCategoryLabels
        {
            get;
        }

        protected abstract bool SupportsItemLabels
        {
            get;
        }

        protected abstract bool SupportsGroupLables
        {
            get;
        }

        bool IInternalUse.InternalSupportsCategoryLables
        {
            get { return SupportsCategoryLabels; }
        }

        bool IInternalUse.InternalSupportsGroupLabels
        {
            get { return SupportsGroupLables; }
        }

        bool IInternalUse.InternalSupportsItemLabels
        {
            get { return SupportsItemLabels; }
        }
        #endregion
    }
}
