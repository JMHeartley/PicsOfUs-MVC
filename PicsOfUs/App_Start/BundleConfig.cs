using System.Web.Optimization;

namespace PicsOfUs
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/lib").Include(
                        "~/Content/Js/Plugins/jQuery/jquery-{version}.js", 
                        "~/Content/Js/Plugins/Bootbox/bootbox.js",
                        "~/Content/Js/Plugins/Bootstrap/bootstrap.js",
                        "~/Content/Js/Plugins/Bootstrap-Datepicker/bootstrap-datepicker.js",
                        "~/Content/Js/Plugins/Popper/popper.js"));

            bundles.Add(new ScriptBundle("~/bundles/internal").Include(
                        "~/Content/Js/*.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Content/Js/Plugins/jQuery.validate/*.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Content/Js/Plugins/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/Css/bootstrap-4-darkly.css",
                      "~/Content/Css/Bootstrap-Datepicker/bootstrap-datepicker.css",
                      "~/Content/Css/site.css",
                      "~/Content/Css/lightbox.css"));
        }
    }
}
