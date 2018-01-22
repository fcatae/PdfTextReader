using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.Base
{
    class BlockPage2 : BlockPage
    {
        List<BlockPageSegment> _segments = new List<BlockPageSegment>();

        public void AddSegment(BlockPageSegment segment)
        {
            _segments.Add(segment);
        }

        public override BlockSet<IBlock> AllBlocks
        {
            get
            {
                var blocks = new BlockSet<IBlock>(this);

                foreach(var segment in _segments)
                {
                    foreach(var column in segment)
                    {
                        blocks.AddRange(column);
                    }
                    
                }

                return blocks;
            }
        }

        public override string ToString()
        {
            var names = _segments.Select(s => s.GetName());
            return String.Join("", names);
        }
    }
}
