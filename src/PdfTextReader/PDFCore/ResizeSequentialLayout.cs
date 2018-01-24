using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class ResizeSequentialLayout : IProcessBlock
    {
        const float WIDTH_DIFFERENCE = 2f;

        public BlockPage Process(BlockPage page2)
        {
            var page = page2 as BlockPage2;

            if (page == null)
                PdfReaderException.AlwaysThrow("ResizeSequentialLayout must execute AFTER OrganizePageLayout");

            var result = new BlockPage2();
                        
            foreach(var segment in page.Segments)
            {
                BlockPageSegment newsegment = new BlockPageSegment(result, segment.NumberOfColumns);

                foreach(var column in segment.Columns)
                {
                    BlockColumn newcolumn = new BlockColumn(result, column.ColumnType, column.X1, column.W);

                    float minX = column.Min(b => b.GetX());
                    float maxX = column.Max(b => b.GetX() + b.GetWidth());

                    foreach (var block in column)
                    {
                        var bset = block as IBlockSet<IBlock>;

                        if( block is TableSet || block is ImageBlock )
                        {
                            newcolumn.Add(block);
                            continue;
                        }

                        if (bset == null)
                            PdfReaderException.AlwaysThrow("not expected");

                        var resizeBset = ResizeBlockSet(bset, minX, maxX);

                        newcolumn.Add(resizeBset);
                    }

                    newsegment.AddColumn(newcolumn);
                }

                result.AddSegment(newsegment);
            }
            
            return result;
        }

        IBlockSet<IBlock> ResizeBlockSet(IBlockSet<IBlock> bset, float minX, float maxX)
        {
            float maxWidth = maxX - minX;
            float width = bset.GetWidth();
            bool shouldResize = Math.Abs(maxWidth - width) > WIDTH_DIFFERENCE;

            float x1 = Math.Min(minX, bset.GetX());
            float x2 = Math.Max(maxX, bset.GetX() + bset.GetWidth());
            float h1 = bset.GetH();
            float h2 = bset.GetH() + bset.GetHeight();
            
            if(shouldResize)
            {
                return new BlockSet2<IBlock>(bset, x1, h1, x2, h2);
            }            

            return bset;
        }
    }
}
