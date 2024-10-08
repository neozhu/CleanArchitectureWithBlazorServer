using System.Globalization;

namespace CleanArchitecture.Blazor.Server.UI.Components.Autocompletes;

public class LanguageAutocomplete<T> : MudAutocomplete<string>
{
    private List<CultureInfo> Languages { get;  set; }= CultureInfo.GetCultures(CultureTypes.SpecificCultures).ToList();
    public LanguageAutocomplete()
    {
        SearchFunc = SearchFunc_;
        Clearable = true;
        Dense = true;
        ResetValueOnEmptyText = true;
        ToStringFunc = x =>
        {
            var language = Languages.FirstOrDefault(lang => lang.Name.Equals(x));
            return language != null ? $"{language.DisplayName} ({language.Name})" : x;
        };
    }

    private Task<IEnumerable<string>> SearchFunc_(string value, CancellationToken cancellation = default)
    {
        // 如果输入为空，返回完整的语言列表；否则进行模糊搜索
        return string.IsNullOrEmpty(value)
            ? Task.FromResult(Languages.Select(lang => lang.Name).AsEnumerable())
            : Task.FromResult(Languages
                .Where(lang => Contains(lang, value))
                .Select(lang => lang.Name));
    }

    private static bool Contains(CultureInfo language, string value)
    {
        return language.DisplayName.Contains(value, StringComparison.InvariantCultureIgnoreCase) ||
               language.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase);
    }
}
