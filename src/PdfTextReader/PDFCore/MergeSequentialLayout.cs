using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class MergeSequentialLayout : IProcessBlock
    {
        const float VERTICAL_LINE_DIFFERENCE = .5f;

        public BlockPage Process(BlockPage page2)
        {
            var page = page2 as BlockPage2;

            if (page == null)
                PdfReaderException.AlwaysThrow("MergeSequentialLayout must execute AFTER OrganizePageLayout");

            var result = new BlockPage2();
                        
            foreach(var segment in page.Segments)
            {
                BlockPageSegment newsegment = new BlockPageSegment(result, segment.NumberOfColumns);

                foreach(var column in segment.Columns)
                {
                    BlockColumn newcolumn = new BlockColumn(result, column.ColumnType, column.X1, column.W);

                    IBlock last = null;

                    var orderedColumns = column.OrderByDescending( b => b.GetH() );

                    foreach (var block in orderedColumns)
                    {
                        if (last != null)
                        {
                            var b1 = last as IBlockSet<IBlock>;
                            var b2 = block as IBlockSet<IBlock>;

                            if (b1 == null || b2 == null)
                                PdfReaderException.AlwaysThrow("not expected");

                            if (Block.SameHeight(b1, b2))
                            {
                                last = Merge(b1, b2);

                                // merge
                                continue;
                            }

                            newcolumn.Add(last);
                        }

                        last = block;
                    }

                    if (last != null)
                        newcolumn.Add(last);

                    newsegment.AddColumn(newcolumn);
                }

                result.AddSegment(newsegment);
            }
            
            return result;
        }

        IBlockSet<IBlock> Merge(IBlockSet<IBlock> b1, IBlockSet<IBlock> b2)
        {
            bool first = b1.GetX() < b2.GetX();
            float x1 = Math.Min(b1.GetX(), b2.GetX());
            float x2 = Math.Max(b1.GetX()+ b1.GetWidth(), b2.GetX()+b2.GetWidth());
            float h1 = Math.Min(b1.GetH(), b2.GetH());
            float h2 = Math.Max(b1.GetH() + b1.GetHeight(), b2.GetH() + b2.GetHeight());

            var blocks = (first) ? b1.Concat(b2) : b2.Concat(b1);

            var newblock = new BlockSet2<IBlock>(blocks, x1, h1, x2, h2);

            return newblock;
        }
    }
}
