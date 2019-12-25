using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace QApp.Web.Controllers.Helpers {
    public class InjectDependencies : ActionFilterAttribute, IActionFilter {
        public override void OnActionExecuting(ActionExecutingContext actionContext) {
            base.OnActionExecuting(actionContext);
            try {
                App.Director.WithExternal_ResolveDependencies(actionContext.Controller);
            }
            catch {
                //Just in case this isn't handled nicely. Better to not crash the app repeatedly when restarting
                //if outside resources are frequently calling the API.
            }
        }
    }
}
