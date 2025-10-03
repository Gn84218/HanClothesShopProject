using System;
using System.Collections.Generic;

namespace HanClothesShopProject.Models
{
    public partial class ImageChart
    {
        public int Id { get; set; }
        public string Url { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public DateTime Createtime { get; set; }
        public short State { get; set; }
    }
}
