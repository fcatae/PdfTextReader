using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.PDFCore
{
    public class BlockPage
    {
        BlockSet<IBlock> _blocks = new BlockSet<IBlock>();

        public BlockSet<IBlock> AllBlocks => _blocks;

        public void Add(IBlock block)
        {
            if (block == null)
                throw new ArgumentNullException(nameof(block));

            _blocks.Add(block);
        }
        public void AddRange(IEnumerable<IBlock> blockList)
        {
            foreach(var block in blockList)
            {
                if (block == null)
                    throw new ArgumentNullException(nameof(block));

                Add(block);
            }
        }
    }
}
