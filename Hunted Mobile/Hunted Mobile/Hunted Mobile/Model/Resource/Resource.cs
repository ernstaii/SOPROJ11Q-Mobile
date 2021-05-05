namespace Hunted_Mobile.Model.Resource {
    public interface Resource {
        string AbsolutePath { get; }
        byte[] Data { get; }
    }
}