using PdfTextReader.PDFCore;
using PdfTextReader.TextStructures;
using System;
using System.Collections.Generic;
using System.Drawing;
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
            if (condition != null)
            {
                bool shouldBreak = condition(instance);

                if (!shouldBreak)
                    return;
            }

            System.Diagnostics.Debugger.Break();
        }

        static public void Show(PipelineInputPdf pdf, BlockPage blockPage, Color color)
        {
            var blocks = blockPage.AllBlocks;

            foreach(var b in blocks)
            {
                pdf.CurrentPage.DrawRectangle(b.GetX(), b.GetH(), b.GetWidth(), b.GetHeight(), color);
            }
        }

        static public void Show(PipelineInputPdf pdf, TextSet textSet, Color color)
        {            
            var texts = textSet.AllText;

            foreach (var t in texts)
            {
                var b = (IBlock)t;
                pdf.CurrentPage.DrawRectangle(b.GetX(), b.GetH(), b.GetWidth(), b.GetHeight(), color);
            }
        }

        static public void Show(PipelineInputPdf pdf, System.Collections.IEnumerable objectList, Color color)
        {
            foreach (var t in objectList)
            {
                var b = (IBlock)t;
                pdf.CurrentPage.DrawRectangle(b.GetX(), b.GetH(), b.GetWidth(), b.GetHeight(), color);
            }
        }

        static public void ShowLine(PipelineInputPdf pdf, BlockPage blockPage, Color color)
        {
            var blocks = blockPage.AllBlocks;

            float x1 = float.NaN;
            float h1 = float.NaN;

            foreach (var b in blocks)
            {
                float x2 = b.GetX() + b.GetWidth() / 2;
                float h2 = b.GetH() + b.GetHeight() / 2;

                if ( (!float.IsNaN(x1)) && (!float.IsNaN(h1)) )
                {
                    pdf.CurrentPage.DrawLine(x1, h1, x2, h2, color);
                }

                x1 = x2;
                h1 = h2;
            }
        }
    }

    static class PipelineDebugExtensions
    {
        static public PipelinePage Output(this PipelinePage page, string filename)
        {
            PipelineDebug.Output((PipelineInputPdf)page.Context, filename);
            return page;
        }
        static public PipelinePage DebugBreak(this PipelinePage page, Func<PipelinePage,bool> condition = null)
        {
            PipelineDebug.DebugBreak(page, condition);
            return page;
        }

        static public PipelinePage Show(this PipelinePage page, Color color)
        {            
            PipelineDebug.Show((PipelineInputPdf)page.Context, page.LastResult, color);
            return page;
        }

        static public PipelinePage ShowLine(this PipelinePage page, Color color)
        {
            PipelineDebug.ShowLine((PipelineInputPdf)page.Context, page.LastResult, color);
            return page;
        }
        //static public PipelineText Output(this PipelineText page, string filename)
        //{
        //    PipelineDebug.Output(page.Context, filename);

        //    return page;
        //}

        static public PipelineText<T> DebugBreak<T>(this PipelineText<T> page, Func<PipelineText<T>, bool> condition = null)
        {
            PipelineDebug.DebugBreak(page, condition);

            return page;
        }

        //static public PipelineText Show(this PipelineText page, Color color)
        //{
        //    PipelineDebug.Show(page.LastResult, color);

        //    return page;
        //}
    }
}
