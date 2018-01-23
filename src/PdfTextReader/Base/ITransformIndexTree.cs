using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Base
{
    interface ITransformIndexTree
    {
        int FindPageStart<T>(T instance);
    }
}
