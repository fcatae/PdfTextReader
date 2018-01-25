using PdfTextReader.TextStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.Parser
{
    class AggregateSingularBody : IAggregateStructure<Conteudo, Conteudo>
    {
        public bool Aggregate(Conteudo line)
        {
            if (line.Titulo.ToLower().Contains("seção") || line.Titulo.ToLower().Contains("capítulo"))
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
            if (conteudos.Count() > 1)
            {
                for (int i = 1; i < conteudos.Count; i++)
                {
                    //Verificando se na hierarquia entrou o título da lei (Capitulo)
                    var titleParts = conteudos[i].Hierarquia.Split(":");
                    foreach (string title in titleParts)
                    {
                        if (title.Contains("CAPÍTULO"))
                            conteudos[i].Titulo = title + "\n" + conteudos[i].Titulo;
                    }
                    newConteudo.Corpo = newConteudo.Corpo + "\n" + conteudos[i].Titulo + "\n" + conteudos[i].Corpo;
                }
            }
            return newConteudo;
        }

        public void Init(Conteudo line)
        {
        }
    }
}
