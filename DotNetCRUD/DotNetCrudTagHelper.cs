using Microsoft.AspNetCore.Razor.TagHelpers;
using DotNetCrud.Utils;

namespace DotNetCrud
{
    public class DotNetCrudTagHelper : TagHelper
    {
        public string BuildName { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "";
            output.PostElement.AppendHtml(Cache.Singleton.Get(BuildName)["Page"]);
        }
    }
}
