using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.Base
{
    class BlockPage
    {
        BlockSet<IBlock> _blocks;

        public virtual BlockSet<IBlock> AllBlocks => _blocks;
        
        public virtual bool IsEmpty()
        {
            return (_blocks.Count() == 0);
        }

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
