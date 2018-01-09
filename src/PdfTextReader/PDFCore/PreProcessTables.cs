using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using PdfTextReader.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.PDFCore
{
    public class PreProcessTables : IEventListener, IPipelineResults<BlockPage>
    {
        private readonly List<EventType> _supportedEvents = new List<EventType>() { EventType.RENDER_PATH };

        private BlockSet<TableCell> _blockSet = new BlockSet<TableCell>();

        float GetColor(Color color)
        {
            float[] components = color.GetColorValue();
            int size = components.Length;

            // 1=Gray, 3=RGB, 4=CMYK
            if (size == 1 || size == 3 || size == 4)
            {
                // return Gray, Blue or BlacK
                return components[size - 1];
            }

            throw new InvalidOperationException("invalid color space");
        }

        public void EventOccurred(IEventData data, EventType type)
        {
            var line = data as PathRenderInfo;

            if (line != null)
            {
                var bgcolor = GetColor(line.GetFillColor());
                var fgcolor = GetColor(line.GetStrokeColor()); 
                int op = line.GetOperation();
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
                    BgColor = bgcolor
                };

                if (tableCell.Width < 0 || tableCell.Height < 0)
                    throw new InvalidOperationException();

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

        public BlockPage Process(BlockPage page)
        {
            throw new NotImplementedException();
        }
    }
}
