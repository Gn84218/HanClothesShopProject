using System;
using System.Collections.Generic;

namespace HanClothesShopProject.Models
{
    public partial class User
    {
        public User()
        {
            Addresses = new HashSet<Address>();
            ApplyReturns = new HashSet<ApplyReturn>();
            Carts = new HashSet<Cart>();
            GetCoupons = new HashSet<GetCoupon>();
            OrderComments = new HashSet<OrderComment>();
            Orders = new HashSet<Order>();
            ProductSaves = new HashSet<ProductSave>();
        }

        public int Id { get; set; }
        public string Phone { get; set; } = null!;
        public string Pwd { get; set; } = null!;
        public string Nickname { get; set; } = null!;
        public string? Sex { get; set; }
        public string? Introduce { get; set; }
        public int? Age { get; set; }
        public string? Img { get; set; }
        public string? Mibao { get; set; }
        public short? Role { get; set; }

        public virtual ICollection<Address> Addresses { get; set; }
        public virtual ICollection<ApplyReturn> ApplyReturns { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<GetCoupon> GetCoupons { get; set; }
        public virtual ICollection<OrderComment> OrderComments { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<ProductSave> ProductSaves { get; set; }
    }
}
