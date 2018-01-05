﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class BreakColumns : IProcessBlock, IValidateBlock
    {
        public BlockPage Process(BlockPage page)
        {
            return BreakPage(page);            
        }

        public BlockPage Validate(BlockPage page)
        {
            var blocks = page.AllBlocks.ToList();
            var overlapped = new bool[blocks.Count];
            var result = new BlockPage();

            for(int i=0; i<blocks.Count; i++)
            {
                for(int j=i+1; j<blocks.Count; j++)
                {
                    if(Block.HasOverlap(blocks[i], blocks[j]))
                    {
                        overlapped[i] = true;
                        overlapped[j] = true;
                    }
                }

                if(overlapped[i])
                {
                    result.Add(blocks[i]);
                }
            }

            return result;
        }

        public BlockPage BreakPage(BlockPage page)
        {
            var blocks = page.AllBlocks.ToList();            
            var result = new BlockPage();

            var splitted = blocks.Select(b => SplitBlock((BlockSet<IBlock>)b)).ToList();

            for (int i = 0; i < blocks.Count; i++)
            {
                if (blocks[i] == null) continue;

                for (int j = i + 1; j < blocks.Count; j++)
                {
                    if (blocks[j] == null) continue;

                    if (Block.HasOverlap(blocks[i], blocks[j]))
                    {
                        int k = SelectBlock(splitted, blocks, i, j);

                        var selected_block = blocks[k];
                        var selected_block_split = splitted[k];

                        int size = SelectSize(blocks[i], blocks[j], selected_block_split);

                        var newblocks = CreateNewBlocks((BlockSet<IBlock>)selected_block, size);

                        // replace
                        blocks[k] = null;
                        blocks.Add(newblocks[0]);
                        blocks.Add(newblocks[1]);
                        splitted[k] = null;
                        splitted.Add(SplitBlock(newblocks[0]));
                        splitted.Add(SplitBlock(newblocks[1]));
                    }
                }                
            }

            result.AddRange(blocks.Where(b => b != null));

            return result;
        }

        int SelectBlock(List<BlockSet<IBlock>[]> splitted, IList<IBlock> blocks, int i, int j)
        {
            var split1 = splitted[i];
            var container1 = blocks[i];

            var split2 = splitted[j];
            var container2 = blocks[j];

            bool goodCandidate1 = !CheckOverlapCrossIntersection(split1, container2);
            bool goodCandidate2 = !CheckOverlapCrossIntersection(split2, container1);

            if (goodCandidate1)
                return i;

            if (goodCandidate2)
                return j;

            throw new NotImplementedException("can it happen?");
        }

        int SelectSize(IBlock container1, IBlock container2, BlockSet<IBlock>[] candidateBlockArray)
        {
            float[] intersection = GetIntersectionAreaY(container1, container2);
            
            IBlock topBlock = null;
            IBlock bottomBlock = null;
            IBlock coreBlock = null;

            topBlock = candidateBlockArray.First();
            bottomBlock = candidateBlockArray.Last();
            coreBlock = (candidateBlockArray.Length == 3) ? candidateBlockArray[1] : null;

            bool checkTop = Block.HasOverlapY(topBlock, intersection[0], intersection[1]);
            bool checkBottom = Block.HasOverlapY(bottomBlock, intersection[0], intersection[1]);
            bool checkCore = Block.HasOverlapY(coreBlock, intersection[0], intersection[1]);

            int top = ((BlockSet<IBlock>)topBlock).Count();
            int bottom = ((BlockSet<IBlock>)bottomBlock).Count();
            int core = ((BlockSet<IBlock>)coreBlock).Count();

            int total = top + bottom + core;

            if (checkTop)
                total = top;

            if (checkBottom)
                total = total - bottom;

            if (total < 0)
                throw new NotImplementedException();

            return total;
        }

        BlockSet<IBlock>[] CreateNewBlocks(BlockSet<IBlock> blocks, int middle)
        {
            int total = blocks.Count();

            var blockA = new BlockSet<IBlock>();
            var blockB = new BlockSet<IBlock>();

            blockA.AddRange(blocks.Take(middle));
            blockB.AddRange(blocks.TakeLast(total - middle));

            return new BlockSet<IBlock>[] { blockA , blockB };
        }

        bool CheckOverlapCrossIntersection(BlockSet<IBlock>[] splitted, IBlock container)
        {
            foreach(var splittedBlock in splitted)
            {
                bool overlaps = Block.HasOverlap(splittedBlock, container);
                if (overlaps)
                    return true;
            }
            
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
                throw new InvalidOperationException("No intersection?");

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
                blockB.AddRange(blocks.Take(total - middle + 1));

                int count2 = blockA.Count() + blockB.Count();
                if (count2 != blocks.Count)
                    throw new InvalidOperationException();

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
                throw new InvalidOperationException();

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
                    throw new InvalidOperationException("should not reach the end of the sequence");

                x1 = Math.Min(x1, b.GetX());
                x2 = Math.Max(x2, b.GetX() + b.GetWidth());
            }

            if (count == 0)
                throw new InvalidOperationException();

            return count-1;
        }
        
        bool IntersectLine(float point, float x1, float x2)
        {
            return (x1 <= point) && (x2 >= point);
        }
    }
}
