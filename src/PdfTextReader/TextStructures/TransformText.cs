using System;
using System.Collections.Generic;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.TextStructures
{
    class TransformText<T, TI, TO>
        where T: class, IAggregateStructure<TI,TO>, new()
    {
        List<TI> _input;

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

                    // caller returns null when the object should be ignored
                    if (result != null)
                    {
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

    }
}
