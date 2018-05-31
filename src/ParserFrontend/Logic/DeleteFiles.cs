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
        
        public void DeleteOutput(string name)
        {
            if (name.Trim().Length < 3)
                throw new InvalidOperationException();

            _webFS.DeleteFolder("output/" + name);
        }

        public void DestroyAll(string name)
        {
            if (name.Trim().Length < 3)
                throw new InvalidOperationException();

            _webFS.Delete("input/" + name + ".pdf");
            _webFS.DeleteFolder("output/" + name);
        }
    }
}
