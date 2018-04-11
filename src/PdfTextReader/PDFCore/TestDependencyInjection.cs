using PdfTextReader.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.PDFText;
using PdfTextReader.Base;

namespace PdfTextReader.PDFCore
{
    class TestDependencyInjection : IProcessBlock //, IPipelineDependency
    {
        private List<IBlock> _images;

        public TestDependencyInjection(PreProcessImages parserImage)
        {
            var page = parserImage.Images;

            this._images = page.AllBlocks.ToList();
        }

        //public void SetPage(PipelinePage p)
        //{
        //    var parserImage = p.CreateInstance<PreProcessImages>();

        //    var page = parserImage.Images;

        //    this._images = page.AllBlocks.ToList();
        //}

        public BlockPage Process(BlockPage page)
        {
            return page;
        }
    }
}
