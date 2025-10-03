using System;
using System.Collections.Generic;

namespace HanClothesShopProject.Models
{
    public partial class ProductImage
    {
        public int Id { get; set; }
        public int? Pid { get; set; }
        public string? ImageUrl { get; set; }

        public virtual Product? PidNavigation { get; set; }
    }
}
