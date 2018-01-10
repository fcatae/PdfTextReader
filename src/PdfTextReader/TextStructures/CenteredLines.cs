//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using PdfTextReader.Base;

//namespace PdfTextReader.TextStructures
//{
//    class CenteredLines : ITransformStructure<TextLine, TextStructure>
//    {
//        TextStructure _structure;

//        public void Init(TextLine line)
//        {
//            _structure = new TextStructure()
//            {
//                FontName = line.FontName,
//                FontStyle = line.FontStyle,
//                FontSize = line.FontSize,
//                VSpacing = line.Breakline,
//                Lines = new List<TextLine>()
//            };

//            _structure.Lines.Add(line);
//        }

//        public bool Aggregate(TextLine line)
//        {
//            if ((_structure.FontName != line.FontName) ||
//                (_structure.FontStyle != line.FontStyle) ||
//                (_structure.FontSize != line.FontSize))
//                return false;
            
//            if (_structure.VSpacing == null)
//                return false;

//            if (line.VSpacing > line.FontSize / 2)
//                return false;

//            if (_structure.VSpacing != line.VSpacing)
//                return false;

//            _structure.Lines.Add(line);

//            return true;
//        }

//        public TextStructure Create()
//        {
//            var lineset = _structure.Lines;

//            if (lineset.All(t => IsZero(t.MarginLeft - t.MarginRight))
//                    && lineset.Any(t => !IsZero(t.MarginLeft)))
//            {
//                var textArray = lineset.Select(t => t.Text);
//                _structure.Text = String.Join("\n", textArray);

//                return _structure;
//            }

//            return null;
//        }

//        IEnumerable<TextStructure> Transform(IEnumerable<TextLine> lines)
//        {
//            bool active = false;

//            foreach(var line in lines)
//            {
//                if( !active )
//                {
//                    Init(line);
//                    active = true;
//                }
//                else
//                {
//                    bool agg = Aggregate(line);

//                    if ( !agg )
//                    {
//                        yield return Create();

//                        active = false;
//                    }
//                }
//            }
//        }
        
//        bool IsZero(decimal value)
//        {
//            decimal error = 0.1M;

//            return ((value > -error) && (value < error));                
//        }

//        bool IsAlmostZero(decimal value)
//        {
//            decimal error = 4M;

//            return ((value > -error) && (value < error));
//        }

//        bool IsUpperCase(string text)
//        {
//            var upper = text.ToUpper();
//            return (upper == text);
//        }

//    }
//}
