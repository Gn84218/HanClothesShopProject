using HanClothesShopProject.Models;
using Microsoft.EntityFrameworkCore;

namespace HanClothesShopProject.CommonUtil
{
    //消息服務類別（目前為空）儲存和獲取消息
    public class MessageService
    {
        //数据库上下文对象
        private dbContext _context;
        //注入http协议接口到控制器
        private IHttpContextAccessor _contextAccessor;

        //构造方法，实现类实例化时，自动注入相关服务
        public MessageService(dbContext context, IHttpContextAccessor contextAccessor = null)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        //存储发送消息方法
        public async Task SendMessageAsync(int senderId, int receiverId, string content)
        {
            var message = new ChatMessage
            {
                FromUserid = senderId,
                ToUserid = receiverId,
                Message = content,
                SendTime = DateTime.UtcNow,
                IsRead = 0
            };

            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();
        }
        //获取消息方法
        public async Task<IEnumerable<ChatMessage>> GetMessagesAsync(int userId)
        {
            return await _context.ChatMessages
                .Where(m => m.ToUserid == userId || m.FromUserid == userId)
                .ToListAsync();
        }
    }
}
