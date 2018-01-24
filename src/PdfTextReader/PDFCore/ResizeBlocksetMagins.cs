using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.PDFCore
{
    class ResizeBlocksetMagins : IProcessBlock
    {
        const float MAX_PAGE_WIDTH_DIFFERENCE = 8f;

        private List<Data> Values { get; set; }
        private List<Data> ValuesY { get; set; }
        List<IBlock> OrderedBlocks = new List<IBlock>();
        private float _MinX;
        private float _MaxX;
        private float _PageWidth;

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

        void SetupPage(BlockPage page)
        {
            _MinX = page.AllBlocks.GetX();
            _MaxX = page.AllBlocks.GetX() + page.AllBlocks.GetWidth();
            _PageWidth = page.AllBlocks.GetWidth() + 2;

            CheckBasicStats();
        }

        // code copied from OrganizePageLayout
        void CheckBasicStats()
        {
            // sometimes the page width is shorter - should we use another source for page width?

            float pageWidth = BasicFirstPageStats.Global.PageWidth;

            float diff = pageWidth - _PageWidth;

            if (diff > MAX_PAGE_WIDTH_DIFFERENCE)
            {
                PdfReaderException.Warning("Large PageWidth difference -- using the BasicFirstPageStats");
                _MinX = Math.Min( BasicFirstPageStats.Global.MinX , _MinX);
                _MaxX = Math.Max( BasicFirstPageStats.Global.MaxX , _MaxX);

                _PageWidth = _MaxX - _MinX;
            }
        }

        public BlockPage Process(BlockPage page)
        {
            var result = new BlockPage();

            SetupPage(page);

            const float error_othercolumn = 2f;

            var blocksets = page.AllBlocks.ToList();

            if (blocksets.Count == 0)
                return page;

            float x1 = _MinX;
            float x2 = _MaxX;
            float dx = _PageWidth;

            float h1 = page.AllBlocks.GetH();
            float h2 = page.AllBlocks.GetH() + page.AllBlocks.GetHeight();
            float dh = page.AllBlocks.GetHeight() + 2;

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
            .ToList();
            
            for(int i=0; i<values.Count; i++)
            {
                var blsearch = values[i];

                if (blsearch.B is TableSet)
                    continue;

                if (blsearch.B is ImageBlock)
                        continue;

                var bl = blsearch.B;

                Block block = null;

                // set min size
                if ( blsearch.X == 0 )
                {
                    float diff = blsearch.B.GetX() - _MinX;
                    if( diff < 0 )
                        PdfReaderException.AlwaysThrow("invalid difference");

                    if( diff > error_othercolumn )
                    {
                        float width = bl.GetX() + bl.GetWidth() - _MinX;

                        block = new Block()
                        {
                            X = _MinX,
                            Width = width,
                            H = blsearch.B.GetH(),
                            Height = blsearch.B.GetHeight()
                        };
                    }
                }

                if( blsearch.X2 == 6 )
                {
                    float diff = _MaxX - blsearch.B.GetX() - blsearch.B.GetWidth();
                    if (diff < 0)
                        PdfReaderException.AlwaysThrow("invalid difference");

                    if (diff > error_othercolumn)
                    {
                        float width = _MaxX - bl.GetX();
                        float bx1 = _MaxX - width;

                        block = new Block()
                        {
                            X = bx1,
                            Width = width,
                            H = blsearch.B.GetH(),
                            Height = blsearch.B.GetHeight()
                        };
                    }
                }

                if( block != null )
                {
                    // ensure it will increase
                    float diff = block.GetWidth() - blsearch.B.GetWidth();

                    if (diff < 0)
                        PdfReaderException.AlwaysThrow("should never decrease the block size");

                    // may receive multiples - confusing...
                    var original = (IEnumerable<IBlock>)blsearch.B;

                    if ((original is TableSet) || (original is ImageBlock))
                        PdfReaderException.AlwaysThrow("Block should not be resized");

                    var replace = new BlockSet2<IBlock>(original, block.GetX(), block.GetH(), block.GetX() + block.GetWidth(), block.GetH() + block.GetHeight());

                    result.Add(replace);
                }
                else
                {
                    result.Add(blsearch.B);
                }
                
            }
            
            return result;
        }
    }
}
