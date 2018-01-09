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
    public class ProcessImages : IEventListener, IPipelineResults<BlockPage>
    {
        private readonly List<EventType> _supportedEvents = new List<EventType>() { EventType.RENDER_IMAGE };

        private BlockSet<IBlock> _blockSet = new BlockSet<IBlock>();

        public void EventOccurred(IEventData data, EventType type)
        {
            var image = data as ImageRenderInfo;

            if (image != null)
            {
                var name = image.GetImageResourceName().ToString();
                var ctm = image.GetImageCtm();

                var x = ctm.Get(6);
                var h = ctm.Get(7);
                var width = ctm.Get(0);
                var height = ctm.Get(4);

                var imageBlock = new ImageBlock()
                {
                    X = x,
                    H = h,
                    Width = width,
                    Height = height,
                    ResourceName = name
                };
                                
                if (imageBlock.Width <= 0 || imageBlock.Height <= 0)
                    throw new InvalidOperationException();

                _blockSet.Add(imageBlock);
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
