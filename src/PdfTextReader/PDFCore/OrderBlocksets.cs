using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class OrderBlocksets : IProcessBlock
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
            public object B;
        }

        public BlockPage Process(BlockPage page)
        {
            var blocksets = page.AllBlocks.ToList();

            float x1 = page.AllBlocks.GetX();
            float x2 = page.AllBlocks.GetX() + page.AllBlocks.GetWidth();
            float dx = page.AllBlocks.GetWidth() + 2;
            float h1 = page.AllBlocks.GetH();
            float h2 = page.AllBlocks.GetH() + page.AllBlocks.GetHeight();
            float dh = page.AllBlocks.GetHeight() + 2;

            // Prepare the values order by X
            int id = 0;
            this.Values = page.AllBlocks.Select(b => new Data
            {
                ID = id++,
                X = (int)(6.0 * ((b.GetX() - x1) / dx) + 0.5),
                X2 = (int)(6.0 * ((b.GetX() + b.GetWidth() - x1) / dx) + 0.5),
                Y = (int)(1000 * (b.GetH() - h1) / (dh)),
                Y1 = (int)(1000 * (b.GetH() + b.GetHeight() - h1) / (dh)),
                W = (int)(6.0 * (b.GetWidth() / dx) + 0.5),
                B = b
            })
            .OrderBy(p => 10000 * p.X - p.Y)
            .ToList();

            // Prepare the values order by Y
            this.ValuesY = Values.OrderBy(p => -100 * p.Y + p.X).ToList();

            this.ValuesB = new bool[Values.Count];

            OrderedBlocks = new List<IBlock>();

            scan();

            var result = new BlockPage();

            //result.AddRange(Values.Select(p => (IBlock)p.B));

            result.AddRange(OrderedBlocks);

            return result;
        }

        void scan(int x = 2, int min_y = -1, int max_x=6, int cur_w=-1)
        {
            // Algorithm
            // 1. Scan the values in x=2..6
            // 2. Skip processed values
            // 3. Take the parameter while x <= x2
            // 4. Save the max Y
            //Values = Values
            //    .Where(p => p != null)
            //    .OrderBy(p => 10000 * p.X - p.Y)
            //    .ToList();

            for (; x<= max_x; x+=2)
            {
                for(int k=0; k<Values.Count; k++)                
                {
                    var v = Values[k];

                    // skip if already processed
                    if (v == null)
                        continue;

                    // ignore low values
                    if (v.Y < min_y)
                        continue;

                    if (cur_w == -1)
                        cur_w = v.W;

                    bool oneColumn = (cur_w == 6);
                    bool twoColumns = (cur_w == 3);
                    bool threeColumns = (cur_w == 2) || (cur_w == 4);

                    // if W >= 3
                    if ( v.W != cur_w )
                    {
                        bool oneColumn2 = (v.W == 6);
                        bool twoColumns2 = (v.W == 3);
                        bool threeColumns2 = (v.W == 2) || (v.W == 4);

                        if(!(threeColumns && threeColumns2))
                            scan(x: x, min_y: v.Y1 + 1, max_x: 6, cur_w: v.W);
                    }

                    // consume all values < X2
                    if ( x >= v.X2 )
                    {
                        // take the parameter
                        OrderedBlocks.Add((IBlock)v.B);
                        Console.WriteLine($"[{v.ID}] X={v.X} Y={v.Y} X2={v.X2} W={v.W}");
                        Values[k] = null;
                        continue;
                    }

                    if (v.X2 > max_x)
                        continue;

                    // define a new goal
                    scan(x: x+2, min_y: v.Y1 + 1, max_x: v.X2, cur_w: cur_w);

                    // consume X2
                    OrderedBlocks.Add((IBlock)v.B);
                    Console.WriteLine($"[{v.ID}] X={v.X} Y={v.Y} X2={v.X2} W={v.W}");
                    Values[k] = null;

                    // reset
                    k = -1;

                    if( cur_w == 6 )
                        cur_w = -1;

                    if (cur_w == 3 && v.X == 3)
                        cur_w = -1;

                    if (cur_w == 2 && v.X == 4)
                        cur_w = -1;
                }
            }

        }
    }
}
