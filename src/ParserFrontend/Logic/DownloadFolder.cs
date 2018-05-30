using ParserFrontend.Infra;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ParserFrontend.Logic
{
    public class DownloadFolder
    {
        readonly IVirtualFS2 _vfs;

        public DownloadFolder(IVirtualFS2 vfs)
        {
            this._vfs = vfs;
        }

        public Stream Download(string path)
        {
            var filenames = _vfs.ListFolderContent(path);

            using (var zip = new ZipCompression())
            {
                foreach(var filename in filenames)
                {
                    string basename = GetFilename(filename);
                    using (var file = _vfs.OpenReader(filename))
                    {
                        zip.Add(basename, file);
                    }                        
                }

                return zip.DownloadStream();
            }
        }

        string GetFilename(string filename)
        {
            string[] components = filename.Split('/');
            return components[components.Length - 1];
        }
    }
}
