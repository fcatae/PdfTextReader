using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.PDFCore
{
    [Obsolete]
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
                for (int j = i + 1; j < blocks.Count; j++)
                {
                    if (blocks[i] == null) continue;
                    if (blocks[j] == null) continue;

                    if (Block.HasOverlap(blocks[i], blocks[j]))
                    {
                        // precheck: contained block?
                        bool blockContainsA = BlockContains(blocks[i], blocks[j]);
                        bool blockContainsB = BlockContains(blocks[j], blocks[i]);

                        if( blockContainsA || blockContainsB )
                        {

                        }

                        int k = SelectBlock(splitted, blocks, i, j);

                        bool breakInTheMiddle = false;
                        
                        if ((k == -1) && (blockContainsA || blockContainsB))
                        {
                            k = (blockContainsA) ? i : k;
                            k = (blockContainsB) ? j : k;

                            breakInTheMiddle = true;
                        }
                        
                        if ( k == -1 )
                        {
                            // the blocks can merge?
                            float wdiff = Math.Abs(blocks[i].GetWidth() - blocks[j].GetWidth());
                            float xdiff = Math.Abs( blocks[i].GetX() - blocks[j].GetX() );

                            // ignore?
                            if (wdiff < 10f && xdiff < 10f)
                                continue;

                            // breakcolumns have a poor performance when
                            // tables and images get removed.
                            // we could retry after adding them back to the doc
                            // so far it is not supported yet

                            // very likely to have A contains B in Y axis, but not in X
                            // in this case, we need to break both blocks at the same operation
                            PdfReaderException.AlwaysThrow("true overlap?");

                            // throw new NotImplementedException("merge blockLines");

                            // cannot break the blocks ?!?!?!?!
                            //throw new InvalidOperationException("should be handled previously in precheck");
                            //continue;
                        }

                        var selected_block = blocks[k];
                        var selected_block_split = splitted[k];

                        IBlock otherBlock = (selected_block == blocks[i]) ? blocks[j] : blocks[i];
                        float middle = otherBlock.GetH() + otherBlock.GetHeight() / 2;

                        int size = -1;

                        if (breakInTheMiddle)
                        {
                            size = SelectSize(selected_block, middle);
                        }
                        else
                        {
                            size = SelectSize(blocks[i], blocks[j], selected_block_split);

                            if (size == -1)
                                size = SelectSize(selected_block, middle);
                        }
                        
                        if (size == 0)
                            PdfReaderException.AlwaysThrow("size == 0");

                        if (size == -1)
                            PdfReaderException.AlwaysThrow("size == -1");

                        if (size == ((BlockSet<IBlock>)selected_block).Count())
                            PdfReaderException.AlwaysThrow("size > total_blocks");

                        var newblocks = CreateNewBlocks((BlockSet<IBlock>)selected_block, size);
                        
                        if(breakInTheMiddle)
                        {
                            // Check if newblocks has collision
                            bool checkOverlap = CheckOverlapCrossIntersection(newblocks, otherBlock);

                            if (checkOverlap)
                                PdfReaderException.AlwaysThrow("checkOverlap");
                        }
                        
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

            if( goodCandidate1 && goodCandidate2 )
                PdfReaderException.AlwaysThrow("can it happen?");

            // else
            // NOTHING FOUND
            //throw new NotImplementedException("needs to improve the scenario");
            // the blocks are overlapped and requires more than one split
            // adjust (FindInitialBlocks -> statDownInTheBottom)
            return -1;
        }

        int SelectSize(IBlock container, float middle)
        {
            var blockset = (BlockSet<IBlock>)container;
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

            int top = ((BlockSet<IBlock>)topBlock).Count();
            int bottom = ((BlockSet<IBlock>)bottomBlock).Count();

            bool checkCore = false;
            int core = 0;

            if (coreBlock != null)
            {
                checkCore = Block.HasOverlapY(coreBlock, intersection[0], intersection[1]);
                core = ((BlockSet<IBlock>)coreBlock).Count();
            }

            int total = top + bottom + core;

            if ((!checkBottom) && (!checkTop))
                return -1;

            int result = -1;

            if (checkTop)
                result = top;

            if (checkBottom)
                result = total - bottom;

            if (result < 0)
                PdfReaderException.AlwaysThrow("result < 0");

            return result;
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

        bool BlockContains(IBlock a, IBlock b)
        {
            float a_y1 = a.GetH();
            float a_y2 = a.GetH() + a.GetHeight();
            float b_y1 = b.GetH();
            float b_y2 = b.GetH() + b.GetHeight();

            return ((a_y2 > b_y2) && (a_y1 < b_y1));
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
