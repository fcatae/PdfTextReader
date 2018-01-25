using PdfTextReader.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.PDFCore
{
    class AddTableLines : IProcessBlock, IPipelineDependency
    {
        private List<IBlock> _lines;
        private List<IBlock> _background;

        public void SetPage(PipelinePage p)
        {
            var parserTable = p.CreateInstance<PDFCore.IdentifyTables>();

            var page = parserTable.PageLines;

            if (page == null)
            {
                PdfReaderException.AlwaysThrow("AddTableLines requires IdentifyTables");
            }

            this._lines = page.AllBlocks.ToList();
            this._background = parserTable.PageBackground.AllBlocks.ToList();
        }

        public BlockPage Process(BlockPage page)
        {
            if(this._lines == null)
            {
                PdfReaderException.AlwaysThrow("AddTableLines requires IdentifyTables");
            }

            if (page.IsEmpty())
                return page;

            var result = new BlockPage();

            foreach (var block in page.AllBlocks)
            {
                result.Add(block);
            }

            bool foundFooter = false;

            foreach (var block in _lines)
            {
                // ignore the line at the footer
                if (IsBelowBody(block, page))
                {
                    foundFooter = true;
                    continue;
                }

                // if it is part of a table border with background
                if (IsBackgroundGrid(block))
                    continue;

                if (HasOverlapWithBlockset(block, page))
                    continue;

                result.Add(block);
            }

            if (foundFooter == false)
                PdfReaderException.Warning("expected to find a line in the footer");

            return result;
        }

        bool IsBelowBody(IBlock line, BlockPage page)
        {
            float lineH = line.GetH() + line.GetHeight();
            float pageH = page.AllBlocks.GetH();

            return (pageH > lineH);
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
