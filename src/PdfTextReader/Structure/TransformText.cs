using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Structure
{
    class TransformText<T, TI, TO>
        where T: class, ITransformStructure<TI,TO>, new()
    {
        public IEnumerable<TO> Transform(IEnumerable<TI> lines)
        {
            T transform = null;

            foreach (var line in lines)
            {
                if ( transform != null )
                {
                    bool agg = transform.Aggregate(line);

                    if (agg)
                        continue;

                    var result = transform.Create();

                    // caller returns null when the object should be ignored
                    if (result != null)
                    {
                        yield return result;
                    }
                }
                
                transform = new T();
                transform.Init(line);
            }

            var result_value = transform.Create();
            if (result_value != null)
            {
                yield return result_value;
            }
        }

    }
}
