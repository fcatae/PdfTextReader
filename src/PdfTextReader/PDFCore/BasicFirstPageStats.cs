using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class BasicFirstPageStats : IProcessBlock
    {
        public float MinX { get; private set; }
        public float MaxX { get; private set; }
        public float PageWidth { get; private set; }

        public static BasicFirstPageStats Global = null;

        void SetupPage(BlockPage page)
        {
            if (Global == null)
            {
                Global = this;
            }

            var blocks = page.AllBlocks;

            MinX = blocks.Min(b => b.GetX());
            MaxX = blocks.Max(b => b.GetX() + b.GetWidth());
            PageWidth = MaxX - MinX;
        }

        public BlockPage Process(BlockPage page)
        {
            SetupPage(page);

            return page;
        }
    }
}
