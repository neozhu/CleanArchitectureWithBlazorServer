namespace CleanArchitecture.Blazor.Application.Features.Loggers.Specifications;
#nullable disable warnings
public class LoggerAdvancedSpecification : Specification<Logger>
{
   public LoggerAdvancedSpecification(LoggerAdvancedFilter filter)
{
    // 获取当前时区
    var localZone = TimeZoneInfo.Local;
    // 获取当前UTC时间
    var utcNow = DateTime.UtcNow;
    // 计算当前时区相对于UTC的偏移量
    var timeZoneOffset = localZone.GetUtcOffset(utcNow).TotalHours;

    // 将UTC时间加上偏移量转换为本地时间
    var localNow = utcNow.AddHours(timeZoneOffset);
    var localToday = localNow.Date;

    // 转换本地“今天”的开始时间回UTC时间进行查询
    var startOfTodayLocalAsUtc = TimeZoneInfo.ConvertTimeToUtc(localToday, localZone);
    // 转换本地“今天”的结束时间回UTC时间进行查询（注意，我们需要的是结束时间的23:59:59.9999999）
    var endOfTodayLocalAsUtc = TimeZoneInfo.ConvertTimeToUtc(localToday.AddDays(1).AddTicks(-1), localZone);

    // 同样地，转换过去30天的开始时间回UTC时间进行查询
    var startOfLast30DaysLocalAsUtc = TimeZoneInfo.ConvertTimeToUtc(localToday.AddDays(-30), localZone);

    // 构建查询条件
    Query.Where(p => p.TimeStamp >= startOfTodayLocalAsUtc && p.TimeStamp < endOfTodayLocalAsUtc, filter.ListView == LogListView.CreatedToday)
         .Where(p => p.TimeStamp >= startOfLast30DaysLocalAsUtc, filter.ListView == LogListView.Last30days)
         .Where(p => p.Level == filter.Level.ToString(), filter.Level is not null)
         .Where(x => x.Message.Contains(filter.Keyword) || x.Exception.Contains(filter.Keyword), !string.IsNullOrEmpty(filter.Keyword));
}
}
