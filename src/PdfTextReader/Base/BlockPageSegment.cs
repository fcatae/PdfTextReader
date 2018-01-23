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

            return name + suffix;
        }

        string GetNameInternal()
        {
            string name = "(" + String.Join("", this.Select(t => t.GetColumnName())) + ")";

            return ShortenName(name);
        }

        string ShortenName(string name)
        {
            if (name == "(1)" || name == "(12)" || name == "(123)")
                return "";

            if ( name.StartsWith("(1") )
            {
                string shortName = name.Replace("1", "");

                if (shortName.Length == name.Length - 1)
                    return "L" + shortName;
            }

            if( name.EndsWith("3)"))
            {
                string shortName = name.Replace("3", "");

                if (shortName.Length == name.Length - 1)
                    return "R" + shortName;
            }

            return name;
        }
    }
}
