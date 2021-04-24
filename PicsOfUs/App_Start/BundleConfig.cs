using System.Web.Optimization;

namespace PicsOfUs
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/lib").Include(
                        "~/Scripts/jQuery/jquery-{version}.js", 
                        "~/Scripts/ajax-common.js",
                        "~/Scripts/bootbox.js",
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/Popper/popper.js"));

            bundles.Add(new ScriptBundle("~/bundles/lib-internal").Include(
                "~/Scripts/My Scripts/*.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jQuery/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap-4-darkly.css",
                      "~/Content/site.css",
                      "~/Content/lightbox.css"));
        }
    }
}
