using System.Web;
using System.Web.Optimization;

namespace AlertProfiler.WebApp
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

			// Use the development version of Modernizr to develop with and learn from. Then, when you're
			// ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
			//bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
			//            "~/Scripts/modernizr-*"));

			//bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
			//          "~/Scripts/bootstrap.js"));

			//bundles.Add(new StyleBundle("~/Content/css").Include(
			//          "~/Content/bootstrap.css",
			//          "~/Content/site.css"));

			bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
					  "~/Scripts/bootstrap.js",
					  "~/Scripts/bootbox.min.js",
					  "~/Scripts/bootstrap-dialog.min.js",
					  "~/Scripts/listbox.js",
					  "~/Scripts/custom.js",
					  "~/Scripts/jquery.metisMenu.js",
					  "~/Scripts/DataTables/dataTables.min.js",
					  "~/Scripts/DataTables/jquery.dataTables.yadcf.js",
					  "~/Scripts/DataTables/dataTables.fixedColumns.min.js",
					  "~/Scripts/respond.js"));

			bundles.Add(new StyleBundle("~/Content/jqueryui").Include(
				"~/Content/themes/base/all.css"));

			bundles.Add(new StyleBundle("~/Content/css").Include(
					  "~/Content/font-awesome.min.css",
					  "~/Content/bootstrap.min.css",
					  "~/Content/bootstrap-dialog.min.css",
					  "~/Content/googlefont.css",
					  "~/Content/jquery-ui.css",
					  "~/Content/DataTables/css/dataTables.min.css",
					  "~/Content/DataTables/css/jquery.dataTables.yadcf.css",
					  "~/Content/DataTables/css/dataTables.customLoader.walker.css"
					  ));

			bundles.Add(new ScriptBundle("~/bundles/datatables").Include(
					"~/Scripts/DataTables/dataTables.buttons.min.js",
					"~/Scripts/DataTables/buttons.flash.min.js",
					"~/Scripts/DataTables/buttons.html5.min.js",
					"~/Scripts/DataTables/buttons.flash.min.js",
					"~/Scripts/DataTables/buttons.print.min.js",
					"~/Scripts/jszip.min.js",
					"~/Scripts/pdfmake/pdfmake.min.js",
					"~/Scripts/pdfmake/vfs_fonts.js"
					));
		}
    }
}
