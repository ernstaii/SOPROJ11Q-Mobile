﻿using Newtonsoft.Json;
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

        private string _responseContent { get; set; }
        public JObject Item { get; set; }
        public JArray Items { get; set; }

        // Properties for getting and displaying errors
        public Dictionary<string, string> ErrorMessages = new Dictionary<string, string>();
        public string MainErrorMessage { get; set; }
        public bool HasErrors => ErrorMessages.Count > 0 || MainErrorMessage != null && MainErrorMessage.Length > 0;

        public HttpClientResponse() { }

        public async Task Convert(Task<HttpResponseMessage> request) {
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
                MainErrorMessage = null;

                ResponseMessage = await request;
            }
            catch(Exception e) {
                MainErrorMessage = "Er is iets misgegaan met het maken van een request";
            }
        }

        protected async Task ReadingRequestContent() {
            try {
                _responseContent = await ResponseMessage.Content.ReadAsStringAsync();
            }
            catch(Exception e) {
                MainErrorMessage = "Er is iets misgegaan bij het uitlezen van de inhoud van de response";
            }
        }

        protected void ConvertResponseContent() {
            try {
                if(HasMultipleResults)
                    Items = JArray.Parse(_responseContent);
                else
                    Item = (JObject) JsonConvert.DeserializeObject(_responseContent);
            }
            catch(Exception e) {
                MainErrorMessage = "Er is iets misgegaan bij het omzetten van de inhoud van de response";
            }
        }

        protected void GetErrors() {
            try {
                MainErrorMessage = GetStringValue("message");

                // Loop through every error
                foreach(JToken child in GetValue("errors").Children()) {
                    var property = child as JProperty;
                    if(property != null) {
                        ErrorMessages.Add(property.Name.Trim('_'), property.Value.First.ToString());
                    }
                }
            }
            catch(Exception e) {
                MainErrorMessage = "Er is iets misgegaan bij het omzetten van de errors van de response";
            }
        }

        protected JToken GetValue(string key) {
            return Item.GetValue(key);
        }

        public string GetStringValue(string key) {
            return GetValue(key)?.ToString();
        }

        public int GetNumberValue(string key) { 
            var result = GetValue(key);

            return result == null ? 0 : ((int) result);
        }
    }
}