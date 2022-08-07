using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        var descriptionAttribute = memberInfo.GetCustomAttributes(typeof(DescriptionAttribute), inherit: false)[0] as DescriptionAttribute;
        if (descriptionAttribute is not null)
            return descriptionAttribute.Description;
        else
            return memberName;
    }
    public static string GetClassDescription<T>(this T t) where T : class
    {
        var descriptionAttribute = t.GetType().GetCustomAttributes(typeof(DescriptionAttribute), inherit: false)[0]
            as DescriptionAttribute;
        if (descriptionAttribute is not null)
            return descriptionAttribute.Description;
        else
            return nameof(t);

    }
}
