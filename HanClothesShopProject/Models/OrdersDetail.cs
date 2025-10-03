using System;
using System.Collections.Generic;

namespace HanClothesShopProject.Models
{
    public partial class OrdersDetail
    {
        public int Id { get; set; }
        public int? OrderId { get; set; }
        public int? Count { get; set; }
        public decimal? Price { get; set; }
        public decimal? SumPrice { get; set; }
        public int? Pid { get; set; }
        public string? Title { get; set; }
        public int? Aid { get; set; }

        public virtual Order? Order { get; set; }
        public virtual Product? PidNavigation { get; set; }
    }
}
