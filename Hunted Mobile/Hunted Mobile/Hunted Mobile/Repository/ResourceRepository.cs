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
        private readonly Uri mapImageUri;
        private readonly Uri guiImageUri;

        public ResourceRepository() {
            baseUri = new Uri(HttpClientRequestService.IP_ADRESS);
            imageUri = new Uri(baseUri, "images/");
            mapImageUri = new Uri(imageUri, "map/");
            guiImageUri = new Uri(imageUri, "gui/");
        }

        public Resource GetGuiImage(string fileNameAndExtension) {
            return GetResource(new Uri(guiImageUri, fileNameAndExtension));
        }

        public Resource GetMapImage(string fileNameAndExtension) {
            return GetResource(new Uri(mapImageUri, fileNameAndExtension));
        }

        private Resource GetResource(Uri uri) {
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