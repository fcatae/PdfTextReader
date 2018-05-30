using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PdfTextReader;

namespace ParserFrontend.Logic
{
    public class OutputFiles
    {
        private readonly IVirtualFS _webFs;

        public OutputFiles(IVirtualFS virtualFS)
        {
            this._webFs = virtualFS;
        }

        public Dictionary<string, string> Load(string basename)
        {
            using (var stream = _webFs.OpenReader($"output/{basename}/filelist.json"))
            using (var file = new StreamReader(stream))
            {
                string content = file.ReadToEnd();
                var fileListInfo = JsonConvert.DeserializeObject<Dictionary<string,string>>(content);

                return fileListInfo;
            }
        }

        public void Save(string basename, Dictionary<string,string> fileListInfo)
        {
            using (var stream = _webFs.OpenWriter($"output/{basename}/filelist.json"))
            using (var file = new StreamWriter(stream))
            {
                string content = JsonConvert.SerializeObject(fileListInfo, Formatting.Indented);
                file.Write(content);
            }
        }

        string GetFilename(string basename, string file)
        {
            var fileList = Load(basename);

            if (!fileList.ContainsKey(file))
            {
                // plan B: if the fileList hasn't been updated yet
                // eg, because the filelist is old
                string staticFilename = GetStaticFilename(basename, file);

                if (staticFilename != null)
                    return staticFilename;

                throw new FileNotFoundException($"basename={basename}; file={file}");
            }

            return fileList[file];
        }

        string GetStaticFilename(string basename, string filename)
        {
            if( filename == "text-version" )
            {
                return $"output/{basename}/{basename}-{filename}.txt";
            }
            if( filename == "stage0-input" ||
                filename == "stage1-margins" ||
                filename == "stage2-blocksets" ||
                filename == "show-central" ||
                filename == "titles" 
                )
            {
                return $"output/{basename}/{basename}-{filename}.pdf";
            }
            return null;
        }

        Stream OpenReader(string basename, string file)
        {
            string filename = GetFilename(basename, file);
            
            return _webFs.OpenReader(filename);
        }

        Stream OpenReader(string basename, string file, string arg1)
        {
            string filename_format = GetFilename(basename, file);
            string filename = String.Format(filename_format, arg1);

            return _webFs.OpenReader(filename);
        }


        public Stream GetLogFile(string basename, string logfile)
        {
            return OpenReader(basename, logfile);
        }

        public string GetLogFileString(string basename, string logfile)
        {
            using(var stream = OpenReader(basename, logfile))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }                
        }

        public Stream GetOutputFile(string basename)
        {
            return OpenReader(basename, "stage3");
        }

        public string GetOutputTree(string basename)
        {
            using (var stream = OpenReader(basename, "tree"))
            using (var file = new StreamReader(stream))
            {
                string content = file.ReadToEnd();
                return content;
            }
        }

        public string GetOutputArtigo(string basename, int idArtigo)
        {
            using (var stream = OpenReader(basename, "artigosGN", idArtigo.ToString()))
            using (var file = new StreamReader(stream))
            {
                string content = file.ReadToEnd();
                return content;
            }
        }
        
        public bool ExistsArtigo(string basename, int idArtigo)
        {
            bool exists = false;
            try
            {
                using (var stream = OpenReader(basename, "artigosGN", idArtigo.ToString()))
                {
                    exists = true;
                }
            }
            catch { }

            return exists;
        }
    }
}
