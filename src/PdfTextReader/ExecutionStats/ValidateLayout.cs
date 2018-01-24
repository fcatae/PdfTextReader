using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.ExecutionStats
{
    class ValidateLayout : ICalculateStats<StatsPageLayout>
    {
        public object Calculate(IEnumerable<StatsPageLayout> stats)
        {
            var result = new List<string>();

            foreach(var s in stats)
            {
                bool error = false;

                if(s.Layout.Contains("1(") || s.Layout.Contains("2("))
                {
                    error = true;                    
                }
                else if(s.Layout.Contains("3("))
                {
                    if(!s.Layout.Contains("X"))
                    {
                        error = true;
                    }
                }

                if( error )
                {
                    result.Add("ERROR: " + s.Layout);
                }
                else
                {
                    result.Add(s.Layout);
                }
            }

            return new List<string>(stats.Select(s => s.Layout));
        }
    }
}
