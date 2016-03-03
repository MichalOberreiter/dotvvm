using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using DotVVM.Framework.Binding;
using DotVVM.Framework.Binding.Expressions;
using DotVVM.Framework.Routing;
using DotVVM.Framework.Runtime;
using DotVVM.Framework.Hosting;

namespace DotVVM.Framework.Controls
{
    public static class RouteLinkHelpers
    {

        private const string RouteParameterPrefix = "Param-";


        public static void WriteRouteLinkHrefAttribute(string routeName, HtmlGenericControl control, IHtmlWriter writer, IDotvvmRequestContext context)
        {
            if (!control.RenderOnServer)
            {
                var group = new KnockoutBindingGroup();
                group.Add("href", GenerateKnockoutHrefExpression(routeName, control, context));
                writer.AddKnockoutDataBind("attr", group);
            }
            else
            {
                writer.AddAttribute("href", EvaluateRouteUrl(routeName, control, context));
            }
        }

        public static string EvaluateRouteUrl(string routeName, HtmlGenericControl control, IDotvvmRequestContext context)
        {
            var coreUrl = GenerateRouteUrlCore(routeName, control, context);

            if ((bool)control.GetValue(Internal.IsSpaPageProperty))
            {
                return "#!/" + coreUrl;
            }
            else
            {
                return context.TranslateVirtualPath(coreUrl);
            }
        }

        private static string GenerateRouteUrlCore(string routeName, HtmlGenericControl control, IDotvvmRequestContext context)
        {
            var route = GetRoute(context, routeName);
            var parameters = ComposeNewRouteParameters(control, context, route);

            // evaluate bindings on server
            foreach (var param in parameters.Where(p => p.Value is IStaticValueBinding).ToList())
            {
                EnsureValidBindingType(param.Value as BindingExpression);
                parameters[param.Key] = ((ValueBindingExpression)param.Value).Evaluate(control, null);   // TODO: see below
            }

            // generate the URL
            return route.BuildUrl(parameters);
        }

        private static RouteBase GetRoute(IDotvvmRequestContext context, string routeName)
        {
            return context.Configuration.RouteTable[routeName];
        }

        public static string GenerateKnockoutHrefExpression(string routeName, HtmlGenericControl control, IDotvvmRequestContext context)
        {
            var link = GenerateRouteLinkCore(routeName, control, context);

            if ((bool)control.GetValue(Internal.IsSpaPageProperty))
            {
                return string.Format("'#!/' + {0}", link);
            }
            else
            {
                return string.Format("'{0}' + {1}", context.TranslateVirtualPath("~/"), link);
            }
        }

        private static string GenerateRouteLinkCore(string routeName, HtmlGenericControl control, IDotvvmRequestContext context)
        {
            var route = GetRoute(context, routeName);
            var parameters = ComposeNewRouteParameters(control, context, route);

            // generate the function call
            var sb = new StringBuilder();
            sb.Append("dotvvm.buildRouteUrl(");
            sb.Append(JsonConvert.SerializeObject(route.Url));
            sb.Append(", {");
            sb.Append(string.Join(", ", parameters.Select(p => TranslateRouteParameter(control, p))));
            sb.Append("})");
            return sb.ToString();
        }

        private static string TranslateRouteParameter(HtmlGenericControl control, KeyValuePair<string, object> param)
        {
            string expression = "";
            if (param.Value is IBinding)
            {
                EnsureValidBindingType(param.Value as IBinding);

                expression = (param.Value as IValueBinding)?.GetKnockoutBindingExpression()
                    ?? JsonConvert.SerializeObject((param.Value as IStaticValueBinding)?.Evaluate(control, null));
            }
            else
            {
                expression = JsonConvert.SerializeObject(param.Value);
            }
            return JsonConvert.SerializeObject(param.Key.ToLower()) + ": " + expression;
        }

        private static void EnsureValidBindingType(IBinding binding)
        {
            if (!(binding is IStaticValueBinding))
            {
                throw new Exception("Only value bindings are supported in <dot:RouteLink Param-xxx='' /> attributes!");
            }
        }

        private static Dictionary<string, object> ComposeNewRouteParameters(HtmlGenericControl control, IDotvvmRequestContext context, RouteBase route)
        {
            var parameters = new Dictionary<string, object>(route.DefaultValues, StringComparer.OrdinalIgnoreCase);
            foreach (var param in context.Parameters)
            {
                parameters[param.Key] = param.Value;
            }
            foreach (var attr in GetRouteParameters(control))
            {
                var parameterName = attr.Key.Substring(RouteParameterPrefix.Length);
                parameters[parameterName] = attr.Value;

                // remove the attribute because we don't want to be rendered
                control.Attributes.Remove(attr.Key);
            }
            return parameters;
        }

        private static List<KeyValuePair<string, object>> GetRouteParameters(HtmlGenericControl control)
        {
            return control.Attributes.Where(a => a.Key.StartsWith(RouteParameterPrefix, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }
}