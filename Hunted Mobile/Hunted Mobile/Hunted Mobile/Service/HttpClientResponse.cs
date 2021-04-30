using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hunted_Mobile.Service {
    public class HttpClientResponse {
        public HttpStatusCode Status => ResponseMessage != null ? ResponseMessage.StatusCode : HttpStatusCode.NoContent;
        public bool IsSuccessful => ResponseMessage != null && ResponseMessage.IsSuccessStatusCode;
        public bool HasMultipleResults { get; set; }
        public HttpResponseMessage ResponseMessage { get; set; }

        public string ResponseContent { get; set; }
        public JObject Item { get; set; }
        public JArray Items { get; set; }

        // Properties for getting and displaying errors
        public Dictionary<string, string> ErrorMessages = new Dictionary<string, string>();
        public bool HasErrors => ErrorMessages.Count > 0 && !HasServerErrors;
        private bool HasServerErrors { get; set; }

        public HttpClientResponse() { }

        public async Task Convert(Task<HttpResponseMessage> request) {
            HasServerErrors = false;
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
            catch(Exception e) {
                HasServerErrors = true;
            }
        }

        protected async Task ReadingRequestContent() {
            try {
                ResponseContent = await ResponseMessage.Content.ReadAsStringAsync();
            }
            catch(Exception e) {
                HasServerErrors = true;
            }
        }

        protected void ConvertResponseContent() {
            try {
                if(HasMultipleResults)
                    Items = JArray.Parse(ResponseContent);

                if(!HasMultipleResults || HasMultipleResults && !IsSuccessful)
                    Item = (JObject) JsonConvert.DeserializeObject(ResponseContent);
            }
            catch(Exception e) {
                HasServerErrors = true;
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
            catch(Exception e) {
                HasServerErrors = true;
            }
        }

        protected JToken GetValue(string key) {
            return Item.GetValue(key);
        }

        public string GetStringValue(string key) {
            var result = GetValue(key);

            return result != null ? result.ToString() : null;
        }

        public int GetNumberValue(string key) {
            var result = GetValue(key);

            return result == null ? 0 : ((int) result);
        }
    }
}
