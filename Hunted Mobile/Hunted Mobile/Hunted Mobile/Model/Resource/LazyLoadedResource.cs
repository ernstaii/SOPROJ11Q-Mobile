using Hunted_Mobile.Repository;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hunted_Mobile.Model.Resource {
    class LazyLoadedResource : Resource {
        private readonly Uri uri;
        private byte[] data;
        private readonly Task<byte[]> dataGetter;

        public string AbsolutePath => uri.AbsoluteUri;

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
            uri = uriToResource;
            this.dataGetter = dataGetter;
        }
    }
}
