using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace PdfTextReader.PDFCore
{
    class HeaderFooterData : IProcessBlockData
    {
        public float HeaderH = float.NaN;
        public float FooterH = float.NaN;

        public BlockPage LastResult { get; private set; }

        public BlockPage Process(BlockPage page)
        {
            LastResult = page;
            return page;
        }

        public void UpdateInstance(object cache)
        {
            var instance = (HeaderFooterData)cache;

            this.HeaderH = instance.HeaderH;
            this.FooterH = instance.FooterH;
        }
    }
}
