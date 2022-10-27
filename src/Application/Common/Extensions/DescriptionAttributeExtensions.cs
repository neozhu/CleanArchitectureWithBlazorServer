using System.ComponentModel;

namespace CleanArchitecture.Blazor.Application.Common.Extensions;
public static class DescriptionAttributeExtensions
{
    public static string ToDescriptionString(this Enum val)
    {
        var attributes = (DescriptionAttribute[]?)val.GetType().GetField(val.ToString())?.GetCustomAttributes(typeof(DescriptionAttribute), false);

        return attributes?.Length > 0
            ? attributes[0].Description
            : val.ToString();
    }
    public static string GetEnumDescriptionOrName(this Enum e)
    {
        var name = e.ToString();
        var memberInfo = e.GetType().GetMember(name)[0];
        var descriptionAttributes = memberInfo.GetCustomAttributes(typeof(DescriptionAttribute), inherit: false);
        if (!descriptionAttributes.Any())
            return name;
        return ((DescriptionAttribute)descriptionAttributes.First()).Description;
    }
    public static string GetMemberDescription<T>(this T t, string memberName) where T : class
    {
        var memberInfo = t.GetType().GetMember(memberName)[0];
        var descriptionAttributes = memberInfo.GetCustomAttributes(typeof(DescriptionAttribute), inherit: false);
        if (descriptionAttributes.Any())
            return (descriptionAttributes.First() as DescriptionAttribute)!.Description;
        else
            return memberName;
    }
    public static string GetClassDescription<T>(this T t) where T : class
    {
        var descriptionAttributes = t.GetType().GetCustomAttributes(typeof(DescriptionAttribute), inherit: false);
        if (descriptionAttributes.Any())
            return (descriptionAttributes.First() as DescriptionAttribute)!.Description;
        else
            return nameof(t);

    }
}
