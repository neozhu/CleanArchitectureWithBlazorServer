using CleanArchitecture.Blazor.Application.Features.PicklistSets.DTOs;

namespace CleanArchitecture.Blazor.Server.UI.Components.Autocompletes;

public class TimeZoneAutocomplete<T> : MudAutocomplete<string>
{
    private List<TimeZoneInfo> TimeZones { get; set; }= TimeZoneInfo.GetSystemTimeZones().ToList();
    public TimeZoneAutocomplete()
    {
        SearchFunc = SearchFunc_;
        Dense = true;
        ResetValueOnEmptyText = true;
        ToStringFunc = x =>
        {
            var timeZone = TimeZones.FirstOrDefault(tz => tz.Id == x);
            return timeZone != null ? timeZone.DisplayName : x;
        };
    }

    private Task<IEnumerable<string>> SearchFunc_(string value, CancellationToken cancellation = default)
    {
        return string.IsNullOrEmpty(value)
            ? Task.FromResult(TimeZones.Select(tz => tz.Id).AsEnumerable())
            : Task.FromResult(TimeZones
                .Where(tz => Contains(tz, value))
                .Select(tz => tz.Id));
    }

    private static bool Contains(TimeZoneInfo timeZone, string value)
    {
        return timeZone.DisplayName.Contains(value, StringComparison.InvariantCultureIgnoreCase) ||
               timeZone.Id.Contains(value, StringComparison.InvariantCultureIgnoreCase);
    }
}