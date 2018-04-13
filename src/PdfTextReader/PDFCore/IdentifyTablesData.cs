using PdfTextReader.Base;
using PdfTextReader.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class IdentifyTablesData : IProcessBlockData
    {
        public BlockPage PageTables { get; set; }
        public BlockPage PageLines { get; set; }
        public BlockPage PageBackground { get; set; }

        public BlockPage LastResult { get; set; }

        public BlockPage Process(BlockPage page)
        {
            LastResult = page;
            return page;
        }


    }
}
