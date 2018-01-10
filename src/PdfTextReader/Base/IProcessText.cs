using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Base
{
    interface IProcessText
    {
        TextSet ProcessText(TextSet text);
    }
}
