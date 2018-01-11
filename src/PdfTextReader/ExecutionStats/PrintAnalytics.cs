using System;
using System.Collections.Generic;
using System.Text;
using PdfTextReader.Base;
using PdfTextReader.Parser;
using System.IO;

namespace PdfTextReader.ExecutionStats
{
    abstract class PrintAnalytics
    {
        public virtual void StartLog(TextWriter file)
        {
            file.WriteLine("<<<<<>>>>>>>>>>>>>");
            file.WriteLine("START ANALYTICS");
            file.WriteLine("<<<<<>>>>>>>>>>>>>");
            file.WriteLine("");
            file.WriteLine("");
        }

        public virtual void EndLog(TextWriter file)
        {
            file.WriteLine("");
            file.WriteLine("");

            file.WriteLine("<<<<<>>>>>>>>>>>>>");
            file.WriteLine("END ANALYTICS");
            file.WriteLine("<<<<<>>>>>>>>>>>>>");
        }

        public class ShowTextLine : PrintAnalytics, ILogStructure<TextLine>
        {
            public override void StartLog(TextWriter analytics)
            {
                base.StartLog(analytics);

                analytics.WriteLine("<<<<<>>>>>>>>>>>>>");
                analytics.WriteLine("PROCESSING LINES");
                analytics.WriteLine("<<<<<>>>>>>>>>>>>>");
                analytics.WriteLine("");
                analytics.WriteLine("");
            }

            public void Log(TextWriter file, TextLine line)
            {
                file.WriteLine($"Breakline: {line.AfterSpace}");
                file.WriteLine($"FontName: {line.FontName}");
                file.WriteLine($"FontSize: {line.FontSize}");
                file.WriteLine($"FontStyle: {line.FontStyle}");
                file.WriteLine($"MarginLeft: {line.MarginLeft}");
                file.WriteLine($"MarginRight: {line.MarginRight}");
                file.WriteLine($"Text: {line.Text}");
                file.WriteLine("");
                file.WriteLine("-----");
            }
        }
        
        public class ShowTextStructure : PrintAnalytics, ILogStructure<TextStructure>
        {
            public override void StartLog(TextWriter analytics)
            {
                base.StartLog(analytics);

                analytics.WriteLine("<<<<<>>>>>>>>>>>>>");
                analytics.WriteLine("PROCESSING PARAGRAPHS");
                analytics.WriteLine("<<<<<>>>>>>>>>>>>>");
                analytics.WriteLine("");
                analytics.WriteLine("");
            }

            public void Log(TextWriter file, TextStructure structure)
            {
                file.WriteLine($"TextAlignment: {structure.TextAlignment}");
                file.WriteLine($"FontName: {structure.FontName}");
                file.WriteLine($"FontSize: {structure.FontSize}");
                file.WriteLine($"FontStyle: {structure.FontStyle}");
                file.WriteLine($"Text: {structure.Text}");
                file.WriteLine("");
                file.WriteLine("-----");
            }
        }
        
        public class ShowConteudo : PrintAnalytics, ILogStructure<Conteudo>
        {
            public override void StartLog(TextWriter analytics)
            {
                base.StartLog(analytics);

                analytics.WriteLine("<<<<<>>>>>>>>>>>>>");
                analytics.WriteLine("PROCESSING CONTENTS");
                analytics.WriteLine("<<<<<>>>>>>>>>>>>>");
                analytics.WriteLine("");
                analytics.WriteLine("");
            }

            public void Log(TextWriter file, Conteudo content)
            {
                file.WriteLine($"TextAlignment: {content.TextAlignment}");
                file.WriteLine($"FontName: {content.FontName}");
                file.WriteLine($"FontSize: {content.FontSize}");
                file.WriteLine($"FontStyle: {content.FontStyle}");
                file.WriteLine($"Text: {content.Text}");
                file.WriteLine($"Content-Type: {content.ContentType}");
                file.WriteLine("");
                file.WriteLine("-----");
            }
        }        
    }
}
