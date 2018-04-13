using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PdfTextReader.Azure
{
    public class PdfImageConverter : IPdfConverter
    {
        private string _gsPath;
        private string _tempFolder;
        private string _ratio;

        public PdfImageConverter(string gstPath, string tempFolder, string ratio)
        {

            if (!File.Exists(gstPath)) throw new Exception($"Ghost Script not found! {gstPath}");
            if (!Directory.Exists(tempFolder)) throw new Exception($"Ghost Script not found! {gstPath}");

            _gsPath = gstPath;
            _tempFolder = tempFolder;

        }

        public void GenerateImage(Stream pdfInput, ref Stream[] imageListOutput)
        {
            if (!pdfInput.CanSeek) throw new Exception("PdfInput Stream can not be seek!");

            var rand = new Random(DateTime.Now.Second);

            int value = rand.Next();
            string tempPrefix = $"dou_pdf_temp_{value}";
            string pdfDirectory = $@"{_tempFolder}\{tempPrefix}";
            string pdfFileName = $"{tempPrefix}.pdf";
            
            var pdfFile = ToFile(pdfInput, pdfFileName);

            var images = ConvertAsync(pdfFile.FullName, "102.4").GetAwaiter().GetResult();


            if (images == null)
            {
                Console.WriteLine("Error generating the images!");
                return;
            }

            imageListOutput = new Stream[images.Length];           

            for (var i = 0; i < images.Length; i++)
            {
                var bytes = File.ReadAllBytes(images[i]);
                MemoryStream jpgMemory = new MemoryStream(bytes);

                //As the images are not in the proper order it is necessary to retrieve the page index.
                var parts = images[i].Replace(".jpg","").Split('_');
                int pageIdx = int.Parse(parts[parts.Length - 1]);

                imageListOutput[pageIdx-1] = jpgMemory;
                File.Delete(images[i]);
            }

            try
            {
                Directory.Delete($@"{pdfDirectory}");
            }
            catch(Exception ex) {
                Console.WriteLine($"Erro deleting directory {pdfDirectory} - {ex.Message}");
                throw new Exception(ex.Message, ex);
            }
        }

        private FileInfo ToFile(Stream stream, string fileName)
        {
            string pdfFile = $@"{_tempFolder}\{fileName}";

            using (var fileStream = File.Create(pdfFile))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fileStream);
                stream.Close();
            }

            return new FileInfo(pdfFile);

        }

        private async Task<string[]> ConvertAsync(string input, string ratio = "102.4")
        {

            var filename = new FileInfo(input).Name.Replace(".pdf", "");
            var outdir = new FileInfo(input).FullName.Replace(".pdf", "");
            Directory.CreateDirectory(outdir);

            string output = "", err = "";

            try
            {
                int exitCode = await Task.Run(() =>
                {
                    String args = $"-dNOPAUSE -sDEVICE=jpeg -r{ratio} -o\"{outdir}\\{filename}_%d.jpg\" \"{input}\"";
                    Process proc = new Process();
                    proc.StartInfo.FileName = _gsPath;
                    proc.StartInfo.Arguments = args;
                    //proc.StartInfo.CreateNoWindow = false;
                    //proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.RedirectStandardError = true;
                    proc.Start();
                    output = proc.StandardOutput.ReadToEnd();
                    err = proc.StandardError.ReadToEnd();

                    proc.Start();
                    proc.WaitForExit();
                    return proc.ExitCode;
                });

                if (exitCode == 0)
                {
                    File.Delete(input);
                    return Directory.GetFiles(outdir, "*.jpg");
                }
                else
                {
                    Console.WriteLine($"ExitCode: {exitCode} gs: {_gsPath} out: {output} err: {err} ");
                }
            }
            catch (Exception ex) {
                Console.WriteLine($"Error generating image from pdf: {ex.Message}");
                throw new Exception(ex.Message, ex);
            }
            return null;
        }
    }
}
