using PdfTextReader.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.PDFCore
{
    // the same implementation as AddTableHorizontalLines
    // but it does not require the line in the footer
    class AddTableHorizontalLines2 : IProcessBlock
    {
        private List<IBlock> _lines;
        private List<IBlock> _background;

        public AddTableHorizontalLines2(IdentifyTablesData parserTable)
        {
            var page = parserTable.PageLines;

            if (page == null)
            {
                PdfReaderException.AlwaysThrow("AddTableHorizontalLines2 requires IdentifyTablesData");
            }

            this._lines = page.AllBlocks.Where(l => l.GetWidth() > l.GetHeight()).ToList();
            this._background = parserTable.PageBackground.AllBlocks.ToList();
        }
        
        public BlockPage Process(BlockPage page)
        {
            if(this._lines == null)
            {
                PdfReaderException.AlwaysThrow("AddTableHorizontalLines requires IdentifyTables");
            }

            if (page.IsEmpty())
                return page;

            var result = new BlockPage();

            foreach (var block in page.AllBlocks)
            {
                result.Add(block);
            }

            foreach (var block in _lines)
            {
                // if it is part of a table border with background
                if (IsBackgroundGrid(block))
                    continue;

                if (HasOverlapWithBlockset(block, page))
                    continue;

                result.Add(block);
            }

            return result;
        }

        bool IsBackgroundGrid(IBlock line)
        {
            foreach(var back in _background)
            {
                if (Block.HasOverlap(back, line))
                    return true;
            }

            return false;
        }

        bool HasOverlapWithBlockset(IBlock line, BlockPage page)
        {
            foreach(var block in page.AllBlocks)
            {
                if (Block.HasOverlap(block, line))
                    return true;
            }

            return false;
        }
    }
}
