﻿using PdfTextReader.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.PDFText;
using PdfTextReader.Base;

namespace PdfTextReader.PDFCore
{
    class RemoveImageTexts : IProcessBlock
    {
        private List<IBlock> _images;

        public RemoveImageTexts(PreProcessImages parseImage)
        {
            var page = parseImage.Images;

            if (page == null)
            {
                PdfReaderException.AlwaysThrow("RemoveImageTexts requires PreProcessImages");
            }

            this._images = page.AllBlocks.ToList();
        }

        public BlockPage Process(BlockPage page)
        {
            if (this._images == null)
            {
                PdfReaderException.AlwaysThrow("RemoveImageTexts requires PreProcessImages");
            }

            var result = new BlockPage();

            foreach (var block in page.AllBlocks)
            {
                bool insideImage = false;

                foreach (var table in _images)
                {
                    if (Block.HasOverlap(table, block))
                    {
                        insideImage = true;
                        break;
                    }
                }

                if (!insideImage)
                {
                    result.Add(block);
                }
            }

            return result;
        }
    }
}
