using System;
using System.Collections.Generic;

namespace HanClothesShopProject.Models
{
    public partial class Address
    {
        public int Id { get; set; }
        public string? Province { get; set; }
        public string? City { get; set; }
        public string? Area { get; set; }
        public string Detail { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? Mark { get; set; }
        public DateTime? Createtime { get; set; }
        public int? Uid { get; set; }

        public virtual User? UidNavigation { get; set; }
    }
}
