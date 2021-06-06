using Hunted_Mobile.Model.Response.Json;
using Hunted_Mobile.Repository;

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
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
            Culture = AppSettings.Locale
        };

        public JsonConversionService() {
        }

        public virtual string ToJson(NativeType @object) {
            DependencyService.Get<Toast>().Show("(#4) Kon " + @object.GetType().FullName + " niet omzetten naar JSON (JsonConversionService)");
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
                if(string.IsNullOrWhiteSpace(json)) {
                    return new JArray();
                }
                else return JArray.Parse(json);
            }
            catch(Exception e) {
                DependencyService.Get<Toast>().Show("(#5) Kon JSON niet omzetten naar JArray (JsonConversionService)");
                UnitOfWork.Instance.ErrorRepository.Create(e);
                return new JArray();
            }
        }

        protected JObject ToJObject(string json) {
            try {
                if(string.IsNullOrWhiteSpace(json)) {
                    return new JObject();
                }
                else return JsonConvert.DeserializeObject<JObject>(json, serializerSettings);
            }
            catch(Exception e) {
                DependencyService.Get<Toast>().Show("(#6) kon JSON niet omzetten naar JObject (JsonConversionService)");
                UnitOfWork.Instance.ErrorRepository.Create(e);
                return new JObject();
            }
        }

        protected string ConvertToJson(JsonResponseData data) {
            try {
                return JsonConvert.SerializeObject(data, serializerSettings);
            }
            catch(Exception e) {
                DependencyService.Get<Toast>().Show("(#7) Kon parameter data niet omzetten naar JSON (JsonConversionService)");
                UnitOfWork.Instance.ErrorRepository.Create(e);
                return string.Empty;
            }
        }

        protected DataType ConvertFromJson(string json) {
            try {
                if(string.IsNullOrWhiteSpace(json)) {
                    return default;
                }
                else return JsonConvert.DeserializeObject<DataType>(json, serializerSettings);
            }
            catch(Exception e) {
                DependencyService.Get<Toast>().Show("(#8) Kon parameter JSON niet omzetten naar object (JsonConversionService)");
                UnitOfWork.Instance.ErrorRepository.Create(e);
                return default;
            }
        }
    }
}
