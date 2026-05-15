namespace LayerZero.Tools.Web.Bundles
{
    public enum BundleType { Css, Js }

    public record BundleDescriptor(string[] Globs, BundleType Type, bool Minify);

    public record CachedBundle(string Content, string ContentType, string ETag);

    public interface IBundleBuilder
    {
        void RegisterBundle(string route, string[] sourceGlobs, BundleType type, bool minify);
    }
}
