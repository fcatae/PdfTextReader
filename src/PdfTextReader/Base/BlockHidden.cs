using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Base
{
    class BlockHidden : Block
    {
        public string GetHiddenText()
        {
            string id = Text.Replace("<!", "").Replace(">", "").Replace("ID","").Trim();

            return $"((IDMATERIA={id})) ";
        }
    }
}
