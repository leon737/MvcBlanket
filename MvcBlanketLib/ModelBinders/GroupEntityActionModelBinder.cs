using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace MvcBlanketLib.ModelBinders
{
    public class GroupEntityActionModelBinder<T> : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var formData = controllerContext.HttpContext.Request.Form;
            var action = formData["action"];
            var ids = formData["chk"].Split(',').Where(s => !string.IsNullOrWhiteSpace(s));
            var groupAction = new GroupAction<T> { Action = action, Identifiers = ids.Select(i => (T)Convert.ChangeType(i, typeof(T))) };
            return groupAction;
        }
    }

    public interface IGroupAction
    {

    }

    public class GroupAction<T> : IGroupAction
    {
        public string Action { get; internal set; }
        public IEnumerable<T> Identifiers { get; internal set; }
    }
}
