using System;
using System.Collections.Generic;
using System.Linq;

namespace InteligentDimmer.Infrastructure
{
    public class ObjectContainer
    {
        private readonly IList<object> _viewModelContainer;

        private static readonly Lazy<ObjectContainer> _lazyInstance = new Lazy<ObjectContainer>(() => new ObjectContainer(), true);

        public static ObjectContainer Instance => _lazyInstance.Value;

        private ObjectContainer()
        {
            _viewModelContainer = new List<object>();
        }

        public T GetInstance<T>()
        {
            var requestedType = _viewModelContainer.FirstOrDefault(x => x.GetType() == typeof(T));

            if (requestedType != null)
            {
                return (T)requestedType;
            }

            T newType = Activator.CreateInstance<T>();

            _viewModelContainer.Add(newType);

            return newType;
        }
    }
}
