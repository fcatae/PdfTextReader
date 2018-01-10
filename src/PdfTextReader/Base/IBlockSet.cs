using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Base
{
    public interface IBlockSet : IBlockSet<IBlock>
    {
    }

    public interface IBlockSet<T> : IBlock, IEnumerable<T>
    {
    }
}
