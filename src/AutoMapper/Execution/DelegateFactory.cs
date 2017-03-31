using System;
using System.Linq;
using System.Linq.Expressions;

namespace AutoMapper.Execution
{
    using static Expression;

    public static class DelegateFactory
    {
        private static readonly LockingConcurrentDictionary<Type, Func<object>> CtorCache = new LockingConcurrentDictionary<Type, Func<object>>(GenerateConstructor);

        public static Func<object> CreateCtor(Type type) => CtorCache.GetOrAdd(type);

        private static Func<object> GenerateConstructor(Type type)
        {
            var ctorExpr = GenerateConstructorExpression(type);

            return Lambda<Func<object>>(Convert(ctorExpr, typeof(object))).Compile();
        }

        public static Expression GenerateConstructorExpression(Type type, ProfileMap configuration) =>
            configuration.AllowNullDestinationValues
                ? GenerateConstructorExpression(type)
                : GenerateNonNullConstructorExpression(type);

        public static Expression GenerateNonNullConstructorExpression(Type type) => type.IsValueType()
            ? Default(type)
            : (type == typeof(string)
                ? Constant(string.Empty)
                : GenerateConstructorExpression(type)
            );

        public static Expression GenerateConstructorExpression(Type type)
        {
            if (type.IsValueType())
            {
                return Default(type);
            }

            if (type == typeof(string))
            {
                return Constant(null, typeof(string));
            }

            var constructors = type
                .GetDeclaredConstructors()
                .Where(ci => !ci.IsStatic);

            //find a ctor with only optional args
            var ctorWithOptionalArgs = constructors.FirstOrDefault(c => c.GetParameters().All(p => p.IsOptional));
            if (ctorWithOptionalArgs == null)
            {
                var ex = new ArgumentException(type + " needs to have a constructor with 0 args or only optional args", nameof(type));
                return Block(Throw(Constant(ex)), Constant(null));
            }
            //get all optional default values
            var args = ctorWithOptionalArgs
                .GetParameters()
                .Select(p => Constant(p.GetDefaultValue(), p.ParameterType)).ToArray();

            //create the ctor expression
            return New(ctorWithOptionalArgs, args);
        }
    }
}