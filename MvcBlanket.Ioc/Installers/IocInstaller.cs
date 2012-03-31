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
using Castle.Windsor;
using MvcBlanket.Ioc.Factories;
using System.Web.Mvc;
using MvcBlanket.Ioc.ModelBinders;
using MvcBlanket.Ioc.Resolvers;

namespace MvcBlanket.Ioc.Installers
{
    public class IocInstaller<T>
        where T : IController
    {
        private static IocInstaller<T> instance;
        private WindsorContainer container;

        private IocInstaller(string dataLayerAssemblyName)
        {
            container = new WindsorContainer();
            RegisterContainer(dataLayerAssemblyName);
            RegisterModelBinders();
            RegisterResolvers();
        }

        public static IocInstaller<T> Create(string dataLayerAssemblyName)
        {
            instance = new IocInstaller<T>(dataLayerAssemblyName);
            return instance;
        }

        public static IocInstaller<T> Instance
        {
            get { return instance; }
        }

        private void RegisterContainer(string dataLayerAssemblyName)
        {
            var controllerFactory = new WindsorControllerFactory(container.Kernel);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
            container.Install(new ControllerInstaller<T>(), new RepositoryInstaller(dataLayerAssemblyName));
        }

        private void RegisterModelBinders()
        {
            ModelBinderProviders.BinderProviders.Add(new IocViewModelBinderProvider(container.Kernel));
        }

        private void RegisterResolvers() 
        {
            ViewModelsResolver.Create(container.Kernel, typeof(T));
        }


    }
}
