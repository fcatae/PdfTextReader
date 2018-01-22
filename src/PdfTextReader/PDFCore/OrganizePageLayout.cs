using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class OrganizePageLayout : IProcessBlock
    {
        float _minX = float.NaN;
        float _maxX = float.NaN;
        float _pageWidth = float.NaN;

        void SetupPage(BlockPage page)
        {
            var blocks = page.AllBlocks;

            _minX = blocks.Min(b => b.GetX());
            _maxX = blocks.Max(b => b.GetX() + b.GetWidth());
            _pageWidth = _maxX - _minX;
        }

        public BlockPage Process(BlockPage page)
        {
            SetupPage(page);

            BlockPage2 newpage = new BlockPage2();

            int last_columnType = -1;
            int last_columnX = -1;
            int last_columnSize = -1;

            BlockPageSegment segment = null;
            BlockColumn column = null;

            foreach (var block in page.AllBlocks)
            {
                float x = block.GetX() - _minX;
                float x2 = block.GetX() + block.GetWidth() - _minX;
                float w = block.GetWidth();

                int columnSize = GetColumnWidth(w);
                int columnType = GetNumberOfColumns(columnSize);

                // different Page Segment
                if ( columnType != last_columnType )
                {
                    segment = new BlockPageSegment(newpage, columnType);
                    newpage.AddSegment(segment);

                    //Console.WriteLine(columnType);
                    //Console.WriteLine("add new segment/column");

                    last_columnType = columnType;
                    last_columnX = -1;
                    last_columnSize = -1;
                }

                int position = GetColumnX(x, columnType);

                if(last_columnX != position || last_columnSize != columnSize)
                {
                    //Console.WriteLine($"NEW COLUMN");
                    column = new BlockColumn(newpage, columnType, position, columnSize);
                    segment.AddColumn(column);

                    last_columnX = position;
                    last_columnSize = columnSize;
                }

                //Console.WriteLine($"position x: {position} (ADDBLOCK)");

                column.AddBlock(block);
            }

            Console.WriteLine($"Page type = {newpage.ToString()}");

            return newpage;
        }

        int GetColumnWidth(float width)
        {
            if (width < _pageWidth / 3)
                return 2;

            if (width < _pageWidth / 2)
                return 3;

            if (width < _pageWidth * (2.0/3.0))
                return 4;

            return 6;
        }

        int GetColumnX(float x, int columnType)
        {
            return (int)(columnType * x/_pageWidth);
        }

        int GetNumberOfColumns(int width)
        {
            switch (width)
            {
                case 2:
                case 4:
                    return 3;

                case 3:
                    return 2;

                case 6:
                    return 1;
            }

            throw PdfReaderException.AlwaysThrow("Invalid column width");
        }
    }
}
