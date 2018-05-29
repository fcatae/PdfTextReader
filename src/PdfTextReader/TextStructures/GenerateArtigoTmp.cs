using PdfTextReader.Base;
using PdfTextReader.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PdfTextReader.TextStructures
{
    class GenerateArtigoTmp : ILogMultipleStructure<Artigo>
    {
        int _id = 0;
        ProcessParser2 _procParser = new ProcessParser2();

        public string CreateId(Artigo data)
        {
            return (_id++).ToString();
        }

        public void Log(string id, Stream input, Artigo data)
        {
            _procParser.XMLWriter(data, input);
        }
    }
}
