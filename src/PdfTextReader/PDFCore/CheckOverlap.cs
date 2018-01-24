using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class CheckOverlap : IProcessBlock, IValidateBlock, IRetrieveStatistics
    {
        StatsBlocksOverlapped _overlappedBlocks = StatsBlocksOverlapped.Empty;

        public BlockPage Validate(BlockPage page)
        {
            if (HasTableOverlap(page))
            {
                return GetTableOverlap(page);
            }

            var emptyResult = new BlockPage();

            return emptyResult;
        }

        public BlockPage Process(BlockPage page)
        {
            var result = GetTableOverlap(page);
            var list = page.AllBlocks.ToList();
            var overlapped = new List<IBlock>();
            var overlappedIds = new List<int>();

            foreach (var block in result.AllBlocks)
            {
                for(int i=0; i<list.Count; i++)
                {
                    if( block == list[i] )
                    {
                        overlapped.Add(block);
                        overlappedIds.Add(i);
                    }
                }
            }
      
            if(overlapped.Count > 0)
            {
                _overlappedBlocks = new StatsBlocksOverlapped()
                {
                    Blocks = overlapped.ToArray(),
                    BlockIds = overlappedIds.ToArray()
                };
            }

            return page;
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

        public object RetrieveStatistics()
        {
            return _overlappedBlocks;
        }
    }
}
