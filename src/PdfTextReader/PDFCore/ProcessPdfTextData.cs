using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class ProcessPdfTextData : IProcessBlockData
    {
        public BlockPage LastResult { get; private set; }

        public BlockPage Process(BlockPage page)
        {
            LastResult = page;
            return page;
        }

        public void UpdateInstance(object cache)
        {
            var instance = (ProcessPdfTextData)cache;
            this.LastResult = instance.LastResult;
        }
    }
}
