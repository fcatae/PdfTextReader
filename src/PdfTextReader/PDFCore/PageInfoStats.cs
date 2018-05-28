using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class PageInfoStats
    {
        public class HeaderInfo
        {
            public string ISSN;
            public string Local;
            public string DataDia;
            public string DataYMD;
            public string JornalAnoSupl;
            public string JornalEdicao;
        }

        public HeaderInfo Header { get; private set; }

        public void SetInfo(HeaderInfo headerInfo )
        {
            this.Header = headerInfo;
        }
    }
}
