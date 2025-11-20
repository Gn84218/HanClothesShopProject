using Microsoft.AspNetCore.Mvc.Filters;

namespace HanClothesShopProject.Filter
{
    //使用者身份驗證過濾器
    public class UserAuthen : ActionFilterAttribute
    {
        //執行方法 沒有問題才會繼續後續動作
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
        }
        //當前情球過來方法時 正在執行方法就被檢查
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            int uid = (int)context.HttpContext.Session.GetInt32("uid");
            if (uid == null)
            {
                //跳轉到登入頁面
                context.HttpContext.Response.Redirect("/Home/Index");
            }
        }
    }
}
