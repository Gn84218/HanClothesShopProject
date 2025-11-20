using System.Diagnostics;
using HanClothesShopProject.Filter;
using HanClothesShopProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HanClothesShopProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private dbContext _context;
        private IHttpContextAccessor _contextAccessor;

        public HomeController(ILogger<HomeController> logger, dbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _context = dbContext;
            _contextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            return View();
        }
        //登入
        public IActionResult Login()
        {
            return View();
        }
        //註冊
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        //发送消息页面
        [UserAuthen]
        public IActionResult send(int rid = 0)
        {
            ViewBag.rid = rid;
            //获取我的消息列表用户显示
            int uid = (int)_contextAccessor.HttpContext.Session.GetInt32("uid");
            //获取发信息给我和我发的用户
            IEnumerable<Message> list = _context.ChatMessages.Where(p => p.ToUserid == uid || p.FromUserid == uid)
                                                             .GroupBy(p => p.FromUserid)
                                                             .Select(p => new Message()
            {
                FromUserId = p.Key,
                ToUserId = uid
            }).ToList();
            ViewBag.uid = uid;
            //获取当前聊天的用户的信息返回
            if (rid != 0)
            {
                ViewBag.uinfo = _context.Users.Where(p => p.Id == rid).FirstOrDefault();
            }

            return View(list);
        }

        // 获取历史聊天记录 (根據接受人ID)查發送者
        [UserAuthen]
        [HttpGet]
        [Route("api/chat/history/{receiverId}")]
        public async Task<IActionResult> GetChatHistory(int receiverId)
        {
            // 查询双方的聊天记录
            int senderId = (int)_contextAccessor.HttpContext.Session.GetInt32("uid");
            var messages = await _context.ChatMessages
                .Where(m => (m.FromUserid == senderId && m.ToUserid == receiverId) ||
                            (m.FromUserid == receiverId && m.ToUserid == senderId))
                .OrderBy(m => m.SendTime) // 按时间顺序排列
                .ToListAsync();
            //批量修改聊天记录为已读 is_read(0=未讀, 1=已讀)
            string sql = $"update chat_message set is_read = 1 where ((from_userid = {senderId} and to_userid = {receiverId}) or (from_userid = {receiverId} and to_userid = {senderId})) and is_read = 0";
            //执行sql修改状态为已读
            _context.Database.ExecuteSqlRaw(sql);

            return Ok(messages);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
