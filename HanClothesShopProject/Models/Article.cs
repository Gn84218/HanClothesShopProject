using System;
using System.Collections.Generic;

namespace HanClothesShopProject.Models
{
    public partial class Article
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Detail { get; set; } = null!;
        public DateTime Createtime { get; set; }
        public int Sight { get; set; }
    }
}
