
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
    /// Base class for all axis charts that are scrollable. This class implements some important functionallity that is common to all scrollable charts
    /// </summary>
    public abstract class ScrollableAxisChart : AxisChart
    {
        private GameObject mMask;
        private Vector2? mLastPosition;
        private GraphicRaycaster mCaster;
        protected Dictionary<string, Dictionary<int, BillboardText>> mTexts = new Dictionary<string, Dictionary<int, BillboardText>>();
        protected HashSet<BillboardText> mActiveTexts = new HashSet<BillboardText>();



        public ScrollableChartData ScrollableData
        {
            get { return (ScrollableChartData)DataLink; }
        }

        [HideInInspector]
        [SerializeField]
        protected bool autoScrollHorizontally = false;

        public bool AutoScrollHorizontally
        {
            get { return autoScrollHorizontally; }
            set
            {
                autoScrollHorizontally = value;
                InvalidateRealtime();
            }
        }

        protected override void Update()
        {
            base.Update();
            if(IsCanvas)
            {
                // handle mouse events for canvas charts
                HandleMouseDrag();
            }
        }

        public override void GenerateRealtime()
        {
            base.GenerateRealtime();
            if(SupportRealtimeGeneration)
            {
                GenerateAxis(false);
            }
        }

        protected override double GetScrollOffset(int axis)
        {
            if (scrollable == false)
                return 0f;
            if ((autoScrollHorizontally && axis == 0) || (autoScrollVertically && axis == 1))
            {                
                double sMax = ScrollableData.GetMaxValue(axis, false);
                //float sMin = (float)((IInternalGraphData)Data).GetMinValue(axis,false);
                double dMax = ScrollableData.GetMaxValue(axis, true);
                //float dMin = (float)((IInternalGraphData)Data).GetMinValue(axis, true);
                double scrolling = dMax - sMax;
                if (axis == 1)
                    verticalScrolling = scrolling;
                else if (axis == 0)
                    horizontalScrolling = scrolling;
                return scrolling;
            }
            if (axis == 1)
                return verticalScrolling;
            else if (axis == 0)
                return horizontalScrolling;
            return base.GetScrollOffset(axis);
        }

        protected void AddBillboardText(string cat, int index, BillboardText text)
        {
            if (text.UIText != null)
                text.UIText.maskable = false;
            Dictionary<int, BillboardText> addTo;
            if (mTexts.TryGetValue(cat, out addTo) == false)
            {
                addTo = new Dictionary<int, BillboardText>(ChartCommon.DefaultIntComparer);
                mTexts.Add(cat, addTo);
            }
            addTo.Add(index, text);
        }

        protected void ClearBillboard()
        {
            mTexts.Clear();
        }

        protected void ClearBillboardCategories()
        {
            foreach (Dictionary<int, BillboardText> d in mTexts.Values)
                d.Clear();
        }
        [HideInInspector]
        [SerializeField]
        protected bool scrollable = true;

        public bool Scrollable
        {
            get { return scrollable; }
            set
            {
                scrollable = value;
                Invalidate();
            }
        }

        [HideInInspector]
        [SerializeField]
        protected double horizontalScrolling = 0f;

        public double HorizontalScrolling
        {
            get { return horizontalScrolling; }
            set
            {
                horizontalScrolling = value;
                InvalidateRealtime();
            }
        }

        [HideInInspector]
        [SerializeField]
        protected bool autoScrollVertically = false;

        public bool AutoScrollVertically
        {
            get { return autoScrollVertically; }
            set
            {
                autoScrollVertically = value;
                InvalidateRealtime();
            }
        }

        [HideInInspector]
        [SerializeField]
        protected double verticalScrolling = 0f;

        public double VerticalScrolling
        {
            get { return verticalScrolling; }
            set
            {
                verticalScrolling = value;
                InvalidateRealtime();
            }
        }

        public bool PointToClient(Vector3 worldPoint, out double x, out DateTime y)
        {
            double dx, dy;
            bool res = PointToClient(worldPoint, out dx, out dy);
            x = dx;
            y = ChartDateUtility.ValueToDate(dy);
            return res;

        }

        public bool PointToClient(Vector3 worldPoint, out DateTime x, out DateTime y)
        {
            double dx, dy;
            bool res = PointToClient(worldPoint, out dx, out dy);
            x = ChartDateUtility.ValueToDate(dx);
            y = ChartDateUtility.ValueToDate(dy);
            return res;

        }

        public bool PointToClient(Vector3 worldPoint, out DateTime x, out double y)
        {
            double dx, dy;
            bool res = PointToClient(worldPoint, out dx, out dy);
            x = ChartDateUtility.ValueToDate(dx);
            y = dy;
            return res;
        }

        /// <summary>
        /// transform a point from axis units into world space. returns true on success and false on failure (failure should never happen for this implementation)
        /// </summary>
        /// <param name="result"> the resulting world space point</param>
        /// <param name="x">x coordinate in axis units</param>
        /// <param name="y">y coodinate in axis units</param>
        /// <param name="category">for 3d chart specifing a catgory will return a point with the proper depth setting</param>
        /// <returns></returns>
        public bool PointToWorldSpace(out Vector3 result,DateTime x, double y, string category = null)
        {
            return PointToWorldSpace(out result,ChartDateUtility.DateToValue(x), y, category);
        }

        /// <summary>
        /// transform a point from axis units into world space. returns true on success and false on failure (failure should never happen for this implementation)
        /// </summary>
        /// <param name="result"> the resulting world space point</param>
        /// <param name="x">x coordinate in axis units</param>
        /// <param name="y">y coodinate in axis units</param>
        /// <param name="category">for 3d chart specifing a catgory will return a point with the proper depth setting</param>
        /// <returns></returns>
        public bool PointToWorldSpace(out Vector3 result,double x, DateTime y, string category = null)
        {
            return PointToWorldSpace(out result,x, ChartDateUtility.DateToValue(y), category);
        }

        /// <summary>
        /// transform a point from axis units into world space. returns true on success and false on failure (failure should never happen for this implementation)
        /// </summary>
        /// <param name="result"> the resulting world space point</param>
        /// <param name="x">x coordinate in axis units</param>
        /// <param name="y">y coodinate in axis units</param>
        /// <param name="category">for 3d chart specifing a catgory will return a point with the proper depth setting</param>
        /// <returns></returns>
        public bool PointToWorldSpace(out Vector3 result,DateTime x, DateTime y, string category = null)
        {
            return PointToWorldSpace(out result,ChartDateUtility.DateToValue(x), ChartDateUtility.DateToValue(y), category);
        }

        protected abstract double GetCategoryDepth(string category);

        /// <summary>
        /// internal method used bu the mixed series chart to set this chart to default settings
        /// </summary>
        internal abstract void SetAsMixedSeries();

        /// <summary>
        /// internal method used by the mixed series chart to get the default category filter for this chart. This is used to convert generic data to data specific for this chart type
        /// </summary>
        /// <returns></returns>
        internal virtual MixedSeriesData.CategoryFilter GetDefaultFilter()
        {
            return new IdentityCategoryFilter();
        }

        private DoubleVector3 PointToNormalized(double x, double y)
        {
            double minX, minY, maxX, maxY, xScroll, yScroll, xSize, ySize, xOut;
            GetScrollParams(out minX, out minY, out maxX, out maxY, out xScroll, out yScroll, out xSize, out ySize, out xOut);
            double resX = ((x - xScroll) / xSize);
            double resY = ((y - yScroll) / ySize);
            return new DoubleVector3(resX, resY,0.0);
        }

        private DoubleVector3 NormalizedToPoint(double x, double y)
        {
            double minX, minY, maxX, maxY, xScroll, yScroll, xSize, ySize, xOut;
            GetScrollParams(out minX, out minY, out maxX, out maxY, out xScroll, out yScroll, out xSize, out ySize, out xOut);
            double resX = x * xSize + xScroll;
            double resY = y * ySize + yScroll;
            return new DoubleVector3(resX, resY, 0.0);
        }

        private Vector3 PointShift
        {
            get { return Vector3.zero; } // return CanvasFitOffset; }
        }

        /// <summary>
        /// transform a point from axis units into world space. returns true on success and false on failure (failure should never happen for this implementation)
        /// </summary>
        /// <param name="result"> the resulting world space point</param>
        /// <param name="x">x coordinate in axis units</param>
        /// <param name="y">y coodinate in axis units</param>
        /// <param name="category">for 3d chart specifing a catgory will return a point with the proper depth setting</param>
        /// <returns></returns>
        public bool PointToWorldSpace(out Vector3 result,double x,double y, string category = null)
        {
            Vector3 fit = PointShift;
            double minX, minY, maxX, maxY, xScroll, yScroll, xSize, ySize, xOut;
            GetScrollParams(out minX, out minY, out maxX, out maxY, out xScroll, out yScroll, out xSize, out ySize, out xOut);
            double resX = (((x - xScroll) / xSize)- fit.x) * ((IInternalUse)this).InternalTotalWidth;
            double resY = (((y-yScroll) / ySize) - fit.y) * ((IInternalUse)this).InternalTotalHeight;
            double resZ = 0.0;
            if(category != null)
                resZ = GetCategoryDepth(category);
            Transform t = transform;
            if (FixPosition != null)
                t = FixPosition.transform;
            result = t.TransformPoint(new Vector3((float)resX, (float)resY, (float)resZ));
            return true;
        }

        /// <summary>
        /// gets the mouse position on the graph in axis units. returns true if the mouse is in bounds of the chart , false otherwise
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool MouseToClient(out double x,out double y)
        {
            Vector2 mousePos;
            x = y = 0.0;
            mCaster = GetComponentInParent<GraphicRaycaster>();
            if (mCaster == null)
                return false;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, Input.mousePosition, mCaster.eventCamera, out mousePos);

            if(FixPosition != null)
            {
                mousePos = transform.TransformPoint(mousePos);
                mousePos = FixPosition.transform.InverseTransformPoint(mousePos);
            }

            mousePos.x /= TotalWidth;
            mousePos.y /= TotalHeight;

            Vector3 fit = PointShift;
            mousePos.x += fit.x;
            mousePos.y += fit.y;
            bool mouseIn = RectTransformUtility.RectangleContainsScreenPoint(transform as RectTransform, Input.mousePosition);

            DoubleVector3 res =  NormalizedToPoint(mousePos.x, mousePos.y);
            x = res.x;
            y = res.y;
            return mouseIn;
        }

        /// <summary>
        /// transform a point from world space into axis units. for best use , provide a point which is place on the canvas plane
        /// </summary>
        /// <param name="worldPoint"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool PointToClient(Vector3 worldPoint, out double x, out double y)
        {
            double minX, minY, maxX, maxY, xScroll, yScroll, xSize, ySize, xOut;
            GetScrollParams(out minX, out minY, out maxX, out maxY, out xScroll, out yScroll, out xSize, out ySize, out xOut);
            Transform t = transform;
            if (FixPosition != null)
                t = FixPosition.transform;
            worldPoint = t.InverseTransformPoint(worldPoint);
            x = xScroll + xSize * (((double)worldPoint.x) / ((IInternalUse)this).InternalTotalWidth);
            y = yScroll + ySize * (((double)worldPoint.y) / ((IInternalUse)this).InternalTotalHeight);
            return true;
        }

        /// <summary>
        /// Trims a rect in axis units into the visible portion of it in axis units. returns false if the parameter rect is completely out of view , true otherwise
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="trimmed"></param>
        /// <returns></returns>
        public bool TrimRect(DoubleRect rect, out DoubleRect trimmed)
        {
            DoubleVector3 min = rect.min;
            DoubleVector3 max = rect.max;
            trimmed = new DoubleRect();
            min = PointToNormalized(min.x, min.y);
            max = PointToNormalized(max.x, max.y);

            if (min.x > 1f || min.y > 1f)
                return false;
            if (max.x < 0f || max.y < 0f)
                return false;

            double minX = ChartCommon.Clamp(Math.Min(min.x, max.x));
            double minY = ChartCommon.Clamp(Math.Min(min.y, max.y));
            double maxX = ChartCommon.Clamp(Math.Max(min.x, max.x));
            double maxY = ChartCommon.Clamp(Math.Max(min.y, max.y));

            min = NormalizedToPoint(minX, minY);
            max = NormalizedToPoint(maxX, maxY);

            trimmed = new DoubleRect(min.x,min.y, max.x -min.x,max.y - min.y);
            return true;
        }

        /// <summary>
        /// returns true if the axis unity rect is visible on the chart, even if it is only partially visible. false otherwise
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public bool IsRectVisible(DoubleRect rect)
        {
            DoubleVector3 min = rect.min;
            DoubleVector3 max = rect.max;

            min = PointToNormalized(min.x, min.y);
            max = PointToNormalized(max.x, max.y);

            if (min.x > 1f || min.y > 1f)
                return false;
            if (max.x < 0f || max.y < 0f)
                return false;
            return true;
        }

        /// <summary>
        /// tranforms an axis unit rect into a recttransform. The rect transform must have a parent assigned to it. Also the chart and the rectTransform must share a common ancestor canvas
        /// </summary>
        /// <param name="assignTo">The rect tranform to which the result is assigned to</param>
        /// <param name="rect"></param>
        /// <param name="catgeory"></param>
        /// <returns></returns>
        public bool RectToCanvas(RectTransform assignTo, DoubleRect rect, string catgeory = null)
        {
            DoubleVector3 min = rect.min;
            DoubleVector3 max = rect.max;

            Vector3 worldMin, worldMax;
            if(PointToWorldSpace(out worldMin, min.x, min.y, catgeory) == false)
                return false;
            if (PointToWorldSpace(out worldMax, max.x, max.y, catgeory) == false)
                return false;


            Transform parent = assignTo.parent;

            if (parent == null)
                return false;

            worldMin = parent.transform.InverseTransformPoint(worldMin);
            worldMax = parent.transform.InverseTransformPoint(worldMax);

            float minX = Math.Min(worldMin.x, worldMax.x);
            float minY = Math.Min(worldMin.y, worldMax.y);
            float sizeX = Math.Max(worldMin.x, worldMax.x) - minX;
            float sizeY = Math.Max(worldMin.y, worldMax.y) - minY;
            assignTo.anchorMin = new Vector2(0.5f, 0.5f);
            assignTo.anchorMax = new Vector2(0.5f, 0.5f);
            assignTo.pivot = new Vector2(0f, 0f);
            assignTo.anchoredPosition = new Vector2(minX, minY);
            assignTo.sizeDelta = new Vector2(sizeX, sizeY);
            return true;
        }

        protected abstract float GetScrollingRange(int axis);

        public UnityEvent MousePan;

        [SerializeField]
        private bool horizontalPanning;

        public bool HorizontalPanning
        {
            get { return horizontalPanning; ; }
            set
            {
                horizontalPanning = value;
                Invalidate();
            }
        }

        [SerializeField]
        private bool verticalPanning;

        public bool VerticalPanning
        {
            get { return verticalPanning; ; }
            set
            {
                verticalPanning = value;
                Invalidate();
            }
        }

        protected GameObject CreateRectMask(Rect viewRect)
        {
            GameObject obj = Instantiate(Resources.Load("Chart And Graph/RectMask") as GameObject);
            ChartCommon.HideObject(obj, hideHierarchy);
            obj.AddComponent<ChartItem>();
            obj.transform.SetParent(transform, false);
            var rectTransform = obj.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0f, 0f);
            rectTransform.anchorMax = new Vector2(0f, 0f);
            rectTransform.pivot = new Vector2(0f, 1f);
            rectTransform.sizeDelta = viewRect.size;
            rectTransform.anchoredPosition = new Vector2(0f, viewRect.size.y);
            Mask m = obj.GetComponent<Mask>();
            if (m != null)
                m.enabled = Scrollable;
            mMask = obj;
            return obj;
        }

        protected override void ClearChart()
        {
            base.ClearChart();
            if (mMask != null)
            {
                Mask m = mMask.GetComponent<Mask>();
                m.enabled = false;
            }
        }

        protected string StringFromAxisFormat(double val, AxisBase axis)
        {
            if (axis == null)
                return ChartAdancedSettings.Instance.FormatFractionDigits(2, val);
            return StringFromAxisFormat(val, axis, axis.MainDivisions.FractionDigits);
        }
        
        protected string StringFromAxisFormat(double val, AxisBase axis, int fractionDigits)
        {
            if (axis == null)
                return ChartAdancedSettings.Instance.FormatFractionDigits(fractionDigits, val);
            string toSet = "";
            if (axis.Format == AxisFormat.Number)
                toSet = ChartAdancedSettings.Instance.FormatFractionDigits(fractionDigits, val);
            else
            {
                DateTime date = ChartDateUtility.ValueToDate(val);
                if (axis.Format == AxisFormat.DateTime)
                    toSet = ChartDateUtility.DateToDateTimeString(date);
                else
                {
                    if (axis.Format == AxisFormat.Date)
                        toSet = ChartDateUtility.DateToDateString(date);
                    else
                        toSet = ChartDateUtility.DateToTimeString(date);
                }
            }
            return toSet;
        }

        protected void GetScrollParams(out double minX,out double minY,out double maxX,out double maxY,out double xScroll,out double yScroll,out double xSize,out double ySize,out double xOut)
        {
            minX = ScrollableData.GetMinValue(0, false);
            minY = ScrollableData.GetMinValue(1, false);
            maxX = ScrollableData.GetMaxValue(0, false);
            maxY = ScrollableData.GetMaxValue(1, false);
            xScroll = GetScrollOffset(0) + minX;
            yScroll = GetScrollOffset(1) + minY;
            xSize = maxX - minX;
            ySize = maxY - minY;
            xOut = xScroll + xSize;
        }

        private void MouseDraged(Vector2 delta)
        {
            bool drag = false;

            if (VerticalPanning)
            {                
                float range = GetScrollingRange(1);
                VerticalScrolling -= (delta.y / TotalHeightLink) * range;
                if (Mathf.Abs(delta.y) > 1f)
                    drag = true;
            }

            if (HorizontalPanning)
            {
                float range = GetScrollingRange(0);
                HorizontalScrolling -= (delta.x / TotalWidthLink) * range;
                if (Mathf.Abs(delta.x) > 1f)
                    drag = true;
            }

            if (drag)
            {
                InvalidateRealtime();
                if (MousePan != null)
                    MousePan.Invoke();
            }
        }
        private void HandleMouseDrag()
        {
            if (verticalPanning == false && horizontalPanning == false)
                return;
            mCaster = GetComponentInParent<GraphicRaycaster>();
            if (mCaster == null)
                return;
            Vector2 mousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, Input.mousePosition, mCaster.eventCamera, out mousePos);

            bool mouseIn = RectTransformUtility.RectangleContainsScreenPoint(transform as RectTransform, Input.mousePosition);
            if (Input.GetMouseButton(0) && mouseIn)
            {
                if (mLastPosition.HasValue)
                {
                    Vector2 delta = mousePos - mLastPosition.Value;
                    MouseDraged(delta);
                }
                mLastPosition = mousePos;
            }
            else
                mLastPosition = null;
        }

        protected void TriggerActiveTextsOut()
        {
            foreach (BillboardText t in mActiveTexts)
            {
                if (t == null)
                    continue;
                if (t.UIText == null)
                    continue;
                foreach (ChartItemEffect effect in t.UIText.GetComponents<ChartItemEffect>())
                {
                    if (effect != null)
                        effect.TriggerOut(false);
                }
            }
            mActiveTexts.Clear();
        }

        protected void AddActiveText(BillboardText b)
        {
            mActiveTexts.Add(b);
        }

        protected void SelectActiveText(BillboardText b)
        {
            TriggerActiveTextsOut();
            Text tx = b.UIText;
            if (tx != null)
            {
                ChartItemEvents e = tx.GetComponent<ChartItemEvents>();
                if (e != null)
                {
                    e.OnMouseHover.Invoke(e.gameObject);
                    AddActiveText(b);
                }
            }
        }

    }
}
