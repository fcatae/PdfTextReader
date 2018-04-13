using PdfTextReader.Base;
using PdfTextReader.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class RemoveBackgroundNonText : IProcessBlock
    {
        const float MINIMUM_BACKGROUND_SIZE = 5f;

        private List<IBlock> _region;

        public RemoveBackgroundNonText(PDFCore.IdentifyTables parserTable)
        {
            var backgrounds = parserTable.PageBackground.AllBlocks;

            if (backgrounds == null)
            {
                PdfReaderException.AlwaysThrow("RemoveBackgroundNonText requires IdentifyTables");
            }

            var region = new List<IBlock>();
            region.AddRange(backgrounds);
            this._region = region;
        }

        public BlockPage Process(BlockPage page)
        {
            if(this._region == null)
            {
                PdfReaderException.AlwaysThrow("RemoveBackgroundNonText requires IdentifyTables");
            }

            var result = new BlockPage();

            foreach(var block in page.AllBlocks)
            {
                bool isTableOrImage = (block is TableSet) || (block is ImageBlock);
                bool isInsideBackground = false;
                
                if (isTableOrImage)
                {
                    foreach (var table in _region)
                    {
                        if (Block.Contains(table, block))
                        {
                            isInsideBackground = true;
                            break;
                        }
                    }

                    if (isInsideBackground)
                        continue;
                }

                result.Add(block);
            }

            return result;
        }        
    }
}
