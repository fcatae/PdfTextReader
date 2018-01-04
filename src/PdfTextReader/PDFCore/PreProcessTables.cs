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

        public void EventOccurred(IEventData data, EventType type)
        {
            var line = data as PathRenderInfo;

            if (line != null)
            {
                var bgcolor = line.GetFillColor();
                var fgcolor = line.GetStrokeColor();
                int op = line.GetOperation();
                float width = line.GetLineWidth();
                var path = line.GetPath();
                var subpaths = path.GetSubpaths();
                int count = subpaths.Count;
                var ctm = line.GetCtm();
                var dx = ctm.Get(6);
                var dy = ctm.Get(7);

                var segs = subpaths
                                .SelectMany(p => p.GetSegments())
                                .SelectMany(s => s.GetBasePoints())
                                .ToArray();

                foreach (var sub in subpaths)
                {
                    var segments = sub.GetSegments();

                    foreach (var s in segments)
                    {
                        var ps = s.GetBasePoints();
                        var ps_count = ps.Count;

                        var ps2 = ps.Select(p => p.GetLocation()).ToArray();
                    }
                }

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
                    Height = y2 - y1
                };

                if (tableCell.Width < 0 || tableCell.Height < 0)
                    throw new InvalidOperationException();

                if( tableCell.Op == 1 )
                {
                    _blockSet.Add(tableCell);
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
