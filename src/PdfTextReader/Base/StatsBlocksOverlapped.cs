using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Base
{
    class StatsBlocksOverlapped
    {
        public static StatsBlocksOverlapped Empty = new StatsBlocksOverlapped();

        public IBlock[] Blocks;
        public int[] BlockIds;
    }
}
