using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Base
{
    class StatsExceptionHandled
    {
        private readonly int _pageNumber;
        private readonly Exception _exception;

        public string Error => _exception.ToString();
        public int PageNumber => _pageNumber;

        public StatsExceptionHandled(int pageNumber, Exception ex)
        {
            this._pageNumber = pageNumber;
            this._exception = ex;
        }
    }
}
