using PdfTextReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParserFrontend.Logic
{
    public class DeleteFiles
    {
        private readonly IVirtualFS2 _webFS;

        public DeleteFiles(AccessManager amgr)
        {
            this._webFS = amgr.GetFullAccessFileSystem(); 
        }
        
        public void Delete(string name)
        {
            _webFS.Delete("input/" + name);
        }
    }
}
