using PdfTextReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParserFrontend.Logic
{
    public class AccessManager
    {
        IVirtualFS _virtualFS;

        public AccessManager(IVirtualFS virtualFS)
        {
            _virtualFS = virtualFS;
        }

        public IVirtualFS GetReadOnlyFileSystem()
        {
            return _virtualFS;
        }

        public IVirtualFS GetFullAccessFileSystem()
        {
            return _virtualFS;
        }
    }
}
