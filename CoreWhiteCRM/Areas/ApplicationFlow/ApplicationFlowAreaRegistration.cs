using System.Web.Mvc;

namespace CoreWhiteCRM_Web.Areas.ApplicationFlow
{
    public class ApplicationFlowAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ApplicationFlow";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ApplicationFlow_default",
                "ApplicationFlow/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}