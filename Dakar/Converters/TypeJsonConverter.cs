using Newtonsoft.Json;
using System;

namespace Dakar.Converters
{
    public class TypeJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Type);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var typeName = reader.Value as string;
            return typeName == null ? null : Type.GetType(typeName);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var type = (Type)value;
            writer.WriteValue(type.AssemblyQualifiedName);
        }
    }
}
