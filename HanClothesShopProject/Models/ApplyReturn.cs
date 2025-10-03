using System;
using System.Collections.Generic;

namespace HanClothesShopProject.Models
{
    public partial class ApplyReturn
    {
        public int Id { get; set; }
        public int Uid { get; set; }
        public int Pid { get; set; }
        public string? BusinessMark { get; set; }
        public string ReturnReason { get; set; } = null!;
        public string? UserMark { get; set; }
        public short Status { get; set; }
        public int OrderId { get; set; }
        public DateTime Createtime { get; set; }

        public virtual Order Order { get; set; } = null!;
        public virtual Product PidNavigation { get; set; } = null!;
        public virtual User UidNavigation { get; set; } = null!;
    }
}
