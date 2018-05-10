using PdfTextReader.Execution;
using PdfTextReader.PDFCore;
using PdfTextReader.PDFText;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PdfTextReader.ParserStages
{
    class StageRetrieveBlocks
    {
        private readonly string _input;
        private readonly string _output;
        private readonly StageContext _context;

        public StageRetrieveBlocks(StageContext context)
        {
            this._input = context.InputFolder;
            this._output = context.OutputFolder;
            this._context = context;
        }

        public void Process()
        {
            string basename = _context.Basename;
            Pipeline pipeline = _context.GetPipeline();

            pipeline.Input($"{_context.InputFilePrefix}.pdf")
                    .Output($"{_context.OutputFilePrefix}-stage3-retrieve.pdf")
                    .Global<BasicFirstPageStats>()
                    .StageProcess(RetrieveBlocks);

            _context.AddOutput("stage3", $"{_context.OutputFilePrefix}-stage3-retrieve.pdf");
        }

        void RetrieveBlocks(PipelineInputPdf.PipelineInputPdfPage page)
        {
            page
                .FromCache<IdentifyTablesData>()
                    .ParseBlock<SetIdentifyTablesCompatibility>()
                .FromCache<ProcessImageData>()
                    .ParseBlock<SetProcessImageCompatibility>()
                    .ParseBlock<RemoveOverlapedImages2>()      // 3
                .FromCache<HeaderFooterData>()
                .FromCache<BlocksetData>()
                    .Show(Color.Gray)
                .FromCache<ProcessPdfTextData>()
                
                    .ParseBlock<RemoveImageLineFromHeaderFooter>()
                    .ParseBlock<FilterHeaderFooter>()

                    //.ParseBlock<RemoveSmallFonts>()           // 5
                    .ParseBlock<FindDouIdMateria>()           // 5
                    .Validate<RemoveBlockHidden>().ShowErrors(p => p.Show(Color.Green))
                    //.ParseBlock<RemoveBlockHidden>()

                    // considera como parte da tabela? em principio sim..
                    .ParseBlock<MergeTableText>()             // 6

                    .ParseBlock<HighlightTextTable>()         // 7

                    // precisa gravar o texto dentro da tabela?
                    .ParseBlock<RemoveTableText>()            // 8

                    .ParseBlock<ReplaceCharacters>()          // 9
                    .ParseBlock<GroupLines>()                 // 10
                    .ParseBlock<DouIgnoreLongDotSequence>()     // fix
                        .Show(Color.Yellow)
                    .ParseBlock<RemoveTableDotChar>()         // 11

                    .ParseBlock<FindInitialBlocksetWithBlockInfo>()  // 13(b)

                    .ParseBlock<AddTableSpace>()              // 15
                    .ParseBlock<RemoveTableOverImage>()       // 16
                    .ParseBlock<RemoveImageTexts>()           // 17
                        .Show(Color.Red)
                    .ParseBlock<AddImageSpace>()              // 18
                    .ParseBlock<RemoveBackgroundNonText>()    // 21

                    // REPLACE 1: Merge text with text
                    // REPLACE 2: Break text with image/table

                    .ParseBlock<BreakInlineElements>()        // 23
                        //.Show(Color.Gray)

                    .ParseBlock<OrderBlocksetsWithBlockInfo>()
                    .ParseBlock<ResizeBlocksetsWithBlockInfo>()

                    .StoreCache<FinalBlockResultData>()

                        //.Show(Color.Orange)
                        .ShowLine(Color.Black);

            //.PrintWarnings();
        }
    }
}
