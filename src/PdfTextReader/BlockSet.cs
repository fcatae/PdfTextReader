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

        public string GetText() => GetTextInternal();

        public float GetX() => _list.Min(b => b.GetX());
        public float GetH() => _list.Min(b => b.GetH());
        public float GetWidth() => _list.Max(b => b.GetX() + b.GetWidth()) - _list.Min(b => b.GetX());
        public float GetHeight() => _list.Max(b => b.GetH() + b.GetHeight()) - _list.Min(b => b.GetH());
        public float GetWordSpacing() => _list.Max(b => b.GetWordSpacing());

        string GetTextInternal()
        {
            if (_list.Count == 0)
                return "";

            StringBuilder sb = new StringBuilder();

            float err = 10/2f; // close to font size
            float err_x = 1f;

            float lastX = _list[0].GetX();
            float lastH = _list[0].GetH();
            string lastT = null;

            foreach(var b in _list)
            {
                string fragment = b.GetText();

                // should use baseline instead?
                float curH = b.GetH();
                float curX = b.GetX();
                float curHeight = b.GetHeight();
                float errH = Math.Abs( curH - lastH );
                float errX = curX - lastX;

                char ch = (char)0;

                if( errH >= err )
                {
                    ch = '\n';
                } else if(errX >= err_x)
                {
                    ch = ' ';
                } else
                {
                    // just concatenate
                }                

                if( ch != 0 )
                {
                    sb.Append(ch);
                }

                sb.Append(fragment);

                lastH = curH;
                lastX = curX + b.GetWidth();
                lastT = b.GetText();
            }

            return sb.ToString();
        }

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
