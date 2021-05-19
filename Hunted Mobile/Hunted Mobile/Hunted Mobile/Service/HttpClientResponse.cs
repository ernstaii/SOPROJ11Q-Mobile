using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hunted_Mobile.Service {
    public class HttpClientResponse {
        protected JObject item;
        protected JArray items;

        public HttpStatusCode Status => ResponseMessage != null ? ResponseMessage.StatusCode : HttpStatusCode.NoContent;
        public bool IsSuccessful => ResponseMessage != null && ResponseMessage.IsSuccessStatusCode;
        public bool HasMultipleResults { get; set; }
        public HttpResponseMessage ResponseMessage { get; set; }

        public string ResponseContent { get; set; }

        // Properties for getting and displaying errors
        public Dictionary<string, string> ErrorMessages { get; set; } = new Dictionary<string, string>();
        public bool HasErrors => ErrorMessages.Count > 0 && !hasServerErrors;
        private bool hasServerErrors;

        public HttpClientResponse() { }

        public async Task Convert(Task<HttpResponseMessage> request) {
            hasServerErrors = false;
            ErrorMessages.Clear();
            await ExecuteRequest(request);

            if(!HasErrors) {
                await ReadingRequestContent();
                ConvertResponseContent();

                // Check if request went successfully
                if(!IsSuccessful)
                    GetErrors();
            }
        }

        protected async Task ExecuteRequest(Task<HttpResponseMessage> request) {
            try {
                ErrorMessages.Clear();

                ResponseMessage = await request;
            }
            catch(Exception) {
                hasServerErrors = true;
                DependencyService.Get<Toast>().Show("Er was een probleem met het uitvoeren van een request");
            }
        }

        protected async Task ReadingRequestContent() {
            try {
                ResponseContent = await ResponseMessage.Content.ReadAsStringAsync();
            }
            catch(Exception) {
                hasServerErrors = true;
                DependencyService.Get<Toast>().Show("Er was een probleem met het uitlezen van de response");
            }
        }

        protected void ConvertResponseContent() {
            try {
                if(HasMultipleResults)
                    items = JArray.Parse(ResponseContent);

                if(!HasMultipleResults || HasMultipleResults && !IsSuccessful)
                    item = (JObject) JsonConvert.DeserializeObject(ResponseContent);
            }
            catch(Exception) {
                hasServerErrors = true;
                DependencyService.Get<Toast>().Show("Er was een probleem met het converteren van de response");
            }
        }

        protected void GetErrors() {
            try {
                // Loop through every error
                foreach(JToken child in GetValue("errors").Children()) {
                    var property = child as JProperty;
                    if(property != null) {
                        ErrorMessages.Add(property.Name.Trim('_'), property.Value.First.ToString());
                    }
                }
            }
            catch(Exception) {
                hasServerErrors = true;
                DependencyService.Get<Toast>().Show("Er was een probleem met de server");
            }
        }

        protected JToken GetValue(string key) {
            return item.GetValue(key);
        }

        protected string GetStringValue(string key) {
            var result = GetValue(key);

            return result != null ? result.ToString() : null;
        }

        protected int GetNumberValue(string key) {
            var result = GetValue(key);

            return result == null ? 0 : ((int) result);
        }
    }
}
