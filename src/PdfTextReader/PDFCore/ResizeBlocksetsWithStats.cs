using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class ResizeBlocksetsWithStats : IProcessBlock
    {
        private readonly BasicFirstPageStats _basicStats;

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

        public ResizeBlocksetsWithStats(BasicFirstPageStats basicStats)
        {
            this._basicStats = basicStats;
        }

        public BlockPage Process(BlockPage page)
        {
            // not implemented yet
            return page;

            //var blocksets = page.AllBlocks.ToList();

            //if (blocksets.Count == 0)
            //    return page;

            //float x1 = page.AllBlocks.GetX();
            //float x2 = page.AllBlocks.GetX() + page.AllBlocks.GetWidth();
            //float dx = page.AllBlocks.GetWidth() + 2;
            //float h1 = page.AllBlocks.GetH();
            //float h2 = page.AllBlocks.GetH() + page.AllBlocks.GetHeight();
            //float dh = page.AllBlocks.GetHeight() + 2;

            //float pageSize = page.AllBlocks.Max(b => b.GetX() + b.GetWidth());

            //// Prepare the values order by X
            //int id = 0;
            //var values = page.AllBlocks.Select(b => new Data
            //{
            //    ID = id++,
            //    X = (int)(6.0 * ((b.GetX() - x1) / dx) + 0.5),
            //    X2 = (int)(6.0 * ((b.GetX() + b.GetWidth() - x1) / dx) + 0.5),
            //    Y = (int)(1000 * (b.GetH() - h1) / (dh)),
            //    Y1 = (int)(1000 * (b.GetH() + b.GetHeight() - h1) / (dh)),
            //    W = (int)(6.0 * (b.GetWidth() / dx) + 0.5),
            //    RW = b.GetWidth(),
            //    B = b
            //})
            //.OrderByDescending(p => p.W)
            //.ToList();

            //var columnW = (from v in values
            //               group v by v.W into g
            //               select new { g.Key, size = g.Max(ta => ta.RW) }).ToDictionary(t => t.Key);

            //return page;
        }
    }
}
