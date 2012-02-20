/*******************************************************************\
* Module Name: Lt.Helpers
* 
* File Name: Helpers/ParameterRebinder.cs
*
* Warnings:
*
* Issues:
*
* Created:  09 Jul 2011
* Author:   Leonid Gordo  [ leonardpt@gmail.com ]
*
\***********************************************************************/

using System.Collections.Generic;
using System.Linq.Expressions;

namespace MvcBlanketLib.Helpers
{
    public class ParameterRebinder : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> map;

        public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }
        

        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {

            ParameterExpression replacement;
            if (map.TryGetValue(p, out replacement))
                p = replacement;
            return base.VisitParameter(p);
        }
    }
}
