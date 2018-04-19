using PdfTextReader.Base;
using PdfTextReader.Parser;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.ParserStages
{
    class StageConvertArtigoGN
    {
        private readonly string _input;
        private readonly string _output;
        private readonly StageContext _context;

        public StageConvertArtigoGN(StageContext context)
        {
            this._input = context.InputFolder;
            this._output = context.OutputFolder;
            this._context = context;
        }

        public void Process()
        {
            var pipelineText = _context.GetPipelineText<TextSegment>();

            var conteudo = pipelineText
                            .ConvertText<TransformConteudo, Conteudo>()
                            .ToList();

            var createArticle = new TransformArtigo();
            var artigos = createArticle.Create(conteudo);
            createArticle.CreateXML(artigos, $"{_context.OutputFolder}/{_context.Basename}/artigos", _context.Basename);

            _context.AddOutput("artigosGN", $"{_context.OutputFolder}/{_context.Basename}/artigos/{_context.Basename}-artigo{{0}}.xml");
        }
    }
}
