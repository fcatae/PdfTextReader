using Microsoft.Extensions.Options;
using ParserFrontend.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ParserFrontend.Logic
{
    public class CopyFiles
    {
        private readonly IVirtualFS2 _webFS;
        private readonly IVirtualFS2 _sourceFS;

        public CopyFiles(IVirtualFS2 webFS, IOptions<CopyFilesConfig> config)
        {
            string sourceUrl = config.Value.StorageAccount;

            if (String.IsNullOrEmpty(sourceUrl))
                throw new InvalidOperationException(nameof(sourceUrl));

            this._webFS = webFS;
            this._sourceFS = new AzureFS(sourceUrl, "pdf");
        }

        public void EnsureFile(string filename, string basename)
        {
            try
            {
                using (var file = _webFS.OpenReader(filename))
                {
                    // file exists!
                }
            }
            catch(Exception ex)
            {
                // not found
                // copy the file
                StartCopyBasename(filename, basename);
            }
        }

        void StartCopyBasename(string filename, string basename)
        {
            string folder = FindFolder(basename);
            string path = $"2017/{folder}/{basename}.pdf";

            using (var srcFile = _sourceFS.OpenReader(path))
            {
                using (var dstFile = _webFS.OpenWriter(filename))
                {
                    srcFile.CopyTo(dstFile);
                }
            }
        }

        string FindFolder(string basename)
        {
            if(basename.StartsWith("DO1_"))
            {
                return basename.Substring(4);
            }

            throw new NotImplementedException();
        }
    }
}
