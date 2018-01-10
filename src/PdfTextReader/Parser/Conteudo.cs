using System;
using System.Collections.Generic;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.Parser
{
    class Conteudo : TextStructure
    {
        public TipoDoConteudo ContentType { get; set; }

        public Conteudo() { }

        public Conteudo(TextStructure structure, TipoDoConteudo type)
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
