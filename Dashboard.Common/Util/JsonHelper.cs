using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
namespace Dashboard.Common.Util
{
    public class JsonHelper
    {
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public T JsonDeserialize<T>(string jsonString)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            System.IO.MemoryStream ms = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonString));
            T obj = (T)ser.ReadObject(ms);
            return obj;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static string[] JsonDeserialize(string jsonString)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(string[]));
            System.IO.MemoryStream ms = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonString));
            string[] obj = (string[])ser.ReadObject(ms);
            return obj;
        }

    }


}
