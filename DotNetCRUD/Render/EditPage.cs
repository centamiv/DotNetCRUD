using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetCrud.Render
{
    class EditPage
    {
        internal void Render<T>(string key, T row, StringBuilder page, List<string> _fields, Dictionary<string, string> classFields) where T : class, new()
        {
            page.Append("<div class=\"row\"><div class=\"col-12\"><h3>Edit Product</h3></div></div>");
            page.Append("<hr/>");
            page.Append("<div class=\"row\"><div class=\"col-12\"><form method=\"POST\">");
            foreach (var item in _fields.Where(i => !i.Contains(".")))
            {
                var value = typeof(T).GetProperty(item).GetValue(row);

                page.Append("<div class=\"form-group\">");
                page.Append("<label>" + item + "</label>");

                if (classFields[item].StartsWith("Boolean"))
                {
                    if ((bool)value)
                    {
                        page.Append("<div class=\"form-check\"><input name=\"" + item + "\" value=\"1\" class=\"form-check-input\" type=\"checkbox\" checked><label class=\"form-check-label\"> " + item + "</label></div>");
                    }
                    else
                    {
                        page.Append("<div class=\"form-check\"><input name=\"" + item + "\" value=\"1\" class=\"form-check-input\" type=\"checkbox\"><label class=\"form-check-label\"> " + item + "</label></div>");
                    }
                }
                else if (classFields[item].StartsWith("Int"))
                {
                    page.Append("<input name=\"" + item + "\" type=\"number\" value=\"" + value + "\" class=\"form-control\">");
                }
                else if (classFields[item].StartsWith("DateTime"))
                {
                    page.Append("<div class=\"input-group date\" data-target-input=\"nearest\" id=\"datetimepicker-" + item + "\">");
                    page.Append("<input name=\"" + item + "\" data-toggle=\"datetimepicker\" type=\"text\" value=\"" + value + "\" class=\"form-control datetimepicker-input\" data-target=\"#datetimepicker-" + item + "\">");
                    page.Append("<div class=\"input-group-append\" data-target=\"#datetimepicker-" + item + "\" data-toggle=\"datetimepicker\"><div class=\"input-group-text\"><i class=\"fa fa-calendar\"></i></div></div>");
                    page.Append("</div>");
                }
                else
                {
                    page.Append("<input name=\"" + item + "\" type=\"text\" value=\"" + value + "\" class=\"form-control\">");
                }

                page.Append("</div>");
            }
            page.Append("<button type=\"submit\" class=\"btn btn-warning\">Save</button>");
            page.Append(" <a href=\"/home/index\" class=\"btn btn-secondary\">Cancel</a></form></div></div>");
        }

        internal void Execute<T>(Microsoft.AspNetCore.Http.HttpRequest _request, Microsoft.EntityFrameworkCore.DbContext _db, string key, T row, List<string> _fields, Dictionary<string, string> classFields) where T : class, new()
        {
            foreach (var item in _fields)
            {
                var value = _request.Form[item].ToString();
                if (classFields[item].StartsWith("DateTime"))
                {
                    typeof(T).GetProperty(item).SetValue(row, DateTime.Parse(value));
                }
                else if (classFields[item].StartsWith("Boolean"))
                {
                    if (value == "1")
                    {
                        typeof(T).GetProperty(item).SetValue(row, true);
                    }
                    else
                    {
                        typeof(T).GetProperty(item).SetValue(row, false);
                    }
                }
                else if (classFields[item].StartsWith("Int"))
                {
                    typeof(T).GetProperty(item).SetValue(row, int.Parse(value));
                }
                else if (classFields[item].StartsWith("Decimal"))
                {
                    typeof(T).GetProperty(item).SetValue(row, decimal.Parse(value));
                }
                else
                {
                    typeof(T).GetProperty(item).SetValue(row, value);
                }
            }
            _db.Update(row);
            _db.SaveChanges();
        }
    }
}
