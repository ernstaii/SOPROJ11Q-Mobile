using Hunted_Mobile.Model.Resource;
using Hunted_Mobile.Service;

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Hunted_Mobile.Repository {
    public class ResourceRepository {
        private static readonly Dictionary<Uri, Resource> existingResources = new Dictionary<Uri, Resource>();

        private readonly Uri baseUri;
        private readonly Uri imageUri;

        public ResourceRepository() {
            baseUri = new Uri(HttpClientRequestService.IP_ADDRESS);
            imageUri = new Uri(baseUri, "images/");
        }

        public Resource GetImage(string fileNameAndExtension) {
            Uri uri = new Uri(imageUri, fileNameAndExtension);

            if(existingResources.ContainsKey(uri)) {
                return existingResources[uri];
            }
            else {
                WebClient webClient = new WebClient();

                Resource resource = new LazyLoadedResource(uri, new Task<byte[]>(() => webClient.DownloadData(uri)));

                existingResources.Add(uri, resource);

                return resource;
            }
        }
    }
}
