using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Base
{
    interface IConfigurationStore
    {
        string Get(string filename);
    }
}
