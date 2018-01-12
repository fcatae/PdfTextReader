using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.Execution
{
    class PipelineStats
    {
        private List<object> _statsCollection;

        public PipelineStats(List<object> statsCollection)
        {
            _statsCollection = statsCollection;
        }

        public void StoreStatistics(object stats)
        {
            _statsCollection.Add(stats);
        }

        public IEnumerable<S> RetrieveStatistics<S>()
            where S : class
        {
            var availableStats = _statsCollection
                                    .Select(s => s as S)
                                    .Where(s => s != null);

            return availableStats;
        }

        public object Calculate<T, S>()
            where T: ICalculateStats<S>, new()
            where S: class
        {
            var instance = new T();

            var stats = RetrieveStatistics<S>();
            var value = instance.Calculate(stats);

            return value;
        }

        public object Summary(Func<PipelineStats, object> func)
        {
            return func(this);
        }        
    }
}
