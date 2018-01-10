using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.Base
{
    public class BlockSet2<T> : IBlockSet<T>
        where T: IBlock
    {
        public BlockSet2(IEnumerable<T> blockset, float x1, float h1, float x2, float h2)
        {
            AddRange(blockset);
            _x1 = x1;
            _x2 = x2;
            _h1 = h1;
            _h2 = h2;
        }

        List<T> _blockList = new List<T>();
        
        float _x1 = float.MaxValue;
        float _h1 = float.MaxValue;
        float _x2 = float.MinValue;
        float _h2 = float.MinValue;

        public static BlockSet2<IBlock> GetBlockCache(BlockSet<IBlock> block)
        {
            return new BlockSet2<IBlock>(block, block.GetX(), block.GetH(), block.GetX() + block.GetWidth(), block.GetH() + block.GetHeight());
        }

        public string GetText() => throw new InvalidOperationException();
        public float GetX() => _x1;
        public float GetH() => _h1;
        public float GetWidth() => (_x2 - _x1);
        public float GetHeight() => (_h2 - _h1);

        public float GetWordSpacing() => throw new NotImplementedException();

        void AddRange(IEnumerable<T> blockList)
        {
            _blockList.AddRange(blockList);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _blockList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _blockList.GetEnumerator();
        }
    }
}
