using PdfTextReader.Base;
using PdfTextReader.PDFText;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class SetProcessImageCompatibility : IProcessBlock
    {
        private readonly PreProcessImages _pre;
        private readonly ProcessImageData _data;

        public SetProcessImageCompatibility(PreProcessImages pre, ProcessImageData data)
        {
            this._pre = pre;
            this._data = data;
        }

        public void SetCompatibility(PreProcessImages pre, ProcessImageData data)
        {
            if (data.Images == null)
                PdfReaderException.AlwaysThrow("Null image");

            // set the compatibility between PreProcessImages and ProcessImageData
            pre.SetCompatibility(data);
        }

        public BlockPage Process(BlockPage page)
        {
            SetCompatibility(_pre, _data);

            // do nothing
            return page;
        }
    }
}
