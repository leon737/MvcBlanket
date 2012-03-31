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
using System.Web.Mvc;
using Castle.MicroKernel;
using MvcBlanket.Ioc.Resolvers;

namespace MvcBlanket.Ioc.ModelBinders
{
    public class IocViewModelBinderProvider : IModelBinderProvider
    {

        private readonly IKernel kernel;

        public IocViewModelBinderProvider(IKernel kernel)
        {
            this.kernel = kernel;
        }
        public IModelBinder GetBinder(Type modelType)
        {
            if (!typeof(IIocViewModel).IsAssignableFrom(modelType))
                return null;
            return new IocViewModelBinder(kernel);
        }
    }
}
