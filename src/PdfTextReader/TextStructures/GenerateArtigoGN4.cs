using PdfTextReader.Base;
using PdfTextReader.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PdfTextReader.TextStructures
{
    class GenerateArtigoGN4 : ILogMultipleStructure<Artigo>
    {
        int _id = 0;
        ProcessParser2 _procParser = new ProcessParser2();
        Converter2GN _convert = new Converter2GN();
        private InjectFilename _filename;

        public GenerateArtigoGN4(InjectFilename filename)
        {
            this._filename = filename;
        }

        public string CreateId(Artigo data)
        {
            // return (_id++).ToString();
            return data.Conteudo.PID;
        }

        public void Log(string id, Stream input, Artigo data)
        {
            MemoryStream memstream = new MemoryStream();
            _procParser.XMLWriter(data, memstream);
            
            memstream.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(memstream);

            string pdfname = _filename.Filename;
            string article = reader.ReadToEnd();
            string edition = _filename?.InfoStats?.Header?.JornalEdicao ?? "";
            string result = _convert.Convert(pdfname, id, article, edition);

            using (var writer = new StreamWriter(input))
            {
                writer.Write(result);
            }
        }
    }
}
