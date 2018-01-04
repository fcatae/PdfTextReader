using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Parser
{
    public class Conteudo : Structure.TextStructure
    {
        public TipoDoConteudo ContentType { get; set; }

        public Conteudo() { }

        public Conteudo(Structure.TextStructure structure, TipoDoConteudo type)
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
