using System;

namespace Hunted_Mobile.Model.Resource {
    public interface Resource {
        Uri Uri { get; }
        byte[] Data { get; }
    }
}