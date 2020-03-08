﻿using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace KDSVideo.Infrastructure
{
    public static class JsonHelper
        {
            public static string ToJson<T>(T instance)
            {
                var settings = new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true };
                var serializer = new DataContractJsonSerializer(typeof(T), settings);

                using (var tempStream = new MemoryStream())
                {
                    serializer.WriteObject(tempStream, instance);
                    return Encoding.Default.GetString(tempStream.ToArray());
                }
            }

            public static T FromJson<T>(string json)
            {
                var settings = new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true };
                var serializer = new DataContractJsonSerializer(typeof(T), settings);
                using (var tempStream = new MemoryStream(Encoding.Unicode.GetBytes(json)))
                {
                    return (T)serializer.ReadObject(tempStream);
                }
            }
        }
}