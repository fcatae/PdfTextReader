using Autofac;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace PdfTextReader.Execution
{
    class PipelineFactoryContext : IDisposable
    {
        Dictionary<Type, object> _dictionary = new Dictionary<Type, object>();
        static IContainer g_defaultScope;
        ILifetimeScope _scope;
        PipelineFactoryContext _parentContext;

        static PipelineFactoryContext()
        {
            RegisterAssemblyComponents(Assembly.GetExecutingAssembly());
        }

        static void RegisterAssemblyComponents(Assembly currentAssembly)
        {
            var builder = new ContainerBuilder();

            builder.RegisterAssemblyTypes(currentAssembly)
                //.Where(t => t.IsAssignableTo<IProcessBlock>())
                .InstancePerLifetimeScope();

            g_defaultScope = builder.Build();
        }

        public PipelineFactoryContext()
        {
            _parentContext = null;
            _scope = CreateScope();
        }

        public PipelineFactoryContext(PipelineFactoryContext parentContext)
        {
            if (parentContext == null)
                throw new ArgumentNullException(nameof(parentContext));

            // At this time, there is no need to have multiple nested context
            // We limit this to one to prevent unecessary nesting
            // But we can review this policy later whenever needed
            if (parentContext._parentContext != null)
                throw new InvalidOperationException("only single nested level is allowed at this time");

            _parentContext = parentContext;
            _scope = CreateScope();
        }

        ILifetimeScope CreateScope()
        {
            var scope = (_parentContext != null) ? _parentContext._scope : g_defaultScope;

            if (scope == null)
                throw new InvalidOperationException();

            return scope.BeginLifetimeScope();
        }

        T CreateLocalInstance<T>() where T : class
        {
            var objectType = typeof(T);

            if (!_dictionary.ContainsKey(objectType))
            {
                _dictionary[objectType] = _scope.Resolve<T>();
            }

            object instance = _dictionary[objectType];

            return (T)instance;
        }

        T GetLocalInstance<T>() where T : class
        {
            var objectType = typeof(T);

            _dictionary.TryGetValue(objectType, out object instance);

            return (T)instance;
        }

        public T CreateGlobalInstance<T>() where T : class
        {
            if (_parentContext != null)
                return _parentContext.CreateGlobalInstance<T>();

            // create at the local level with no parent = global level
            return CreateLocalInstance<T>();
        }

        T GetGlobalInstance<T>() where T: class
        {
            T instance = null;

            if (_parentContext != null)
                instance = _parentContext.GetGlobalInstance<T>();

            return (instance == null) ? GetLocalInstance<T>() : instance;
        }

        public T CreateInstance<T>() where T : class
        {
            if (_parentContext == null)
                throw new InvalidOperationException("creating local instance at the Global level");

            var globalInstance = GetGlobalInstance<T>();

            if (globalInstance != null)
                return globalInstance;

            return CreateLocalInstance<T>();
        }

        public void Dispose()
        {
            if (_scope != null)
            {
                _scope.Dispose();
                _scope = null;
            }
        }
    }
}
