using System.Globalization;

namespace Tgs.GameInterface.JDBService.Core;

public static class DateTimeExtensions
{
	private static readonly TimeZoneInfo _TimeZone;

	static DateTimeExtensions()
		=> _TimeZone = TimeZoneInfo.CreateCustomTimeZone(id: "eastern time", baseUtcOffset: TimeSpan.FromHours(-4), "eastern time", "eastern time");

	public static DateTime StringConvertToDatetime(this string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return DateTime.UnixEpoch;
		}

		var date = DateTime.ParseExact(str, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);

		return new DateTimeOffset(date, _TimeZone.GetUtcOffset(date)).UtcDateTime;
	}

	public static string DatetimeConvertToString(this DateTime date)
		=> TimeZoneInfo.ConvertTime(date.ToUniversalTime(), _TimeZone).ToString("dd-MM-yyyy HH:mm:00");
}
