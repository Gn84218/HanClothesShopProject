using System;
using System.Collections.Generic;

namespace HanClothesShopProject.Models
{
    public partial class ReturnSet
    {
        public int Id { get; set; }
        public string Province { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Area { get; set; } = null!;
        public string Detail { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? Mark { get; set; }
        public DateTime Createtime { get; set; }
    }
}
