using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DotNetCrud
{
    public class DotNetCrudStyleTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "";

            output.PostElement.AppendHtml("<link rel=\"stylesheet\" href=\"//cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css\" />");
            output.PostElement.AppendHtml("<link rel=\"stylesheet\" href=\"//cdn.datatables.net/1.10.20/css/dataTables.bootstrap4.min.css\" />");
            output.PostElement.AppendHtml("<link rel=\"stylesheet\" href=\"//cdnjs.cloudflare.com/ajax/libs/tempusdominus-bootstrap-4/5.0.0-alpha14/css/tempusdominus-bootstrap-4.min.css\" />");
        }
    }
}
