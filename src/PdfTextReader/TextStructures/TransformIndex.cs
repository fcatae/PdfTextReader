using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.TextStructures
{
    interface ITransformIndex
    {
        object GetStart(object instance);
        object FindObject(object instance);
        int GetStartId(int instanceId);
        int GetEndId(int instanceId);
        object GetInstance(int instanceId);
        int FindObjectId(int instanceId);
        int GetObjectId(object instance);
    }

    class TransformIndex<TI,TO> : ITransformIndex
    {
        int _position;
        Dictionary<TO,int> _lookup = new Dictionary<TO, int>();
        List<TransformIndexEntry<TI, TO>> _entries = new List<TransformIndexEntry<TI, TO>>();
        List<TransformIndexEntry2<TO>> _entries2 = new List<TransformIndexEntry2<TO>>();

        public void Add(TransformIndexEntry<TI,TO> entry, TransformIndexEntry2<TO> entry2)
        {
            entry2.Id = _position;
            _entries2.Add(entry2);

            _lookup.Add(entry.Key, _position);

            entry.Id = (_position++);
            _entries.Add(entry);
        }

        //public IList<TransformIndexEntry<TI, TO>> GetEnum()
        //{
        //    return _entries;
        //}

        public object GetStart(object instance)
        {
            var entry = Lookup(instance);

            return entry.Start;
        }

        public int GetStartId(int instanceId)
        {
            var entry = _entries2[instanceId];

            return entry.StartId;
        }

        public int GetEndId(int instanceId)
        {
            var entry = _entries2[instanceId];

            return entry.EndId;
        }

        public object GetInstance(int instanceId)
        {
            var entry = _entries2[instanceId];

            return entry.Key;
        }
        

        public object FindObject(object instance)
        {
            var entry = Lookup(instance);

            return entry.Key;
        }
        public int GetObjectId(object instance)
        {
            var entry = Lookup(instance);

            return entry.Id;
        }

        public TO FindObjectId(int instanceId)
        {
            var entry = _entries2[instanceId];

            return entry.Key;
        }

        TransformIndexEntry<TI, TO> Lookup(object instance)
        {
            int id = _lookup[(TO)instance];
            var entry = _entries[id];

            return entry;
        }

        int ITransformIndex.FindObjectId(int instanceId)
        {
            throw new NotImplementedException();
        }
    }
}
