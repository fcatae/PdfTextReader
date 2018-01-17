using PdfTextReader.TextStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;
using System.Text.RegularExpressions;

namespace PdfTextReader.Parser
{
    class TransformConteudo : IAggregateStructure<TextSegment, Conteudo>
    {
        public bool Aggregate(TextSegment line)
        {
            return false;
        }

        public Conteudo Create(List<TextSegment> segments)
        {
            TextSegment segment = segments[0];

            string titulo = null;
            string hierarchy = null;
            string body = null;
            string caput = null;
            List<string> resultProcess = new List<string>() { null, null, null };


            //Definindo Titulo e hierarquia
            int idxTitle = segment.Title.Count() - 1;

            if (idxTitle == 0)
            {
                titulo = segment.Title[0].Text;
            }
            else if (idxTitle > 0)
            {
                for (int i = 0; i < segment.Title.Count() - 1; i++)
                {
                    hierarchy = hierarchy + segment.Title[i].Text + ":";
                }
                titulo = segment.Title[idxTitle].Text;
            }

            //Definindo Caput
            if (segment.Body[0].TextAlignment == TextAlignment.RIGHT && segment.Body[1].TextAlignment == TextAlignment.JUSTIFY)
                caput = segment.Body[0].Text;
        
            //Definindo Assinatura, Cargo e Data
            int idxSigna = segment.Body.ToList().FindLastIndex(s => s.TextAlignment == TextAlignment.JUSTIFY) + 1;
            
            if (idxSigna > 0 && idxSigna < segment.Body.Count())
            {
                resultProcess.Clear();
                resultProcess = ProcessSignatureAndRole(segment.Body[idxSigna].Lines);
            }


            //Definindo Body
            if (caput != null)
            {
                body = String.Join("\n", segment.Body.Take(idxSigna - 1).Select(s => s.Text));
            }
            

            return new Conteudo()
            {
                IntenalId = 0,
                Hierarquia = hierarchy,
                Titulo = titulo,
                Caput = caput,
                Corpo = body,
                Assinatura = ProcessListOfSignatures(resultProcess[0]),
                Cargo = resultProcess[1],
                Data = resultProcess[2]
            };
        }


        List<string> ProcessSignatureAndRole(List<TextLine> lines)
        {

            string signature = null;
            string role = null;
            string date = null;


            foreach (var item in lines)
            {
                if (item.Text.ToUpper() == item.Text)
                {
                    signature = signature + "\n" + item.Text;
                }
                else if (item.FontStyle == "Italic")
                {
                    signature = signature + "\n" + item.Text;
                }
                else if (item.Text.All(Char.IsDigit))
                {
                    date = item.Text;
                }
                else
                {
                    role = role + "\n" + item.Text;
                }
            }

            return new List<string>() { signature, role, date };
        }

        string[] ProcessListOfSignatures(string signature)
        {
            if (signature != null)
            {
                if (signature.Contains("\n"))
                {
                    return signature.Split("\n");
                }
                else
                {
                    return new string[] { signature };
                }
            }
            return null;
        }

        public void Init(TextSegment line)
        {
           
        }
    }
}
