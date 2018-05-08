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
        bool _hasFullAccess;

        public AccessManager(IVirtualFS virtualFS, bool hasFullAccess)
        {
            _virtualFS = virtualFS;
            _hasFullAccess = hasFullAccess;
        }

        public IVirtualFS GetReadOnlyFileSystem()
        {
            return _virtualFS;
        }

        public IVirtualFS GetFullAccessFileSystem()
        {
            if (!_hasFullAccess)
                throw new InvalidOperationException("no full access");

            return _virtualFS;
        }
    }
}
