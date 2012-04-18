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
using System.Linq.Expressions;
using EmitMapper;
using EmitMapper.MappingConfiguration;
using MvcBlanket.Ioc.Resolvers;
using MvcBlanketLib.Fluent;

namespace MvcBlanket.Ioc.ObjectMapper
{
    public class Mapper
    {
        static Maybe<TResult> MapCore<TSource, TResult>(TSource from, IMappingConfigurator configurator)
        {
            return GetMapperCore<TSource, TResult>(configurator).Map(from).ToMaybe();
        }

        static Maybe<TResult> MapCore<TSource, TResult>(TSource from, TResult to, IMappingConfigurator configurator)
        {
            return GetMapperCore<TSource, TResult>(configurator).Map(from, to).ToMaybe();
        }

        static ObjectsMapper<TSource, TResult> GetMapperCore<TSource, TResult>(IMappingConfigurator configurator)
        {
            return ObjectMapperManager.DefaultInstance.GetMapper<TSource, TResult>(configurator);
        }

        public static Maybe<TResult> Map<TSource, TResult>(TSource from)
        {
            return MapCore<TSource, TResult>(from, new DefaultMapConfig());
        }

        public static Maybe<TResult> Map<TSource, TResult>(TSource from, TResult to)
        {
            return MapCore(from, to, new DefaultMapConfig());
        }

        public static Maybe<TResult> MapIoc<TSource, TResult>(TSource from) where TResult : IIocViewModel
        {
            return GetMapperCore<TSource, TResult>(new DefaultMapConfig()).Map(from, ViewModelsResolver.Resolve<TResult>()).ToMaybe();
        }

        public static Maybe<TResult> MapTo<TSource, TResult>(TSource from, params Expression<Func<TSource, object>>[] ignore)
        {
            return MapCore<TSource, TResult>(from, new DefaultMapConfig().IgnoreMembers<TSource, TResult>(GetIgnoreList(ignore)));
        }

        public static Maybe<TResult> MapTo<TSource, TResult>(TSource from, TResult to, params Expression<Func<TSource, object>>[] ignore)
        {
            return MapCore(from, to, new DefaultMapConfig().IgnoreMembers<TSource, TResult>(GetIgnoreList(ignore)));
        }

        static string[] GetIgnoreList<TSource>(IEnumerable<Expression<Func<TSource, object>>> ignore)
        {
            return (from entry in ignore select (MemberExpression)entry.Body into body select body.Member into member select member.Name).ToArray();
        }
    }
}
