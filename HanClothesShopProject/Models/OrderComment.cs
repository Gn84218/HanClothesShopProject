using System;
using System.Collections.Generic;

namespace HanClothesShopProject.Models
{
    public partial class OrderComment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int OrderId { get; set; }
        public string Detail { get; set; } = null!;
        public int? Pid { get; set; }
        public byte? Score { get; set; }
        public DateTime? Createtime { get; set; }

        public virtual Order Order { get; set; } = null!;
        public virtual Product? PidNavigation { get; set; }
        public virtual User User { get; set; } = null!;
    }
}
