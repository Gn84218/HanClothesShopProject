using System;
using System.Collections.Generic;

namespace HanClothesShopProject.Models
{
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string? Catename { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
