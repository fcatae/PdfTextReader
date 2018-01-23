using PdfTextReader.TextStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.Parser
{
    class AggregateAnexo : IAggregateStructure<Conteudo, Conteudo>
    {
        public bool Aggregate(Conteudo line)
        {
            if (line.Titulo.ToLower().Contains("anexo"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Conteudo Create(List<Conteudo> conteudos)
        {
            Conteudo newConteudo = conteudos[0];
            newConteudo.Anexos = new List<Anexo>();
            if (conteudos.Count() > 1)
            {
                for (int i = 1; i < conteudos.Count; i++)
                {
                    Anexo a = new Anexo()
                    {
                        Titulo = conteudos[i].Titulo,
                        Texto = conteudos[i].Corpo
                    };
                    newConteudo.Anexos.Add(a);
                }

            }

            return newConteudo;

        }

        public void Init(Conteudo line)
        {
        }
    }
}
