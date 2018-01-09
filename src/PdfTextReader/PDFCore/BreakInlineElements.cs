using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class BreakInlineElements : IProcessBlock, IValidateBlock
    {
        public BlockPage Process(BlockPage page)
        {
            return BreakElements(page);         
        }

        public BlockPage Validate(BlockPage page)
        {
            return FindInlineElements(page);
        }

        BlockPage FindInlineElements(BlockPage page)
        {
            var blocks = page.AllBlocks.ToList();
            var overlapped = new bool[blocks.Count];
            var result = new BlockPage();

            for(int i=0; i<blocks.Count; i++)
            {
                for(int j=0; j<blocks.Count; j++)
                {
                    // same block
                    if (i == j)
                        continue;

                    if(OverlapContains(blocks[i], blocks[j]))
                    {
                        overlapped[j] = true;
                    }
                }
            }

            for (int i = 0; i < blocks.Count; i++)
            {
                if(overlapped[i] == true)
                {
                    result.Add(blocks[i]);
                }
            }

            return result;
        }

        BlockPage BreakElements(BlockPage page)
        {
            var blocks = page.AllBlocks.ToList();
            var replacements = new IBlock[blocks.Count][];
            var result = new BlockPage();

            for (int i = 0; i < blocks.Count; i++)
            {
                if (blocks[i] == null) continue;

                for (int j = 0; j < blocks.Count; j++)
                {
                    if (blocks[j] == null) continue;

                    // same block
                    if (i == j)
                        continue;

                    if (OverlapContains(blocks[i], blocks[j]))
                    {
                        var elems = BreakElements(blocks[i], blocks[j]);

                        if (elems == null)
                            throw new InvalidOperationException();

                        // has to do replacement in place
                        blocks[i] = null;
                        blocks.AddRange(elems);

                        //replacements[i] = elems;
                        break;
                    }

                    if( Block.HasOverlap(blocks[i], blocks[j]))
                    {

                    }
                }
            }

            result.AddRange(blocks.Where(b=> b!=null));

            //for (int i = 0; i < blocks.Count; i++)
            //{
            //    if (replacements[i] != null)
            //    {
            //        result.AddRange(replacements[i]);
            //    }
            //    else
            //    {
            //        result.Add(blocks[i]);
            //    }
            //}

            return result;
        }

        IBlock[] BreakElements(IBlock a, IBlock b)
        {
            float middle = b.GetH() + b.GetHeight() / 2;

            int size = SelectSize((BlockSet<IBlock>)a, middle);

            if (size == -1)
                throw new InvalidOperationException();

            var blocks = CreateNewBlocks((BlockSet<IBlock>)a, size);

            bool overlap1 = OverlapContains(b, blocks[0]);
            bool overlap2 = OverlapContains(b, blocks[1]);

            if (overlap1 || overlap2)
                throw new InvalidOperationException();

            return blocks;
        }

        int SelectSize(BlockSet<IBlock> blockset, float middle)
        {
            int k = 0;
            foreach(var b in blockset)
            {
                float h = b.GetH() + b.GetHeight();
                if (h < middle)
                    return k;
                k++;
            }

            return -1;
        }
        
        IBlock[] CreateNewBlocks(BlockSet<IBlock> blocks, int middle)
        {
            int total = blocks.Count();

            var blockA = new BlockSet<IBlock>();
            var blockB = new BlockSet<IBlock>();

            blockA.AddRange(blocks.Take(middle));
            blockB.AddRange(blocks.TakeLast(total - middle));

            return new IBlock[] { blockA , blockB };
        }
        
        public static bool OverlapContains(IBlock a, IBlock b)
        {
            float a_x1 = a.GetX();
            float a_x2 = a.GetX() + a.GetWidth();
            float a_y1 = a.GetH();
            float a_y2 = a.GetH() + a.GetHeight();

            float b_x1 = b.GetX();
            float b_x2 = b.GetX() + b.GetWidth();
            float b_y1 = b.GetH();
            float b_y2 = b.GetH() + b.GetHeight();

            bool hasIntersectionX = HasOverlap(a_x1, a_x2, b_x1, b_x2);
            bool hasOverlapContainsY = OverlapContains(a_y1, a_y2, b_y1, b_y2);

            return (hasIntersectionX && hasOverlapContainsY);
        }

        static bool OverlapContains(float a1, float a2, float b1, float b2)
        {
            if ((b1 > b2) || (a1 > a2))
                throw new InvalidOperationException();

            bool contains = (a1 <= b1) && (a2 >= b2);

            return contains;
        }
        static bool HasOverlap(float a1, float a2, float b1, float b2)
        {
            if ((b1 > b2) || (a1 > a2))
                throw new InvalidOperationException();

            bool separated = ((a1 < b1) && (a2 < b1)) ||
                             ((a1 > b2) && (a2 > b2));
            return !separated;
        }
    }
}
