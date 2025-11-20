using Microsoft.AspNetCore.Mvc.Filters;

namespace HanClothesShopProject.Filter
{
    //超級管理員專用  權限驗證過濾器
    public class AdminPowerAuthen : ActionFilterAttribute
    {
        //執行方法 沒有問題才會繼續後續動作
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
        }
        //當前情球過來方法時 正在執行方法就被檢查
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            int role = (int)context.HttpContext.Session.GetInt32("role");
            //角色權限非超及管理員(2)返回登入頁面
            if (role== null || role!=null&& role!=2)
            {
                //跳轉到登入頁面
                context.HttpContext.Response.Redirect("/Login/Index");
            }
        }
    }
}
