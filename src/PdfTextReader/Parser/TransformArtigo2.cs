using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Parser
{
    class TransformArtigo2
    {
        public void Create(IList<Conteudo> input, IList<TextSegment> input2, string basename)
        {





            var article =  new Artigo
            {

            };
        }

        public void CreateXML(IEnumerable<Artigo> artigos, string basename)
        {
            var procParser = new ProcessParser();
            procParser.XMLWriterMultiple(artigos, $"bin/{basename}-artigo");
        }
    }
}
