using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Parser
{
    class ProcessParser
    {
        decimal Tolerance = 3;
        public IEnumerable<Conteudo> ProcessStructures(IEnumerable<Structure.TextStructure> structures)
        {
            List<Conteudo> contents = new List<Conteudo>();
            foreach (Structure.TextStructure structure in structures)
            {
                if (structure.CountLines() == 1 && structure.TextAlignment == Structure.TextAlignment.RIGHT && structure.MarginRight > Tolerance && structure.Text.ToUpper() == structure.Text)
                {
                    contents.Add(new Conteudo(structure, TipoDoConteudo.Assinatura));
                }
                else if (structure.CountLines() == 1 && structure.TextAlignment == Structure.TextAlignment.RIGHT && structure.MarginRight > Tolerance)
                {
                    contents.Add(new Conteudo(structure, TipoDoConteudo.Cargo));
                }
                else if (structure.CountLines() > 1 && structure.TextAlignment == Structure.TextAlignment.JUSTIFY)
                {
                    contents.Add(new Conteudo(structure, TipoDoConteudo.Corpo));
                }
                else if (structure.CountLines() == 1 && structure.TextAlignment == Structure.TextAlignment.RIGHT && structure.MarginRight < Tolerance)
                {
                    contents.Add(new Conteudo(structure, TipoDoConteudo.Caput));
                }
                else if (structure.TextAlignment == Structure.TextAlignment.CENTER && structure.FontStyle == "Bold")
                {
                    if (Stats.ProcessStats.GetGridStyle() != null && structure.FontName == Stats.ProcessStats.GetGridStyle().FontName)
                    {
                        contents.Add(new Conteudo(structure, TipoDoConteudo.Grade));
                    }
                    else if (structure.FontSize > 9) // Preciso pegar do Stats
                    {
                        contents.Add(new Conteudo(structure, TipoDoConteudo.Seção));
                    }
                    else
                    {
                        contents.Add(new Conteudo(structure, TipoDoConteudo.Título));
                    }
                }
                else if (structure.TextAlignment == Structure.TextAlignment.CENTER && structure.Text.ToUpper() != structure.Text)
                {
                    contents.Add(new Conteudo(structure, TipoDoConteudo.Data));
                }
                else if (structure.TextAlignment == Structure.TextAlignment.CENTER)
                {
                    contents.Add(new Conteudo(structure, TipoDoConteudo.Departamento));
                }
            }
            return contents;
        }
    }
}
