using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.ParserStages
{
    class StageConvertContent
    {
        private readonly string _input;
        private readonly string _output;
        private readonly StageContext _context;

        public StageConvertContent(StageContext context)
        {
            this._input = context.InputFolder;
            this._output = context.OutputFolder;
            this._context = context;
        }

        public void Process()
        {
        }
    }
}
