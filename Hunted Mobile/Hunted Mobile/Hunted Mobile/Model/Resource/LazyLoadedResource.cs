using Hunted_Mobile.Repository;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hunted_Mobile.Model.Resource {
    class LazyLoadedResource : Resource {
        private readonly Uri _uri;
        private byte[] _data;
        private readonly Task<byte[]> _dataGetter;

        public string AbsolutePath => _uri.AbsoluteUri;

        public byte[] Data {
            get {
                if(_dataGetter.Status == TaskStatus.Created) {
                    _dataGetter.Start();
                    _data = _dataGetter.Result;
                    return _data;
                }
                else return _data;
            }
        }

        public LazyLoadedResource(Uri uriToResource, Task<byte[]> dataGetter) {
            _uri = uriToResource;
            _dataGetter = dataGetter;
        }
    }
}
