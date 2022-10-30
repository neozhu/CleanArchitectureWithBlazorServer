using System.ComponentModel;

namespace CleanArchitecture.Blazor.Application.Common.Extensions;
public static class DescriptionAttributeExtensions
{
    public static string GetDescription(this Enum e)
    {
        var name = e.ToString();
        var memberInfo = e.GetType().GetMember(name)[0];
        var descriptionAttributes = memberInfo.GetCustomAttributes(typeof(DescriptionAttribute), inherit: false);
        if (descriptionAttributes.Any())
        {
            return ((DescriptionAttribute)descriptionAttributes.First()).Description;
        }
        return name;
    }
    public static string GetMemberDescription<T>(this T t, string memberName) where T : class
    {
        if (t is null) t = (T)Activator.CreateInstance(typeof(T))!;
        var memberInfo = t.GetType().GetMember(memberName)[0];
        var descriptionAttributes = memberInfo.GetCustomAttributes(typeof(DescriptionAttribute), inherit: false);
        if (descriptionAttributes.Any())
        {
            return (descriptionAttributes.First() as DescriptionAttribute)!.Description;
        }
        return memberName;
    }
    public static string GetClassDescription<T>(this T t) where T : class
    {
        if (t is null) t = (T)Activator.CreateInstance(typeof(T))!;
        var descriptionAttributes = t.GetType().GetCustomAttributes(typeof(DescriptionAttribute), inherit: false);
        if (descriptionAttributes.Any())
        {
            return (descriptionAttributes.First() as DescriptionAttribute)!.Description;
        }
        return nameof(t);

    }
}
