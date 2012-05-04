/*
MVC Blanket Library Copyright (C) 2009-2012 Leonid Gordo

This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; 
either version 3.0 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; 
if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MvcBlanketLib.ModelBinders
{
    public class GroupEntityActionModelBinder<TAction, TValues> : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var formData = controllerContext.HttpContext.Request.Form;
            var action = formData["action"];
            var ids = formData["chk"].Split(',').Where(s => !string.IsNullOrWhiteSpace(s));
            if (typeof(TAction) != typeof(string))
                return new GroupAction<TAction, TValues> { Action = ConvertAction(action), Identifiers = ids.Select(i => (TValues)Convert.ChangeType(i, typeof(TValues))) };
            return new GroupAction<TValues> { Action = action, Identifiers = ids.Select(i => (TValues)Convert.ChangeType(i, typeof(TValues))) };
        }

        private static TAction ConvertAction(string action)
        {
            return typeof(TAction).IsEnum ? ((ActionEnumConverter<TAction>)action).Value : (TAction)Convert.ChangeType(action, typeof(TAction));
        }
    }

    internal class ActionEnumConverter<T> : FlagBase<ActionEnumConverter<T>, T>
    {
        public ActionEnumConverter() { }

        public ActionEnumConverter(T enumValue)
        {
            Flag = enumValue;
        }

        public T Value
        {
            get { return Flag; }
        }
    }

    public interface IGroupAction { }

    public class GroupAction<T> : GroupAction<string, T> { }

    public class GroupAction<TAction, TValues> : IGroupAction
    {
        public TAction Action { get; internal set; }
        public IEnumerable<TValues> Identifiers { get; internal set; }
    }
}
