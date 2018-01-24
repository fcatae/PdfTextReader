using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.ExecutionStats
{
    class ValidateOverlap : ICalculateStats<StatsBlocksOverlapped>
    {
        public object Calculate(IEnumerable<StatsBlocksOverlapped> stats)
        {
            var result = new List<StatsBlocksOverlapped>();

            foreach (var s in stats)
            {
                var r = (s == StatsBlocksOverlapped.Empty) ? null : s;

                result.Add(r);
            }

            return result;
        }
    }
}
