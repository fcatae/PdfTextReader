using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System;
using System.Collections.Generic;
using System.Text;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using System.Linq;

namespace PdfTextReader
{
    class UserListenerExtra : IEventListener
    {
        private readonly List<EventType> _supportedEvents = new List<EventType>() {
            EventType.CLIP_PATH_CHANGED,
            EventType.RENDER_PATH,
            EventType.BEGIN_TEXT
            //EventType.RENDER_IMAGE
        };
        private readonly Action<Block> _callback;

        public UserListenerExtra(Action<Block> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            this._callback = callback;
        }

        public void EventOccurred(IEventData data, EventType type)
        {
            var clip = data as ClippingPathInfo;
            var line = data as PathRenderInfo;

            if( clip != null)
            {
                var path = clip.GetClippingPath();
                var ps = path.GetSubpaths().SelectMany(p => p.GetSegments())
                    .SelectMany(s => s.GetBasePoints())
                    .ToArray();
                var ctm = clip.GetCtm();
                var gs = clip.GetGraphicsState();
            }
            if( line != null )
            {
                var bgcolor = line.GetFillColor();
                var fgcolor = line.GetStrokeColor();
                int st_op = PathRenderInfo.STROKE;
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

                foreach(var sub in subpaths)
                {
                    var segments = sub.GetSegments();

                    foreach(var s in segments)
                    {
                        var ps = s.GetBasePoints();
                        var ps_count = ps.Count;

                        var ps2 = ps.Select(p => p.GetLocation()).ToArray();
                    }
                }

                
            }

            var textInfo = data as TextRenderInfo;

            if ( textInfo != null )
            {
                var baseline = textInfo.GetBaseline().GetStartPoint();
                var descent = textInfo.GetDescentLine().GetStartPoint();
                var ascent = textInfo.GetAscentLine().GetEndPoint();

                var font = textInfo.GetFont().GetFontProgram();

                var block = new Block()
                {
                    Text = textInfo.GetText(),
                    X = descent.Get(0),
                    H = descent.Get(1),
                    B = baseline.Get(1),
                    Width = ascent.Get(0) - descent.Get(0),
                    Height = ascent.Get(1) - descent.Get(1),
                    Lower = baseline.Get(1) - descent.Get(1),
                    FontName = font.ToString(),
                    FontSize = textInfo.GetFontSize()
                };

                string text = textInfo.GetText();

                _callback(block);
            }
        }

        public ICollection<EventType> GetSupportedEvents()
        {
            return _supportedEvents;
        }
    }
}
