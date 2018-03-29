using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PdfTextReader.Azure
{
    public interface IPdfConverter
    {
        void GenerateImage(Stream pdfInput, ref Stream[] imageListOutput);

    }
}
