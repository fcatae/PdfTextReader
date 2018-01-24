using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.PDFCore
{
    class BreakColumnsRewrite : IProcessBlock, IValidateBlock
    {
        const float VERTICAL_MARGIN_DIFF = 1f;

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
            
            for (int i = 0; i < blocks.Count; i++)
            {
                var current = blocks[i] as BlockSet<IBlock>;

                if (current == null)
                    continue;

                for (int j = 0; j < blocks.Count; j++)
                {
                    if (i == j) continue;
                    if (blocks[j] == null) continue;
                    if (blocks[i] == null) break;

                    if (Block.HasOverlap(blocks[i], blocks[j]))
                    {
                        float otherH_bottom = blocks[j].GetH();
                        float otherH_top = blocks[j].GetH() + blocks[j].GetHeight();

                        if (otherH_bottom > otherH_top)
                            PdfReaderException.AlwaysThrow("negative height");

                        var blockList = current.ToList();
                        
                        int idxTop = FindTop(blockList, otherH_top);
                        int idxBottom = FindBottom(blockList, otherH_bottom);

                        var topBlock = RewriteBlockTop(blockList, idxTop);
                        var bodyBlock = RewriteBlockBody(blockList, idxBottom, idxTop);
                        var bottomBlock = RewriteBlockBottom(blockList, idxBottom);

                        if( topBlock != null || bottomBlock != null )
                        {
                            // replace the blocks
                            blocks[i] = null;

                            if (topBlock != null)
                            {
                                blocks.Add(CreateNewBlock(result, topBlock));
                            }

                            if (bodyBlock != null)
                            {
                                blocks.Add(CreateNewBlock(result, bodyBlock));
                            }

                            if (bottomBlock != null)
                            {
                                blocks.Add(CreateNewBlock(result, bottomBlock));
                            }
                        }
                        else
                        {
                            
                        }
                        // replace
                        
                        //blocks.Add(newblocks[0]);
                        //blocks.Add(newblocks[1]);                        
                    }
                }                
            }

            result.AddRange(blocks.Where(b => b != null));

            return result;
        }

        BlockSet<IBlock> CreateNewBlock(BlockPage page, IList<IBlock> blockList)
        {
            var block = new BlockSet<IBlock>(page);

            block.AddRange(blockList);

            return block;
        }

        IList<IBlock> RewriteBlockTop(List<IBlock> blockList, int idxTop)
        {
            if ( idxTop == -1 )
                return null;

            if ((idxTop >= blockList.Count) || (idxTop < -1))
                PdfReaderException.AlwaysThrow("invalid index");

            return blockList.Take(idxTop+1).ToList();
        }

        IList<IBlock> RewriteBlockBody(List<IBlock> blockList, int idxBottom, int idxTop)
        {
            int total = blockList.Count;

            if ( idxBottom <= idxTop )
                PdfReaderException.AlwaysThrow("invalid index");

            return blockList.Skip(idxTop + 1).Take( idxBottom - idxTop - 1 ).ToList();
        }

        IList<IBlock> RewriteBlockBottom(List<IBlock> blockList, int idxBottom)
        {
            int total = blockList.Count;

            if (idxBottom == blockList.Count)
                return null;

            if ((idxBottom < 0) || (idxBottom > blockList.Count))
                PdfReaderException.AlwaysThrow("invalid index");

            return blockList.Skip(idxBottom+1).Take(total - idxBottom).ToList();
        }

        int FindTop(List<IBlock> blockList, float top)
        {
            for (int k=0; k<blockList.Count; k++)
            {
                var b = blockList[k];

                float h = b.GetH();
                if (h < top + VERTICAL_MARGIN_DIFF)
                    return k - 1;
            }

            return -1;
        }

        int FindBottom(List<IBlock> blockList, float bottom)
        {
            for(int k=blockList.Count-1; k>=0; k--)
            {
                var b = blockList[k];

                float h = b.GetH() + b.GetHeight();
                if (h > bottom - VERTICAL_MARGIN_DIFF)
                    return k+1;                
            }

            return 0;
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
