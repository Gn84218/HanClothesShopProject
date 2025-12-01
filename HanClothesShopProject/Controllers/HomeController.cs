using System.Diagnostics;
using System.Text;
using HanClothesShopProject.CommonUtil;
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
        //使用算法協同過濾法 推薦商品
        private readonly RecommendationService _recommendationService;

        public HomeController(ILogger<HomeController> logger, 
                              dbContext dbContext, 
                              IHttpContextAccessor httpContextAccessor,
                              RecommendationService recommendationService=null)
        {
            _logger = logger;
            _context = dbContext;
            _contextAccessor = httpContextAccessor;
            _recommendationService = recommendationService;
        }

        public IActionResult Index()
        {
            //1.輪播圖 展示實現
            ViewBag.ChatList = _context.ImageCharts.Where(p => p.State == 1 ).OrderByDescending(p => p.Id).ToList();
            //獲取協同過濾法 推薦商品
            int id=0;
            if (_contextAccessor.HttpContext.Session.GetInt32("uid") != null)
            {
                id = (int)_contextAccessor.HttpContext.Session.GetInt32("uid");
                ViewBag.reList = _recommendationService.GetRecommendedProductsAsync(id, 12);
            }
            else
            {
                //用戶未登入 反回熱門商品12條
                ViewBag.reList = _recommendationService.GetPopularProductsAsync(12);
            }
            int numberOfProducts = 10;
            Random random = new Random();
            int total =_context.Products.Count();
            int min= 0;
            int max= total - numberOfProducts;
            int startIndex = random.Next(min, max+1);
            var rList=_context.Products.Where(p=>p.State==1)
                                       .OrderByDescending(p => p.Id)
                                       .Skip(startIndex)
                                       .Take(numberOfProducts)
                                       .ToList();
            ViewBag.blogList = _context.Articles.OrderByDescending(p => p.Id).Take(5).ToList();
            return View(rList);
        }
        //商品列表 
        public IActionResult List(string keyword = "", int cid = -1, string catename = "", int page = -1)
        {
            IEnumerable<Product> list = _context.Products.Include(p => p.CidNavigation);

            if (!string.IsNullOrEmpty(keyword))
            {
                list = list.Where(p => p.Title.Contains(keyword));
            }
            if (cid != -1)
            {
                list = list.Where(p => p.Cid == cid);
            }
            
            ViewBag.Keyword = keyword;
            ViewBag.Cid = cid;
            ViewBag.catename = catename;
            ViewBag.page = page;


            //獲取商業分類列表
            ViewBag.clist = _context.Categories.ToList();
            //獲取協同過濾法 推薦商品
            int id = 0;
            if (_contextAccessor.HttpContext.Session.GetInt32("uid") != null)
            {
                id = (int)_contextAccessor.HttpContext.Session.GetInt32("uid");
                ViewBag.reList = _recommendationService.GetRecommendedProductsAsync(id, 12);
            }
            else
            {
                //用戶未登入 反回熱門商品12條
                ViewBag.reList = _recommendationService.GetPopularProductsAsync(12);
            }
            //分頁條數
            int pageSize = 10;
            //總條數有多少
            var total = list.Count();
            //每頁10條的話，總共的可以分多少頁total/10
            // 21筆資料 每頁10條 問：可以分成幾頁？ 21/10 = 2.1 向上取整得到3 實際上可以分3頁
            ViewBag.pageNum = Math.Ceiling(Convert.ToDecimal(total) / Convert.ToDecimal(pageSize));
            // 分頁演算法原理顯示第一頁：（1-1）*10 = 0，10 得到的是 0-10 條
            // 顯示第二頁：（2-1）*10 = 10，10 得到的是 10-20 條
            list = list.OrderByDescending(p => p.Id).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return View(list);

        }
        //文章列表
        public IActionResult Article(string keyword = "", int page = 1)
        {
            IEnumerable<Article> list = _context.Articles;
            if (!string.IsNullOrEmpty(keyword))
            {
                list = list.Where(p => p.Title.Contains(keyword));
            }
            ViewBag.keyword = keyword;
            ViewBag.page = page;
            //獲取商業分類列表
            ViewBag.clist = _context.Categories.ToList();
            //獲取協同過濾法 推薦商品
            int id = 0;
            if (_contextAccessor.HttpContext.Session.GetInt32("uid") != null)
            {
                id = (int)_contextAccessor.HttpContext.Session.GetInt32("uid");
                ViewBag.reList = _recommendationService.GetRecommendedProductsAsync(id, 12);
            }
            else
            {
                //用戶未登入 反回熱門商品12條
                ViewBag.reList = _recommendationService.GetPopularProductsAsync(12);
            }

            //分頁條數
            int pageSize = 10;
            //總條數有多少
            var total = list.Count();
            //每頁10條的話，總共的可以分多少頁total/10
            // 21筆資料 每頁10條 問：可以分成幾頁？ 21/10 = 2.1 向上取整得到3 實際上可以分3頁
            ViewBag.pageNum = Math.Ceiling(Convert.ToDecimal(total) / Convert.ToDecimal(pageSize));
            // 分頁演算法原理顯示第一頁：（1-1）*10 = 0，10 得到的是 0-10 條
            // 顯示第二頁：（2-1）*10 = 10，10 得到的是 10-20 條
            list = list.OrderByDescending(p => p.Id).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return View(list);
        }
        //
        public IActionResult articeDetail(int id)
        {
             var info = _context.Articles.Where(p => p.Id == id).FirstOrDefault();
            if (info == null)
            {
                return Content("<script>" +
                    "alert('該手機已使用,請更換手機號');" +
                    "window.history.back(-1);" +
                    "</script>",
                    "text/html"
                    , Encoding.UTF8); 
            }
            info.Sight += 1;
            _context.Entry(info).State = EntityState.Modified;
            _context.SaveChanges();
            //獲取商業分類列表
            ViewBag.clist = _context.Categories.ToList();
            //獲取協同過濾法 推薦商品
            int uid = 0;
            if (_contextAccessor.HttpContext.Session.GetInt32("uid") != null)
            {
                uid = (int)_contextAccessor.HttpContext.Session.GetInt32("uid");
                ViewBag.reList = _recommendationService.GetRecommendedProductsAsync(uid, 12);
            }
            else
            {
                //用戶未登入 反回熱門商品12條
                ViewBag.reList = _recommendationService.GetPopularProductsAsync(12);
            }
            return View(info);
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

        //商品收藏 根據商品id儲存
        [UserAuthen]
        [HttpPost]
        public IActionResult AddSave(int id)
        {
            if (id == null)
            {
                return Json(new { code = 201, msg = "未找到商品ID" });
            }
            Product product = _context.Products.Where(p => p.Id == id).FirstOrDefault();
            if (product == null)
            {
                return Json(new { code = 202, msg = "未找到商品" });
            }
            int uid = (int)_contextAccessor.HttpContext.Session.GetInt32("uid");
            var info = _context.ProductSaves.Where(p => p.Uid == uid && p.Pid == id).FirstOrDefault();
            if (info != null)
            {
                return Json(new { code = 203, msg = "您已收藏該商品，請勿重複收藏" });
            }
            ProductSave ps = new ProductSave()
            {
                Uid = uid,
                Pid = id,
                Createtime = DateTime.Now
            };
            _context.ProductSaves.Add(ps);
            _context.SaveChanges();
            return Json(new { code = 200, msg = "收藏成功" });    
        }

        //添加購物車
        [UserAuthen]
        [HttpPost]
        public IActionResult AddCart(int id,int aid,int number)
        {
            if (id == null)
            {
                return Json(new { code = 201, msg = "未找到商品ID" });
            }
            if (aid == null)
            {
                return Json(new { code = 202, msg = "未找到商品屬性ID" });
            }
            Product product = _context.Products.Where(p => p.Id == id).FirstOrDefault();
            if (product == null)
            {
                return Json(new { code = 203, msg = "未找到商品" });
            }
            int uid = (int)_contextAccessor.HttpContext.Session.GetInt32("uid");
            Cart info = _context.Carts.Where(c => c.Uid == uid && c.Pid == id).FirstOrDefault();
            if (info == null) {
                //新增購物車
                Cart c = new Cart()
                {
                    Uid = uid,
                    Pid = id,
                    Number = number,
                    Createtime = DateTime.Now,
                    Aid = aid
                };
                _context.Carts.Add(c);
            }
            else
            {
                //修改購物車數量
                info.Number += number;
               _context.Entry(info).State = EntityState.Modified;
            }
            return Json(new { code = 200, msg = "加入購物車成功" });
        }

        //錯誤頁面
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
