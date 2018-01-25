using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PdfTextReader.TextStructures
{
    class ShowStructureCentral : ILogStructurePdf<TextStructure>
    {
        public void EndLogPdf(IPipelineDebug pipeline)
        {
        }

        public void LogPdf(IPipelineDebug pipeline, TextStructure data)
        {
            if (data.TextAlignment == TextAlignment.CENTER)
            {
                pipeline.ShowLine(data.Lines, Color.Red);
            }

            if (data.TextAlignment == TextAlignment.RIGHT)
            {
                pipeline.ShowLine(data.Lines, Color.Blue);
            }
        }

        public void StartLogPdf(IPipelineDebug pipeline)
        {
        }
    }
}
