using PdfTextReader.Base;
using PdfTextReader.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.PDFValidation
{
    class IdentifyValidationMarks : IProcessBlock
    {        
        public BlockPage Process(BlockPage page)
        {
            var result = ProcessColors(page);
            
            return result;
        }
        
        static HashSet<int> _Colors = new HashSet<int>();

        public BlockPage ProcessColors(BlockPage page)
        {
            var result = new BlockPage();

            var colored_lines = page.AllBlocks.Cast<MarkLine>();
            
            foreach (var cur in colored_lines)
            {
                int color = cur.Color;

                result.Add(cur);

                if (!_Colors.Contains(color))
                {
                    _Colors.Add(color);
                    System.Diagnostics.Debug.WriteLine($"color = {cur.Color}");
                }
                
            }

            return result;
        }        
    }
}
