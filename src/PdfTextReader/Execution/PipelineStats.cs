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
        private List<List<object>> _statsCollectionPerPage;

        public PipelineStats(List<object> statsCollection, List<List<object>> statsCollectionPerPage)
        {
            _statsCollection = statsCollection;
            _statsCollectionPerPage = statsCollectionPerPage;
        }

        //public void StoreStatistics(object stats)
        //{
        //    _statsCollection.Add(stats);
        //}

        public IEnumerable<S> RetrieveStatistics<S>()
            where S : class
        {
            var availableStats = _statsCollectionPerPage
                                 .Select(col => col.Select(s => s as S).Where(s => s != null).FirstOrDefault());

            return availableStats;

            //var availableStats = _statsCollection
            //                        .Select(s => s as S)
            //                        .Where(s => s != null);

            //return availableStats;
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

        public void SaveStats<T>(string filename)
            where T: class
        {
            using (var file = VirtualFS.OpenStreamWriter(filename))
            {
                var stats = RetrieveStatistics<T>();

                int page = 1;
                foreach(var s in stats)
                {
                    string layout = (s != null) ? s.ToString() : "";
                    file.WriteLine($"{page++}:{layout}");
                }
            }
        }
    }
}
