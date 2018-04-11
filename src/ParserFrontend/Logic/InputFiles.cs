using PdfTextReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParserFrontend.Logic
{
    public class InputFiles
    {
        private readonly IVirtualFS _virtualFS;

        public InputFiles(IVirtualFS virtualFS)
        {
            this._virtualFS = virtualFS;
        }

    }
}
