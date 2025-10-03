using System;
using System.Collections.Generic;

namespace HanClothesShopProject.Models
{
    public partial class GetCoupon
    {
        public int Id { get; set; }
        public int CouponId { get; set; }
        public int UserId { get; set; }
        public DateTime Createtime { get; set; }
        public short? IsUse { get; set; }

        public virtual Coupon Coupon { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
