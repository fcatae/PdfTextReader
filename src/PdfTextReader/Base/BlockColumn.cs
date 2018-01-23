using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Base
{
    class BlockColumn : BlockSet<IBlock>
    {
        private int _columnType;
        public int X1;
        public int X2;
        public int W;
        public int Y1;
        public int Y2;
        public int H;

        public BlockColumn(BlockPage page, int columnType, int x, int w) : base(page)
        {
            this._columnType = columnType;
            this.X1 = x;
            this.X2 = x + w;
            this.W = w;
        }

        public void AddBlock(IBlock block)
        {
            this.Add(block);
        }

        public string GetColumnName()
        {
            string columnId = (this.X1 + 1).ToString();
            string suffix = "";

            if ((_columnType == 3) && (this.W > 3))
                return columnId + "X";

            return columnId;
        }
    }
}
