using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace StdUtils
{
    public class ObjectUtils
    {
        public static string HumanReadable(object _object, string formatString = "{2}[{0}]: {1}", List<string> objPrefix = null, string objPrefixFormat = "  ")
        {
            var outStrList = new List<string>();
            var keyPrefix = objPrefix ?? new List<string>();

            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(_object))
            {
                string name = descriptor.Name;
                object value = descriptor.GetValue(_object);
                if (value != null)
                {
                    var namePrefix = string.Join("", (from obj in keyPrefix select string.Format(objPrefixFormat, obj)).ToArray());
                    string strValue = ValueToString(name, value, formatString, keyPrefix, objPrefixFormat);
                    if (strValue != string.Empty)
                    {
                        outStrList.Add(string.Format(formatString, name, strValue, namePrefix));
                    }
                }
            }
            if (objPrefix != null)
            {
                objPrefix.RemoveAt(objPrefix.Count - 1);
            }
            return string.Join("\n", outStrList);
        }

        public static string ValueToString(string name, object value, string formatString = "{2}[{0}]: {1}", List<string> objPrefix = null, string objPrefixFormat = "\t")
        {
            if (value == null)
            {
                return "null";
            }
            if (value is string)
            {
                return (string)value;
            }
            if (value is ValueType)
            {
                return value.ToString();
            }
            if (value is IEnumerable)
            {
                var result = new List<string>();
                foreach (var obj in (IEnumerable)value)
                {
                    result.Add(ValueToString(name, obj, formatString, objPrefix, objPrefixFormat));
                }
                return string.Join("\n", result);
            }
            var newPrefix = objPrefix ?? new List<string>();
            newPrefix.Add(name);
            return $"\n{HumanReadable(value, formatString, newPrefix, objPrefixFormat)}";
        }
    }
}
