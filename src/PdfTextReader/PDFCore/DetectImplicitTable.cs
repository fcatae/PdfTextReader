using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.PDFCore
{
    class DetectImplicitTable : IProcessBlock, IValidateBlock
    {
        public BlockPage Process(BlockPage page)
        {
            var blocks = page.AllBlocks.ToList();
            var overlapped = new bool[blocks.Count];
            var result = new BlockPage();

            for (int i = 0; i < blocks.Count - 1; i++)
            {
                int j = i + 1;

                if (Block.HasOverlap(blocks[i], blocks[j]))
                {
                    if (HasSmallerFont((BlockSet<IBlock>)blocks[i], (BlockSet<IBlock>)blocks[j]) ||
                        HasLineOverlap((BlockSet<IBlock>)blocks[i], (BlockSet<IBlock>)blocks[j]))
                    {
                        var merge = Merge( (BlockSet<IBlock>)blocks[i], (BlockSet<IBlock>)blocks[j] );

                        blocks[i] = null;
                        blocks[j] = merge;
                    }
                }

                if (blocks[i] != null)
                {
                    result.Add(blocks[i]);
                }
            }

            return result;
        }

        BlockSet<IBlock> Merge(BlockSet<IBlock> a, BlockSet<IBlock> b)
        {
            var result = new BlockSet<IBlock>();
            result.AddRange(a);
            result.AddRange(b);

            return result;
        }

        public BlockPage Validate(BlockPage page)
        {
            var blocks = page.AllBlocks.ToList();
            var result = new BlockPage();

            for(int i=0; i<blocks.Count - 1; i++)
            {
                bool overlapped = false;
                int j = i + 1;

                if (Block.HasOverlap(blocks[i], blocks[j]))
                {
                    if( HasSmallerFont((BlockSet<IBlock>)blocks[i], (BlockSet<IBlock>)blocks[j]) ||
                        HasLineOverlap((BlockSet<IBlock>)blocks[i], (BlockSet<IBlock>)blocks[j]))
                    {
                        overlapped = true;
                    }
                }

                if (overlapped)
                {
                    result.Add(blocks[i]);
                }
            }
            
            return result;
        }
        
        bool HasSmallerFont(BlockSet<IBlock> a, BlockSet<IBlock> b)
        {
            var lastLine = (Block)a.TakeLast(1).First();
            var firstLine = (Block)b.Take(1).First();
            
            if (Block.IsSuperscriptFont(lastLine, firstLine))
                return true;

            return false;
        }

        bool HasLineOverlap(BlockSet<IBlock> a, BlockSet<IBlock> b)
        {
            // why it would happen?
            if (a.Count() < 2)
                PdfReaderException.AlwaysThrow("a.Count() < 2");

            var lastLines = a.TakeLast(2);
            var firstLine = b.Take(1).First();

            var last = lastLines.First();

            if (Block.AreSameLine(last, firstLine))
                return true;

            return false;
        }
        
        float[] GetIntersectionAreaY(IBlock a, IBlock b)
        {
            float a_y1 = a.GetH();
            float a_y2 = a.GetH() + a.GetHeight();
            float b_y1 = b.GetH();
            float b_y2 = b.GetH() + b.GetHeight();

            float[] ys = new float[] { a_y1, a_y2, b_y1, b_y2 };

            var ys_ordered = ys.OrderBy(f => f).ToArray();

            return new float[] { ys_ordered[1], ys_ordered[2]};
        }
        
        void TryBreak(IBlock topBlock, IBlock bottomBlock)
        {
            float a_x1 = topBlock.GetX();
            float a_x2 = topBlock.GetX() + topBlock.GetWidth();
            float a_y1 = topBlock.GetH();
            float a_y2 = topBlock.GetH() + topBlock.GetHeight();

            float b_x1 = bottomBlock.GetX();
            float b_x2 = bottomBlock.GetX() + bottomBlock.GetWidth();
            float b_y1 = bottomBlock.GetH();
            float b_y2 = bottomBlock.GetH() + bottomBlock.GetHeight();

            // assume a > y
            if (a_y2 > b_y2)
                return;

            // ensure the intersection ( expect to exists b > a , too)
            if (!(b_y2 > a_y1))
                PdfReaderException.AlwaysThrow("No intersection?");

            bool topContainsBottom = (b_y1 > a_y1);

            // hope it works anyway
            //if (topContainsBottom)
            //    throw new NotImplementedException("should break inside the topBlock");

            // intersection region
            float it_y1 = a_y1;
            float it_y2 = b_y2;

            // find cores top and bottom
        }

        BlockSet<IBlock>[] SplitBlock(BlockSet<IBlock> blockset)
        {
            var blocks = blockset.ToList();

            int total = blocks.Count - 1;
            float limit = blockset.GetWidth() / 2;

            int start = ScanBlock(i => blocks[i], blockset.GetX() + limit);
            int end = ScanBlock(i => blocks[total - i], blockset.GetX() + limit);

            // no split
            if (start == 0 && end == 0)
            {
                // VALIDATE
                //System.Diagnostics.Debugger.Break();

                return new BlockSet<IBlock>[] { blockset };
            }

            // split into 2 pieces
            int middle = -1;

            // split into 2 pieces: there is a clear division
            middle = (start + end > total) ? (start) : middle;
            middle = (start == 0) ? (total - end + 1) : middle;
            middle = (end == 0) ? (start) : middle;

            if (middle > 0)
            {
                var blockA = new BlockSet<IBlock>();
                var blockB = new BlockSet<IBlock>();

                blockA.AddRange(blocks.Take(middle));
                blockB.AddRange(blocks.TakeLast(total - middle + 1));

                int count2 = blockA.Count() + blockB.Count();
                if (count2 != blocks.Count)
                    PdfReaderException.AlwaysThrow("count2 != blocks.Count");

                // VALIDATE
                //System.Diagnostics.Debugger.Break();

                return new BlockSet<IBlock>[] { blockA, blockB };
            }

            // split into 3 pieces
            var topBlock = new BlockSet<IBlock>();
            var coreBlock = new BlockSet<IBlock>();
            var bottomBlock = new BlockSet<IBlock>();

            topBlock.AddRange(blocks.Take(start));

            for (int i = start; i <= total - end; i++)
            {
                coreBlock.Add(blocks[i]);
            }

            bottomBlock.AddRange(blocks.TakeLast(end));

            int count3 = topBlock.Count() + coreBlock.Count() + bottomBlock.Count();
            if (count3 != blocks.Count)
                PdfReaderException.AlwaysThrow("count3 != blocks.Count");

            // VALIDATE
            //System.Diagnostics.Debugger.Break();
            return new BlockSet<IBlock>[] { topBlock, coreBlock, bottomBlock };
        }

        BlockSet<IBlock> FindBlockCore(BlockSet<IBlock> blockset)
        {
            var blocks = blockset.ToList();

            int total = blocks.Count - 1;
            float limit = blockset.GetWidth() / 2;

            int start = ScanBlock(i => blocks[i], blockset.GetX() + limit);
            int end = ScanBlock(i => blocks[total - i], blockset.GetX() + limit);

            int ini = start;
            int tot = total + 1 - end;

            var core = new BlockSet<IBlock>();
            
            // get the core
            for (int i = start; i <= total - end; i++)
            {
                core.Add(blocks[i]);
            }

            return core;
        }

        int ScanBlock(Func<int,IBlock> getBlock, float point)
        {
            float x1 = float.MaxValue;
            float x2 = float.MinValue;
            int count = 0;

            while(!IntersectLine(point, x1, x2))
            {
                var b = getBlock(count++);

                if (b == null)
                    PdfReaderException.AlwaysThrow("should not reach the end of the sequence");

                x1 = Math.Min(x1, b.GetX());
                x2 = Math.Max(x2, b.GetX() + b.GetWidth());
            }

            if (count == 0)
                PdfReaderException.AlwaysThrow("count == 0");

            return count-1;
        }
        
        bool IntersectLine(float point, float x1, float x2)
        {
            return (x1 <= point) && (x2 >= point);
        }
    }
}
