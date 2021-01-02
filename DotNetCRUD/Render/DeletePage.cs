using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetCrud.Render
{
    class DeletePage
    {
        internal void Render<T>(string key, T row, StringBuilder page, List<string> _fields, Dictionary<string, string> classFields) where T : class, new()
        {
            page.Append("<div class=\"row\"><div class=\"col-12\"><h3>Delete Product</h3></div></div>");
            page.Append("<hr/>");
            page.Append("<div class=\"row\"><div class=\"col-12\"><form method=\"POST\">");

            page.Append($"<p>Do you really want to delete product {key}?</p>");
            page.Append("<button type=\"submit\" class=\"btn btn-warning\">Delete</button>");
            page.Append(" <a href=\"/home/index\" class=\"btn btn-secondary\">Cancel</a>");

            page.Append("</form></div></div>");
        }

        internal void Execute<T>(HttpRequest request, DbContext _db, string key, T row, List<string> fields, Dictionary<string, string> classFields) where T : class, new()
        {
            try
            {
                _db.Remove(row);
                _db.SaveChanges();
            }
            catch (Exception) { }
        }
    }
}
