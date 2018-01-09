using PdfTextReader.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class AddTableSpace : IProcessBlock, IPipelineDependency
    {
        private List<IBlock> _tables;

        public void SetPage(PipelinePage p)
        {
            var parserTable = p.CreateInstance<PDFCore.IdentifyTables>();

            var page = parserTable.PageTables;

            if (page == null)
                throw new InvalidOperationException("HighlightTextTable requires IdentifyTables");

            this._tables = page.AllBlocks.ToList();
        }

        public BlockPage Process(BlockPage page)
        {
            if(this._tables == null)
                throw new InvalidOperationException("HighlightTextTable requires IdentifyTables");

            var result = new BlockPage();

            foreach (var block in page.AllBlocks)
            {
                result.Add(block);
            }
            foreach (var block in _tables)
            {
                result.Add(block);
            }

            return result;
        }
    }
}
