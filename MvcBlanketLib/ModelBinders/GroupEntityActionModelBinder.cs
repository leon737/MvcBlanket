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
