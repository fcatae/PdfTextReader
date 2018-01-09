using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.PDFCore
{
    public class TableSet : IBlock, IEnumerable<IBlock>
    {
        List<IBlock> _blockList = new List<IBlock>();

        float _x1 = float.MaxValue;
        float _h1 = float.MaxValue;
        float _x2 = float.MinValue;
        float _h2 = float.MinValue;

        public string GetText() => throw new InvalidOperationException();

        public float GetX() => _x1;
        public float GetH() => _h1;
        public float GetWidth() => (_x2 - _x1);
        public float GetHeight() => (_h2 - _h1);

        public float GetWordSpacing() => throw new NotImplementedException();

        public void Add(IBlock block)
        {
            if (block == null)
                throw new ArgumentNullException(nameof(block));

            UpdateBoundary(block.GetX(), block.GetH(), block.GetX() + block.GetWidth(), block.GetH() + block.GetHeight());

            _blockList.Add(block);
        }

        public void AddRange(IEnumerable<IBlock> blockList)
        {
            foreach(var block in blockList)
            {
                Add(block);
            }
        }

        void UpdateBoundary(float x1, float h1, float x2, float h2)
        {
            _x1 = (x1 < _x1) ? x1 : _x1;
            _h1 = (h1 < _h1) ? h1 : _h1;
            _x2 = (x2 > _x2) ? x2 : _x2;
            _h2 = (h2 > _h2) ? h2 : _h2;
        }

        public IEnumerator<IBlock> GetEnumerator()
        {
            return _blockList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _blockList.GetEnumerator();
        }

        public void MergeWith(TableSet other)
        {
            foreach(var b in other._blockList)
            {
                this.Add(b);
            }            
        }
    }
}
