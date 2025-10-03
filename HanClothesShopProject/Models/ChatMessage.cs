using System;
using System.Collections.Generic;

namespace HanClothesShopProject.Models
{
    public partial class ChatMessage
    {
        public int Id { get; set; }
        public string? Message { get; set; }
        public int FromUserid { get; set; }
        public int ToUserid { get; set; }
        public DateTime SendTime { get; set; }
        public short IsRead { get; set; }
    }
}
