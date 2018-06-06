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
        Regex _quickFix2 = new Regex(@"<Identifica>(\S+)");
        Regex _quickFixIdentifica = new Regex(@"<Identifica>(.*)</Identifica>");

        string QuickFix(string input)
        {
            return _quickFix.Replace(input, @"pubDate=""$3/$2/$1");
        }
        string QuickFix2(string input)
        {
            var identificaMatch = _quickFix2.Match(input);
            string identName = identificaMatch.Success ? identificaMatch.Groups[1].Value : "Ato";
            string artType = $"artType=\"{identName}\"";
            string artName = $"name=\"{identName}\"";

            return input.Replace("<Identifica>", "<Identifica><![CDATA[")
                        .Replace("</Identifica>", "]]></Identifica>")                        
                        .Replace("pubDate", $"{artName} {artType} pubDate");
        }

        public Stream DownloadQuickFix(string path, bool filtroTipoArtigo = false)
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
                        var text = txtFile.ReadToEnd();

                        // HACK 4: filtrar por tipo de artigo
                        if (filtroTipoArtigo && FiltrarTipoArtigo(text))
                            continue;

                        // HACK 2: convert date YYYY_MM_DD to DD/MM/YYYY
                        var newtext = QuickFix2( QuickFix(text) );

                        using (var newstream = new MemoryStream())
                        {
                            using (var memwrite = new StreamWriter(newstream))
                            {
                                memwrite.Write(newtext);
                                memwrite.Flush();
                                newstream.Seek(0, SeekOrigin.Begin);

                                // HACK3: remove "article" from name
                                string newbasename = basename
                                    .Replace("-artigo", "")
                                    .Replace("DO","")
                                    .Replace("_", "");

                                int idxP = newbasename.IndexOf("-p");
                                newbasename = newbasename.Substring(0, idxP);

                                // add XML extension
                                newbasename = newbasename + ".xml";

                                zip.Add(newbasename, newstream);
                            }
                        }
                    }
                }

                return zip.DownloadStream();
            }
        }

        bool FiltrarTipoArtigo(string text)
        {
            var regIdent = _quickFixIdentifica.Match(text);
            if (!regIdent.Success)
                return true;
            string input = regIdent.Groups[1].Value;

            if(input.Contains("No ") || input.Contains("N°") || input.Contains("Nº"))
            {
                return false;
            }

            return true;
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
