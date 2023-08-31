using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DPMGallery.Utils
{
    public static class Mapping<TSource, TDestination>
        where TSource : class
        where TDestination : class, new()
    {
        private static Action<TSource, TDestination> _mapping;

        public static void Configure(Action<TSource, TDestination> mapping)
        {
            _mapping = mapping;
        }

        public static TDestination Map(TSource source)
        {
            if (source == null)
                return null;

            var destination = new TDestination(); // Activator.CreateInstance<TDestination>();

            Map(source, destination);

            return destination;
        }

        public static TDestination Map(TSource source, Action<TDestination> modifier)
        {
            var destination = Map(source);
            modifier(destination);

            return destination;
        }
        public static TDestination Map(TSource source, Action<TSource, TDestination> modifier)
        {
            var destination = Map(source);
            modifier(source, destination);

            return destination;
        }


        public static TDestination Map(TSource source, TDestination destination)
        {
            if (_mapping == null)
                throw new InvalidOperationException($"No mapping has been configured between type '{typeof(TSource).Name}' and '{typeof(TDestination).Name}'.");

            if (source != null)
                _mapping(source, destination);

            return destination;
        }

        public static void Map(TSource source, TDestination destination, Action<TDestination> modifier)
        {
            _mapping(source, destination);

            modifier(destination);
        }
        public static void Map(TSource source, TDestination destination, Action<TSource, TDestination> modifier)
        {
            _mapping(source, destination);

            modifier(source, destination);
        }

        public static void Map(IEnumerable<TSource> sources, IList<TDestination> destination)
        {
            foreach (var s in sources)
                destination.Add(Map(s));
        }

        public static IList<TDestination> Map(IEnumerable<TSource> sources)
        {
            if (sources == null)
                return null;
            var result = new List<TDestination>();
            result.AddRange(sources.Select(s => Map(s)).ToList());
            return result;
        }

        public static IList<TDestination> Map(IEnumerable<TSource> sources, Action<TDestination> modifier)
        {
            if (sources == null)
                return null;

            var result = sources.Select(s => Map(s)).ToList();
            result.ForEach(modifier);
            return result;
        }
        public static IList<TDestination> Map(IEnumerable<TSource> sources, Action<TSource, TDestination> modifier)
        {
            if (sources == null)
                return null;

            var result = new List<TDestination>();

            foreach (var source in sources)
            {
                var map = Map(source);
                modifier(source, map);
                result.Add(map);
            }

            return result;
        }

        public static IDictionary<TDestination, TValueDestination> MapDictionary<TValueSource, TValueDestination>(IDictionary<TSource, TValueSource> source)
            where TValueSource : class
            where TValueDestination : class, new()
        {
            var dictionary = new Dictionary<TDestination, TValueDestination>();

            foreach (var keyValuePair in source)
            {
                var key = Mapping<TSource, TDestination>.Map(keyValuePair.Key);
                var value = Mapping<TValueSource, TValueDestination>.Map(keyValuePair.Value);

                dictionary.Add(key, value);
            }

            return dictionary;
        }

        public static IDictionary<TDestination, IEnumerable<TValueDestination>> MapDictionary<TValueSource, TValueDestination>(IDictionary<TSource, IEnumerable<TValueSource>> source)
            where TValueSource : class
            where TValueDestination : class, new()
        {
            var dictionary = new Dictionary<TDestination, IEnumerable<TValueDestination>>();

            foreach (var keyValuePair in source)
            {
                var key = Mapping<TSource, TDestination>.Map(keyValuePair.Key);
                var values = Mapping<TValueSource, TValueDestination>.Map(keyValuePair.Value);

                dictionary.Add(key, values);
            }

            return dictionary;
        }
    }

}
