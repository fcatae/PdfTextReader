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
        public string Tag = "";

        public void Add(Block block)
        {
            _list.Add(block);
        }
        public void Add(IEnumerable<IBlock> blocks)
        {
            _list.AddRange(blocks);
        }

        public string GetText() => GetTextInternal();

        public float GetX() => _list.Min(b => b.GetX());
        public float GetH() => _list.Min(b => b.GetH());
        public float GetWidth() => _list.Max(b => b.GetX() + b.GetWidth()) - _list.Min(b => b.GetX());
        public float GetHeight() => _list.Max(b => b.GetH() + b.GetHeight()) - _list.Min(b => b.GetH());

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

        public BlockSet[] BreakBlock(float centery)
        {
            var list1 = _list.Where(b => b.GetH() < centery);
            var list2 = _list.Where(b => b.GetH() > centery);

            var b1 = new BlockSet();
            var b2 = new BlockSet();

            b1.Add(list1);
            b2.Add(list2);

            int c1 = b1._list.Count();
            int c2 = b2._list.Count();

            // sometimes it cannot be broken
            if (c1 == 0 || c2 == 0)
                return null;

            return new BlockSet[] { b1, b2 }; 

            //// setup
            //float minx = _list.Min(b => b.GetX());
            //float maxx = _list.Max(b => b.GetX() + b.GetWidth());

            //int position;
            //float curx1 = float.MaxValue;
            //float curx2 = float.MinValue;

            //// scan down until find the maximum margin
            //for(position = 0; position<_list.Count; position++)
            //{
            //    var block = _list[position];
            //    float x1 = block.GetX();
            //    float x2 = block.GetX() + block.GetWidth();

            //    curx1 = (x1 < curx1) ? x1 : curx1;
            //    curx2 = (x2 > curx2) ? x2 : curx2;

            //    if (curx1 == minx && curx2 == maxx)
            //    {
            //        int potential_checkpoint1 = position;

            //        // there should be no other max 

            //        break;
            //    }
            //}

            //if( position == _list.Count )
            //{
            //    // should not reach this point
            //    throw new InvalidOperationException();
            //}

            //int checkpoint1 = position;

            return null;
        }

    }
}
