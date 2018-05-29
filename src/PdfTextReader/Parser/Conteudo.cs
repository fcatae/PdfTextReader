using System;
using System.Collections.Generic;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.Parser
{
    class Conteudo
    {
        //For internal use
        public int IntenalId { get; set; }
        public int Page { get; set; }
        public string PID { get; set; }

        public string Hierarquia { get; set; }
        public string Titulo { get; set; }
        public string Corpo { get; set; }
        public List<Autor> Autor { get; set; }
        public string Caput { get; set; }
        public string Grade { get; set; }
        public string Data { get; set; }
        public string Setor { get; set; }
        public string Departamento { get; set; }

        public string[] HierarquiaTitulo { get; set; }
        public string Texto { get; set; }

        //Just for while
        public List<Anexo> Anexos { get; set; }

        public override string ToString() => Titulo;
    }
}
