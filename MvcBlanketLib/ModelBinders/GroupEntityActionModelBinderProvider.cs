using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace MvcBlanketLib.ModelBinders
{
    public class GroupEntityActionModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(Type modelType)
        {
            if (!typeof(IGroupAction).IsAssignableFrom(modelType))
                return null;
            var genericType = modelType.GetGenericArguments().First();
            var modelBinderType = typeof(GroupEntityActionModelBinder<>).MakeGenericType(genericType);
            var modelBinder = Activator.CreateInstance(modelBinderType);
            return (IModelBinder)modelBinder;

        }
    }

}
