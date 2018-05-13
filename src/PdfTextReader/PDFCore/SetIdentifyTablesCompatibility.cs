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
            if (data.Ready == false)
            {
                if (pre.PageTables == null && pre.PageLines == null && pre.PageBackground == null)
                    PdfReaderException.AlwaysThrow("there is no data available");

                data.PageFooterLine = pre.PageFooterLine;
                data.PageTables = pre.PageTables;
                data.PageLines = pre.PageLines;
                data.PageBackground = pre.PageBackground;
                data.Ready = true;
            }

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
