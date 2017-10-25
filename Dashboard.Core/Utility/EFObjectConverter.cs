using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Reflection;
using System.Collections;
using Dashboard.Core.FrogEntities;

namespace Dashboard.Core.Utility
{
    public class EFJavaScriptConverter : JavaScriptConverter
    {
        private int _currentDepth = 1;
        private readonly int _maxDepth = 1;

        private readonly List<object> _processedObjects = new List<object>();

        private readonly Type[] _builtInTypes = new[]
        {
          typeof(int?),
          typeof(Int16?),
          typeof(Int32?),
          typeof(double?),
          typeof(bool?),
          typeof(bool),
          typeof(byte),
          typeof(sbyte),
          typeof(char),
          typeof(decimal),
          typeof(decimal?),
          typeof(double),
          typeof(float),
          typeof(int),
          typeof(Int16),
          typeof(Int32),
          typeof(uint),
          typeof(long),
          typeof(ulong),
          typeof(short),
          typeof(ushort),
          typeof(string),
          typeof(DateTime),
          typeof(DateTime?),
          typeof(Guid)
      };
        public EFJavaScriptConverter() : this(1, null) { }

        public EFJavaScriptConverter(int maxDepth = 1, EFJavaScriptConverter parent = null)
        {
            _maxDepth = maxDepth;
            if (parent != null)
            {
                _currentDepth += parent._currentDepth;
            }
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            return null;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            _processedObjects.Add(obj.GetHashCode());
            var type = obj.GetType();

            var properties = from p in type.GetProperties()
                             where p.CanRead && p.GetIndexParameters().Count() == 0 &&
                                   _builtInTypes.Contains(p.PropertyType)
                             select p;

            var result = properties.ToDictionary(
                          p => p.Name,
                          p => (Object)TryGetStringValue(p, obj));

            if (_maxDepth >= _currentDepth)
            {
                var complexProperties = from p in type.GetProperties()
                                        where p.CanRead &&
                                              p.GetIndexParameters().Count() == 0 &&
                                              !_builtInTypes.Contains(p.PropertyType) &&
                                              p.Name != "RelationshipManager" &&
                                              !AllreadyAdded(p, obj)
                                        select p;

                foreach (var property in complexProperties)
                {
                    var complexValue = TryGetValue(property, obj);

                    if (complexValue != null)
                    {
                        var js = new EFJavaScriptConverter(_maxDepth - _currentDepth, this);

                        //result.Add(property.Name, js.Serialize(complexValue, new EFJavaScriptSerializer()));
                    }
                }
            }

            return result;
        }

        private bool AllreadyAdded(PropertyInfo p, object obj)
        {
            var val = TryGetValue(p, obj);
            return _processedObjects.Contains(val == null ? 0 : val.GetHashCode());
        }

        private static object TryGetValue(PropertyInfo p, object obj)
        {
            var parameters = p.GetIndexParameters();
            if (parameters.Length == 0)
            {
                return p.GetValue(obj, null);
            }
            else
            {
                //cant serialize these
                return null;
            }
        }

        private static object TryGetStringValue(PropertyInfo p, object obj)
        {
            if (p.GetIndexParameters().Length == 0)
            {
                var val = p.GetValue(obj, null);
                return val;
            }
            else
            {
                return string.Empty;
            }
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                var types = new List<Type>();
                types.AddRange(Assembly.GetAssembly(typeof(DashboardDbContext)).GetTypes());
                return types;
            }
        }
    }


}
