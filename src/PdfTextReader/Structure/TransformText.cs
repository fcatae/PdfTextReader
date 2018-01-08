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
                if ( transform == null)
                {
                    transform = new T();
                    transform.Init(line);                    
                }
                else
                {
                    bool agg = transform.Aggregate(line);

                    if (!agg)
                    {
                        yield return transform.Create();

                        transform = null;
                    }
                }
            }
        }

    }
}
