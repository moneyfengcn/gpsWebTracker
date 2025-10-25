using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GpsTracker.Infrastructure.Razor
{
    [TypeFilter(typeof(LogFilterAttribute))]
    public abstract class BaseController : Controller
    {



 

        //public new ContentResult Json(object data)
        //{
        //    var json = data.ToJson();
        //    return Content(json, "application/json");
        //}

        public ActionResult PageView<TModel, TParam>(Query<TModel, TParam> model)
            where TParam : new()
        {
            if (model.__defaultView)
                return View(model);

            if (Request.IsAjax())
                return PartialView(model.__partialView, model);

            return View(model);
        }

        public ActionResult PageView<TModel, TParam>(string viewName, Query<TModel, TParam> model)
            where TParam : new()
        {
            if (model.__defaultView)
                return View(viewName, model);

            if (Request.IsAjax())
                return PartialView(model.__partialView, model);

            return View(viewName, model);
        }

        public ActionResult AjaxView<TModel>(string viewName, TModel model)
        {
            if (Request.IsAjax())
                return PartialView(viewName, model);
            return View(model);
        }

        protected string GetModelErrorMessage()
        {
            List<string> sb = new List<string>();
            //获取所有错误的Key
            List<string> Keys = ModelState.Keys.ToList();
            //获取每一个key对应的ModelStateDictionary
            foreach (var key in Keys)
            {
                var errors = ModelState[key].Errors.ToList();
                //将错误描述添加到sb中
                foreach (var error in errors)
                {
                    sb.Add(error.ErrorMessage);
                }
            }

            return string.Join(',', sb);
        }

    }

    static class HTTPExtensions
    {

        public static bool IsAjax(this HttpRequest req)
        {

            var result = req.Headers.ContainsKey("x-requested-with");
            //if (result)
            //{
            //    result = req.Headers["x-requested-with"] == "XMLHttpRequest";
            //}

            return result;
        }
    }
}
