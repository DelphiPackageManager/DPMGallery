using Dapper;
using System;
using System.Data;
using System.Linq;
using System.Reflection;

namespace DPMGallery.Data
{
    public static class TypeMapper
    {
        public static void Initialize(string @namespace)
        {
            DefaultTypeMap.MatchNamesWithUnderscores = true;

            var types = from assem in AppDomain.CurrentDomain.GetAssemblies().ToList()
                        from type in assem.GetTypes()
                        where type.IsClass && type.Namespace == @namespace
                        select type;

            types.ToList().ForEach(entityType =>
            {
                var fallback = new DefaultTypeMap(entityType);
                SqlMapper.SetTypeMap(
                   entityType,
                   new CustomPropertyTypeMap(entityType, (t, columnName) =>
                   {
                       PropertyInfo pi = t.GetProperties().FirstOrDefault(prop =>
                                         prop.GetCustomAttributes(false)
                                             .OfType<ColumnAttribute>()
                                             .Any(attr => attr.Name == columnName || prop.Name == columnName));

                       if (pi == null)
                       {
                          pi = fallback.GetMember(columnName)?.Property;
                       }
                       return pi;
                   }));
            });
        }
    }
}
