using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.ExecutionStats
{
    class ValidateOverlap : ICalculateStats<StatsBlocksOverlapped>
    {
        public IList<StatsBlocksOverlapped> Results { get; private set; }

        public IEnumerable<int> GetPageErrors()
        {
            for (int i = 0; i < Results.Count; i++)
            {
                if ((Results[i] != null) && (Results[i] != StatsBlocksOverlapped.Empty))
                    yield return i+1;
            }
        }

        public object Calculate(IEnumerable<StatsBlocksOverlapped> stats)
        {
            var result = new List<StatsBlocksOverlapped>();

            foreach (var s in stats)
            {
                var r = (s == StatsBlocksOverlapped.Empty) ? null : s;

                result.Add(r);
            }

            Results = result;

            return this;
        }
    }
}
