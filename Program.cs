using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace DataContractJsonSerializeExample
{
    [DataContract]
    public class ContractGeneric : IExtensibleDataObject
    {
        public ExtensionDataObject ExtensionData { get; set; }
    }

    [DataContract]
    public class ContractExtended
    {
        [DataMember(Name = "item", Order = 1)]
        public Item Item;
    }

    [DataContract]
    public class Item
    {
        [DataMember(Name = "id", Order = 1, EmitDefaultValue = false)]
        public long? Id;

        [DataMember(Name = "code", Order = 2, EmitDefaultValue = false)]
        public long? Code;
    }

    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("Console application started.");
            Console.WriteLine("Trying to serialize/deserialize object '{\"item\":{\"id\":1,\"code\":2}}' - everything should be OK");
            SerializeThenDeserialize(new ContractExtended {Item = new Item {Id = 1, Code = 2}});
            Console.WriteLine("Trying to serialize/deserialize object '{\"item\":{\"id\":1}}' - deserializer should throw Exception");
            SerializeThenDeserialize(new ContractExtended {Item = new Item {Id = 1}});
        }

        public static void SerializeThenDeserialize(ContractExtended extendedData)
        {
            var extendedContractJson = ToJson(extendedData);
            Console.WriteLine("  " + extendedContractJson);
            var reducedData = FromJson<ContractGeneric>(extendedContractJson);
            var reducedContractJson = ToJson(reducedData);
            Console.WriteLine("  " + reducedContractJson);
        }

        public static string ToJson<T>(T value)
        {
            using (var memoryStream = new MemoryStream())
            {
                new DataContractJsonSerializer(typeof(T)).WriteObject(memoryStream, value);
                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }

        public static T FromJson<T>(string json)
        {
            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                return (T) new DataContractJsonSerializer(typeof(T)).ReadObject(memoryStream);
            }
        }
    }
}