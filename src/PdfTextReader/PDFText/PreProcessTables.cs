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
    class PreProcessTables : IEventListener, IPipelineResults<BlockPage>
    {
        private readonly List<EventType> _supportedEvents = new List<EventType>() { EventType.RENDER_PATH };

        private BlockSet<TableCell> _blockSet = new BlockSet<TableCell>();

        float GetColor(Color color)
        {
            float[] components = color.GetColorValue();
            int size = components.Length;
            // 1=Gray, 3=RGB, 4=CMYK

            if (size == 1)
            {
                // 0=black, 1=white
                return components[0];
            }

            if(size == 3)
            {
                // RGB
                return (components[0] + components[1] + components[2]) / 3;
            }
                        
            if (size == 4)
            {
                // CMYK = Cyan Magenta Yellow blacK
                return (1 - components[3]);
            }

            throw PdfReaderException.AlwaysThrow("invalid color space");
        }

        public void EventOccurred(IEventData data, EventType type)
        {
            var line = data as PathRenderInfo;

            if (line != null)
            {
                int op = line.GetOperation();
                var bgcolor = ( op == 2 ) ? GetColor(line.GetFillColor()) : 0;
                var fgcolor = GetColor(line.GetStrokeColor()); 
                float linewidth = line.GetLineWidth();
                var path = line.GetPath();
                var subpaths = path.GetSubpaths();
                int count = subpaths.Count;
                var ctm = line.GetCtm();
                var dx = ctm.Get(6);
                var dy = ctm.Get(7);
                
                if (op == 0)
                    return;
                                
                var segs = subpaths
                                .SelectMany(p => p.GetSegments())
                                .SelectMany(s => s.GetBasePoints())
                                .ToArray();

                int segcount = segs.Length;
                
                float minerr = .5f;

                var sign_x = ctm.Get(0);
                var sign_y = ctm.Get(4);

                float x1 = (float)segs.Min(s => s.x) - minerr;
                float x2 = (float)segs.Max(s => s.x) + minerr;
                float y1 = (float)segs.Min(s => sign_y * s.y) - minerr;
                float y2 = (float)segs.Max(s => sign_y * s.y) + minerr;

                float translate_x = dx;
                float translate_y = dy;
                
                var tableCell = new TableCell()
                {
                    Op = op,
                    X = x1 + translate_x,
                    H = y1 + translate_y,
                    Width = x2 - x1,
                    Height = y2 - y1,
                    LineWidth = linewidth,
                    BgColor = (op == 1) ?  fgcolor : bgcolor
                };

                if (tableCell.Width < 0 || tableCell.Height < 0)
                    PdfReaderException.AlwaysThrow("tableCell.Width < 0 || tableCell.Height < 0");

                if (tableCell.X >= 0 || tableCell.H >= 0)
                {
                    if (tableCell.Op != 0)
                    {
                        _blockSet.Add(tableCell);
                    }
                }
                else
                {
                    // sometimes it draws a large rectangle to fill the background
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
