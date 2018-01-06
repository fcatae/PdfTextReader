using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Execution
{
    // TODO: move to pipeline page
    // It cannot be a singleton because it will break when 
    // processing multiple pages in a single execution

    class PipelineFactory
    {
        static PipelineFactory g_singleton = new PipelineFactory();

        static public T Create<T>()
            where T : new()
        {
            return g_singleton.CreateInstance<T>();
        }

        Dictionary<Type, object> _dictionary = new Dictionary<Type, object>();

        public T CreateInstance<T>()
            where T: new()
        {
            var objectType = typeof(T);

            if ( !_dictionary.ContainsKey(objectType) )
            {
                _dictionary[objectType] = new T();
            }

            object instance = _dictionary[objectType];
            
            return (T)instance;
        }
    }
}
