using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ChartAndGraph
{
    abstract class MixedSeriesChart : ScrollableAxisChart, IMixedChartDelegate
    {
        HashSet<ScrollableAxisChart> mContainedCharts = new HashSet<ScrollableAxisChart>();
        List<MixedSeriesGenericValue> mFilteredData = new List<MixedSeriesGenericValue>();
        MixedSeriesData mData;
        Dictionary<string, BaseScrollableCategoryData> mCategories;
        public MixedSeriesData Data
        {
            get { return mData; }
        }

        static Dictionary<string, ScrollableAxisChart> mPrefabs = null;

        /// <summary>
        /// Call this method in your loading sequence if you wish the prefabs to be preloaded. If not , this method will be called upon the first creation of a MixedSeriesChart
        /// This method will load all default prefabs that are used by the mixed value chart
        /// </summary>
        public static void EnsureLoadPrefabs()
        {
        }

        public MixedSeriesChart()
        {
            mData = new MixedSeriesData(this);
        }

        protected override void Start()
        {
            EnsureLoadPrefabs();
            base.Start();
        }

        public override bool SupportRealtimeGeneration
        {
            get
            {
                return true;
            }
        }

        protected override IChartData DataLink
        {
            get
            {
                return mData;
            }
        }

        protected override LegenedData LegendInfo
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override bool SupportsCategoryLabels
        {
            get
            {
                return true;
            }
        }

        protected override bool SupportsGroupLables
        {
            get
            {
                return false;
            }
        }

        protected override bool SupportsItemLabels
        {
            get
            {
                return true;
            }
        }

        protected override float TotalDepthLink
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override float TotalHeightLink
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override float TotalWidthLink
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override float GetScrollingRange(int axis)
        {
            float min = (float)(Data.GetMinValue(axis, false));
            float max = (float)(Data.GetMaxValue(axis, false));
            return max - min; ;
        }

        protected override bool HasValues(AxisBase axis)
        {
            return true;
        }

        protected override double MaxValue(AxisBase axis)
        {
            if (axis == null)
                return 0.0;
            if (axis == mHorizontalAxis)
                return Data.GetMaxValue(0, false);
            if (axis == mVerticalAxis)
                return Data.GetMaxValue(1, false);
            return 0.0;
        }

        protected override double MinValue(AxisBase axis)
        {
            if (axis == null)
                return 0.0;
            if (axis == mHorizontalAxis)
                return Data.GetMinValue(0, false);
            if (axis == mVerticalAxis)
                return Data.GetMinValue(1, false);
            return 0.0;
        }

        protected override void ClearChart()
        {
            base.ClearChart();
            throw new NotImplementedException(); //TODO: clear without destroying the underlaying charts
        }

        public override void InternalGenerateChart()
        {
            base.InternalGenerateChart();
            ClearChart();
            
            foreach(BaseScrollableCategoryData data in mCategories.Values)
            {
                MixedSeriesData.CategoryData cat = data as MixedSeriesData.CategoryData;
                if (cat == null)
                    continue;
                MixedSeriesData.CategoryChartView view = cat.getCurrent();
                if (view == null || view.mObject == null)
                    continue;

                var filter = view.Filter; // obtain the filter from the view.
                if (filter == null)     
                    filter = view.mObject.GetDefaultFilter();   // if not present in the view use the default one
                mFilteredData.Clear();
                view.Filter.FilterCategory(cat.Data, mFilteredData);    // filter the data before applying to the chart
                IMixedSeriesProxy dataProxy = view.mObject.ScrollableData;
                if (dataProxy.HasCategory(cat.Name) == false)
                {
                    view.mObject.ScrollableData.Clear();
                    if (dataProxy.AddCategory(cat.Name, view.mCategory) == false)
                    {
                        Debug.LogWarning("failed to add category to mixedChartData hosted chart");
                        continue;
                    }
                }
                view.mObject.ScrollableData.StartBatch();
                dataProxy.ClearCategory(cat.Name);
                dataProxy.AppendDatum(cat.Name, mFilteredData);
                view.mObject.ScrollableData.EndBatch();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="prefab"></param>
        /// <returns></returns>
        ScrollableAxisChart IMixedChartDelegate.CreateCategoryView(Type t, ScrollableAxisChart prefab)
        {
            string type = t.FullName;
            ScrollableAxisChart chart = prefab;

            if (chart == null)
            {
                if (mPrefabs == null || mPrefabs.TryGetValue(type, out chart) == false)
                    return null;
            }

            if (chart.IsCanvas != IsCanvas)
                return null;

            GameObject obj = (GameObject)GameObject.Instantiate(chart.gameObject);
            obj.transform.SetParent(transform);
            ChartCommon.HideObject(obj, true);
            chart = obj.GetComponent<ScrollableAxisChart>();
            mContainedCharts.Add(chart);
            return chart;
        }

        void IMixedChartDelegate.RealaseChart(ScrollableAxisChart chart)
        {
            if (mContainedCharts.Remove(chart) == false)
                Debug.LogWarning("chart is not contained within the mixedSeriesChart");
            ChartCommon.SafeDestroy(chart.gameObject);
        }

        void IMixedChartDelegate.DeactivateChart(ScrollableAxisChart chart)
        {
            if(mContainedCharts.Contains(chart) == false)
                Debug.LogWarning("chart is not contained within the mixedSeriesChart");
            chart.gameObject.SetActive(false);
        }
        
        void IMixedChartDelegate.SetData(Dictionary<string, BaseScrollableCategoryData> data)
        {
            mCategories = data;
        }
        void IMixedChartDelegate.ReactivateChart(ScrollableAxisChart chart)
        {
            if (mContainedCharts.Contains(chart) == false)
                Debug.LogWarning("chart is not contained within the mixedSeriesChart");
            chart.gameObject.SetActive(true);
        }
    }
}
