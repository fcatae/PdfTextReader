using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class ValidatePositiveCoordinates : IValidateBlock
    {
        public BlockPage Validate(BlockPage page)
        {
            var emptyResult = new BlockPage();

            float initial = 10f;

            foreach (var b in page.AllBlocks)
            {
                if ((b.GetX() < 0) || (b.GetH() < 0) || (b.GetWidth() < 0) || (b.GetHeight() < 0))
                {
                    // a bit lame.. but works
                    emptyResult.Add(new Block() { X = initial, H = initial, Width = initial, Height = initial });
                    initial += initial;
                }
            }

            return emptyResult;
        }

        bool HasTableOverlap(BlockPage page)
        {
            foreach (var a in page.AllBlocks)
            {

            }
            return false;
        }
        BlockPage GetTableOverlap(BlockPage page)
        {
            BlockPage result = new BlockPage();

            foreach (var a in page.AllBlocks)
            {
                foreach (var b in page.AllBlocks)
                {
                    if (a == b)
                        continue;

                    if (Block.HasOverlap(a, b))
                    {
                        result.Add(a);
                    }
                }
            }
            return result;
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
