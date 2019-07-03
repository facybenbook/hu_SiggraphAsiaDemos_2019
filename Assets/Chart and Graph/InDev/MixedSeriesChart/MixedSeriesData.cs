using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ChartAndGraph
{
    class MixedSeriesData : ScrollableChartData
    {
        private IMixedChartDelegate mParent;
        public MixedSeriesData(MixedSeriesChart parent)
        {
            mParent = parent;
            mParent.SetData(mData);
        }
        
        internal class CategoryChartView
        {
            public Type mType;
            public bool IsPrefabed = false;
            public BaseScrollableCategoryData mCategory;
            public ScrollableAxisChart mObject;
            public CategoryFilter Filter;
        }

        internal class CategoryData : BaseScrollableCategoryData
        {
            public Type Selected;
            public Dictionary<string, CategoryChartView> CategoryViews = new Dictionary<string, CategoryChartView>();
            public List<MixedSeriesGenericValue> Data = new List<MixedSeriesGenericValue>();
            public CategoryChartView getCurrent()
            {
                if (Selected == null)
                    return null;
                CategoryChartView res;
                if (CategoryViews.TryGetValue(Selected.FullName, out res))
                    return res;
                return null;
            }
            public float Depth;   
        }

        /// <summary>
        /// filters data before it is applied to a chart. This can be used to modify the data based on the type of the chart it is passed to. Raw data can be converted to candles for example
        /// </summary>
        public abstract class CategoryFilter
        {
            /// <summary>
            /// called by the mixedserieschart . filters the input data and adds the result to the output
            /// </summary>
            /// <param name="input"></param>
            /// <returns></returns>
            public abstract void FilterCategory(IList<MixedSeriesGenericValue> input,List<MixedSeriesGenericValue> output);
        }

        CategoryChartView CreateCategoryView(Type t,ScrollableAxisChart prefab)
        {
            var chart = mParent.CreateCategoryView(t, prefab);
            if(chart == null)
                return null;
            var cat = chart.ScrollableData.GetDefaultCategory();
            CategoryChartView view = new CategoryChartView();
            view.mCategory = cat;
            view.Filter = null;
            view.mObject = chart;
            view.mType = t;
            return view;
        }

        public void ClearCateogory(string category)
        {
            if (mData.ContainsKey(category) == false)
            {
                Debug.LogWarning("Invalid category name. Make sure the category is present in the graph");
                return;
            }

//            mSliders.RemoveAll(x => (((Slider)x).category == category));
            mData[category].MaxX = null;
            mData[category].MaxY = null;
            mData[category].MinX = null;
            mData[category].MinY = null;
            mData[category].MaxRadius = null;
            ((CategoryData)mData[category]).Data.Clear();
        }

        /// <summary>
        /// sets the category filter for the specified type. category filters can be used to match generic data to a specific type. For example match raw points into a candle chart
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="category"></param>
        /// <param name="filter"></param>
        public void SetCategoryFilter<T>(string category, CategoryFilter filter)
        {
            if (mData.ContainsKey(category) == false)
            {
                Debug.LogWarning("Invalid category name. Make sure the category is present in the graph");
                return;
            }
            CategoryChartView view;
            if(((CategoryData)mData[category]).CategoryViews.TryGetValue(typeof(T).FullName,out view) == false || view == null)
            {
                Debug.LogWarning("Invalid type name. Make sure the type is present in the graph by calling ChangeCatgory first");
                return;
            }
            view.Filter = filter;
        }

        /// <summary>
        /// appends a datum value to the end of the specified category
        /// </summary>
        /// <param name="category"></param>
        /// <param name="data"></param>
        public void AppendDatumToCategory(string category, MixedSeriesGenericValue data)
        {
            if (mData.ContainsKey(category) == false)
            {
                Debug.LogWarning("Invalid category name. Make sure the category is present in the graph");
                return;
            }
            ((CategoryData)mData[category]).Data.Add(data);
        }

        /// <summary>
        /// Adds a new category with the specified depth. If a prefab is specified it will be used instead of the deafult one for type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="category"></param>
        /// <param name="depth"></param>
        /// /// <param name="filter"></param>
        /// <param name="prefab">a prefab for the selected type , or null to use the default prefab </param>
        public void AddCategory<T>(string category,float depth,CategoryFilter filter = null,T prefab = null) where T : ScrollableAxisChart
        {
            if (mData.ContainsKey(category))
            {
                Debug.LogWarning(String.Format("A category named {0} already exists", category));
                return;
            }
            CategoryData data = new CategoryData();
            data.Depth = depth;
            data.Selected = typeof(T);
            CategoryChartView view = CreateCategoryView(typeof(T), prefab);
            if (view == null)
            {
                Debug.LogWarning("Invalid chart type, no category added");
                return;
            }
            view.Filter = filter;
            data.CategoryViews.Add(typeof(T).FullName,view);
            mData.Add(category, data);
        }
       
        /// <summary>
        /// Changes the category to the specified type. If a prefab is specified it will be maintained for this catgeory and type. if a prefab is not specified then either the previously specified prefab is used or the default prefab.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="category">the category</param>
        /// <param name="Prefab">a prefab for the catgeory or null</param>
        public void ChangeCategory<T>(string category, T prefab = null) where T : ScrollableAxisChart
        {
            BaseScrollableCategoryData baseData;
            if (mData.TryGetValue(category, out baseData) == false)  // category does not exist
            {
                Debug.LogWarning(String.Format("Category named {0} does not exist", category));
                return;
            }

            CategoryData data = baseData as CategoryData;  

            if (data == null)   // invalid data , should never happen
            {
                Debug.LogWarning(String.Format("Category named {0} is invalid", category));
                return;
            }

            CategoryChartView view = null;

            bool hasView = data.CategoryViews.TryGetValue(typeof(T).FullName, out view); // find the view in the data

            if (prefab != null || hasView == false) //create a new catgeory view if it doesn't exist, if the prefab is not null then create a new catgeory view anyway. 
            {
                CategoryChartView tmpView = CreateCategoryView(typeof(T), prefab);
                if (tmpView != null)
                {

                    if (hasView)
                    {
                        mParent.RealaseChart(view.mObject);     // if there was a previous view and the prefab is not null , release the old view
                        view.mObject = tmpView.mObject;         // and assign the new one to the current view
                    }
                    else
                    {
                        view = tmpView;                         // if this is a new category view then assign it 
                        data.CategoryViews.Add(typeof(T).FullName, view);       // and add it to the category data object
                    }

                    view.IsPrefabed = prefab != null;
                }
            }

            if (view == null)  // T is an invalid chart type , This could happen when using canvas chart type for non canvas mixed series chart or vice versa
            {
                Debug.LogWarning("Invalid chart type, no category added");
                return;
            }
            
            CategoryChartView current = data.getCurrent();

            if(current != null && current != view)  // if the current view is not also the new one , deactivate it
                mParent.DeactivateChart(current.mObject);

            data.Selected = typeof(T);              // set the selected type 

            mParent.ReactivateChart(view.mObject); // make sure the new view is activated
        }

        /// <summary>
        /// gets the datasource linked to a specifed category
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="category"></param>
        /// <returns></returns>
        public T GetDataSource<T>(string category) where T : ScrollableChartData
        {
            throw new NotImplementedException();
        }

        public override void OnAfterDeserialize()
        {
            throw new NotImplementedException();
        }

        public override void OnBeforeSerialize()
        {
            throw new NotImplementedException();
        }

        protected override void AppendDatum(string category, MixedSeriesGenericValue value)
        {
            throw new InvalidOperationException("Mixed series chart cannot be part of a mixed series chart");
        }

        public override BaseScrollableCategoryData GetDefaultCategory()
        {
            return null;
        }

        protected override void InnerClearCategory(string category)
        {
            throw new NotImplementedException();
        }
         
        protected override bool AddCategory(string category, BaseScrollableCategoryData data)
        {
            throw new NotImplementedException();
        }

        protected override void AppendDatum(string category, IList<MixedSeriesGenericValue> value)
        {
            throw new NotImplementedException();
        }
    }
}
