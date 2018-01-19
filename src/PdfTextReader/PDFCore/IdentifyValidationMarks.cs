using PdfTextReader.Base;
using PdfTextReader.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class IdentifyValidationMarks : IProcessBlock
    {
        //public const float YELLOW = 8;
        //public const float ORANGE = 6;

        const float MINIMUM_BACKGROUND_SIZE = 5f;

        private BlockPage _pageResult;
        private BlockPage _pageLines;
        private BlockPage _pageBackground;
        public BlockPage PageTables => _pageResult;
        public BlockPage PageLines=> _pageLines;
        public BlockPage PageBackground => _pageBackground;

        public void SetPageTables(IEnumerable<IBlock> tables)
        {
            var page = new BlockPage();
            page.AddRange(tables);

            if (HasTableOverlap(page))
                PdfReaderException.AlwaysThrow("blocks already have overlapped elements");

            _pageResult = page;
        }

        public BlockPage Process(BlockPage page)
        {
            var result = ProcessColors(page);
            
            return result;
        }

        static bool IsColoredLine(TableCell l) => (l.BgColor > 2f);

        static HashSet<int> _Colors = new HashSet<int>();

        public BlockPage ProcessColors(BlockPage page)
        {
            var result = new BlockPage();

            var colored_lines = page.AllBlocks.Cast<MarkLine>();
            
            foreach (var cur in colored_lines)
            {
                int color = cur.Color;
                
                if (cur.Color == MarkLine.PURPLE)
                    result.Add(cur);

                if (!_Colors.Contains(color))
                {
                    _Colors.Add(color);
                    System.Diagnostics.Debug.WriteLine($"color = {cur.Color}");
                }
                
            }

            return result;
        }
        
        bool HasTableOverlap(BlockPage page)
        {
            foreach(var a in page.AllBlocks)
            {
                foreach (var b in page.AllBlocks)
                {
                    if (a == b)
                        continue;

                    if (Block.HasOverlap(a, b))
                        return true;
                }
            }
            return false;
        }
                
        static bool HasOverlap(IBlock blockSet, float x, float h)
        {
            float a_x1 = blockSet.GetX();
            float a_x2 = blockSet.GetX() + blockSet.GetWidth();
            float a_y1 = blockSet.GetH();
            float a_y2 = blockSet.GetH() + blockSet.GetHeight();

            bool hasOverlap = ((a_x1 <= x) && (a_x2 >= x) && (a_y1 <= h) && (a_y2 >= h));

            return hasOverlap;
        }
    }
}
