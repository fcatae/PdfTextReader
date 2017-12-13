using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using iText.Kernel.Geom;

namespace PdfTextReader
{
    class BlockSet
    {
        List<Block> _list = new List<Block>();

        public void Add(Block block)
        {
            _list.Add(block);
        }

        public float GetX() => _list.Min(b => b.X);
        public float GetH() => _list.Min(b => b.H);
        public float GetWidth() => _list.Max(b => b.X + b.Width) - _list.Min(b => b.X);
        public float GetHeight() => _list.Max(b => b.H + b.Height) - _list.Min(b => b.H);

        public float Update()
        {
            float minX = _list.Min(b => b.X);
            float maxX = _list.Max(b => b.X + b.Width);
            float minH = _list.Min(b => b.H);
            float maxH = _list.Max(b => b.H + b.Height);

            return minX;
        }

    }
}
