using HanClothesShopProject.CommonUtil;
using HanClothesShopProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HanClothesShopProject.Controllers
{
    //一般用戶 登入註冊控制器
    public class LoginController : Controller
    {
        private readonly dbContext _dbContext;
        //HttpContextAccessor的接口 添加依賴
        private readonly IHttpContextAccessor _httpContextAccessor;
       
        
        //注入
        public LoginController(dbContext dbContext, IHttpContextAccessor HttpContextAccessor) { 
          _dbContext=dbContext;
            _httpContextAccessor = HttpContextAccessor;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Index(string phone ,string pwd)
        {
            //查詢帳號密碼是否一致
            string newPwd = PasswordHelper.HashPasswordWithMD5(pwd, PasswordHelper.GenerateSalt());
            //取出 數據庫 帳號符合輸入帳號 密碼符合加密後密碼 的第一筆資料
            User info = _dbContext.Users.Where(p => p.Phone == phone && p.Pwd == newPwd).FirstOrDefault();

            if (info != null)
            {
                //登入成功 使用 Session 記住當前訊息
                _httpContextAccessor.HttpContext.Session.SetInt32("id", info.Id);
                _httpContextAccessor.HttpContext.Session.SetInt32("role", (int)info.Role);
                _httpContextAccessor.HttpContext.Session.SetString("nickname", info.Nickname);
                _httpContextAccessor.HttpContext.Session.SetString("img2", info.Img);
                return Ok(new { code = 200, msg = "login success" });       
            }
            return Ok(new { code = 201,msg = "帳號或密碼錯誤" });
        }
        //管理員退出 (清除當前管理員Session資料)
        public IActionResult LogOut() {
            _httpContextAccessor.HttpContext.Session.Remove("id");
            _httpContextAccessor.HttpContext.Session.Remove("role");
            _httpContextAccessor.HttpContext.Session.Remove("nickname");
            _httpContextAccessor.HttpContext.Session.Remove("img");
            //跳回登入介面
            return Redirect("/Login/Index");
        }

        //執行用戶登入功能
        [HttpPost]
        public IActionResult DoLogin(string phone, string pwd)
        {
            //查詢帳號密碼是否一致
            //前端輸入的密碼加密 (數據庫為加密之密碼)
            string newPwd = PasswordHelper.HashPasswordWithMD5(pwd, PasswordHelper.GenerateSalt());
            //取出 數據庫 帳號符合輸入帳號 密碼符合加密後密碼 的第一筆資料
            User info = _dbContext.Users.Where(p => p.Phone == phone && p.Pwd == newPwd && p.Role ==0 ).FirstOrDefault();

            if (info != null)
            {
                //登入成功 使用 Session 記住當前訊息
                _httpContextAccessor.HttpContext.Session.SetInt32("uid", info.Id);               
                _httpContextAccessor.HttpContext.Session.SetString("nickname", info.Nickname);
                _httpContextAccessor.HttpContext.Session.SetString("img", info.Img);
                return Ok(new { code = 200, msg = "login success" });
            }
            return Ok(new { code = 201, msg = "帳號或密碼錯誤" });
        }
        //執行用戶註冊功能
        public IActionResult DoRegister(User user)
        {
            //1.判斷帳號是否已存在
            //1.檢查手機(帳號)是否存在 //FirstOrDefault()默認沒有返回null
            var info = _dbContext.Users.Where(p => p.Phone == user.Phone).FirstOrDefault();
            if (info != null)
            {
                return Content(
                    "<script>" +
                    "alert('該手機已使用,請更換手機號');" +
                    "window.history.back(-1);" +
                    "</script>",
                    "text/html"
                    , Encoding.UTF8);
            }
            //2.密碼加密 和預設個人資料值
            user.Pwd = PasswordHelper.HashPasswordWithMD5(user.Pwd, PasswordHelper.GenerateSalt());
            user.Role = 0;//用戶
            user.Img = "/assets/head.png";
            user.Nickname = "新用戶";
            user.Sex = "男";
            user.Age = 18;
            _dbContext.Add(user);
            int state= _dbContext.SaveChanges();
            //2.判斷是否註冊成功
            if (state > 0)
            {
                return Content(
                    "<script>" +
                    "alert('註冊成功,請登入');" +
                    "window.location.href='/Home/Login'"+
                    "</script>",
                    "text/html"
                    , Encoding.UTF8);
            }
            else
            {
                return Content(
                    "<script>" +
                    "alert('註冊失敗，請重新註冊');" +
                    "window.history.back(-1);" +
                    "</script>",
                    "text/html"
                    , Encoding.UTF8);
            }
            
            //3.執行修改
        }
        //用戶退出
        public IActionResult UserLogOut()
        {
            _httpContextAccessor.HttpContext.Session.Remove("uid");
            _httpContextAccessor.HttpContext.Session.Remove("nickname");
            _httpContextAccessor.HttpContext.Session.Remove("img");
            //跳回登入介面
            return Redirect("/Home/Index");
        }
    }
}
