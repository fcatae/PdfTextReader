using System;
using System.Collections.Generic;
using System.Text;
using PdfTextReader.Base;
using System.Linq;

namespace PdfTextReader.TextStructures
{
    class TransformText<T, TI, TO>
        where T: class, IAggregateStructure<TI,TO>
    {
        List<TI> _input;
        TransformIndex<TI,TO> _index = new TransformIndex<TI,TO>();
        private readonly T _transform;

        public TransformIndex<TI, TO> GetIndexRef() => _index;

        public TransformText(T transform)
        {
            this._transform = transform;
        }

        public IEnumerable<TO> Transform(IEnumerable<TI> lines)
        {
            T transform = _transform;

            int startId = 0;
            int endId = -1;

            foreach (var line in lines)
            {
                endId++;

                // process all the lines
                if (_input != null )
                {
                    bool agg = transform.Aggregate(line);

                    if (agg)
                    {
                        _input.Add(line);
                        continue;
                    }

                    var result = transform.Create(_input);
                    var indexEntry = CreateIndexEntry(result, _input);
                    var indexEntry2 = CreateIndexEntry2(result, startId, endId);
                    startId = endId;
                    // caller returns null when the object should be ignored
                    if (result != null)
                    {
                        // add to index
                        _index.Add(indexEntry, indexEntry2);

                        // return result
                        yield return result;
                    }
                }

                // first line: initialize data
                _input = new List<TI>();
                _input.Add(line);

                transform.Init(line);
            }

            // process the last result, if it exists
            if( _input != null )
            {
                var result_value = transform.Create(_input);

                if (result_value != null)
                {
                    var indexEntry = CreateIndexEntry(result_value, _input);
                    var indexEntry2 = CreateIndexEntry2(result_value, startId, endId+1);
                    _index.Add(indexEntry, indexEntry2);

                    yield return result_value;
                }
            }
        }

        TransformIndexEntry<TI, TO> CreateIndexEntry(TO key, List<TI> list)
        {
            return CreateIndexEntry(key, list[0], list[list.Count - 1], list);
        }

        TransformIndexEntry<TI, TO> CreateIndexEntry(TO key, TI start, TI end, List<TI> list)
        {
            return new TransformIndexEntry<TI, TO>()
            {
                Key = key,
                Start = start,
                End = end,
                Items = list
            };
        }
        
        TransformIndexEntry2<TO> CreateIndexEntry2(TO key, int start, int end)
        {
            return new TransformIndexEntry2<TO>()
            {
                Key = key,
                StartId = start,
                EndId = end
            };
        }
    }
}
