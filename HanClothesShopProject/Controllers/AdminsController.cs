using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HanClothesShopProject.Models;
using System.Text;
using Microsoft.AspNetCore.Identity;
using HanClothesShopProject.CommonUtil;

namespace HanClothesShopProject.Controllers
{
    public class AdminsController : Controller
    {
        private readonly dbContext _context;

        public AdminsController(dbContext context)
        {
            _context = context;
        }

        // 管理員列表
        public async Task<IActionResult> Index(string phone="",string nickname="",string sex="",int page=1)
        {
            IEnumerable<User> list =_context.Users.Where(p => p.Role == 1);
            if (!string.IsNullOrEmpty(phone))
            {
                list = list.Where(p => p.Phone.Contains(phone));
            }
            if (!string.IsNullOrEmpty(nickname))
            {
                list = list.Where(p => p.Nickname.Contains(nickname));
            }
            if (!string.IsNullOrEmpty(sex))
            {
                list = list.Where(p => p.Sex.Contains(sex));
            }
            ViewBag.Phone = phone;
            ViewBag.NickName = nickname;
            ViewBag.Sex = sex;
            
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

        // GET: User/Details/5 
        //管理員詳情
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: User/Create
        //添加管理員
        public IActionResult Create()
        {
            return View();
        }

        //儲存內容
        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Phone,Pwd,Nickname,Sex,Introduce,Age,Img,Mibao,Role")] User user)
        {
            //1.檢查手機(帳號)是否存在 //FirstOrDefault()默認沒有返回null
            var info =_context.Users.Where(p=>p.Phone==user.Phone).FirstOrDefault();
            if (info != null)
            {
                return Content(
                    "<script>" +
                    "alert('該手機已使用,請更換手機號');" +
                    "window.history.back(-1);" +
                    "</script>",
                    "text/html"
                    ,Encoding.UTF8);
            }
            //2.當前密碼加密
            user.Pwd = PasswordHelper.HashPasswordWithMD5(user.Pwd, PasswordHelper.GenerateSalt());
            user.Role = 1;//管理員
            //
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        //編輯
        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        //保存編輯
        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Phone,Pwd,Nickname,Sex,Introduce,Age,Img,Mibao,Role")] User user,string oldPwd)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //更改窗密碼不等於 原有密碼 重新加密保存˙
                    if (oldPwd != user.Pwd)
                    {
                        user.Pwd = PasswordHelper.HashPasswordWithMD5(user.Pwd,PasswordHelper.GenerateSalt());
                    }
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'dbContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
          return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
