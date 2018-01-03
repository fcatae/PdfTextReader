using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class ProcessBlockLines
    {
        public List<BlockSet> FindLines(List<BlockSet> blockLists)
        {
            var result = new List<BlockSet>();
            foreach (var bset in blockLists)
            {
                var lines = GetLinesInternal(bset);

                var nbset = new BlockSet();
                nbset.Add(lines);
                result.Add(nbset);
            }

            return result;
        }

        List<BlockLine> GetLinesInternal(BlockSet bset)
        {
            var list = bset.GetList();

            if (list.Count == 0)
                throw new InvalidOperationException();

            StringBuilder sb = new StringBuilder();
            List<BlockLine> lb = new List<BlockLine>();

            // invalid state???
            if (list.Count == 0)
                return lb;

            BlockLine cb = new BlockLine();
            lb.Add(cb);

            float err = 10 / 2f; // close to font size
            float err_x = 1f;

            float lastX = list[0].GetX();
            float lastH = list[0].GetH();
            string lastT = null;

            foreach (var b in list)
            {
                string fragment = b.GetText();

                // should use baseline instead?
                float curH = b.GetH();
                float curX = b.GetX();
                float curHeight = b.GetHeight();
                float errH = Math.Abs(curH - lastH);
                float errX = curX - lastX;

                char ch = (char)0;

                if (errH >= err)
                {
                    ch = '\n';
                    sb.Append(ch);
                    
                    cb = new BlockLine();
                    lb.Add(cb);
                }
                else if (errX >= err_x)
                {
                    ch = ' ';
                    sb.Append(ch);
                }
                else
                {
                    // just concatenate
                }
                
                sb.Append(fragment);
                cb.Add(b);

                // set the current font information
                cb.FontName = ((Block)b).FontName;
                cb.FontSize = ((Block)b).FontSize;


                lastH = curH;
                lastX = curX + b.GetWidth();
                lastT = b.GetText();
            }

            return lb;
            //return sb.ToString();
        }
    }
}
