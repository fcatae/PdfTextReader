using PdfTextReader.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class HighlightTextTable : IProcessBlock, IValidateBlock, IPipelineDependency
    {
        const float DARKCOLOR = 0.5f;

        private List<IBlock> _region;

        public void SetPage(PipelinePage p)
        {
            var parserTable = p.CreateInstance<PDFCore.IdentifyTables>();

            var lines = parserTable.PageLines.AllBlocks;
            var backgrounds = parserTable.PageBackground.AllBlocks;

            if ((lines == null)|| (backgrounds == null))
                throw new InvalidOperationException("HighlightTextTable requires IdentifyTables");

            var region = new List<IBlock>();
            region.AddRange(lines.AsEnumerable());
            region.AddRange(backgrounds.AsEnumerable());
            this._region = region;            
        }

        public BlockPage FindHighlightBlocks(BlockPage page)
        {
            if(this._region == null)
                throw new InvalidOperationException("HighlightTextTable requires IdentifyTables");

            var result = new BlockPage();

            foreach(var block in page.AllBlocks)
            {
                foreach(var table in _region)
                {
                    if( Block.HasOverlap(table, block) )
                    {
                        var cell = (TableCell)((TableSet)table).First();
                        float width = cell.LineWidth;
                        float bgcolor = cell.BgColor;
                        int op = cell.Op;

                        if (op == 1 && width < block.GetHeight() / 2)
                            continue; // throw new InvalidOperationException("not expected");

                        if (op == 2 && bgcolor < DARKCOLOR)
                            continue; // throw new InvalidOperationException("not expected");

                        result.Add(block);
                        break;
                    }
                }                
            }

            return result;
        }

        public BlockPage Process(BlockPage page)
        {
            var blocks = FindHighlightBlocks(page);
            
            foreach(var block in blocks.AllBlocks)
            {
                ((Block)block).SetHighlight(100);
            }

            return page;
        }

        public BlockPage Validate(BlockPage page)
        {
            return FindHighlightBlocks(page);
        }
    }
}
