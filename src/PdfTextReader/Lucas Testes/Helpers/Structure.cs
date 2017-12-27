using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Lucas_Testes.Helpers
{
    public class StructureItem : LineItem
    {
        public StructureItem(List<MainItem> items) : base(items)
        {
        }

        static bool AreInSameStructure(LineItem i1, LineItem i2)
        {
            if (!i1.GetColor().Equals(i2.GetColor()))
                return false;
            else if (i2.GetRectangle().GetLeft() - i1.GetRectangle().GetLeft() >= MainItem.itemPositionTolerance)
                return false;
            return true;
        }

        public static List<StructureItem> GetStructures(List<LineItem> items, List<BlockSet> _list = null)
        {
            List<StructureItem> structures = new List<StructureItem>();
            List<MainItem> structure = new List<MainItem>();

            List<MainItem> ItemsInBlockSets;


            foreach (var b in _list)
            {
                ItemsInBlockSets = new List<MainItem>();
                foreach (var i in items)
                {
                    if (LineItem.IsInsideBlock(i, b))
                    {
                        ItemsInBlockSets.Add(i);
                    }
                }

                foreach (var line in ItemsInBlockSets)
                {
                    if (structure.Count == 0)
                    {
                        structure.Add(line);
                        continue;
                    }
                    if (AreInSameStructure((LineItem)structure[structure.Count - 1], (LineItem)line))
                    {
                        structure.Add(line);
                    }
                    else
                    {
                        structures.Add(new StructureItem(structure));
                        structure = new List<MainItem>
                    {
                        line
                    };
                    }
                }
                if (structure.Count > 0)
                    structures.Add(new StructureItem(structure));
            }

            return structures;
        }
    }
}
