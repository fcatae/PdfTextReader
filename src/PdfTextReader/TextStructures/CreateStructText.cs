using PdfTextReader.Base;
using PdfTextReader.Execution;
using PdfTextReader.Parser;
using PdfTextReader.PDFCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.TextStructures
{
    class CreateStructText : IAggregateStructure<TextSegment, TextSegment>
    {
        public CreateStructText(BasicFirstPageStats basicFirstPageStats, PipelinePageStats<int> teste, PipelineDocumentStats docstats)
        {
        }

        public bool Aggregate(TextSegment line)
        {
            return false;
        }
        
        IEnumerable<string> GetBody(TextStructure[] body)
        {
            TextStructure lastStructure = null;
            bool lastWasImage = false;

            foreach(var structure in body)
            {
                bool isImage = IsImage(structure);

                if( isImage )
                {
                    string imgText = structure.Text.Trim('\n');

                    if (!lastWasImage)
                        yield return "";

                    yield return imgText;
                }
                else
                {
                    //if (lastWasImage)
                    //    yield return "";

                    if (lastStructure != null)
                        yield return "";

                    //if (lastStructure != null && lastStructure.TextAlignment != structure.TextAlignment)
                    //    yield return "";

                    yield return GenerateText(structure);
                    lastStructure = structure;
                }

                lastWasImage = isImage;
            }
        }

        public TextSegment Create(List<TextSegment> input)
        {
            var segment = input[0];

            string titleText = String.Join("\n\n", segment.Title.Select(GenerateText));
            string text = String.Join("\n", GetBody(segment.Body));

            var newseg = new TextSegment
            {
                OriginalTitle = segment.OriginalTitle,
                Body = segment.Body,
                Title = segment.Title,
                TitleText = titleText,
                BodyText = text
            };

            return newseg;
        }

        public void Init(TextSegment line)
        {
        }

        string GenerateText(TextStructure s)
        {
            string prefix = "";

            if (s.TextAlignment == TextAlignment.LEFT || s.TextAlignment == TextAlignment.UNKNOWN)
            {
                PdfReaderException.Warning("s.TextAlignment == TextAlignment.LEFT || s.TextAlignment == TextAlignment.UNKNOWN");
            }
            else if (s.TextAlignment == TextAlignment.JUSTIFY)
            {
                return s.Text.Replace("\n\t", "\n\n\t");
            }

            if (s.TextAlignment == TextAlignment.CENTER)
                prefix = "\t\t";

            if (s.TextAlignment == TextAlignment.RIGHT)
                prefix = "\t\t\t\t";

            string prefixTab(string lin) => (lin != "") ? prefix + lin : lin;
            var lines = s.Text.Split('\n').Select(prefixTab);

            string text = String.Join("\n", lines);

            return text;
        }

        string CleanupHyphens(string body)
        {
            if (body == null) return null;

            if (!body.Contains("-"))
                return body;

            // Algumas vezes, a deteccao de texto indica que a continuacao
            // do hifen cai sobre o parágrafo seguinte. Nesse caso, aceitamos
            // que pode ser um erro e consideramos igual a uma quebra de linha
            string incluirQuebraParagrafo = body.Replace("-\n\n", "-\n");

            string texto = HifenUtil.ExtrairHifen(incluirQuebraParagrafo);

            return texto;
        }

        string CleanupBreaklines(string body)
        {
            if (body == null) return null;

            return body.Replace("\n", " ");
        }

        string CleanupBreaklinesAndHyphens(string body)
        {
            if (body == null) return null;

            return CleanupBreaklines(CleanupHyphens(body));
        }

        string ReplaceBreaklinesAndHyphensWithHtml(string body)
        {
            if (body == null) return null;

            var cleanBody = CleanupHyphens(body);
            var lines = cleanBody.Split(new string[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
            var cleanLines = lines.Select(l => CleanupBreaklines(l));

            return "<p>" + String.Join("</p><p>", cleanLines) + "</p>";
        }

        bool IsImage(TextStructure structure)
        {
            string text = structure.Text;

            if (text.StartsWith("[[[") || text.StartsWith("\n[[["))
            {
                if(text.Contains("[[[IMG") || text.Contains("[[[TABLE"))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
