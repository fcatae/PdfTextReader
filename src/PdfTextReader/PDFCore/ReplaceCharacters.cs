using System;
using System.Collections.Generic;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.PDFCore
{
    class ReplaceCharacters : IProcessBlock
    {
        const float MINIMUM_CHARACTER_OVERLAP = .5f;

        public BlockPage Process(BlockPage page)
        {
            IBlock last = null;
            var result = new BlockPage();

            foreach (var block in page.AllBlocks)
            {
                string ttt = block.GetText();

                if (last != null)
                {
                    // are the same lines
                    if (HasIntersectionH(block, last))
                    {
                        bool isBackspace = CheckBackspace(last, block);

                        if (isBackspace)
                        {
                            float endofblock = block.GetX() + block.GetWidth();
                            float endoflast = last.GetX() + last.GetWidth();
                            float diff = endofblock - endoflast;

                            string lastText = last.GetText();
                            string curText = block.GetText();

                            if ((lastText.Length == 1) && (curText.Length == 1) && (lastText != " "))
                            {
                                string text = lastText + curText;
                                string replaceText = null;

                                if (text == "o-")
                                {
                                    // Unicode 186 (0xba) = º
                                    replaceText = "\xba";
                                }

                                if (replaceText != null)
                                {
                                    ((Block)last).Text = replaceText;

                                    // do not set last: ignore the current block
                                    continue;
                                }
                            }
                        }
                    }

                    // defer adding the current item
                    // only add the last block
                    result.Add(last);
                }

                last = block;
            }

            // always add the last block
            if (last != null)
                result.Add(last);

            return result;
        }

        bool HasIntersectionH(IBlock a, IBlock b)
        {
            float aH1 = a.GetH();
            float aH2 = a.GetH() + a.GetHeight();
            float bH1 = b.GetH();
            float bH2 = b.GetH() + b.GetHeight();

            return ((aH1 <= bH1) && (aH2 >= bH1)) || ((aH1 <= bH2) && (aH2 >= bH2));
        }
        
        bool CheckBackspace(IBlock line, IBlock block)
        {
            float lineX = line.GetX() + line.GetWidth()*(1-MINIMUM_CHARACTER_OVERLAP);
            float wordX = block.GetX();

            return (wordX < lineX);
        }
    }
}
