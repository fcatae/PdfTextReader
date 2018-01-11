//using System;
//using System.Collections.Generic;
//using System.Text;
//using PdfTextReader.Base;
//using PdfTextReader.Parser;
//using System.IO;

//namespace PdfTextReader.ExecutionStats
//{
//    class PrintAnalytics2 
//    {
//        protected string _pdfname;

//        public class ShowLines : PrintAnalytics2, IProcessStructure2<TextLine>
//        {
//            public ShowLines(string pdfname)
//            {
//                this._pdfname = pdfname;
//            }

//            public IEnumerable<TextLine> Process(IEnumerable<TextLine> input)
//            {
//                using (var file = OpenWriter(_pdfname))
//                {
//                    file.WriteLine("<<<<<>>>>>>>>>>>>>");
//                    file.WriteLine("PROCESSING LINES");
//                    file.WriteLine("<<<<<>>>>>>>>>>>>>");
//                    file.WriteLine("");
//                    file.WriteLine("");

//                    foreach (var line in input)
//                    {
//                        file.WriteLine($"Breakline: {line.Breakline}");
//                        file.WriteLine($"FontName: {line.FontName}");
//                        file.WriteLine($"FontSize: {line.FontSize}");
//                        file.WriteLine($"FontStyle: {line.FontStyle}");
//                        file.WriteLine($"MarginLeft: {line.MarginLeft}");
//                        file.WriteLine($"MarginRight: {line.MarginRight}");
//                        file.WriteLine($"Text: {line.Text}");
//                        file.WriteLine("");
//                        file.WriteLine("-----");

//                        yield return line;
//                    }

//                    CloseFile(file);
//                }
//            }
//        }        

//        protected static StreamWriter OpenWriter(string pdfname)
//        {
//            var file = new StreamWriter($"{pdfname}-Analytics.txt");

//            file.WriteLine("<<<<<>>>>>>>>>>>>>");
//            file.WriteLine("START ANALYTICS");
//            file.WriteLine("<<<<<>>>>>>>>>>>>>");
//            file.WriteLine("");
//            file.WriteLine("");

//            return file;
//        }

//        protected void CloseFile(StreamWriter file)
//        {
//            file.WriteLine("");
//            file.WriteLine("");

//            file.WriteLine("<<<<<>>>>>>>>>>>>>");
//            file.WriteLine("END ANALYTICS");
//            file.WriteLine("<<<<<>>>>>>>>>>>>>");

//            file.Close();
//        }
                
//        public class ShowStructures : PrintAnalytics2, IProcessStructure2<TextStructure>
//        {
//            public ShowStructures(string pdfname)
//            {
//                this._pdfname = pdfname;
//            }

//            public IEnumerable<TextStructure> Process(IEnumerable<TextStructure> input)
//            {
//                using (var file = OpenWriter(_pdfname))
//                {
//                    file.WriteLine("<<<<<>>>>>>>>>>>>>");
//                    file.WriteLine("PROCESSING PARAGRAPHS");
//                    file.WriteLine("<<<<<>>>>>>>>>>>>>");
//                    file.WriteLine("");
//                    file.WriteLine("");

//                    foreach (var structure in input)
//                    {
//                        file.WriteLine($"TextAlignment: {structure.TextAlignment}");
//                        file.WriteLine($"FontName: {structure.FontName}");
//                        file.WriteLine($"FontSize: {structure.FontSize}");
//                        file.WriteLine($"FontStyle: {structure.FontStyle}");
//                        file.WriteLine($"Text: {structure.Text}");
//                        file.WriteLine("");
//                        file.WriteLine("-----");

//                        yield return structure;
//                    }

//                    CloseFile(file);
//                }
//            }

//        }

//        public class ShowConteudo : PrintAnalytics2, IProcessStructure2<Conteudo>
//        {
//            public ShowConteudo(string pdfname)
//            {
//                this._pdfname = pdfname;
//            }

//            public IEnumerable<Conteudo> Process(IEnumerable<Conteudo> input)
//            {
//                using (var file = OpenWriter(_pdfname))
//                {
//                    file.WriteLine("<<<<<>>>>>>>>>>>>>");
//                    file.WriteLine("PROCESSING CONTENTS");
//                    file.WriteLine("<<<<<>>>>>>>>>>>>>");
//                    file.WriteLine("");
//                    file.WriteLine("");

//                    foreach (var content in input)
//                    {
//                        file.WriteLine($"TextAlignment: {content.TextAlignment}");
//                        file.WriteLine($"FontName: {content.FontName}");
//                        file.WriteLine($"FontSize: {content.FontSize}");
//                        file.WriteLine($"FontStyle: {content.FontStyle}");
//                        file.WriteLine($"Text: {content.Text}");
//                        file.WriteLine($"Content-Type: {content.ContentType}");
//                        file.WriteLine("");
//                        file.WriteLine("-----");

//                        yield return content;
//                    }

//                    CloseFile(file);
//                }
//            }
//        }        
//    }
//}
