﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ArmResourceProviderDemo.WebModels
{
    public interface IPropertiesHolder<TPropertiesBase>
    {
        TPropertiesBase Properties { get; set; }
    }

    public class ResourceJsonConverter<TContainer, TPropertiesBase> : JsonConverter
        where TContainer : IPropertiesHolder<TPropertiesBase>
        where TPropertiesBase : class
    {
        private readonly Dictionary<string, Type> _kindToTypeMapping;

        public ResourceJsonConverter(Dictionary<string, Type> kindToTypeMapping)
        {
            _kindToTypeMapping = kindToTypeMapping;
        }

        public override bool CanConvert(Type objectType) => objectType == typeof(TContainer);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);

            var resouceKindString = jObject["kind"]?.Value<object>().ToString() ?? jObject["Kind"]?.Value<object>().ToString();

            if (!string.IsNullOrEmpty(resouceKindString) && _kindToTypeMapping.TryGetValue(resouceKindString, out Type propsType))
            {
                var holder = JsonConvert.DeserializeObject<TContainer>(jObject.ToString());
                object props = JsonConvert.DeserializeObject(jObject["properties"].ToString(), propsType);
                holder.Properties = props as TPropertiesBase;
                return holder;
            }

            throw new JsonSerializationException($"Could not find the concrete type of {typeof(TPropertiesBase).Name} with kind: [{resouceKindString}]");
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
