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
    class PreProcessImages : IEventListener, IPipelineResults<BlockPage>
    {
        private PDFCore.ProcessImageData _processImageData;

        private readonly List<EventType> _supportedEvents = new List<EventType>() { EventType.RENDER_IMAGE };

        private BlockSet<IBlock> _blockSet = new BlockSet<IBlock>();

        // compatibility with ProcessImageData
        // temporarily we need this, until we remove all direct references
        // to PreProcessImages. In the future, we only reference
        // the class ProcessImageData
        public BlockPage Images
        {
            get
            {
                // compatibility with ProcessImageData
                if (_processImageData != null)
                    return _processImageData.Images;

                return _images;
            }
            set
            {
                // compatibility with ProcessImageData
                if (_processImageData != null)
                    _processImageData.Images = value;

                _images = value;
            }
        }

        private BlockPage _images = null;

        public void SetCompatibility(PDFCore.ProcessImageData processImageData)
        {
            _processImageData = processImageData;
        }

        public void RemoveImage(IBlock block)
        {
            if (Images == null)
                PdfReaderException.AlwaysThrow("Images == null");

            int before = Images.AllBlocks.Count();

            var allBlocksMinusOne = Images.AllBlocks.Except(new IBlock[] { block });

            Images = new BlockPage();
            Images.AddRange(allBlocksMinusOne);

            int after = Images.AllBlocks.Count();

            if (after == before)
                PdfReaderException.AlwaysThrow("after == before");
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
                    PdfReaderException.AlwaysThrow("ensure we have a simple transformation matrix");

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
                    PdfReaderException.AlwaysThrow("imageBlock.Width <= 0 || imageBlock.Height <= 0");

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
