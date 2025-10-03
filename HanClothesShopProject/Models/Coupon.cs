using System;
using System.Collections.Generic;

namespace HanClothesShopProject.Models
{
    public partial class Coupon
    {
        public Coupon()
        {
            GetCoupons = new HashSet<GetCoupon>();
        }

        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public decimal DiscountAmount { get; set; }
        public string DiscountType { get; set; } = null!;
        public int SumCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int UsedCount { get; set; }
        public decimal ShouldMoney { get; set; }

        public virtual ICollection<GetCoupon> GetCoupons { get; set; }
    }
}
