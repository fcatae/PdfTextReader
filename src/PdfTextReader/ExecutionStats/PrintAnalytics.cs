using System;
using System.Collections.Generic;
using System.Text;
using PdfTextReader.Base;
using PdfTextReader.Parser;
using System.IO;

namespace PdfTextReader.ExecutionStats
{
    class PrintAnalytics : IProcessStructure<TextLine>, IProcessStructure<TextStructure>, IProcessStructure<Conteudo>, IDisposable
    {
        StreamWriter file;

        public static IProcessStructure<TextLine> ShowTextLine(string pdfname)
        {
            var analytics = new PrintAnalytics(pdfname);

            analytics.WriteLine("<<<<<>>>>>>>>>>>>>");
            analytics.WriteLine("PROCESSING LINES");
            analytics.WriteLine("<<<<<>>>>>>>>>>>>>");
            analytics.WriteLine("");
            analytics.WriteLine("");

            return analytics;
        }

        public static IProcessStructure<TextStructure> ShowTextStructure(string pdfname)
        {
            var analytics = new PrintAnalytics(pdfname);

            analytics.WriteLine("<<<<<>>>>>>>>>>>>>");
            analytics.WriteLine("PROCESSING PARAGRAPHS");
            analytics.WriteLine("<<<<<>>>>>>>>>>>>>");
            analytics.WriteLine("");
            analytics.WriteLine("");

            return analytics;
        }

        public static IProcessStructure<Conteudo> ShowConteudo(string pdfname)
        {
            var analytics = new PrintAnalytics(pdfname);

            analytics.WriteLine("<<<<<>>>>>>>>>>>>>");
            analytics.WriteLine("PROCESSING CONTENTS");
            analytics.WriteLine("<<<<<>>>>>>>>>>>>>");
            analytics.WriteLine("");
            analytics.WriteLine("");

            return analytics;
        }

        private PrintAnalytics(string pdfname)
        {
            this.file = new StreamWriter($"{pdfname}-Analytics.txt");

            file.WriteLine("<<<<<>>>>>>>>>>>>>");
            file.WriteLine("START ANALYTICS");
            file.WriteLine("<<<<<>>>>>>>>>>>>>");
            file.WriteLine("");
            file.WriteLine("");
        }

        private void WriteLine(string text)
        {
            file.WriteLine(text);
        }

        public TextLine Process(TextLine line)
        {
            file.WriteLine($"Breakline: {line.Breakline}");
            file.WriteLine($"FontName: {line.FontName}");
            file.WriteLine($"FontSize: {line.FontSize}");
            file.WriteLine($"FontStyle: {line.FontStyle}");
            file.WriteLine($"MarginLeft: {line.MarginLeft}");
            file.WriteLine($"MarginRight: {line.MarginRight}");
            file.WriteLine($"Text: {line.Text}");
            file.WriteLine("");
            file.WriteLine("-----");

            return line;
        }

        public TextStructure Process(TextStructure structure)
        {
            file.WriteLine($"TextAlignment: {structure.TextAlignment}");
            file.WriteLine($"FontName: {structure.FontName}");
            file.WriteLine($"FontSize: {structure.FontSize}");
            file.WriteLine($"FontStyle: {structure.FontStyle}");
            file.WriteLine($"Text: {structure.Text}");
            file.WriteLine("");
            file.WriteLine("-----");

            return structure;
        }

        public Conteudo Process(Conteudo content)
        {
            file.WriteLine($"TextAlignment: {content.TextAlignment}");
            file.WriteLine($"FontName: {content.FontName}");
            file.WriteLine($"FontSize: {content.FontSize}");
            file.WriteLine($"FontStyle: {content.FontStyle}");
            file.WriteLine($"Text: {content.Text}");
            file.WriteLine($"Content-Type: {content.ContentType}");
            file.WriteLine("");
            file.WriteLine("-----");

            return content;
        }
        
        public void Dispose()
        {
            if( file != null )
            {
                file.WriteLine("");
                file.WriteLine("");

                file.WriteLine("<<<<<>>>>>>>>>>>>>");
                file.WriteLine("END ANALYTICS");
                file.WriteLine("<<<<<>>>>>>>>>>>>>");

                file.Dispose();
                file = null;
            }
        }
    }
}
