using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class GroupLines : IProcessBlock
    {
        const float statSameLine = 0.1f;
        int statSubfonts = 0;
        int statBackspace = 0;

        public BlockPage Process(BlockPage page)
        {
            BlockLine2 line = null;
            IBlock last = null;
            var result = new BlockPage();
            
            foreach (var block in page.AllBlocks)
            {
                if(last != null)
                {
                    if ( CheckSubfonts(line, block) )
                    {
                        bool isBackspace = CheckBackspace(line, block);

                        // conside same line: update text and Width
                        // convention: do not add space here
                        line.Text += block.GetText();
                        line.Width = block.GetX() + block.GetWidth() - line.GetX();

                        // gather statistics
                        statBackspace += (isBackspace) ? 1 : 0;
                        statSubfonts++;

                        // does not update 'last' variable!!
                        continue;
                    }
                }
                
                if (( last == null ) || (CompareLine(block, last) != 0))
                {
                    var b = (Block)block;

                    line = new BlockLine2()
                    {
                        Text = block.GetText(),
                        X = block.GetX(),
                        H = block.GetH(),
                        Width = block.GetWidth(),
                        Height = block.GetHeight(),

                        // might be inaccurate
                        FontFullName = b.FontFullName,
                        FontName = b.FontName,
                        FontSize = b.FontSize,
                        FontStyle = b.FontStyle
                    };

                    result.Add(line);
                }
                else                
                {
                    string separator = (ShouldAddSpace(last, block)) ? " " : "";

                    // same line: update text and Width
                    line.Text += separator + block.GetText();
                    line.Width = block.GetX() + block.GetWidth() - line.GetX();
                }

                last = block;
            }

            return result;
        }

        int CompareLine(IBlock a, IBlock b)
        {
            float diff = a.GetH() - b.GetH();

            if (Math.Abs(diff) < statSameLine)
                return 0;

            return (diff > 0) ? 1 : -1;
        }

        bool ShouldAddSpace(IBlock before, IBlock after)
        {
            float wordSpacing = before.GetWordSpacing();
            float x1 = before.GetX() + before.GetWidth();
            float x2 = after.GetX();
            float distance = x2 - x1;

            if (wordSpacing == 0)
            {
                // WEIRD, but assume it is at the end of the line?
                // Do not add space
                return false;
            }

            return (distance > wordSpacing);
        }

        bool CheckSubfonts(IBlock normal, IBlock sub)
        {
            float subH1 = sub.GetH();
            float subH2 = sub.GetH() + sub.GetHeight();
            float norH1 = normal.GetH();
            float norH2 = normal.GetH() + normal.GetHeight();

            return ((norH1 < subH1) && (norH2 > subH2));
        }

        bool CheckBackspace(IBlock line, IBlock block)
        {
            float lineX = line.GetX() + line.GetWidth();
            float wordX = block.GetX();

            return (wordX < lineX);
        }
    }
}
