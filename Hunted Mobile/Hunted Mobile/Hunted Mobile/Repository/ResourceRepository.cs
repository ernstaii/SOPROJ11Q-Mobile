using Hunted_Mobile.Model.Resource;
using Hunted_Mobile.Service;

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Hunted_Mobile.Repository {
    public class ResourceRepository {
        private static readonly Dictionary<Uri, Resource> _existingResources = new Dictionary<Uri, Resource>();

        private readonly Uri _baseUri;
        private readonly Uri _imageUri;

        public ResourceRepository() {
            _baseUri = new Uri(HttpClientRequestService.IPAdress);
            _imageUri = new Uri(_baseUri, "images/");
        }

        public Resource GetImage(string fileNameAndExtension) {
            Uri uri = new Uri(_imageUri, fileNameAndExtension);

            if(_existingResources.ContainsKey(uri)) {
                return _existingResources[uri];
            }
            else {
                WebClient webClient = new WebClient();

                Resource resource = new LazyLoadedResource(uri, new Task<byte[]>(() => webClient.DownloadData(uri)));

                _existingResources.Add(uri, resource);

                return resource;
            }
        }
    }
}
