using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using DotVVM.Framework.Compilation.ControlTree;

namespace DotVVM.Framework.Utils
{
    public static class ExpressionUtils
    {
        public static Expression For(Expression length, Func<Expression, Expression> bodyFactory)
        {
            var i = Expression.Variable(typeof(int), "i");
            return Expression.Block(new[] { i },
                Expression.Assign(i, Expression.Constant(0)),
                While(Expression.LessThan(i, length),
                    Expression.Block(
                        bodyFactory(i),
                        Expression.PostIncrementAssign(i)
                    )
                )
            );
        }

        public static Expression Foreach(Expression enumerable, Func<Expression, Expression> bodyFactory)
        {
            // use for(int i = 0; i < a.Length; i++) ... for Arrays
            if (enumerable.Type.IsArray) {
                return For(Expression.ArrayLength(enumerable), i => bodyFactory(Expression.ArrayIndex(enumerable, i)));
            } else {
                var getEnumMethod = Expression.Call(enumerable, "GetEnumerator", Type.EmptyTypes);
                var enumerator = Expression.Variable(getEnumMethod.Type, "enumerator");
                var body = new List<Expression>();
                body.Add(Expression.Assign(enumerator, getEnumMethod));
                body.Add(Expression.TryFinally(While(
                    condition: Expression.Call(enumerator, nameof(IEnumerator<int>.MoveNext), Type.EmptyTypes),
                    body: bodyFactory(Expression.Property(enumerator, nameof(IEnumerator<int>.Current)))
                ),
                    Expression.Call(enumerator, "Dispose", Type.EmptyTypes)
                ));
                return Expression.Block(new[] { enumerator }, body);
            }
        }
        public static Expression While(Expression conditionBody)
        {
            var brkLabel = Expression.Label();
            return Expression.Loop(
                Expression.IfThen(Expression.Not(conditionBody), Expression.Goto(brkLabel)), brkLabel);
        }

        public static Expression While(Expression condition, Expression body)
        {
            var brkLabel = Expression.Label();
            return Expression.Loop(
                Expression.IfThenElse(condition, body, Expression.Goto(brkLabel)), brkLabel);
        }

        public static Expression ConvertToObject(this Expression expr)
        {
            if (expr.Type == typeof(object)) return expr;
            else if (expr.Type == typeof(void)) return WrapInReturnNull(expr);
            else return Expression.Convert(expr, typeof(object));
        }

        public static Expression WrapInReturnNull(Expression expr)
        {
            return Expression.Block(expr, Expression.Constant(null));
        }

        public static Expression Indexer(Expression instance, Expression index)
        {
            return Expression.Property(instance,
                            instance.Type.GetTypeInfo().GetProperty("Item", new[] { index.Type }),
                            index);
        }

        public static Expression Replace(Expression ex, string paramName, Expression paramValue)
        {
            return Replace(ex, new[] { new KeyValuePair<string, Expression>(paramName, paramValue) });
        }

        public static Expression Replace(Expression ex, IEnumerable<KeyValuePair<string, Expression>> namedParameters)
        {
            var visitor = new ReplaceVisitor();
            foreach (var p in namedParameters)
            {
                visitor.NamedParams.Add(p.Key, p.Value);
            }
            return visitor.Visit(ex);
        }

        public static Expression Replace(LambdaExpression ex, params Expression[] parameters)
        {
            var visitor = new ReplaceVisitor();
            for (int i = 0; i < parameters.Length; i++)
            {
                visitor.Params.Add(ex.Parameters[i], parameters[i]);
            }
            var result = visitor.Visit(ex.Body);
            if (result.CanReduce) result = result.Reduce();
            return result;
        }

        [Conditional("DEBUG")]
        public static void AddDebug(this IList<Expression> block, [CallerFilePath] string fileName = null, [CallerLineNumber] int lineNumber = -1)
        {
            if (fileName == null || lineNumber < 0) throw new ArgumentException();
            block.Add(Expression.DebugInfo(Expression.SymbolDocument(fileName), lineNumber, 0, lineNumber + 1, 0));
        }

        #region Replace overloads

        public static Expression Replace<T1, TRes>(Expression<Func<T1, TRes>> ex, Expression p1)
        {
            return Replace(ex as LambdaExpression, p1);
        }

        public static Expression Replace<T1, T2, TRes>(Expression<Func<T1, T2, TRes>> ex, Expression p1, Expression p2)
        {
            return Replace(ex as LambdaExpression, p1, p2);
        }

        public static Expression Replace<T1, T2, T3, TRes>(Expression<Func<T1, T2, T3, TRes>> ex, Expression p1, Expression p2, Expression p3)
        {
            return Replace(ex as LambdaExpression, p1, p2, p3);
        }

        public static Expression Replace<T1, T2, T3, T4, TRes>(Expression<Func<T1, T2, T3, T4, TRes>> ex, Expression p1, Expression p2, Expression p3, Expression p4)
        {
            return Replace(ex as LambdaExpression, p1, p2, p3, p4);
        }
        public static Expression Replace(Expression<Action> ex)
        {
            return Replace(ex as LambdaExpression);
        }

        public static Expression Replace<T1>(Expression<Action<T1>> ex, Expression p1)
        {
            return Replace(ex as LambdaExpression, p1);
        }

        public static Expression Replace<T1, T2>(Expression<Action<T1, T2>> ex, Expression p1, Expression p2)
        {
            return Replace(ex as LambdaExpression, p1, p2);
        }

        public static Expression Replace<T1, T2, T3>(Expression<Action<T1, T2, T3>> ex, Expression p1, Expression p2, Expression p3)
        {
            return Replace(ex as LambdaExpression, p1, p2, p3);
        }

        public static Expression Replace<T1, T2, T3, T4>(Expression<Action<T1, T2, T3, T4>> ex, Expression p1, Expression p2, Expression p3, Expression p4)
        {
            return Replace(ex as LambdaExpression, p1, p2, p3, p4);
        }
        #endregion

        public static void ForEachMember(this Expression expression, Action<MemberInfo> memberAction)
        {
            var visitor = new MemberInfoWalkingVisitor();
            visitor.MemberInfoAction = memberAction;
            visitor.Visit(expression);
        }

        private class ReplaceVisitor : ExpressionVisitor
        {
            public Dictionary<ParameterExpression, Expression> Params { get; } = new Dictionary<ParameterExpression, Expression>();
            public Dictionary<string, Expression> NamedParams { get; } = new Dictionary<string, Expression>();
            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (Params.ContainsKey(node)) return Params[node];
                else if (!string.IsNullOrEmpty(node.Name) && NamedParams.ContainsKey(node.Name)) return NamedParams[node.Name];
                else return base.VisitParameter(node);
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (node.Method.DeclaringType == typeof(Stub)) {
                    switch (node.Method.Name) {
                        case nameof(Stub.Assign):
                            return Visit(Expression.Assign(node.Arguments[0], node.Arguments[1]));
                        case nameof(Stub.Throw):
                            return Visit(Expression.Throw(node.Arguments[0], node.Type));
                        default:
                            throw new NotImplementedException();
                    }
                }
                return base.VisitMethodCall(node);
            }
        }

        internal static Expression Switch(Expression condition, Expression defaultCase, SwitchCase[] cases)
        {
            if (cases.Length == 0) {
                return Expression.Block(condition, defaultCase);
            } else {
                return Expression.Switch(condition, defaultCase, cases);
            }
        }

        private class MemberInfoWalkingVisitor: ExpressionVisitor
        {
            public Action<MemberInfo> MemberInfoAction { get; set; }
            public Action<PropertyInfo> PropertyInfoAction { get; set; }
            public Action<MethodInfo> MethodInfoAction { get; set; }

            private void Invoke(MethodInfo method)
            {
                if (method == null) return;
                if (MethodInfoAction != null) MethodInfoAction(method);
                if (MemberInfoAction != null) MemberInfoAction(method);
            }

            private void Invoke(PropertyInfo property)
            {
                if (property == null) return;
                if (MethodInfoAction != null) PropertyInfoAction(property);
                if (MemberInfoAction != null) MemberInfoAction(property);
            }

            private void Invoke(MemberInfo memberInfo)
            {
                if (memberInfo == null) return;
                if (memberInfo is PropertyInfo) Invoke(memberInfo as PropertyInfo);
                else if (memberInfo is MethodInfo) Invoke(memberInfo as MethodInfo);
                else if (MemberInfoAction != null) MemberInfoAction(memberInfo);
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                Invoke(node.Method);
                return base.VisitMethodCall(node);
            }

            protected override Expression VisitBinary(BinaryExpression node)
            {
                Invoke(node.Method);
                return base.VisitBinary(node);
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                Invoke(node.Member);
                return base.VisitMember(node);
            }

            protected override Expression VisitUnary(UnaryExpression node)
            {
                Invoke(node.Method);
                return base.VisitUnary(node);
            }

            protected override Expression VisitNew(NewExpression node)
            {
                Invoke(node.Constructor);
                return base.VisitNew(node);
            }
        }

        public static class Stub
        {
            public static T Assign<T>(T left, T right) => throw new NotImplementedException();
            public static void Throw(Exception e) => throw new NotImplementedException();
            public static T Throw<T>(Exception e) => throw new NotImplementedException();
        }

        /// <summary>
        /// will execute operators, property and field accesses on constant expression, so it will be cleaner
        /// </summary>
        public static Expression OptimizeConstants(this Expression ex)
        {
            var v = new ConstantsOptimizingVisitor();
            return v.Visit(ex);
        }

        private class ConstantsOptimizingVisitor : ExpressionVisitor
        {
            protected override Expression VisitMember(MemberExpression node)
            {
                if (node.Member.MemberType == MemberTypes.Property)
                {
                    var i = Visit(node.Expression);
                    if (i.NodeType == ExpressionType.Constant)
                    {
                        var ce = (ConstantExpression)i;
                        var prop = ce.Type.GetProperty(node.Member.Name);
                        var val = prop.GetValue(ce.Value);
                        return Expression.Constant(val, prop.PropertyType);
                    }
                    else return node;
                }
                else if (node.Member.MemberType == MemberTypes.Field)
                {
                    var i = Visit(node.Expression);
                    if (i.NodeType == ExpressionType.Constant)
                    {
                        var ce = (ConstantExpression)i;
                        var f = node.Member as FieldInfo;
                        var val = f.GetValue(ce.Value);
                        return Expression.Constant(val, f.FieldType);
                    }
                    else return node;
                }
                else return base.VisitMember(node);
            }
            protected override Expression VisitBinary(BinaryExpression node)
            {
                var l = Visit(node.Left);
                var lc = l as ConstantExpression;
                var r = Visit(node.Right);
                var rc = r as ConstantExpression;
                if (lc != null && rc != null)
                {
                    if (node.Method != null)
                        return Expression.Constant(node.Method.Invoke(null, new object[] { lc.Value, rc.Value }));
                    else return node;
                }
                else return base.VisitBinary(node);
            }

            protected override Expression VisitUnary(UnaryExpression node)
            {
                var op = Visit(node.Operand);
                if (op is ConstantExpression)
                {
                    return Expression.Constant(
                        node.Method.Invoke(null, new object[] { (op as ConstantExpression).Value }), node.Type);
                }
                else return base.VisitUnary(node);
            }
        }

        public static List<T> AllDescendants<T>(this Expression expression, Func<T, bool> predicate = null)
            where T: Expression
        {
            var result = new List<T>();
            new AnonymousActionVisitor { Replacer = a => { if (a is T t && predicate?.Invoke(t) != false) result.Add(t); return a; } }.Visit(expression);
            return result;
        }

        public static Expression ReplaceAll(this Expression expr, Func<Expression, Expression> replacer)
        {
            return new AnonymousActionVisitor { Replacer = replacer }.Visit(expr);
        }
        
        class AnonymousActionVisitor: ExpressionVisitor
        {
            public Func<Expression, Expression> Replacer { get; set; }
            public override Expression Visit(Expression expr)
            {
                return Replacer(base.Visit(expr));
            }
        }
    }

}
