using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Base
{
    enum TaggedSegmentEnum
    {
        None, Hierarquia, Titulo, Subtitulo, Ementa, Assinatura, Cargo, Data, Image, Table
    };

    class TextTaggedSegment
    {
        public TextTaggedStructure[] Title { get; set; }
        public TextTaggedStructure[] Body { get; set; }
        public TextSegment OriginalSegment { get; set; }
    }

    class TextTaggedStructure
    {
        public TextStructure TextStructure;
        public TaggedSegmentEnum Tag;
        public TextAlignment TextAlignment;
    }
}
