﻿using System;
using System.Linq.Expressions;
using AutoMapper.Internal;

namespace AutoMapper
{
    internal static class ExpressionExtensions
    {
        public static Expression ReplaceParameters(this LambdaExpression exp, params Expression[] replace)
            => ExpressionFactory.ReplaceParameters(exp, replace);

        public static Expression ConvertReplaceParameters(this LambdaExpression exp, params Expression[] replace)
            => ExpressionFactory.ConvertReplaceParameters(exp, replace);

        public static Expression Replace(this Expression exp, Expression old, Expression replace)
            => ExpressionFactory.Replace(exp, old, replace);

        public static LambdaExpression Concat(this LambdaExpression expr, LambdaExpression concat)
            => ExpressionFactory.Concat(expr, concat);

        public static Expression IfNotNull(this Expression expression, Type destinationType)
            => ExpressionFactory.IfNotNull(expression, destinationType);

        public static Expression RemoveIfNotNull(this Expression expression, params Expression[] expressions)
            => ExpressionFactory.RemoveIfNotNull(expression, expressions);

        public static Expression IfNullElse(this Expression expression, params Expression[] ifElse)
            => ExpressionFactory.IfNullElse(expression, ifElse);
    }
}