using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Base
{
    interface IPipelineDebug
    {
        void ShowLine(TextLine line, System.Drawing.Color color);
        void ShowLine(IEnumerable<TextLine> lines, System.Drawing.Color color);
    }
}
