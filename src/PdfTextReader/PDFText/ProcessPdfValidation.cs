using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using PdfTextReader.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.PDFText
{
    class ProcessPdfValidation : IEventListener, IPipelineResults<BlockPage>
    {
        private readonly List<EventType> _supportedEvents = new List<EventType>() { EventType.RENDER_PATH };

        private BlockSet<MarkLine> _blockSet = new BlockSet<MarkLine>();
        
        int GetColorNumber(Color color)
        {
            float[] components = color.GetColorValue();
            int size = components.Length;
            // 1=Gray, 3=RGB, 4=CMYK

            // RGB
            if (size == 3)
            {
                int r = (int)(2 * components[0] + .01f);
                int g = (int)(2 * components[1] + .01f);
                int b = (int)(2 * components[2] + .01f);

                // black/white/gray = not important
                if ((r == g) && (g == b))
                    return 0;
                                
                return (100*r + 10*g + 1*b) + 10000;
            }

            return 0;
        }

        public void EventOccurred(IEventData data, EventType type)
        {
            var line = data as PathRenderInfo;

            if (line != null)
            {
                int op = line.GetOperation();
                var color = GetColorNumber(line.GetStrokeColor()); 
                float linewidth = line.GetLineWidth();
                var path = line.GetPath();
                var subpaths = path.GetSubpaths();
                int count = subpaths.Count;

                // check for stroke 
                if (op != 1)
                    return;

                if (color < 2)
                    return;

                // check the identity matrix (CTM)
                var ctm = line.GetCtm();
                
                var sign_x = ctm.Get(0);
                var rot_x = ctm.Get(1);
                var zero_x = ctm.Get(2);
                var rot_y = ctm.Get(3);
                var sign_y = ctm.Get(4);
                var zero_y = ctm.Get(5);

                if (zero_x != 0 || zero_y != 0 || rot_x != 0 || rot_y != 0)
                    throw new InvalidOperationException();

                if (sign_x != 1 || sign_y != 1)
                    throw new InvalidOperationException();
                
                foreach(var subp in subpaths)
                {
                    if (subp.IsEmpty() || subp.IsSinglePointOpen())
                        continue;

                    if (!subp.IsClosed())
                        PdfReaderException.AlwaysThrow("!subp.IsClosed()");

                    var segs = subp.GetSegments()
                                    .SelectMany(s => s.GetBasePoints())
                                    .ToArray();

                    float x1 = (float)segs.Min(s => s.x);
                    float x2 = (float)segs.Max(s => s.x);
                    float y1 = (float)segs.Min(s => s.y);
                    float y2 = (float)segs.Max(s => s.y);
                    
                    float translate_x = ctm.Get(6);
                    float translate_y = ctm.Get(7);

                    var mark = new MarkLine()
                    {
                        X = x1 + translate_x,
                        H = y1 + translate_y,
                        Width = x2 - x1,
                        Height = y2 - y1,
                        LineWidth = linewidth,
                        Color = color
                    };

                    if ( color < 2 )
                        PdfReaderException.AlwaysThrow("Invalid color");

                    if (mark.X < 0 || mark.H < 0)
                        PdfReaderException.AlwaysThrow("mark.X < 0 || mark.H < 0");

                    _blockSet.Add(mark);
                }                
            }
        }

        public BlockPage GetResults()
        {
            var page = new BlockPage();

            page.AddRange(_blockSet);

            return page;
        }

        public ICollection<EventType> GetSupportedEvents()
        {
            return _supportedEvents;
        }        
    }
}
