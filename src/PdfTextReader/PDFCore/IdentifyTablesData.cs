using PdfTextReader.Base;
using PdfTextReader.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class IdentifyTablesData : IProcessBlock
    {
        private BlockPage _pageResult;
        private BlockPage _pageLines;
        private BlockPage _pageBackground;
        public BlockPage PageTables => _pageResult;
        public BlockPage PageLines=> _pageLines;
        public BlockPage PageBackground => _pageBackground;

        public BlockPage Process(BlockPage page)
        {
            return page;
        }


    }
}
