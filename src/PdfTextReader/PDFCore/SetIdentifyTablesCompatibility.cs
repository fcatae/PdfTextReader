using PdfTextReader.Base;
using PdfTextReader.PDFText;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class SetIdentifyTablesCompatibility : IProcessBlock
    {
        private readonly IdentifyTables _pre;
        private readonly IdentifyTablesData _data;

        public SetIdentifyTablesCompatibility(IdentifyTables pre, IdentifyTablesData data)
        {
            this._pre = pre;
            this._data = data;
        }

        public void SetCompatibility(IdentifyTables pre, IdentifyTablesData data)
        {
            if (data.PageTables == null || data.PageLines == null || data.PageBackground == null )
                PdfReaderException.AlwaysThrow("Null table properties");

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
