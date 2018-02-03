using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace ParserFunctions.Model
{
    public class PdfStats
    {
        public string JobName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int ElapsedTime { get; set; }
        public int Total { get; set; }
        public int Errors { get; set; }

        public List<File> Files { get; set; }

        public PdfStats(string jobname)
        {
            JobName = jobname;
            StartTime = DateTime.UtcNow;
            ElapsedTime = 0;
            Total = 0;
            Errors = 0;
            Files = new List<File>();
        }

        public File Create(Pdf pdf)
        {
            var file = new File()
            {
                Name = pdf.Name,
                Path = pdf.Path                
            };

            Files.Add(file);

            return file;
        }

        public void Done(File[] results)
        {
            if (results == null)
                throw new ArgumentNullException(nameof(results));

            EndTime = DateTime.UtcNow;
            ElapsedTime = (int)(EndTime - StartTime).TotalMilliseconds;

            Files = new List<File>(results);

            Total = Files.Count();
            Errors = Files.Count(f => f.Error != null);
        }

        public class File
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public DateTime StartTime { get; set; }
            public string Hostname { get; set; }
            public DateTime EndTime { get; set; }
            public int ElapsedTime { get; set; }
            public string Error { get; set; }

            public void Start()
            {
                StartTime = DateTime.UtcNow;
                Hostname = Environment.MachineName;
            }
            public void Done(string error = null)
            {
                EndTime = DateTime.UtcNow;
                ElapsedTime = (int)(EndTime - StartTime).TotalMilliseconds;
                Error = error;
            }
        }
    }
}
