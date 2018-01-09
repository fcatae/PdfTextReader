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
    public class PreProcessImages : IEventListener, IPipelineResults<BlockPage>, IProcessBlock
    {
        private readonly List<EventType> _supportedEvents = new List<EventType>() { EventType.RENDER_IMAGE };

        private BlockSet<IBlock> _blockSet = new BlockSet<IBlock>();

        public BlockPage Images = null;

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

            this.Images = page;

            return page;
        }

        public ICollection<EventType> GetSupportedEvents()
        {
            return _supportedEvents;
        }

        public BlockPage Process(BlockPage page)
        {
            // we need this just because of the IProcessBlock requirement
            // otherwise, we can't use a pipeline factory
            throw new NotImplementedException();
        }
    }
}
