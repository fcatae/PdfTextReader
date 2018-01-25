using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.ExecutionStats
{
    class ValidateLayout : ICalculateStats<StatsPageLayout>
    {
        const string ERRORMSG = "ERROR: ";

        public IList<string> Results { get; private set; }

        public IEnumerable<int> GetPageErrors()
        {
            for(int i=0; i<Results.Count; i++)
            {
                if( Results[i].Contains(ERRORMSG) )
                    yield return i+1;
            }
        }

        public object Calculate(IEnumerable<StatsPageLayout> stats)
        {
            var result = new List<string>();

            foreach(var s in stats)
            {
                bool error = false;

                if (s.Layout.Contains("()"))
                    error = true;

                if (s.Layout.Contains("1(") || s.Layout.Contains("2("))
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
                    result.Add(ERRORMSG + s.Layout);
                }
                else
                {
                    result.Add(s.Layout);
                }
            }

            Results = result;

            return this;
        }
    }
}
