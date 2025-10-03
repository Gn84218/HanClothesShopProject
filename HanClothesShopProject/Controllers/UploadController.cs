using Microsoft.AspNetCore.Mvc;

namespace HanClothesShopProject.Controllers
{
    public class UploadController : Controller

    {
        private readonly IWebHostEnvironment _environment;

        public UploadController(IWebHostEnvironment environment)
        {
            this._environment = environment;
        }

        // 上传方法,用于异步上传功能的实现【其他涉及图片上传操作的，都是执行下面的方法】
        [HttpPost]
        public async Task<IActionResult> file(IFormFile pic)
        {
            try
            {
                if (pic != null)
                {
                    if (pic.Length == 0)
                    {
                        return Content("209"); //获取上传的图片
                    }
                    else
                    {
                        //判断文件的后缀名，是否符合条件
                        string backFix = Path.GetExtension(pic.FileName);
                        if (backFix != ".gif" && backFix != ".png" && backFix != ".jpg" && backFix != ".jpeg")
                        {
                            return Content("210"); //格式不对
                        }
                        string fileName = DateTime.Now.ToString("MMddHHmmss") + backFix;
                        string filePath = _environment.ContentRootPath + "wwwroot\\pic\\" + fileName;

                        //使用文件输入输出流在指定位置创建文件
                        using (var fs = System.IO.File.Create(filePath))
                        {
                            await pic.CopyToAsync(fs);
                        }
                        //返回路径
                        return Content("/pic/" + fileName);
                    }
                }
                else
                {
                    return Content("300"); //图片不能为空
                }
            }
            catch (Exception ex)
            {
                return Content("400"); //上传失败
            }
        }

    }
}
