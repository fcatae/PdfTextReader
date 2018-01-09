using PdfTextReader.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class HighlightTextTable : IProcessBlock, IPipelineDependency
    {
        private List<IBlock> _tables;

        public void SetPage(PipelinePage p)
        {
            var parserTable = p.CreateInstance<PDFCore.IdentifyTables>();

            var page = parserTable.PageLines;

            if (page == null)
                throw new InvalidOperationException("HighlightTextTable requires IdentifyTables");

            this._tables = page.AllBlocks.ToList();
        }

        public BlockPage Process(BlockPage page)
        {
            if(this._tables == null)
                throw new InvalidOperationException("HighlightTextTable requires IdentifyTables");

            var result = new BlockPage();

            foreach(var block in page.AllBlocks)
            {
                bool insideTable = false;

                foreach(var table in _tables)
                {
                    if( Block.HasOverlap(table, block) )
                    {
                        var cell = (TableCell)((BlockSet<IBlock>)table).First();
                        float width = cell.LineWidth;

                        if (width < block.GetHeight() / 2)
                            throw new InvalidOperationException("not expected");


                        ((Block)block).SetHighlight((int)width);

                        insideTable = true;
                        break;
                    }
                }


                if (insideTable)
                {
                }

                result.Add(block);
            }

            return result;
        }
    }
}
