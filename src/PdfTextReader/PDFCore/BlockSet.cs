using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.PDFCore
{
    public class BlockSet<T> : IEnumerable<T>
        where T: IBlock
    {
        List<T> _blockList = new List<T>();

        public void Add(T block)
        {
            if (block == null)
                throw new ArgumentNullException(nameof(block));

            _blockList.Add(block);
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
