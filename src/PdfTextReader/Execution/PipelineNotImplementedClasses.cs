using PdfTextReader.Parser;
using PdfTextReader.Structure;
using System;
using System.Collections.Generic;
using System.Text;
using PdfTextReader.PDFCore;

namespace PdfTextReader.Execution
{
    class CreateStructures : IConvertBlock
    {
        public TextSet Convert(BlockPage page)
        {
            throw new NotImplementedException();
        }
    }

    class ProcessParagraphs : IProcessText
    {
        public TextSet ProcessText(TextSet text)
        {
            throw new NotImplementedException();
        }
    }
    class ProcessStructure : IProcessText
    {
        public TextSet ProcessText(TextSet text)
        {
            throw new NotImplementedException();
        }
    }
    class ProcessArticle : IProcessContent
    {
        public object Process(TextSet textSet)
        {
            throw new NotImplementedException();
        }
    }
}
