using PdfTextReader.TextStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.Parser
{
    class TransformArtigo : IAggregateStructure<TextStructure, Artigo>
    {
        bool _title = true;

        public bool Aggregate(TextStructure line)
        {
            if (_title == true)
            {
                if (line.TextAlignment != TextAlignment.CENTER)
                {
                    _title = false;
                }

                return true;
            }

            bool newTitle = (line.TextAlignment == TextAlignment.CENTER);                

            return (newTitle == false);
        }

        public Artigo Create(List<TextStructure> _structures)
        {
            string body = null;
            string signa = null;
            string ementa = null;
            string caput = null;
            List<string> resultProcess = new List<string>() { null, null, null };

            int idxTitle = _structures.FindIndex(l => l.TextAlignment != TextAlignment.CENTER) - 1;
            int idxSigna = _structures.FindLastIndex(l => l.TextAlignment != TextAlignment.RIGHT) + 1;

            if (idxTitle < 0)
                return null; // throw new InvalidOperationException();

            int idxEmenta = _structures.FindIndex(idxTitle + 1, l => l.TextAlignment != TextAlignment.RIGHT);

            string titulo = _structures[idxTitle].Text;

            if (idxSigna > 0 && idxSigna < _structures.Count )
                signa = String.Join("\n", _structures.Skip(idxSigna).Select(t => t.Text));

            body = String.Join("\n", _structures.Skip(idxTitle + 1).Take(idxSigna-idxTitle-1).Select( t => t.Text ));
            
            if (idxEmenta > 0)
            {
                ementa = String.Join("\n", _structures.Skip(idxTitle + 1).Take(idxEmenta - idxTitle - 1).Select(t => t.Text));
                body = String.Join("\n", _structures.Skip(idxEmenta).Take(idxSigna - idxEmenta).Select(t => t.Text));
            }

            if (_structures[idxTitle + 1].TextAlignment == TextAlignment.RIGHT && _structures[idxTitle + 2].TextAlignment ==  TextAlignment.JUSTIFY)
                caput = _structures[idxTitle + 1].Text;

            if (idxSigna > 0 && idxSigna < _structures.Count)
            {
                resultProcess.Clear();
                resultProcess = processSignatureAndRole(_structures[idxSigna]);
            }

            return new Artigo()
            {
                Titulo = titulo,
                Caput = caput,
                Corpo = body,
                Assinatura = resultProcess[0],
                Cargo = resultProcess[1],
                Data = resultProcess[2]
            };
        }

        List<string> processSignatureAndRole(TextStructure structure)
        {
            string[] data = structure.Text.Split("\n");


            string signature = null;
            string role = null;
            string date = null;


            foreach (string item in data)
            {
                if (item.ToUpper() == item)
                {
                    signature = signature + "\n" + item;
                }
                else if (item.All(Char.IsDigit))
                {
                    date = item;
                }
                else
                {
                    role = role + "\n" + item;
                }
            }

            return new List<string>() {signature, role, date };
        }

        public void Init(TextStructure line)
        {
            _title = (line.TextAlignment == TextAlignment.CENTER);
        }
    }
}
