using Hunted_Mobile.Model.Response.Json;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Hunted_Mobile.Service.Json {
    public abstract class JsonConversionService<NativeType, DataType> where DataType : JsonResponseData {
        protected readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate
        };

        public JsonConversionService() {
        }

        public abstract string ToJson(NativeType @object);

        public NativeType ToObject(string json) {
            return ToObject(ConvertFromJson(json));
        }

        public NativeType[] ToObjects(string json) {

            var jsonArray = ToArray(json);
            NativeType[] objectArray = new NativeType[jsonArray.Count];

            for(int i = 0; i < objectArray.Length; i++) {
                objectArray[i] = ToObject(ConvertFromJson(jsonArray[i]));
            }

            return objectArray;
        }

        public NativeType[] ToObjects(DataType[] data) {
            NativeType[] objects = new NativeType[data.Length];
            for(int i = 0; i < objects.Length; i++) {
                objects[i] = ToObject(data[i]);
            }
            return objects;
        }

        protected abstract NativeType ToObject(DataType data);

        protected ReadOnlyCollection<string> ToArray(string json) {
            List<string> items = new List<string>();
            foreach(var item in ToJArray(json)) {
                items.Add(item.ToString());
            }
            return items.AsReadOnly();
        }

        protected JArray ToJArray(string json) {
            return JArray.Parse(json);
        }

        protected JObject ToJObject(string json) {
            if(json == null) {
                return new JObject();
            }
            else return JsonConvert.DeserializeObject<JObject>(json, serializerSettings);
        }

        protected string ConvertToJson(JsonResponseData data) {
            return JsonConvert.SerializeObject(data, serializerSettings);
        }

        protected DataType ConvertFromJson(string json) {
            return JsonConvert.DeserializeObject<DataType>(json, serializerSettings);
        }
    }
}
