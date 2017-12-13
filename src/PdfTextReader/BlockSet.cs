using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using iText.Kernel.Geom;

namespace PdfTextReader
{
    class BlockSet : IBlock
    {
        List<IBlock> _list = new List<IBlock>();
        Block _cachedBlock = null;

        public void Add(Block block)
        {
            _list.Add(block);
        }

        public string GetText() => throw new InvalidOperationException();

        public float GetX() => _list.Min(b => b.GetX());
        public float GetH() => _list.Min(b => b.GetH());
        public float GetWidth() => _list.Max(b => b.GetX() + b.GetWidth()) - _list.Min(b => b.GetX());
        public float GetHeight() => _list.Max(b => b.GetH() + b.GetHeight()) - _list.Min(b => b.GetH());

        void Update()
        {
            var block = new Block()
            {
                Text = String.Join( "\n", _list.Select( b => b.GetText() ) ),
                X = _list.Min(b => b.GetX()),
                H = _list.Min(b => b.GetH()),
                Width = _list.Max(b => b.GetX() + b.GetWidth()) - _list.Min(b => b.GetX()),
                Height = _list.Max(b => b.GetH() + b.GetHeight()) - _list.Min(b => b.GetH())
            };

            this._cachedBlock = block;
        }
    }
}
