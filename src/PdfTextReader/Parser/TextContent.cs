using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Parser
{
    public class TextContent : Structure.TextStructure
    {
        public ContentType ContentType { get; set; }

        public TextContent() { }

        public TextContent(Structure.TextStructure structure, ContentType type)
        {
            this.FontName = structure.FontName;
            this.FontSize = structure.FontSize;
            this.FontStyle = structure.FontStyle;
            this.Text = structure.Text;
            this.TextAlignment = structure.TextAlignment;
            this.ContentType = type;
        }
    }
}
