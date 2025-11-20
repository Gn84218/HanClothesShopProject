using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace HanClothesShopProject.CommonUtil
{
    public class ChatHub : Hub
    {
        private readonly MessageService _messageService;
        //注入http协议接口到控制器
        private IHttpContextAccessor _contextAccessor;
        public ChatHub(MessageService messageService, IHttpContextAccessor contextAccessor = null)
        {
            _messageService = messageService;
            _contextAccessor = contextAccessor;
        }
        //參數(接受人ID 及發送內容)
        public async Task SendMessage(int receiverId, string content)
        {
            //当前发送人的id
            int senderId = 0;
            if (_contextAccessor.HttpContext.Session.GetInt32("uid") == null)
            {
                senderId = (int)_contextAccessor.HttpContext.Session.GetInt32("id"); //尝试的取管理员的id
            }
            else
            {
                senderId = (int)_contextAccessor.HttpContext.Session.GetInt32("uid"); //否则取用户的id
            }
            // 异步保存消息
            await _messageService.SendMessageAsync(senderId, receiverId, content);
            //通知所有人(接收人以及發送人)為了每發送調訊息都能馬上顯示在畫面中
            await Clients.All.SendAsync("ReceiveMessage", senderId, content);
        }
        //1. signalR连接操作
        public override async Task OnConnectedAsync()
        {
            string userId = "0";
            if (_contextAccessor.HttpContext.Session.GetInt32("uid") == null)
            {
                userId = _contextAccessor.HttpContext.Session.GetInt32("id").ToString(); //尝试的取管理员的id
            }
            else
            {
                userId = _contextAccessor.HttpContext.Session.GetInt32("uid").ToString(); //否则取用户的id
            }
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);//有人發訊息時進行連接 在觸發SendMessage
            await base.OnConnectedAsync();
        }
    }
}
