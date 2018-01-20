using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.PDFValidation
{
    class MarkOrangeNoOverlap : IProcessBlock, IValidateMark
    {
        public string Validate(BlockSet<MarkLine> marks)
        {
            bool overlap = HasTableOverlap(marks);

            return (overlap) ? "mark orange no overlap" : null;
        }

        public BlockPage Process(BlockPage page)
        {
            var orange = page.AllBlocks.Cast<MarkLine>().Where(l => l.Color == MarkLine.ORANGE);
            var result = new BlockPage();
            result.AddRange(orange);

            bool overlap = HasTableOverlap(result);

            if (overlap)
            {
                PdfReaderException.Warning("MarkOrangeNoOverlap: Overlap");
                return result;
            }

            // column
            var bset = new BlockSet<IBlock>();
            bset.Add(new BlockLine() { X = 1, H = 1, Width = 1, Height = 1, Text="MarkOrange" });

            var almostEmpty = new BlockPage();
            almostEmpty.Add(bset);

            return almostEmpty;
        }

        bool HasTableOverlap(BlockSet<MarkLine> marks)
        {
            foreach (var a in marks)
            {
                foreach (var b in marks)
                {
                    if (a == b)
                        continue;

                    if (Block.HasOverlap(a, b))
                        return true;
                }
            }
            return false;
        }

        bool HasTableOverlap(BlockPage page)
        {
            foreach (var a in page.AllBlocks)
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
