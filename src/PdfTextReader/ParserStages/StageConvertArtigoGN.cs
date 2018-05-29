using PdfTextReader.Base;
using PdfTextReader.Parser;
using PdfTextReader.TextStructures;
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

            var filename = _context.CreateGlobalInstance<InjectFilename>();
            filename.Filename = _context.Basename;

            var artigos = pipelineText
                            .ConvertText<CreateTaggedSegments, TextTaggedSegment>()
                            .ConvertText<TransformConteudo4, Conteudo>()
                            .ConvertText<TransformArtigo2, Artigo>()
                            .LogFiles<GenerateArtigoTmp>($"{_context.OutputFolder}/{_context.Basename}/artigos/{_context.Basename}-artigo{{0}}.xml")
                            .LogFiles<GenerateArtigoGN4>($"{_context.OutputFolder}/{_context.Basename}/artigosGN4/{_context.Basename}-artigo{{0}}.xml")                            
                            .ToList();

            _context.AddOutput("artigosGN", $"{_context.OutputFolder}/{_context.Basename}/artigos/{_context.Basename}-artigo{{0}}.xml");
            _context.AddOutput("artigosGN4", $"{_context.OutputFolder}/{_context.Basename}/artigosGN4/{_context.Basename}-artigo{{0}}.xml");
        }
    }
}
