using System;
using System.Collections.Generic;
using System.Text;
using PdfTextReader.Base;
using System.Linq;

namespace PdfTextReader.TextStructures
{
    class TransformText<T, TI, TO>
        where T: class, IAggregateStructure<TI,TO>, new()
    {
        List<TI> _input;
        TransformIndex<TI,TO> _index = new TransformIndex<TI,TO>();

        public TransformIndex<TI, TO> GetIndexRef() => _index;

        public IEnumerable<TO> Transform(IEnumerable<TI> lines)
        {
            T transform = new T();

            foreach (var line in lines)
            {
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

                    // caller returns null when the object should be ignored
                    if (result != null)
                    {
                        // add to index
                        _index.Add(indexEntry);

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
                    yield return result_value;
                }
            }
        }

        TransformIndexEntry<TI, TO> CreateIndexEntry(TO key, List<TI> list)
        {
            return CreateIndexEntry(key, list[0], list[list.Count - 1], list);
        }

        TransformIndexEntry<TI,TO> CreateIndexEntry(TO key, TI start, TI end, List<TI> list)
        {
            return new TransformIndexEntry<TI, TO>()
            {
                Key = key,
                Start = start,
                End = end,
                Items = list
            };
        }
    }
}
