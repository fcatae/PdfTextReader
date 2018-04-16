using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class OrganizePageLayout : IProcessBlock, IRetrieveStatistics
    {
        const float MAX_PAGE_WIDTH_DIFFERENCE = 8f;
        const float HORIZONTAL_LINE_HEIGHT = 5f;
        float _minX = float.NaN;
        float _maxX = float.NaN;
        float _pageWidth = float.NaN;
        string _pageLayout;

        private readonly BasicFirstPageStats _basicStats;

        public OrganizePageLayout(BasicFirstPageStats basicStats)
        {
            this._basicStats = basicStats;
        }

        void SetupPage(BlockPage page)
        {
            var blocks = page.AllBlocks;

            _minX = blocks.Min(b => b.GetX());
            _maxX = blocks.Max(b => b.GetX() + b.GetWidth());
            _pageWidth = _maxX - _minX;

            CheckBasicStats();
        }

        void CheckBasicStats()
        {
            // sometimes the page width is shorter - should we use another source for page width?

            float pageWidth = _basicStats.PageWidth;

            float diff = Math.Abs(pageWidth - _pageWidth);

            if( diff > MAX_PAGE_WIDTH_DIFFERENCE )
            {
                PdfReaderException.Warning("Large PageWidth difference -- using the BasicFirstPageStats");
                _minX = _basicStats.MinX;
                _maxX = _basicStats.MaxX;
                _pageWidth = _basicStats.PageWidth;
            }
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
                    bool isHorizontalLine = (block is TableSet) && (block.GetHeight() < HORIZONTAL_LINE_HEIGHT);
                    if (isHorizontalLine)
                        continue;

                    //Console.WriteLine($"NEW COLUMN");
                    column = new BlockColumn(newpage, columnType, position, columnSize);
                    segment.AddColumn(column);

                    last_columnX = position;
                    last_columnSize = columnSize;
                }

                //Console.WriteLine($"position x: {position} (ADDBLOCK)");

                column.AddBlock(block);
            }

            //Console.WriteLine($"Page type = {newpage.ToString()}");
            _pageLayout = newpage.ToString();

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

        public object RetrieveStatistics()
        {
            return new StatsPageLayout { Layout = _pageLayout };
        }
    }
}
