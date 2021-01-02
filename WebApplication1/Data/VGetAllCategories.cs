using System;
using System.Collections.Generic;

namespace WebApplication1.Models2
{
    public partial class VGetAllCategories
    {
        public string ParentProductCategoryName { get; set; }
        public string ProductCategoryName { get; set; }
        public int? ProductCategoryId { get; set; }
    }
}
