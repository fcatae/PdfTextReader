﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.TextStructures
{
    interface ITransformIndex
    {
        object GetStart(object instance);
        object FindObject(object instance);
    }

    class TransformIndex<TI,TO> : ITransformIndex
    {
        int _position;
        Dictionary<TO,int> _lookup = new Dictionary<TO, int>();
        List<TransformIndexEntry<TI, TO>> _entries = new List<TransformIndexEntry<TI, TO>>();

        public void Add(TransformIndexEntry<TI,TO> entry)
        {
            _lookup.Add(entry.Key, _position);

            entry.Id = (_position++);
            _entries.Add(entry);
        }

        public IList<TransformIndexEntry<TI, TO>> GetEnum()
        {
            return _entries;
        }

        public object GetStart(object instance)
        {
            var entry = Lookup(instance);

            return entry.Start;
        }
        public object FindObject(object instance)
        {
            var entry = Lookup(instance);

            return entry.Key;
        }

        TransformIndexEntry<TI, TO> Lookup(object instance)
        {
            int id = _lookup[(TO)instance];
            var entry = _entries[id];

            return entry;
        }
    }
}
