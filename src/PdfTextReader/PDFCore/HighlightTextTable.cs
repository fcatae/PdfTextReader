using PdfTextReader.Base;
using PdfTextReader.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class HighlightTextTable : IProcessBlock, IValidateBlock
    {
        const float MINIMUM_BACKGROUND_SIZE = 5f;

        private List<IBlock> _region;

        public HighlightTextTable(PDFCore.IdentifyTables parserTable)
        {
            var lines = parserTable.PageLines.AllBlocks;
            var backgrounds = parserTable.PageBackground.AllBlocks;

            if ((lines == null) || (backgrounds == null))
            {
                PdfReaderException.AlwaysThrow("HighlightTextTable requires IdentifyTables");
            }

            var region = new List<IBlock>();
            region.AddRange(lines.AsEnumerable());
            region.AddRange(backgrounds.AsEnumerable());
            this._region = region;
        }

        public BlockPage FindHighlightBlocks(BlockPage page)
        {
            if(this._region == null)
            {
                PdfReaderException.AlwaysThrow("HighlightTextTable requires IdentifyTables");
            }

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

                        // a stroke must be thick
                        if (op == 1 && width > block.GetHeight() / 2)
                            continue;

                        if (TableCell.HasWhiteColor(cell))
                            continue;

                        if (TableCell.HasDarkColor(cell))
                        {
                            // very likely it is just a line
                            if (width < MINIMUM_BACKGROUND_SIZE)
                                continue;

                            // check identify table
                            PdfReaderException.AlwaysThrow("not expected"); // not expected
                        }

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
                ((Block)block).HasBackColor = true;
            }

            return page;
        }

        public BlockPage Validate(BlockPage page)
        {
            return FindHighlightBlocks(page);
        }
    }
}
