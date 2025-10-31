using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HanClothesShopProject.Models;

namespace HanClothesShopProject.Controllers
{
    public class CouponsController : Controller
    {
        private readonly dbContext _context;

        public CouponsController(dbContext context)
        {
            _context = context;
        }

        // GET: Coupons
        public async Task<IActionResult> Index(string keyword = "", string type = "", int page = 1)
        {
            IEnumerable<Coupon> list = _context.Coupons;
            if (!string.IsNullOrEmpty(keyword))
            {
                list = list.Where(p => p.Title.Contains(keyword));
            }
            if (!string.IsNullOrEmpty(type))
            {
                list = list.Where(p => p.DiscountType.Contains(type));
            }
           
            ViewBag.keyword = keyword;
            ViewBag.type = type;
            

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

        

        // GET: Coupons/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Coupons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,DiscountAmount,DiscountType,SumCount,StartDate,EndDate,UsedCount,ShouldMoney")] Coupon coupon)
        {
            if (ModelState.IsValid)
            {
                _context.Add(coupon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(coupon);
        }

        // GET: Coupons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Coupons == null)
            {
                return NotFound();
            }

            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null)
            {
                return NotFound();
            }
            return View(coupon);
        }

        // POST: Coupons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,DiscountAmount,DiscountType,SumCount,StartDate,EndDate,UsedCount,ShouldMoney")] Coupon coupon)
        {
            if (id != coupon.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(coupon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CouponExists(coupon.Id))
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
            return View(coupon);
        }

       

      
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.Coupons == null)
            {
                return Problem("Entity set 'dbContext.Coupons'  is null.");
            }
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon != null)
            {
                _context.Coupons.Remove(coupon);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CouponExists(int id)
        {
          return (_context.Coupons?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
