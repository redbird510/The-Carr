using System.Web;
using System.Web.Optimization;

namespace PI_Portal
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // ---------------------------------------------------------js------------------------------------------------------------------- //
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",                                        // https://jquery.com/
                        "~/Scripts/jquery-ui-1.12.1.min.js",                                    // https://jqueryui.com/
                        "~/Scripts/umd/popper.min.js"                                           // https://popper.js.org/
                        ));
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"                                            // https://jquery.com/
                        ));
                        //jqueryui/jquery-ui
                        //jquery-detectmobile
                        //jquery-animate-numbers
                        //jquery-blockui
                        //jquery-slimscroll
                        //jquery-sparkline
                        //jquery-icheck
                        //jquery-ui-touch

                    // Use the development version of Modernizr to develop with and learn from. Then, when you're
                    // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));                                              // https://modernizr.com/
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap.min.js",                                           // https://getbootstrap.com
                        "~/Scripts/bootstrap-datepicker.min.js"                                 // https://bootstrap-datepicker.readthedocs.io
                        //"~/Scripts/bootstrap-bootbox.js",
                        //"~/Scripts/bootstrap-select.js",
                        //"~/Scripts/bootstrap-select2.js",
                        //"~/Scripts/bootstrap-fileinput.js",   
                      ));
            bundles.Add(new ScriptBundle("~/bundles/dataTables").Include(
                        "~/Scripts/DataTables/media/js/jquery.dataTables.min.js",               // https://datatables.net
                        "~/Scripts/DataTables/media/js/dataTables.bootstrap4.min.js"            // https://datatables.net
                      ));
            bundles.Add(new ScriptBundle("~/bundles/pace").Include(
                        "~/Scripts/pace.min.js*"                                                // https://github.hubspot.com/pace/docs/welcome/
                        ));
            bundles.Add(new ScriptBundle("~/bundles/charts").Include(
                        "~/Scripts/d3/d3.min.js*",                                              // https://d3js.org/
                        "~/Scripts/rickshaw.min.js"                                             // https://tech.shutterstock.com/rickshaw/
                        ));
                        // magnific-popup
                        // nifty-modal
                        // ios7-switch
                        // fastclick
            bundles.Add(new ScriptBundle("~/bundles/ckeditor").Include(
                        //"~/Scripts/ckeditor5-build-classic/ckeditor.js"                        // https://ckeditor.com
                        "~/Scripts/ckeditor5-build-decoupled-document/ckeditor.js"             // https://ckeditor.com
                        ));

            // ---------------------------------------------------------css------------------------------------------------------------------ //
            bundles.Add(new StyleBundle("~/Content/base").Include(                      
                      "~/Content/themes/base/jquery-ui.min.css",                                // https://jqueryui.com/
                      "~/Content/bootstrap.min.css",                                            // https://getbootstrap.com
                      "~/Content/bootstrap-datepicker.min.css",                                 // https://bootstrap-datepicker.readthedocs.io
                      "~/Content/DataTables/media/css/dataTables.bootstrap4.min.css*",          // https://datatables.net
                      "~/Content/pace/themes/pace-theme-loading-bar.css"                        // https://github.hubspot.com/pace/docs/welcome/
                      ));
            bundles.Add(new StyleBundle("~/Content/fontawesome").Include(
                      //"~/Content/fontawesome/css/all.css"
                      "~/Content/fontawesome/css/fontawesome.min.css",                          // https://fontawesome.com
                      "~/Content/fontawesome/css/brands.min.css",                               // https://fontawesome.com/icons?d=gallery&s=brands
                      "~/Content/fontawesome/css/solid.min.css"                                 // https://fontawesome.com/icons?d=gallery&s=solid
                      //"~/Content/fontawesome/css/regular.css",
                      //"~/Content/fontawesome/css/light.css"
                      ));
            bundles.Add(new StyleBundle("~/Content/custom").Include(                      
                      "~/Content/sidebar.css",                                                  // Jim K
                      "~/Content/sidebar-themes.css",                                           // Jim K
                      "~/Content/site.css"                                                      // Jim K
                      ));
            bundles.Add(new StyleBundle("~/Content/charts").Include(
                      "~/Content/rickshaw.min.css"                                              // https://tech.shutterstock.com/rickshaw/
                      ));
        }
    }
}
