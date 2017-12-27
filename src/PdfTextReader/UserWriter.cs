using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Layout;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace PdfTextReader
{
    class UserWriter
    {
        public void Process(string srcpath, string dstpath)
        {
            using (var pdf = new PdfDocument(new PdfReader(srcpath), new PdfWriter(dstpath)))
            {
                //var document = new Document(pdf);
                var page = pdf.GetPage(1);

                var canvas = new PdfCanvas(page);

                canvas.Rectangle(10, 10, 100, 100);
                canvas.Stroke();
            }
        }

        public void Process(string srcpath, Action<Block> func)
        {
            var parser = new PdfCanvasProcessor(new UserListener(func));

            using (var pdf = new PdfDocument(new PdfReader(srcpath)))
            {
                var page = pdf.GetPage(1);

                parser.ProcessPageContent(page);
            }
        }

        public void ProcessBlockExtra(string srcpath, string dstpath)
        {
            using (var pdf = new PdfDocument(new PdfReader(srcpath), new PdfWriter(dstpath)))
            {
                var page = pdf.GetPage(1);
                var canvas = new PdfCanvas(page);
                //var blockList = new List<BlockSet>();
                //var blockSet = new BlockSet();
                //TableCell last = null;
                List<TableCell> cellList = new List<TableCell>();
                
                var parser = new PdfCanvasProcessor(new UserListenerExtra(c => {

                    // notes:
                    // images (op ==0)
                    // background (large stroke width, eg. stroke_width > 10 pt)

                    // consider only strokes
                    if (c.Op != 1)
                        return;

                    cellList.Add(c);
                }));

                parser.ProcessPageContent(page);

                var blockArray = new BlockSet[cellList.Count];

                bool hasModification = true;
                while (hasModification)
                {
                    hasModification = false;

                    // iterate every line found
                    for (int i = 0; i < cellList.Count; i++)
                    {
                        var c = cellList[i];

                        if (blockArray[i] == null)
                        {
                            // create a fresh blockset
                            blockArray[i] = new BlockSet();
                            // add the current element to the blockset
                            blockArray[i].Add(c);
                        }

                        BlockSet currentBlockset = blockArray[i];

                        // assume that currentBlockset ALWAYS contains c
                        // -- it was added during blockArray assignment

                        // look for connected lines
                        for (int j = i + 1; j < cellList.Count; j++)
                        {
                            // skip if it already has block array assigned
                            if (blockArray[j] == currentBlockset)
                                continue;

                            var last = cellList[j];

                            // check if blockSet contains c (two rectangles)
                            float b_x1 = last.GetX();
                            float b_x2 = last.GetX() + last.GetWidth();
                            float b_y1 = last.GetH();
                            float b_y2 = last.GetH() + last.GetHeight();

                            var blockSet = currentBlockset;
                            bool b1 = HasOverlap(blockSet, b_x1, b_y1);
                            bool b2 = HasOverlap(blockSet, b_x1, b_y2);
                            bool b3 = HasOverlap(blockSet, b_x2, b_y2);
                            bool b4 = HasOverlap(blockSet, b_x2, b_y1);

                            bool hasOverlap = b1 || b2 || b3 || b4;

                            // FOUND A CONNECTED LINE!
                            if (hasOverlap)
                            {
                                hasModification = true;

                                // assign the blockarray
                                blockArray[j] = currentBlockset;
                                // and add the element
                                blockArray[j].Add(last);
                            }
                            else
                            {
                                // do nothing
                            }
                        }
                    }
                }
                
                // transform blockArray into blockList
                var blockList = blockArray.Distinct().ToList();
                int count1 = blockArray.Length;
                int count2 = blockList.Count;

                DrawRectangle(canvas, blockList, ColorConstants.RED);
            }
        }

        bool HasOverlap(BlockSet blockSet, float x, float h)
        {
            float a_x1 = blockSet.GetX();
            float a_x2 = blockSet.GetX() + blockSet.GetWidth();
            float a_y1 = blockSet.GetH();
            float a_y2 = blockSet.GetH() + blockSet.GetHeight();

            bool hasOverlap = ((a_x1 <= x) && (a_x2 >= x) && (a_y1 <= h) && (a_y2 >= h));

            return hasOverlap;
        }

        public void ProcessBlock(string srcpath, string dstpath)
        {
            using (var pdf = new PdfDocument(new PdfReader(srcpath), new PdfWriter(dstpath)))
            {
                var page = pdf.GetPage(1);
                var canvas = new PdfCanvas(page);
                var blockList = new List<BlockSet>();
                var blockSet = new BlockSet();
                Block last = null;

                var parser = new PdfCanvasProcessor(new UserListener( b => {

                    bool shouldBreak = false;
                    
                    if( last != null )
                    {
                        // expect: previous >~ next
                        float previous = last.H;
                        float next = b.H;

                        // previous >> next
                        if( previous > next + 100)
                        {
                            shouldBreak = true;
                        }

                        // previous < next
                        if( previous < next - 5 )
                        {
                            shouldBreak = true;
                        }
                    }

                    // DOES NOT WORK - columns are considered table cells
                    // check if it is an inline table
                    //bool mightBeTable = false;
                    //if(shouldBreak && last != null)
                    //{
                    //    // assume last is not null
                    //    // assume blockset is not null
                    //    float a_x1 = blockSet.GetX();
                    //    float a_x2 = blockSet.GetX() + blockSet.GetWidth();
                    //    float a_y1 = blockSet.GetH();
                    //    float a_y2 = blockSet.GetH() + blockSet.GetHeight();

                    //    float b_x1 = b.GetX();
                    //    float b_x2 = b.GetX() + b.GetWidth();
                    //    float b_y1 = b.GetH();
                    //    float b_y2 = b.GetH() + b.GetHeight();

                    //    bool blockInsideLastBlockset = ((a_y1 < b_y1) && (a_y2 > b_y2));
                    //    bool blockInsideWidth = ((a_x1 < b_x1) && (a_x2 > b_x2));

                    //    if(blockInsideLastBlockset && blockInsideWidth)
                    //    {
                    //        mightBeTable = true;
                    //    }                        
                    //}

                    // should break block
                    if(shouldBreak)
                    {
                        blockList.Add(blockSet);

                        // reset
                        blockSet = new BlockSet();
                    }

                    blockSet.Add(b);

                    last = b;
                }));
                
                parser.ProcessPageContent(page);

                blockList.Add(blockSet);

                FinalProcess(canvas, blockList);

                //var header = FindHeader(blockList);
                //var footer = FindFooter(blockList);

                //RemoveList(blockList, header);
                //RemoveList(blockList, footer);

                //// post-processing
                //DrawRectangle(canvas, footer, ColorConstants.BLUE);
                //DrawRectangle(canvas, header, ColorConstants.BLUE);

                //DrawRectangle(canvas, blockList, ColorConstants.YELLOW);

                //PrintText(blockList);
            }
        }

        void FinalProcess(PdfCanvas canvas, List<BlockSet> blockList)
        {
            //int count1 = blockList.Count;
            //TryMergeBlockSets(blockList);
            // merge does not work

            int count2 = blockList.Count;
            BreakBlockSets(blockList);

            int count3 = blockList.Count;

            var header = FindHeader(blockList);
            var footer = FindFooter(blockList);

            RemoveList(blockList, header);
            RemoveList(blockList, footer);

            // post-processing
            DrawRectangle(canvas, footer, ColorConstants.BLUE);
            DrawRectangle(canvas, header, ColorConstants.BLUE);

            DrawRectangle(canvas, blockList, ColorConstants.YELLOW);

            DrawRectangle(canvas, blockList.Where(b => b.Tag == "gray"), ColorConstants.LIGHT_GRAY);
            DrawRectangle(canvas, blockList.Where(b => b.Tag == "orange"), ColorConstants.ORANGE);

            PrintText(blockList);
        }

        void TryMergeBlockSets(List<BlockSet> blockList)
        {
            for(int i=0; i<blockList.Count-1; i++)
            {
                var curBlock = blockList[i];
                var nextBlock = blockList[i + 1];

                Console.WriteLine(curBlock.GetText());

                float a_x1 = curBlock.GetX();
                float a_x2 = curBlock.GetX() + curBlock.GetWidth();
                float b_x1 = nextBlock.GetX();
                float b_x2 = nextBlock.GetX() + nextBlock.GetWidth();

                // a contains b
                bool hasOverlapA = ((a_x1 <= b_x1) && (a_x2 >= b_x2));
                bool hasOverlapB = ((b_x1 <= a_x1) && (b_x2 >= a_x2));
                bool hasOverlap = hasOverlapA;// || hasOverlapB;

                if (hasOverlap)
                {
                    float a_y2 = curBlock.GetH() + curBlock.GetHeight();
                    float b_y2 = nextBlock.GetH() + nextBlock.GetHeight();

                    if( a_y2 > b_y2 )
                    {

                    }
                    else
                    {

                    }

                    // Merge current and next blocks
                    var newblock = BlockSet.MergeBlocks(curBlock, nextBlock);

                    blockList[i] = null;
                    blockList[i + 1] = newblock;
                }
            }

            // remove nulled blocks
            blockList.RemoveAll(b => b == null);
        }

        void BreakBlockSets(List<BlockSet> blockList)
        {
            List<BlockSet> list = new List<BlockSet>(blockList);

            // this number increases
            int initial_total = list.Count;

            for(int i=0; i< list.Count; i++)
            {
                if (list[i] == null) continue;

                for(int j=i+1; j<list.Count; j++)
                {
                    if (list[i] == null) throw new InvalidOperationException();

                    if (list[j] == null) continue;

                    bool hasOverlap = HasAreaOverlap(list[i], list[j]);

                    if( hasOverlap )
                    {
                        var larger = GetBlockWithLargerWidth(list[i], list[j]);
                        var smaller = GetBlockWithSmallerWidth(list[i], list[j]);

                        // check is table?

                        // break block 
                        float center = CalculateCenterBreak(larger, smaller);
                        var result = larger.BreakBlock(center);

                        bool hackCenter = false;

                        // hack is required because we assume only splitting into 2 block
                        // the correct is to split into 3 - and then merge back

retryResult: // EXTREME HACK

                        if (hackCenter)
                        {
                            float y1 = smaller.GetH();
                            float y2 = smaller.GetH() + smaller.GetHeight();

                            float force = 3f;
                            float newcenter = (center == y1) ? y2+force : y1-force;

                            result = larger.BreakBlock(newcenter);
                        }

                        if( result != null )
                        {
                            // get results
                            if (result.Length != 2)
                                throw new InvalidOperationException();
                            
                            var r0 = (result[0]);
                            var r1 = (result[1]);

                            // check: was good decision?
                            bool hasOverlapR0 = HasAreaOverlap(r0, smaller);
                            bool hasOverlapR1 = HasAreaOverlap(r1, smaller);

                            bool badDecision = hasOverlapR0 || hasOverlapR1;

                            if ( hasOverlapR0 || hasOverlapR1 )
                            {
                                if(hackCenter == false)
                                {
                                    hackCenter = true;
                                    goto retryResult;
                                }

                                smaller.Tag = "gray";
                                larger.Tag = "orange";
                                
                                // not so good
                                continue;
                            }

                            // replace old item: 
                            // 1. remove
                            if (list[i] == larger)
                                list[i] = null;

                            if (list[j] == larger)
                                list[j] = null;

                            // 2. add new items
                            list.Add(r0);
                            list.Add(r1);

                            // 3. update the original blocklist
                            blockList.Remove(larger);
                            blockList.Add(r0);
                            blockList.Add(r1);

                            // 4. reset the counter
                            i--; break;
                        }

                        if(hackCenter == false)
                        {
                            hackCenter = true;
                            goto retryResult;
                        }

                        smaller.Tag = "gray";
                        larger.Tag = "orange";
                    }
                }
            }
        }

        float CalculateCenterBreak(BlockSet larger, BlockSet smaller)
        {
            float a_y1 = larger.GetH();
            float a_y2 = larger.GetH() + larger.GetHeight();

            float b_x1 = smaller.GetX();
            float b_x2 = smaller.GetX() + smaller.GetWidth();
            float b_y1 = smaller.GetH();
            float b_y2 = smaller.GetH() + smaller.GetHeight();

            float x = float.NaN;
            float y = float.NaN;

            // larger contains smaller?
            if ((a_y1 <= b_y1) && (a_y2 >= b_y2))
            {
                // calculate the center
                float cx = larger.GetX() + larger.GetWidth() / 2.0f;
                float cy = larger.GetH() + larger.GetHeight() / 2.0f;

                float dx1 = Math.Abs(b_x1 - cx);
                float dx2 = Math.Abs(b_x2 - cx);
                float dy1 = Math.Abs(b_y1 - cy);
                float dy2 = Math.Abs(b_y2 - cy);

                x = (dx1 < dx2) ? b_x1 : b_x2;
                y = (dy1 < dy2) ? b_y1 : b_y2;
            }
            else
            {
                // use the point inside the larger block
                if(( b_y1 >= a_y1 ) && ( b_y1 <= a_y2))
                {
                    y = b_y1;
                }
                if ((b_y2 >= a_y1) && (b_y2 <= a_y2))
                {
                    y = b_y2;
                }
            }

            // what??? smaller > larger ???
            if ((a_y1 > b_y1) && (a_y2 < b_y2))
            {
                float w1 = larger.GetWidth();
                float w2 = smaller.GetWidth();

                // if width are both small
                if( w1 < 50 && w2 < 50 )
                {
                    // return any value
                    y = b_y1;
                }

            }

            if (float.IsNaN(y))
                throw new InvalidOperationException();

            // use force
            float force = 3f;
            float fx = (x == b_x2) ? force : -force;
            float fy = (y == b_y2) ? force : -force;

            // ignore x
            return y + fy;
        }

        BlockSet GetBlockWithLargerWidth(BlockSet a, BlockSet b)
        {
            float a_width = a.GetWidth();
            float b_width = b.GetWidth();

            return (a_width > b_width) ? a : b;
        }

        BlockSet GetBlockWithSmallerWidth(BlockSet a, BlockSet b)
        {
            float a_width = a.GetWidth();
            float b_width = b.GetWidth();

            return (a_width > b_width) ? b : a;
        }

        bool HasAreaOverlap(BlockSet b1, BlockSet b2)
        {
            float a_x1 = b1.GetX();
            float a_x2 = b1.GetX() + b1.GetWidth();
            float a_y1 = b1.GetH();
            float a_y2 = b1.GetH() + b1.GetHeight();
            float b_x1 = b2.GetX();
            float b_x2 = b2.GetX() + b2.GetWidth();
            float b_y1 = b2.GetH();
            float b_y2 = b2.GetH() + b2.GetHeight();

            bool overlapsX = HasOverlap(a_x1, a_x2, b_x1, b_x2);
            bool overlapsY = HasOverlap(a_y1, a_y2, b_y1, b_y2);

            return (overlapsX && overlapsY);
        }

        bool HasOverlap(float a1, float a2, float b1, float b2)
        {
            if (a1 < b1)
            {
                return (a2 > b1);
            }

            if (a1 > b1)
            {
                return (b2 > a1);
            }

            // a1 == b1
            return true;
        }

        void PrintText(List<BlockSet> blockList)
        {
            foreach(var b in blockList)
            {
                string text = b.GetText();
                System.Diagnostics.Debug.WriteLine(text);
                System.Diagnostics.Debug.WriteLine("=============================");
            }
        }        

        void RemoveList(List<BlockSet> blockList, IEnumerable<BlockSet> blocksToBeRemoved)
        {
            foreach(var b in blocksToBeRemoved)
            {
                blockList.Remove(b);
            }
        }

        void DrawRectangle(PdfCanvas canvas, IEnumerable<BlockSet> blockList, Color color)
        {
            foreach (var bset in blockList)
            {
                canvas.SetStrokeColor(color);
                canvas.Rectangle(bset.GetX(), bset.GetH(), bset.GetWidth(), bset.GetHeight());
                canvas.Stroke();
            }
        }

        IEnumerable<BlockSet> FindHeader(List<BlockSet> blockList)
        {
            float err = 1f;
            float maxH = blockList.Max(b => b.GetH()) - err;

            var blocksAtHeader = blockList.Where(b => b.GetH() >= maxH);

            return blocksAtHeader.ToArray();
        }

        IEnumerable<BlockSet> FindFooter(List<BlockSet> blockList)
        {
            float err = 1f;
            float minH = blockList.Min(b => b.GetH()) + err;

            var blocksAtFooter = blockList.Where(b => b.GetH() <= minH);

            return blocksAtFooter.ToArray();
        }

        public void ExampleMarker(Block b, PdfCanvas canvas)
        {
            // letra maiuscula: if(b.Text.Length > 0 &&  (b.Text.ToUpper()[0] == b.Text[0]) )
            canvas.SetStrokeColor(ColorConstants.YELLOW);
            canvas.Rectangle(b.X, b.B, b.Width, b.Height - b.Lower);
            canvas.Stroke();
        }

        public void ProcessMarker2(string srcpath, string dstpath)
        {
            using (var pdf = new PdfDocument(new PdfReader(srcpath), new PdfWriter(dstpath)))
            {
                var page = pdf.GetPage(1);
                var canvas = new PdfCanvas(page);

                var parser = new PdfCanvasProcessor(new UserListener(b => {
                    canvas.SetStrokeColor(ColorConstants.YELLOW);
                    canvas.Rectangle(b.X, b.B, b.Width, b.Height - b.Lower);
                    canvas.Stroke();
                }));

                parser.ProcessPageContent(page);
            }
        }

    }
}
