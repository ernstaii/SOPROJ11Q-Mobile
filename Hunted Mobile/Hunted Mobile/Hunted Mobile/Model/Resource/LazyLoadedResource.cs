
using System;
using System.Threading.Tasks;

namespace Hunted_Mobile.Model.Resource {
    class LazyLoadedResource : Resource {
        private byte[] data;
        private readonly Task<byte[]> dataGetter;

        public Uri Uri { get; }

        public byte[] Data {
            get {
                if(dataGetter.Status == TaskStatus.Created) {
                    dataGetter.Start();
                    data = dataGetter.Result;
                    return data;
                }
                else return data;
            }
        }

        public LazyLoadedResource(Uri uriToResource, Task<byte[]> dataGetter) {
            Uri = uriToResource;
            this.dataGetter = dataGetter;
        }
    }
}