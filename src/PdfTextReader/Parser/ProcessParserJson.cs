using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PdfTextReader.Base;
using Newtonsoft.Json;

namespace PdfTextReader.Parser
{
    class ProcessParserJson
    {
        public void Write(Artigo artigo, string doc)
        {
            // TODO: fix it
            // Rollback to previous name
            //string finalURL = ProcessName(artigos.FirstOrDefault(), doc);
            string finalURL = doc;

            JsonSerializerSettings settings = new JsonSerializerSettings() { Formatting = Formatting.Indented };
            using (Stream virtualStream = VirtualFS.OpenWrite($"{finalURL}.json"))
            {
                string content = JsonConvert.SerializeObject(artigo, settings);

                using (var writer = new StreamWriter(virtualStream))
                {
                    writer.Write(content);
                }
            }
        }
        
        public void WriteJson(IEnumerable<Artigo> artigos, string doc)
        {
            int i = 1;
            foreach(var artigo in artigos)
            {
                string doc_i = doc + (i++);
                this.Write(artigo, doc_i);
            }
        }
    }
}
