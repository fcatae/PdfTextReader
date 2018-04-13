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
        public bool Ready { get; set; }
        public BlockPage PageTables { get; set; }
        public BlockPage PageLines { get; set; }
        public BlockPage PageBackground { get; set; }

        public BlockPage LastResult { get; set; }

        public BlockPage Process(BlockPage page)
        {
            LastResult = page;
            return page;
        }

        public void UpdateInstance(object cache)
        {
            var instance = (IdentifyTablesData)cache;
            this.LastResult = instance.LastResult;
            this.Ready = instance.Ready;
            this.PageTables = instance.PageTables;
            this.PageLines = instance.PageLines;
            this.PageBackground = instance.PageBackground;
        }
    }
}
