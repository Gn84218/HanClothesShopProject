using System;
using System.Collections.Generic;

namespace HanClothesShopProject.Models
{
    public partial class ProductAttribute
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public decimal? Price { get; set; }
        public int? Pid { get; set; }

        public virtual Product? PidNavigation { get; set; }
    }
}
