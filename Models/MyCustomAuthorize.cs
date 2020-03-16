using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ePRJ.Models
{
    public class MyCustomAuthorize : AuthorizeAttribute
    {
        public string LoginPage { get; set; }
        public string CustomRole { get; set; }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            bool skipAuthorization = filterContext.
                                                    ActionDescriptor.
                                                    IsDefined(typeof(AllowAnonymousAttribute), true)
                                                    || filterContext.ActionDescriptor.
                                                     ControllerDescriptor.
                                                    IsDefined(typeof(AllowAnonymousAttribute), true);

            if (skipAuthorization)
            {
                return;
            }
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.HttpContext.Response.Redirect(LoginPage);
            }
            base.OnAuthorization(filterContext);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {

                filterContext.HttpContext.Response.Redirect(LoginPage);

            base.HandleUnauthorizedRequest(filterContext);
        }
    }
}