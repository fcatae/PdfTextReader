using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.PDFCore
{
    class ResizeBlocksets : IProcessBlock, IValidateBlock
    {
        private List<Data> Values { get; set; }
        private List<Data> ValuesY { get; set; }
        List<IBlock> OrderedBlocks = new List<IBlock>();
        public bool[] ValuesB { get; private set; }

        class Data
        {
            public int ID;
            public int X;
            public int X2;
            public int Y;
            public int Y1;
            public int W;
            public float RW;
            public IBlock B;
        }

        public BlockPage Process(BlockPage page)
        {
            float error_othercolumn = 2f;

            var blocksets = page.AllBlocks.ToList();

            float x1 = page.AllBlocks.GetX();
            float x2 = page.AllBlocks.GetX() + page.AllBlocks.GetWidth();
            float dx = page.AllBlocks.GetWidth() + 2;
            float h1 = page.AllBlocks.GetH();
            float h2 = page.AllBlocks.GetH() + page.AllBlocks.GetHeight();
            float dh = page.AllBlocks.GetHeight() + 2;

            float pageSize = page.AllBlocks.Max(b => b.GetX() + b.GetWidth());

            // Prepare the values order by X
            int id = 0;
            var values = page.AllBlocks.Select(b => new Data
            {
                ID = id++,
                X = (int)(6.0 * ((b.GetX() - x1) / dx) + 0.5),
                X2 = (int)(6.0 * ((b.GetX() + b.GetWidth() - x1) / dx) + 0.5),
                Y = (int)(1000 * (b.GetH() - h1) / (dh)),
                Y1 = (int)(1000 * (b.GetH() + b.GetHeight() - h1) / (dh)),
                W = (int)(6.0 * (b.GetWidth() / dx) + 0.5),
                RW = b.GetWidth(),
                B = b
            })
            .OrderByDescending(p => p.W)
            .ToList();

            var columnW = (from v in values
                          group v by v.W into g
                          select new { g.Key, size = g.Max(ta => ta.RW) }).ToDictionary( t => t.Key );
            
            foreach(var blsearch in values)
            {
                if (blsearch.B is ImageBlock)
                    continue;

                // we could have used predefined blocks (w=6, w=3, etc)
                var predefinedBlocks = values;

                var over = predefinedBlocks
                    .Where(v => v != blsearch && v.X <= blsearch.X && v.X2 >= blsearch.X2)
                    .Where(v => v.RW > blsearch.RW )
                    .Where(v => Math.Abs(v.RW - blsearch.RW) > error_othercolumn)
                    .Select(v => v.B)
                    .ToList();

                var curblocks = values.Select(v => v.B).ToList();

                List<IBlock> repls = new List<IBlock>();

                foreach (var bl in over)
                {
                    var compareBlocks = curblocks.Except(new IBlock[] { bl, blsearch.B });

                    var block = new Block() {
                        X = bl.GetX(),
                        Width = bl.GetWidth(),
                        H = blsearch.B.GetH(),
                        Height = blsearch.B.GetHeight()
                    };

                    // ensure it will increase
                    float diff = block.GetWidth() - blsearch.B.GetWidth();

                    if (diff < 0)
                        throw new InvalidOperationException("should never decrease the block size");

                    if (CheckBoundary(compareBlocks, block))
                    {
                        // may receive multiples
                        var original = (IEnumerable<IBlock>)blsearch.B;
                        var replace = new BlockSet2<IBlock>(original, block.GetX(), block.GetH(), block.GetX()+block.GetWidth(), block.GetH()+block.GetHeight());
                        repls.Add(replace);
                    }
                }

                if(repls.Count > 0)
                {
                    // this is important because repls.Count can be > 1
                    if( repls.Count > 1 )
                    {
                        // add a breakpoint to monitor if needed
                    }

                    var largest_replace = repls.OrderByDescending(t => t.GetWidth()).First();
                    blsearch.B = largest_replace;
                }
            }

            var result = new BlockPage();

            result.AddRange(values.Select(p => (IBlock)p.B));

            //result.AddRange(OrderedBlocks);

            return result;
        }
        
        bool CheckBoundary(IEnumerable<IBlock> blockset, IBlock block)
        {
            foreach(var bl in blockset)
            {
                if (Block.HasOverlap(bl, block))
                    return false;
            }

            return true;
        }

        public BlockPage Validate(BlockPage page)
        {
            float error_column = 2f;

            var blocksets = page.AllBlocks.ToList();

            float x1 = page.AllBlocks.GetX();
            float x2 = page.AllBlocks.GetX() + page.AllBlocks.GetWidth();
            float dx = page.AllBlocks.GetWidth() + 2;
            float h1 = page.AllBlocks.GetH();
            float h2 = page.AllBlocks.GetH() + page.AllBlocks.GetHeight();
            float dh = page.AllBlocks.GetHeight() + 2;

            float pageSize = page.AllBlocks.Max(b => b.GetX() + b.GetWidth());

            // Prepare the values order by X
            int id = 0;
            var values = page.AllBlocks.Select(b => new Data
            {
                ID = id++,
                X = (int)(6.0 * ((b.GetX() - x1) / dx) + 0.5),
                X2 = (int)(6.0 * ((b.GetX() + b.GetWidth() - x1) / dx) + 0.5),
                Y = (int)(1000 * (b.GetH() - h1) / (dh)),
                Y1 = (int)(1000 * (b.GetH() + b.GetHeight() - h1) / (dh)),
                W = (int)(6.0 * (b.GetWidth() / dx) + 0.5),
                RW = b.GetWidth(),
                B = b
            })
            .OrderByDescending(p => p.W)
            .ToList();
            
            var columnW = (from v in values
                           group v by v.W into g
                           select new { g.Key, MaxRW = g.Max(ta => ta.RW) }).ToDictionary(t => t.Key);

            var result = new BlockPage();

            foreach (var block in values)
            {
                if( block.W != 2 && block.W != 3 && block.W != 4 && block.W != 6)
                {
                    result.Add(block.B);
                    continue;
                }

                if ((block.X % block.W) != 0)
                {
                    result.Add(block.B);
                    continue;
                }

                float rw = columnW[block.W].MaxRW;

                if( (rw - block.RW) > error_column)
                {
                    result.Add(block.B);
                    continue;
                }
            }            

            return result;
        }
    }
}
