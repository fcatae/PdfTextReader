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
        // UpdateBoundary bug: revert to dynamic Linq calculation
        //float _x1 = float.MaxValue;
        //float _h1 = float.MaxValue;
        //float _x2 = float.MinValue;
        //float _h2 = float.MinValue;

        public string GetText() => throw new InvalidOperationException();
        // UpdateBoundary bug: revert to dynamic Linq calculation
        //public float GetX() => _x1;
        //public float GetH() => _h1;
        //public float GetWidth() => (_x2 - _x1);
        //public float GetHeight() => (_h2 - _h1);
        public float GetX() => _blockList.Min(b => b.GetX());
        public float GetH() => _blockList.Min(b => b.GetH());
        public float GetWidth() => _blockList.Max(b => b.GetX() + b.GetWidth()) - _blockList.Min(b => b.GetX());
        public float GetHeight() => _blockList.Max(b => b.GetH() + b.GetHeight()) - _blockList.Min(b => b.GetH());
        public float GetWordSpacing() => throw new NotImplementedException();

        public void Add(IBlock block)
        {
            if (block == null)
                throw new ArgumentNullException(nameof(block));

            //UpdateBoundary(block.GetX(), block.GetH(), block.GetX() + block.GetWidth(), block.GetH() + block.GetHeight());

            _blockList.Add(block);
        }

        public void AddRange(IEnumerable<IBlock> blockList)
        {
            foreach(var block in blockList)
            {
                Add(block);
            }
        }

        // UpdateBoundary has a MAJOR BUG:
        // if BlockSet owns a collection of updatable IBlocks 
        // (eg, another BlockSet objects), then the boundary
        // is not updated when the child is updated
        //void UpdateBoundary(float x1, float h1, float x2, float h2)
        //{
        //    _x1 = (x1 < _x1) ? x1 : _x1;
        //    _h1 = (h1 < _h1) ? h1 : _h1;
        //    _x2 = (x2 > _x2) ? x2 : _x2;
        //    _h2 = (h2 > _h2) ? h2 : _h2;
        //}

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
