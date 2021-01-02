using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetCrud.Render
{
    class ListPage
    {
        internal void Render(StringBuilder page, List<string> _fields, Dictionary<string, string> classFields)
        {
            page.Append("<div class=\"row\"><div class=\"col-6\"><h3>Products List</h3></div><div class=\"col-6 text-right\"><a type=\"button\" class=\"btn btn-success align-self-end\" href=\"/home/index?insert=true\"><i class=\"fa fa-file\"></i> Insert New Product</a></div></div>");
            page.Append("<hr/>");
            page.Append("<div class=\"row\"><div class=\"col-12\"><table id=\"example\" class=\"table \" style=\"width: 100%\"><thead><tr>");
            foreach (var item in _fields)
            {
                page.Append("<th>" + item + "</th>");
            }
            page.Append("<th></th></tr></thead></table></div></div>");
        }
    }
}
