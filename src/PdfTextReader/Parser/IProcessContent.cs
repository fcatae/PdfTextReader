using PdfTextReader.Structure;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Parser
{
    interface IProcessContent
    {
        object Process(TextSet textSet);
    }
}
