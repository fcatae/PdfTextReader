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
    public class PreProcessImages : IEventListener, IPipelineResults<BlockPage>
    {
        private readonly List<EventType> _supportedEvents = new List<EventType>() { EventType.RENDER_IMAGE };

        private BlockSet<IBlock> _blockSet = new BlockSet<IBlock>();

        public BlockPage Images = null;

        public void RemoveImage(IBlock block)
        {
            if (Images == null)
                throw new InvalidOperationException();

            int before = Images.AllBlocks.Count();

            var allBlocksMinusOne = Images.AllBlocks.Except(new IBlock[] { block });

            Images = new BlockPage();
            Images.AddRange(allBlocksMinusOne);

            int after = Images.AllBlocks.Count();

            if (after == before)
                throw new InvalidOperationException();
        }

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

                // ensure we have a simple transformation matrix
                if ((ctm.Get(1) != 0) || (ctm.Get(2) != 0) || (ctm.Get(3) != 0)
                    || (ctm.Get(5) != 0) || (ctm.Get(8) != 1))
                    throw new InvalidOperationException();

                if ( height < 0 )
                {
                    h = h + height;
                    height = -height;
                }

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
    }
}
