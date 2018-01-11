using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Base
{
    class BlockPage
    {
        BlockSet<IBlock> _blocks;

        public BlockSet<IBlock> AllBlocks => _blocks;

        public BlockPage()
        {
            _blocks = new BlockSet<IBlock>(this);
        }

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
