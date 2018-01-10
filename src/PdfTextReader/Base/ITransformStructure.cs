using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.TextStructures
{
    interface ITransformStructure<TI, TO>
    {
        void Init(TI line);

        bool Aggregate(TI line);

        TO Create();
    }
}
