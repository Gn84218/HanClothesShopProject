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
    public class ImageChartController : Controller
    {
        private readonly dbContext _context;

        public ImageChartController(dbContext context)
        {
            _context = context;
        }

        // GET: 輪播圖管理列表
        public async Task<IActionResult> Index(string url = "", int state=-1, int page = 1)
        {
            IEnumerable<ImageChart> list = _context.ImageCharts;
            if (!string.IsNullOrEmpty(url))
            {
                list = list.Where(p => p.Url.Contains(url));
            }
            if (state!=-1)
            {
                list = list.Where(p => p.State == state);
            }
            
            ViewBag.Url = url;
            ViewBag.State = state;
            

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

        

        // GET: ImageChart/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ImageChart/Create 保存增加
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Url,ImageUrl,Createtime,State")] ImageChart imageChart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(imageChart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(imageChart);
        }

        // GET: ImageChart/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ImageCharts == null)
            {
                return NotFound();
            }

            var imageChart = await _context.ImageCharts.FindAsync(id);
            if (imageChart == null)
            {
                return NotFound();
            }
            return View(imageChart);
        }

        // POST: ImageChart/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Url,ImageUrl,Createtime,State")] ImageChart imageChart)
        {
            if (id != imageChart.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(imageChart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImageChartExists(imageChart.Id))
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
            return View(imageChart);
        }

      

        // POST: ImageChart/Delete/5
       
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.ImageCharts == null)
            {
                return Problem("Entity set 'dbContext.ImageCharts'  is null.");
            }
            var imageChart = await _context.ImageCharts.FindAsync(id);
            if (imageChart != null)
            {
                _context.ImageCharts.Remove(imageChart);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ImageChartExists(int id)
        {
          return (_context.ImageCharts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
