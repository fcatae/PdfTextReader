using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Base
{
    interface IBlockSet : IBlockSet<IBlock>
    {
    }

    interface IBlockSet<T> : IBlock, IEnumerable<T>
    {
    }
}
