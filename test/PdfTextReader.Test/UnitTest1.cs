using PdfTextReader.Azure;
using System;
using System.IO;
using Xunit;

namespace PdfTextReader.Test
{
    public class UnitTest1
    {
        [Fact]
        public async void Test1()
        {
           
            var pdfFile = @"C:\Users\visouza\Repos\DOU-OCR\data\pdf\D141.pdf";
            int pdfPages = 48;
            var gs = @"C:\Program Files\gs\gs9.23\bin\gswin64.exe";
            var tempFolder = @"C:\temp\dou";

            var pdfInput = File.OpenRead(pdfFile);

            PdfImageConverter imageConverter = new PdfImageConverter(gs, tempFolder, "102.4");

            Stream[] pdfPageImageList = null;

            //The array of streams will respect the page number-1, page 1 equal index 0;
            imageConverter.GenerateImage(pdfInput, ref pdfPageImageList);

            Assert.Equal(pdfPages, pdfPageImageList.Length);
        }
    }
}
