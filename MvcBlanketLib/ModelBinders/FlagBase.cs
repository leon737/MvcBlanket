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
using System.Linq;
using System.Reflection;

namespace MvcBlanketLib.ModelBinders
{
    public class FlagBase<TType, TFlag>
        where TType : FlagBase<TType, TFlag>, new()
    {
        protected TFlag Flag { get; set; }

        public static implicit operator string(FlagBase<TType, TFlag> flagValue)
        {
            var enumerationType = typeof(TFlag);
            if (!enumerationType.IsEnum)
                throw new ArgumentException();

            string enumerationFieldName = Enum.GetName(enumerationType, flagValue.Flag);
            if (string.IsNullOrEmpty(enumerationFieldName))
                throw new ArgumentException();

            var members = enumerationType.GetMember(enumerationFieldName);
            if (members.Count() != 1)
                throw new ArgumentException();

            var member = members.First();
            object[] attributes = member.GetCustomAttributes(typeof(FlagStringAttribute), false);
            if (attributes.Count() != 1)
                throw new ArgumentException();

            var attribute = attributes.First() as FlagStringAttribute;
            if (attribute == null)
                throw new ArgumentException();

            return attribute.StringRepresentation;
        }

        public static implicit operator FlagBase<TType, TFlag>(string flagValue)
        {
            var enumerationType = typeof(TFlag);
            if (!enumerationType.IsEnum)
                throw new ArgumentException();

            var members = enumerationType.GetFields();
            foreach (var member in members)
            {
                var attributes = member.GetCustomAttributes(typeof(FlagStringAttribute), false);
                if (attributes.Count() != 1) continue;

                var attribute = attributes.First() as FlagStringAttribute;
                if (attribute == null)
                    throw new ArgumentException();

                if (attribute.StringRepresentation != flagValue) continue;

                var flag = (TFlag)member.GetValue(null);
                return new TType { Flag = flag };
            }
            return null;
        }

        public static implicit operator TFlag(FlagBase<TType, TFlag> flagValue)
        {
            return flagValue.Flag;
        }
        
        public static implicit operator FlagBase<TType, TFlag>(TFlag flagValue)
        {
            return new FlagBase<TType, TFlag> { Flag = flagValue };
        }

    }
}
