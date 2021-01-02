using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;
using System.Text.Json;
using DotNetCrud.Utils;

namespace DotNetCrud
{
    public class DotNetCrudScriptTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "";
            output.PostElement.AppendHtml("<script src=\"//cdn.datatables.net/1.10.20/js/jquery.dataTables.min.js\"></script>");
            output.PostElement.AppendHtml("<script src=\"//cdn.datatables.net/1.10.20/js/dataTables.bootstrap4.min.js\"></script>");
            output.PostElement.AppendHtml("<script src=\"//cdnjs.cloudflare.com/ajax/libs/moment.js/2.24.0/moment.min.js\"></script>");
            output.PostElement.AppendHtml("<script src=\"//cdnjs.cloudflare.com/ajax/libs/tempusdominus-bootstrap-4/5.0.0-alpha14/js/tempusdominus-bootstrap-4.min.js\"></script>");

            var script = "" +
                "    <script>" +
                "        var table = $('#example').DataTable({" +
                "            'processing': true," +
                "            'serverSide': true," +
                "            'ajax': {" +
                "                'url': '/home/index?datatables=true'," +
                "                'method': 'POST'" +
                //                "                ,'data': {'datatables' : true}" +
                "            }," +
                "            'columns': [";

            var types = JsonSerializer.Deserialize<Dictionary<string, string>>(Cache.Singleton.Get("")["Types"]);
            var fields = JsonSerializer.Deserialize<List<string>>(Cache.Singleton.Get("")["Fields"]);
            foreach (var item in fields)
            {
                script += "{'data': '" + item + "',";

                if (types[item].StartsWith("DateTime"))
                {
                    script += "'render': function ( data, type, row, meta ) { return (data == null ? '' : moment(data).format('DD/MM/YYYY HH:mm:ss') ); }";
                }
                else if(types[item].StartsWith("Boolean"))
                {
                    script += "'render': function ( data, type, row, meta ) { return (data == true ? '<i class=\\'fa fa-check-square-o\\'></i>' : '<i class=\\'fa fa-square-o\\'></i>') ; }";
                }
                else if (types[item].StartsWith("Int"))
                {
                    script += "'render': function ( data, type, row, meta ) { return '<div class=\\'text-right\\'>' + data + '</div>'; }";
                }
                else if (types[item].StartsWith("Decimal"))
                {
                    script += "'render': function ( data, type, row, meta ) { return '<div class=\\'text-right\\'>' + Number(data).toFixed(2) + ' $</div>'; }";
                }
                else
                {
                    script += "'render': function ( data, type, row, meta ) { return data; }";
                }

                script += "},";
            }

            script +=
                "{'orderable': false, 'render': function (data, type, row) { return '<div class=\"text-right\">" +
                "<a type=\"button\" class=\"btn btn-warning btn-sm\" href=\"/home/index?id=' + row." + Cache.Singleton.Get("")["Id"] + " + '&edit=true\"><i class=\"fa fa-pencil\"></i> Edit</a> " +
                "<a type=\"button\" class=\"btn btn-danger btn-sm\" href=\"/home/index?id=' + row." + Cache.Singleton.Get("")["Id"] + " + '&delete=true\"><i class=\"fa fa-trash\"></i> Delete</a>" +
                "</div>' }} " +
                "            ]" +
                "        });";


            script += " $('#datetimepicker-SellStartDate').datetimepicker({format:'DD/MM/YYYY HH:mm:ss'});";
            script += " $('#datetimepicker-SellEndDate').datetimepicker({format:'DD/MM/YYYY HH:mm:ss'});";
            script += " $('#datetimepicker-ModifiedDate').datetimepicker({format:'DD/MM/YYYY HH:mm:ss'});";
            


            script += "    </script>";

            output.PostElement.AppendHtml(script);

        }
    }
}
