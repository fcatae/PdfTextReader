using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.PDFCore
{
    class RemoveFooter : IProcessBlock, IValidateBlock, IRetrieveStatistics
    {
        const float statRegionTooLarge = 200f;
        StatsPageFooter _stats = new StatsPageFooter();

        public BlockPage Process(BlockPage page)
        {
            if (page.AllBlocks.Count() == 0)
                return page;

            float err = 1f;
            float minH = page.AllBlocks.Min(b => b.GetH()) + err;

            var blocksAtFooter = page.AllBlocks.Where(b => b.GetH() > minH);

            var result = new BlockPage();
            
            if(!HasFooter(result))
            {
                return page;
            }

            // remove blockset that corresponds to footer
            result.AddRange(blocksAtFooter);

            return result;
        }

        public object RetrieveStatistics()
        {
            return _stats;
        }

        public BlockPage Validate(BlockPage page)
        {
            if (page.AllBlocks.Count() == 0)
                return page;

            float err = 1f;
            float minH = page.AllBlocks.Min(b => b.GetH()) + err;

            var blocksAtFooter = page.AllBlocks.Where(b => b.GetH() <= minH);

            var result = new BlockPage();
            
            if(HasFooter(result))
            {
                result.AddRange(blocksAtFooter);
            }

            return result;
        }

        bool HasFooter(BlockPage result)
        {
            if (result.AllBlocks.Count() > 0)
            {
                float height = result.AllBlocks.GetHeight();

                _stats.FooterHeight = height;

                // ignore footer too large
                if (height < statRegionTooLarge)
                {
                    _stats.HasFooter = true;
                    return true;
                }
            }

            return false;
        }
    }
}
