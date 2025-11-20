using HanClothesShopProject.CommonUtil;
using HanClothesShopProject.Models;
using Microsoft.EntityFrameworkCore;
using UEditorNetCore;

namespace HanClothesShopProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            //==== 添加數據庫上下文  ======
            builder.Services.AddDbContext<dbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("dblink")));
            //添加會話服務
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSession();
            //添加ueditor服務
            builder.Services.AddUEditorService();
            builder.Services.AddMvc();

            builder.Services.AddScoped<MessageService>();
            // 添加 SignalR 服务
            builder.Services.AddSignalR();


            //建構一個過程
            var app = builder.Build();
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            
            //添加會話使用
            app.UseSession();
            // 將 HTTP 請求重導向 HTTPS
            app.UseHttpsRedirection();
            // 啟用靜態檔案服務，例如 HTML、CSS、圖片和 JavaScript 等
            app.UseStaticFiles();
            // 向中介軟體管道加入路由配置
            app.UseRouting();
            // 授權使用者存取安全資源
            app.UseAuthorization();
            // 配置 SignalR 路由
            app.MapHub<ChatHub>("/chathub");


            // 配置控制器與路由的對應關係，預設顯示控制器中對應的方法
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Login}/{id?}");

            // 執行應用程式
            app.Run();

        }
    }
}