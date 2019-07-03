using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChartAndGraph
{
    /// <summary>
    /// returns the input as is without changes
    /// </summary>
    class IdentityCategoryFilter : MixedSeriesData.CategoryFilter
    {
        public override void FilterCategory(IList<MixedSeriesGenericValue> input, List<MixedSeriesGenericValue> output)
        {
            for(int i=0; i<input.Count; i++)
            {
                output.Add(input[i]);
            }
        }
    }
}
