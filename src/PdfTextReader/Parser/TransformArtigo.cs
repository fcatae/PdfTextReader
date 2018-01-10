using PdfTextReader.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.Parser
{
    class TransformArtigo : ITransformStructure<TextStructure, Artigo>
    {
        List<TextStructure> _structures;
        bool _title = true;

        public bool Aggregate(TextStructure line)
        {
            if ( _title == true )
            {
                if (line.TextAlignment == TextAlignment.CENTER)
                {
                }
                else
                {
                    _title = false;
                }

                _structures.Add(line);
                return true;
            }

            if (line.TextAlignment == TextAlignment.CENTER)
                return false;

            _structures.Add(line);
            return true;
        }

        public Artigo Create()
        {
            string body = null;
            string signa = null;
            string ementa = null;

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

            return new Artigo()
            {
                Titulo = titulo,
                Caput = ementa,
                Corpo = body,
                Assinatura = signa
            };
        }

        public void Init(TextStructure line)
        {
            _structures = new List<TextStructure>();
            _structures.Add(line);
            _title = (line.TextAlignment == TextAlignment.CENTER);
        }
    }
}
