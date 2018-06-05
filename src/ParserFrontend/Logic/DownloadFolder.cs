using ParserFrontend.Infra;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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

        Regex _quickFix = new Regex(@"pubDate=""(\d\d\d\d)_(\d\d)_(\d\d)");
        string QuickFix(string input)
        {
            return _quickFix.Replace(input, @"pubDate=""$3/$2/$1");
        }

        public Stream DownloadQuickFix(string path)
        {
            var filenames = _vfs.ListFolderContent(path);

            using (var zip = new ZipCompression())
            {
                foreach (var filename in filenames)
                {
                    // HACK 1: ignore article 0
                    if (filename.Contains("-artigo0-"))
                        continue;
                    
                    string basename = GetFilename(filename);
                    using (var file = _vfs.OpenReader(filename))
                    using (var txtFile = new StreamReader(file))
                    {
                        // HACK 2: convert date YYYY_MM_DD to DD/MM/YYYY
                        var text = txtFile.ReadToEnd();
                        var newtext = QuickFix(text);
                        using (var newstream = new MemoryStream())
                        {
                            using (var memwrite = new StreamWriter(newstream))
                            {
                                memwrite.Write(newtext);
                                memwrite.Flush();
                                newstream.Seek(0, SeekOrigin.Begin);
                                zip.Add(basename, newstream);
                            }
                        }
                    }
                }

                return zip.DownloadStream();
            }
        }

        public Stream Download2(string path)
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
