using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Blazor.Application.Common.Extensions;

public static class DescriptionAttributeExtensions
{
    
    
    public static string GetMemberDisplayName<T, TProperty>(this T t, Expression<Func<T, TProperty>> property)
        where T : class
    {
        MemberExpression memberExpression = null;
        if (property.Body is MemberExpression me)
        {
            memberExpression = me;
        }
        else if (property.Body is UnaryExpression ue)
        {
           
            memberExpression = ue.Operand as MemberExpression;
        }

        if (memberExpression == null) return null;

        var memberInfo = memberExpression.Member;

        
        var displayAttribute = memberInfo.GetCustomAttribute<DisplayAttribute>(false);

        if (displayAttribute != null)
        {
           
            return displayAttribute.GetName();
        }
        return memberInfo.Name;
    }
    public static string GetClassDescription<T>(this T t) where T : class
    {
        if (t is null) t = (T)Activator.CreateInstance(typeof(T))!;
        var descriptionAttributes = t.GetType().GetCustomAttributes(typeof(DescriptionAttribute), false);
        if (descriptionAttributes.Any()) return (descriptionAttributes.First() as DescriptionAttribute)!.Description;
        return nameof(t);
    }
}
