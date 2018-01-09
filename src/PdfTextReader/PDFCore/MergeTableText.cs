using PdfTextReader.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class MergeTableText : IProcessBlock, IValidateBlock, IPipelineDependency
    {
        private List<IBlock> _tables;
        private IdentifyTables _parser;

        public void SetPage(PipelinePage p)
        {
            var parserTable = p.CreateInstance<PDFCore.IdentifyTables>();

            var page = parserTable.PageTables;

            if (page == null)
                throw new InvalidOperationException("MergeTableText requires IdentifyTables");

            this._tables = page.AllBlocks.ToList();
            this._parser = parserTable;
        }

        public BlockPage Process(BlockPage page)
        {            
            if (this._tables == null)
                throw new InvalidOperationException("MergeTableText requires IdentifyTables");

            var tables = LoopMergeTables(page, _tables);
            
            _parser.SetPageTables(tables);

            return page;
        }


        public BlockPage Validate(BlockPage page)
        {
            var result = new BlockPage();

            if (this._tables == null)
                throw new InvalidOperationException("MergeTableText requires IdentifyTables");

            var tables = MergeTables(page, _tables);

            result.AddRange(tables);

            return result;
        }

        public IEnumerable<IBlock> LoopMergeTables(BlockPage page, IEnumerable<IBlock> initialTables)
        {
            var tables = initialTables;

            while(true)
            {
                int before = tables.Count();
                tables = MergeTables(page, tables);
                int after = tables.Count();

                if (before == after)
                    break;
            }

            return tables;
        }

        public IEnumerable<IBlock> MergeTables(BlockPage page, IEnumerable<IBlock> initialTables)
        {
            float errorY = 1f;

            var tables = initialTables.ToList();

            foreach(var block in page.AllBlocks)
            {
                List<IBlock> tabs = new List<IBlock>();

                foreach (var table in tables)
                {
                    if( Block.HasOverlapWithY(table, block, errorY) )
                    {
                        tabs.Add(table);
                    }
                }

                if( tabs.Count > 1 )
                {
                    var newtab = new TableSet();
                    newtab.AddRange(tabs);

                    foreach(var table in tabs)
                    {
                        tables.Remove(table);
                    }

                    tables.Add(newtab);
                }                
            }

            // cannot increase the number of tables
            if (tables.Count > initialTables.Count())
                throw new InvalidOperationException();

            return tables;
        }
    }
}
