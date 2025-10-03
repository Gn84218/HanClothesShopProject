using HanClothesShopProject.CommonUtil;
using HanClothesShopProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HanClothesShopProject.Controllers
{
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
            //取出 前端輸入帳密 與 資料庫一致
            User info = _dbContext.Users.Where(p => p.Phone == phone && p.Pwd == newPwd).FirstOrDefault();

            if (info != null)
            {
                //登入成功 使用 Session 記住當前訊息
                _httpContextAccessor.HttpContext.Session.SetInt32("id", info.Id);
                _httpContextAccessor.HttpContext.Session.SetInt32("role", (int)info.Role);
                _httpContextAccessor.HttpContext.Session.SetString("nickname", info.Nickname);
                _httpContextAccessor.HttpContext.Session.SetString("img", info.Img);
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
    }
}
