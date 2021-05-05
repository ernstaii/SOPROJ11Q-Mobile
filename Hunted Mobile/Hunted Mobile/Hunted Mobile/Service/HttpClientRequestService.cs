using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Hunted_Mobile.Service {
    public class HttpClientRequestService {
        public const string IP_ADDRESS = "http://soproj11q.herokuapp.com";

        public static async Task<HttpResponseMessage> Get(string path) {
            return await GetHttpClient().GetAsync(GetUrl(path));
        }

        // This methode is exact like the Get request, but it can be extended with pagination, filtering and sorting
        public static async Task<HttpResponseMessage> GetAll(string path) {
            return await GetHttpClient().GetAsync(GetUrl(path));
        }

        public static async Task<HttpResponseMessage> Create(string path, object parameters) {
            return await GetHttpClient().PostAsync(GetUrl(path), GetEncodedParameters(parameters));
        }

        public static async Task<HttpResponseMessage> Put(string path, object parameters) {
            return await GetHttpClient().PutAsync(GetUrl(path), GetEncodedParameters(parameters));
        }

        public static async Task<HttpResponseMessage> Patch(string path) {
            var requestMessage = new HttpRequestMessage(new HttpMethod("PATCH"), GetUrl(path));
            // If parameters are ever needed you can add them like this:
            // requestMessage.Content = GetEncodedParameters(parameters);
            return await GetHttpClient().SendAsync(requestMessage);
        }

        public static async Task<HttpResponseMessage> Delete(string path) {
            return await GetHttpClient().DeleteAsync(GetUrl(path));
        }

        /// <summary>
        /// Always prepare url with the IPAdress and api path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected static string GetUrl(string path) {
            return $"{IP_ADDRESS}/api/{path}";
        }

        /// <summary>
        /// Always returns a HttpClient with the DefaultHeaders "application/json"
        /// </summary>
        /// <returns></returns>
        protected static HttpClient GetHttpClient() {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return httpClient;
        }

        /// <summary>
        /// This methode get the properties of a class and place all in FormUrlEncodedContent
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected static FormUrlEncodedContent GetEncodedParameters(object parameters) {
            var content = new List<KeyValuePair<string, string>>();

            // Place all properties and values inside a list with KeyValuePairs
            foreach(var property in parameters.GetType().GetProperties()) {
                content.Add(new KeyValuePair<string, string>(property.Name, property.GetValue(parameters).ToString()));
            }

            return new FormUrlEncodedContent(content);
        }
    }
}
