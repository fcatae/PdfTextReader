using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.TextStructures
{
    interface IProcessText
    {
        TextSet ProcessText(TextSet text);
    }
}
