using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Structure
{
    interface ITransformStructure<TI, TO>
    {
        void Init(TI line);

        bool Aggregate(TI line);

        TO Create();
    }
}
