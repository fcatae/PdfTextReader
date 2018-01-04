using PdfTextReader.PDFCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Execution
{
    static class PipelineDebug
    {
        static public void Output(PipelineInputPdf pdf, string filename)
        {
            pdf.Output(filename);
        }

        static public void DebugBreak<T>(T instance, Func<T, bool> condition)
        {
            if( condition != null )
            {
                bool shouldBreak = condition(instance);

                if (!shouldBreak)
                    return;
            }

            System.Diagnostics.Debugger.Break();
        }
    }

    static class PipelineDebugExtensions
    {
        static public PipelinePage Output(this PipelinePage page, string filename)
        {
            PipelineDebug.Output(page.Context, filename);

            return page;
        }
        static public PipelinePage DebugBreak(this PipelinePage page, Func<PipelinePage,bool> condition = null)
        {
            PipelineDebug.DebugBreak(page, condition);

            return page;
        }
    }
}
