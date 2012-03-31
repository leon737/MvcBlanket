/*
MVC Blanket Library Copyright (C) 2009-2012 Leonid Gordo

This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; 
either version 3.0 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; 
if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 
*/

using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using System;

namespace MvcBlanket.Ioc.Resolvers
{
    public class ViewModelsResolver
    {
        private readonly IKernel kernel;
        private static ViewModelsResolver resolver;

        private ViewModelsResolver(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public static ViewModelsResolver Create(IKernel kernel, Type t)
        {
            resolver = new ViewModelsResolver(kernel);
            resolver.InstallViewModels(t);
            return resolver;
        }

        private void InstallViewModels(Type t)
        {
            kernel.Register(FindViewModels(t).Configure(c => c.LifestylePerWebRequest()));
        }

        private BasedOnDescriptor FindViewModels(Type t)
        {
            return AllTypes.FromAssemblyContaining(t).BasedOn<IIocViewModel>();
        }

        public static ViewModelsResolver Instance
        {
            get { return resolver; }
        }

        public T ResolveViewModel<T>()
             where T : IIocViewModel
        {
            return kernel.Resolve<T>();
        }

        public static T Resolve<T>()
            where T : IIocViewModel
        {
            return Instance.ResolveViewModel<T>();
        }
    }

    public interface IIocViewModel { }
}
