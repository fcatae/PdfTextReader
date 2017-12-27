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
using PdfTextReader.Lucas_Testes.Helpers;
using iText.Kernel.Geom;

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

        public void ProcessText(string srcpath, string dstpath)
        {
            PdfReader reader = new PdfReader(srcpath);
            using (var pdf = new PdfDocument(reader, new PdfWriter(dstpath)))
            {
                int pages = pdf.GetNumberOfPages();
                for (int i = 1; i <= pages; i++)
                {
                    var page = pdf.GetPage(i);
                    var canvas = new PdfCanvas(page);
                    var blockList = new List<BlockSet>();
                    var blockSet = new BlockSet();
                    Block last = null;
                    List<Block> AllBlocks = new List<Block>();

                    CustomListener listener = new CustomListener(page.GetPageSize().GetHeight());
                    var parser = new PdfCanvasProcessor(listener);
                    List<MainItem> items = listener.GetItems();
                    parser.ProcessPageContent(page);
                    items.Sort();
                    List<LineItem> lines = LineItem.GetLines(items);

                    //HighlightItems(lines, canvas);
                }
            }
        }

        void HighlightItems(List<MainItem> listOfItems, PdfCanvas canvas)
        {
            foreach (var item in listOfItems)
            {
                canvas.SaveState();
                canvas.SetStrokeColor(item.GetColor());
                canvas.SetLineWidth(1);
                Rectangle r = item.GetRectangle();
                canvas.Rectangle(r.GetLeft(), r.GetBottom(), r.GetWidth(), r.GetHeight());
                canvas.Stroke();
                canvas.RestoreState();
            }
        }
        void HighlightLines(List<LineItem> listOfItems, PdfCanvas canvas)
        {
            foreach (var item in listOfItems)
            {
                canvas.SaveState();
                canvas.SetStrokeColor(item.GetColor());
                canvas.SetLineWidth(1);
                Rectangle r = item.GetRectangle();
                canvas.Rectangle(r.GetLeft(), r.GetBottom(), r.GetWidth(), r.GetHeight());
                canvas.Stroke();
                canvas.RestoreState();
            }
        }
        void HighlightStructureItems(List<StructureItem> listOfItems, PdfCanvas canvas)
        {
            foreach (var item in listOfItems)
            {
                canvas.SaveState();
                canvas.SetStrokeColor(item.GetColor());
                canvas.SetLineWidth(1);
                Rectangle r = item.GetRectangle();
                canvas.Rectangle(r.GetLeft(), r.GetBottom(), r.GetWidth(), r.GetHeight());
                canvas.Stroke();
                canvas.RestoreState();
            }
        }

        public void ProcessBlockExtra(string srcpath, string dstpath)
        {
            using (var pdf = new PdfDocument(new PdfReader(srcpath), new PdfWriter(dstpath)))
            {
                var page = pdf.GetPage(1);
                var canvas = new PdfCanvas(page);
                var blockList = new List<BlockSet>();
                var blockSet = new BlockSet();

                var parser = new PdfCanvasProcessor(new UserListenerExtra(b => {
                    Console.WriteLine("Test");
                }));

                parser.ProcessPageContent(page);

                blockList.Add(blockSet);

                FinalProcess(canvas, blockList, page, parser);
            }
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
                List<Block> AllBlocks = new List<Block>();

                var parser = new PdfCanvasProcessor(new UserListener(b =>
                {
                    AllBlocks.Add(b);
                    bool shouldBreak = false;

                    if (last != null)
                    {
                        // expect: previous >~ next
                        float previous = last.H;
                        float next = b.H;

                        // previous >> next
                        if (previous > next + 100)
                        {
                            shouldBreak = true;
                        }

                        // previous < next
                        if (previous < next - 5)
                        {
                            shouldBreak = true;
                        }
                    }

                    if (shouldBreak)
                    {
                        blockList.Add(blockSet);

                        // reset
                        blockSet = new BlockSet();
                    }

                    blockSet.Add(b);

                    //if (b.Text.Contains("<!ID"))
                    //{
                    //    canvas.SetStrokeColor(ColorConstants.RED);
                    //    canvas.Rectangle(b.X, b.B, b.Width + 5, b.Height - b.Lower - 5);
                    //    canvas.Stroke();
                    //}

                    last = b;
                }));

                parser.ProcessPageContent(page);

                blockList.Add(blockSet);

                FinalProcess(canvas, blockList, page, parser);

            }
        }

        void FinalProcess(PdfCanvas canvas, List<BlockSet> blockList, PdfPage page, PdfCanvasProcessor parser)
        {
            BreakBlockSets(blockList);

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

            ProcessStructure(page, parser, blockList, canvas);

            PrintText(blockList);
        }

        void BreakBlockSets(List<BlockSet> blockList)
        {
            List<BlockSet> list = new List<BlockSet>(blockList);

            // this number increases
            int initial_total = list.Count;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == null) continue;

                for (int j = i + 1; j < list.Count; j++)
                {
                    if (list[i] == null) throw new InvalidOperationException();

                    if (list[j] == null) continue;

                    bool hasOverlap = HasAreaOverlap(list[i], list[j]);

                    if (hasOverlap)
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
                            float newcenter = (center == y1) ? y2 + force : y1 - force;

                            result = larger.BreakBlock(newcenter);
                        }

                        if (result != null)
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

                            if (hasOverlapR0 || hasOverlapR1)
                            {
                                if (hackCenter == false)
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

                        if (hackCenter == false)
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
                if ((b_y1 >= a_y1) && (b_y1 <= a_y2))
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
                if (w1 < 50 && w2 < 50)
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


        void ProcessStructure(PdfPage page, PdfCanvasProcessor parser, List<BlockSet> blockList, PdfCanvas canvas)
        {
            //ProcessLine
            CustomListener listener = new CustomListener(page.GetPageSize().GetHeight());
            parser = new PdfCanvasProcessor(listener);
            List<MainItem> items = listener.GetItems();
            parser.ProcessPageContent(page);
            items.Sort();
            List<LineItem> lines = LineItem.GetLines(items, blockList);

            List<StructureItem> structures = StructureItem.GetStructures(lines, blockList);

            HighlightStructureItems(structures, canvas);
        }

        void PrintText(List<BlockSet> blockList)
        {
            foreach (var b in blockList)
            {
                string text = b.GetText();
                System.Diagnostics.Debug.WriteLine(text);
                System.Diagnostics.Debug.WriteLine("=============================");
            }
        }

        void RemoveList(List<BlockSet> blockList, IEnumerable<BlockSet> blocksToBeRemoved)
        {
            foreach (var b in blocksToBeRemoved)
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

        void DrawRectangle(PdfCanvas canvas, IEnumerable<BlockSet> blockList, Color color, string type)
        {
            foreach (var bset in blockList)
            {
                canvas.SetStrokeColor(ColorConstants.RED);
                canvas.Rectangle(bset.GetX(), bset.GetH(), bset.GetWidth() + 10, bset.GetHeight() + 10);
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

                var parser = new PdfCanvasProcessor(new UserListener(b =>
                {
                    canvas.SetStrokeColor(ColorConstants.YELLOW);
                    canvas.Rectangle(b.X, b.B, b.Width, b.Height - b.Lower);
                    canvas.Stroke();
                }));

                parser.ProcessPageContent(page);
            }
        }

    }
}
