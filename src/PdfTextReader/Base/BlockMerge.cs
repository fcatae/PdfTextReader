using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.Base
{
    class BlockMerge : IBlockSet<IBlock>
    {
        BlockSet<IBlock> _blocks = new BlockSet<IBlock>();
        List<BlockSet<IBlock>> _list = new List<BlockSet<IBlock>>();

        public string GetText() => GetLine()[0].GetText();

        List<IBlock> GetLine()
        {
            var textList = _list.OrderBy(b => b.GetX()).Select(b => GetText(b)).ToArray();
            string text = String.Join("     |     ", textList);

            var first = (Block)_list.OrderBy(b => b.GetX()).First().First();
            var line = new BlockLine(first)
            {
                Text = text,
                HasLargeSpace = true
            };

            var getline = new List<IBlock>() { line };

            return getline;
        }

        string GetText(BlockSet<IBlock> block)
        {
            return String.Join(" ", block.Select(b => b.GetText()));
        }

        public void Merge(IEnumerable<IBlock> blocks)
        {
            _blocks.AddRange(blocks);

            var bset = new BlockSet<IBlock>();
            bset.AddRange(blocks);

            _list.Add(bset);
        }

        public IEnumerator<IBlock> GetEnumerator() 
        {
            return GetLine().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetLine().GetEnumerator();
        }

        public float GetH() => _blocks.GetH();

        public float GetHeight() => _blocks.GetHeight();

        public float GetWidth() => _blocks.GetWidth();

        public float GetWordSpacing() => _blocks.GetWordSpacing();

        public float GetX() => _blocks.GetX();
    }
}
