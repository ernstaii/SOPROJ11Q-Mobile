using Hunted_Mobile.Model.Response.Json;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Xamarin.Forms;

namespace Hunted_Mobile.Service.Json {
    public abstract class JsonConversionService<NativeType, DataType> where DataType : JsonResponseData {
        protected readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate
        };

        public JsonConversionService() {
        }

        public virtual string ToJson(NativeType @object) {
            DependencyService.Get<Toast>().Show("Kon " + @object.GetType().FullName + " niet omzetten naar JSON");
            return string.Empty;
        }

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

        public abstract NativeType ToObject(DataType data);

        protected ReadOnlyCollection<string> ToArray(string json) {
            List<string> items = new List<string>();
            foreach(var item in ToJArray(json)) {
                items.Add(item.ToString());
            }
            return items.AsReadOnly();
        }

        protected JArray ToJArray(string json) {
            try {
                return JArray.Parse(json);
            }
            catch(Exception) {
                DependencyService.Get<Toast>().Show("Kon JSON niet omzetten naar array");
                return new JArray();
            }
        }

        protected JObject ToJObject(string json) {
            if(json == null) {
                DependencyService.Get<Toast>().Show("Er werd lege JSON data verwerkt");
                return new JObject();
            }
            else return JsonConvert.DeserializeObject<JObject>(json, serializerSettings);
        }

        protected string ConvertToJson(JsonResponseData data) {
            try {
                return JsonConvert.SerializeObject(data, serializerSettings);
            }
            catch(Exception) {
                DependencyService.Get<Toast>().Show("Kon data niet omzetten naar JSON");
                return string.Empty;
            }
        }

        protected DataType ConvertFromJson(string json) {
            try {
                return JsonConvert.DeserializeObject<DataType>(json, serializerSettings);
            }
            catch(Exception) {
                DependencyService.Get<Toast>().Show("Kon JSON niet omzetten naar object");
                return default;
            }
        }
    }
}
