//==========================
// - FileName:      Assets/Frameworks/Scripts/Utility/DateTimeUtils.cs
// - Created:       ChenJC	
// - CreateTime:    2023-06-21-17:36:50
// - UnityVersion:  2021.3.22f1
// - Version:       1.0
// - Description:   
//==========================
using System;
using System.Globalization;

public class DateTimeUtils
{

    /// <summary>
    /// 当前时间转utc时间戳
    /// </summary>
    /// <param name="local_dateTime"></param>
    /// <returns></returns>
    public static long Convert2UTCTimestamp( DateTime local_dateTime )
    {
        return ( local_dateTime.ToUniversalTime( ).Ticks - 621355968000000000 ) / TimeSpan.TicksPerSecond;
    }

    /// <summary>
    /// 时间戳转utc时间
    /// </summary>
    /// <param name="timestamps"></param>
    /// <returns></returns>
    public static DateTime Convert2DateTime( long timestamps )
    {
        return new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc ).AddSeconds( timestamps );
    }

    /// <summary>
    /// 当前时间转换成北京的时间戳
    /// </summary>
    /// <param name="local_dateTime"></param>
    /// <returns></returns>
    public static long Convert2BeijingTimestamp( DateTime local_dateTime )
    {
        return ( Convert2BeijingDateTime( local_dateTime ).Ticks - 621355968000000000 ) / TimeSpan.TicksPerSecond;
    }

    /// <summary>
    /// 当前时间转换成北京时间
    /// </summary>
    /// <param name="local_dateTime"></param>
    /// <returns></returns>
    public static DateTime Convert2BeijingDateTime( DateTime local_dateTime )
    {
        return local_dateTime.ToUniversalTime( ).AddHours( 8 );
    }

    /// <summary>
    /// 获取当天0点0分0秒
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static DateTime GetMidnight( DateTime dateTime )
    {
        return dateTime.AddHours( -dateTime.Hour ).AddMinutes( -dateTime.Minute ).AddSeconds( -dateTime.Second ).AddMilliseconds( -dateTime.Millisecond );
    }

    public const string defaultDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";
    public static string DateTime2String( DateTime dateTime )
    {
        return dateTime.ToString( defaultDateTimeFormat, CultureInfo.InvariantCulture );
    }

    public static DateTime String2DateTime( string str )
    {
        return DateTime.ParseExact( str, defaultDateTimeFormat, CultureInfo.InvariantCulture );
    }
}
