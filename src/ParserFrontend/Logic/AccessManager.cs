using PdfTextReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParserFrontend.Logic
{
    public class AccessManager
    {
        IVirtualFS2 _virtualFS;
        bool _hasFullAccess;

        public AccessManager(IVirtualFS2 virtualFS, bool hasFullAccess)
        {
            _virtualFS = virtualFS;
            _hasFullAccess = hasFullAccess;
        }

        public bool HasFullAccess => _hasFullAccess;

        public IVirtualFS2 GetReadOnlyFileSystem()
        {
            return _virtualFS;
        }

        public IVirtualFS2 GetFullAccessFileSystem()
        {
            if (!_hasFullAccess)
                throw new InvalidOperationException("no full access");

            return _virtualFS;
        }
    }
}
