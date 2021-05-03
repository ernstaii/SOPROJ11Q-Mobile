using System;
using System.Collections.Generic;
using System.Text;

namespace Hunted_Mobile.Model.Resource {
    public interface Resource {
        string AbsolutePath { get; }
        byte[] Data { get; }
    }
}
