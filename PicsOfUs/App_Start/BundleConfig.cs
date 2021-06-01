using System.Web.Optimization;

namespace PicsOfUs
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/lib").Include(
                        "~/Content/Scripts/jQuery/jquery-{version}.js", 
                        "~/Content/Scripts/Bootbox/bootbox.js",
                        "~/Content/Scripts/Bootstrap/bootstrap.js",
                        "~/Content/Scripts/Popper/popper.js"));

            bundles.Add(new ScriptBundle("~/bundles/internal").Include(
                        "~/Content/Scripts/My Scripts/*.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Content/Scripts/jQuery.validate/*.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Content/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/css/bootstrap-4-darkly.css",
                      "~/Content/css/site.css",
                      "~/Content/css/lightbox.css"));
        }
    }
}
