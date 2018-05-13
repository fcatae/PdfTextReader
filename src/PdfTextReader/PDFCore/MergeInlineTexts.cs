using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.PDFCore
{
    class MergeInlineTexts : IProcessBlock
    {
        public BlockPage Process(BlockPage page)
        {
            return MergeElements(page);         
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

        BlockPage MergeElements(BlockPage page)
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

                    bool doesntApplyI = !(blocks[i] is BlockSet<IBlock>);
                    bool doesntApplyJ = !(blocks[j] is BlockSet<IBlock>);

                    if (doesntApplyI || doesntApplyJ)
                        continue;

                    if (HasOverlap(blocks[i], blocks[j]))
                    {   
                        var elems = BreakElements(blocks[i], blocks[j]);

                        if (elems == null || elems.Length != 2 )
                            PdfReaderException.AlwaysThrow("merge: (elems == null || elems.Length != 2 )");

                        // has to do replacement in place
                        blocks[i] = elems[0];
                        blocks[j] = elems[1];
                        //blocks.AddRange(elems);

                        break;
                    }
                }
            }

            result.AddRange(blocks.Where(b=> b!=null));
            
            return result;
        }

        IBlock[] BreakElements(IBlock a, IBlock b)
        {
            var over = GetOverlapBlock(a, b);
            //var overlapBlockset = GetOverlapBlockSet(over, (BlockSet<IBlock>)a, (BlockSet<IBlock>)b);

            //if (String.IsNullOrEmpty(overlapBlockset.GetText()))
            //    PdfReaderException.AlwaysThrow("overlapBlockset.GetText is null");

            var new_a = MarkBlockLineWithLargeSpace(over, (BlockSet<IBlock>)a);
            var new_b = MarkBlockLineWithLargeSpace(over, (BlockSet<IBlock>)b);

            var result = new BlockSet<IBlock>[] { new_a, new_b };

            // result.First(bl => bl != null).AddRange(overlapBlockset);

            return result;
        }
                
        Block GetOverlapBlock(IBlock a, IBlock b)
        {
            float a_x1 = a.GetX();
            float a_x2 = a.GetX() + a.GetWidth();
            float a_y1 = a.GetH();
            float a_y2 = a.GetH() + a.GetHeight();

            float b_x1 = b.GetX();
            float b_x2 = b.GetX() + b.GetWidth();
            float b_y1 = b.GetH();
            float b_y2 = b.GetH() + b.GetHeight();

            float x = Math.Max(a_x1, b_x1);
            float h = Math.Max(a_y1, b_y1);

            return new Block
            {
                Text = null,
                X = x,
                Width = Math.Min(a_x2, b_x2) - x,
                H = h,
                Height = Math.Min(a_y2, b_y2) - h
            };
        }

        IBlockSet<IBlock> GetOverlapBlockSet(Block overlap, BlockSet<IBlock> block1, BlockSet<IBlock> block2)
        {
            var b1 = block1.Where(b => HasOverlapY(b, overlap));
            var b2 = block2.Where(b => HasOverlapY(b, overlap));

            if (b1.Count() == 0 || b2.Count() == 0)
                PdfReaderException.Throw("b1.Count() == 0 || b2.Count() == 0");

            var blockMerge = new BlockMerge();
            blockMerge.Merge(b1);
            blockMerge.Merge(b2);

            return blockMerge;
        }

        BlockSet<IBlock> GetCleanBlockSet(Block overlap, BlockSet<IBlock> block1)
        {
            var blockSet = new BlockSet<IBlock>();

            var b1 = block1.Where(b => !HasOverlapY(b, overlap));
            
            blockSet.AddRange(b1);

            if (blockSet.Count() == 0)
                return null;

            return blockSet;
        }

        BlockSet<IBlock> MarkBlockLineWithLargeSpace(Block overlap, BlockSet<IBlock> block1)
        {            
            var overlappedLines = block1.Where(b => HasOverlapY(b, overlap));

            var count = overlappedLines
                .Cast<BlockLine>()
                .Select(bl => { bl.HasLargeSpace = true; return 1; })
                .Count();

            return block1;
        }

        public static bool HasOverlapY(IBlock a, IBlock b)
        {
            //float a_x1 = a.GetX();
            //float a_x2 = a.GetX() + a.GetWidth();
            float a_y1 = a.GetH();
            float a_y2 = a.GetH() + a.GetHeight();

            //float b_x1 = b.GetX();
            //float b_x2 = b.GetX() + b.GetWidth();
            float b_y1 = b.GetH();
            float b_y2 = b.GetH() + b.GetHeight();

            return HasOverlap(a_y1, a_y2, b_y1, b_y2);
            //bool hasIntersectionX = HasOverlap(a_x1, a_x2, b_x1, b_x2);
            //bool hasOverlapContainsY = HasOverlap(a_y1, a_y2, b_y1, b_y2);

            //return (hasIntersectionX && hasOverlapContainsY);
        }

        public static bool HasOverlap(IBlock a, IBlock b)
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
            bool HasOverlapY = HasOverlap(a_y1, a_y2, b_y1, b_y2);

            return (hasIntersectionX && HasOverlapY);
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
                PdfReaderException.AlwaysThrow("(b1 > b2) || (a1 > a2)");

            bool contains = (a1 <= b1) && (a2 >= b2);

            return contains;
        }
        static bool HasOverlap(float a1, float a2, float b1, float b2)
        {
            if ((b1 > b2) || (a1 > a2))
                PdfReaderException.AlwaysThrow("(b1 > b2) || (a1 > a2)");

            bool separated = ((a1 < b1) && (a2 < b1)) ||
                             ((a1 > b2) && (a2 > b2));
            return !separated;
        }
    }
}
