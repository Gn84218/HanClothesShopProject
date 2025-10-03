using System;
using System.Collections.Generic;

namespace HanClothesShopProject.Models
{
    public partial class Product
    {
        public Product()
        {
            ApplyReturns = new HashSet<ApplyReturn>();
            Carts = new HashSet<Cart>();
            OrderComments = new HashSet<OrderComment>();
            OrdersDetails = new HashSet<OrdersDetail>();
            ProductAttributes = new HashSet<ProductAttribute>();
            ProductImages = new HashSet<ProductImage>();
            ProductSaves = new HashSet<ProductSave>();
        }

        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public int Cid { get; set; }
        public decimal Price { get; set; }
        public decimal SalePrice { get; set; }
        public int Number { get; set; }
        public string Detail { get; set; } = null!;
        public string Img { get; set; } = null!;
        public short State { get; set; }
        public DateTime Createtime { get; set; }
        public byte Score { get; set; }
        public decimal Postage { get; set; }

        public virtual Category CidNavigation { get; set; } = null!;
        public virtual ICollection<ApplyReturn> ApplyReturns { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<OrderComment> OrderComments { get; set; }
        public virtual ICollection<OrdersDetail> OrdersDetails { get; set; }
        public virtual ICollection<ProductAttribute> ProductAttributes { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
        public virtual ICollection<ProductSave> ProductSaves { get; set; }
    }
}
