using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Structure
{
    interface IProcessText
    {
        TextSet ProcessText(TextSet text);
    }
}
