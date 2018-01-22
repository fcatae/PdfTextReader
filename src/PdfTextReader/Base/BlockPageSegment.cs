using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.Base
{
    class BlockPageSegment : BlockSet<BlockColumn>
    {
        public int NumberOfColumns { get; private set; }

        public BlockPageSegment(BlockPage page, int columnType) : base(page)
        {
            this.NumberOfColumns = columnType;
        }

        public void AddColumn(BlockColumn column)
        {
            this.Add(column);
        }
        
        public string GetName()
        {
            string name = NumberOfColumns.ToString();
            string suffix = GetNameInternal();

            if (suffix == "(1)" || suffix == "(12)" || suffix == "(123)")
                suffix = "";

            return name + suffix;
        }

        string GetNameInternal()
        {
            return "(" + String.Join("", this.Select(t => t.GetColumnName())) + ")";
        }
    }
}
