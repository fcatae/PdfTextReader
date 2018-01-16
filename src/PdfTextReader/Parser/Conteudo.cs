using System;
using System.Collections.Generic;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.Parser
{
    class Conteudo
    {
        public string Hierarquia { get; set; }
        public string Titulo { get; set; }
        public string Corpo { get; set; }
        public string[] Assinatura { get; set; }
        public string Cargo { get; set; }
        public string Caput { get; set; }
        public string Grade { get; set; }
        public string Data { get; set; }
        public string Setor { get; set; }
        public string Departamento { get; set; }
        public override string ToString() => Titulo;
    }
}
