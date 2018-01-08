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
            if( _title == true )
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
            int idxTitle = _structures.FindIndex(l => l.TextAlignment != TextAlignment.CENTER) - 1;
            int idxSigna = _structures.FindLastIndex(l => l.TextAlignment != TextAlignment.RIGHT) + 1;
            int idxEmenta = 0; 

            if (idxTitle < 0)
                return null;
            //    idxTitle = 0;

            if (idxSigna >= _structures.Count || idxSigna == 0)
                return null;
            //    idxSigna = 

            string title = _structures[idxTitle].Text;

            string body = String.Join("\n", _structures.Skip(idxTitle + 1).Take(idxSigna-idxTitle-1).Select( t => t.Text ));

            string signa = String.Join("\n", _structures.Skip(idxSigna).Select(t => t.Text));

            // ementa
            string ementa = null;

            idxEmenta = _structures.FindIndex(idxTitle + 1, l => l.TextAlignment != TextAlignment.RIGHT);

            if (idxEmenta > 0)
            {
                ementa = String.Join("\n", _structures.Skip(idxTitle + 1).Take(idxEmenta - idxTitle - 1).Select(t => t.Text));
                body = String.Join("\n", _structures.Skip(idxEmenta).Take(idxSigna - idxEmenta).Select(t => t.Text));
            }


            return new Artigo()
            {
                Titulo = title,
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
