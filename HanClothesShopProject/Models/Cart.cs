using System;
using System.Collections.Generic;

namespace HanClothesShopProject.Models
{
    public partial class Cart
    {
        public int Id { get; set; }
        public int? Uid { get; set; }
        public int? Pid { get; set; }
        public int? Number { get; set; }
        public DateTime? Createtime { get; set; }
        public int? Aid { get; set; }

        public virtual Product? PidNavigation { get; set; }
        public virtual User? UidNavigation { get; set; }
    }
}
