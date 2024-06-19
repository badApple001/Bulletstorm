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
    /// ��ǰʱ��תutcʱ���
    /// </summary>
    /// <param name="local_dateTime"></param>
    /// <returns></returns>
    public static long Convert2UTCTimestamp( DateTime local_dateTime )
    {
        return ( local_dateTime.ToUniversalTime( ).Ticks - 621355968000000000 ) / TimeSpan.TicksPerSecond;
    }

    /// <summary>
    /// ʱ���תutcʱ��
    /// </summary>
    /// <param name="timestamps"></param>
    /// <returns></returns>
    public static DateTime Convert2DateTime( long timestamps )
    {
        return new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc ).AddSeconds( timestamps );
    }

    /// <summary>
    /// ��ǰʱ��ת���ɱ�����ʱ���
    /// </summary>
    /// <param name="local_dateTime"></param>
    /// <returns></returns>
    public static long Convert2BeijingTimestamp( DateTime local_dateTime )
    {
        return ( Convert2BeijingDateTime( local_dateTime ).Ticks - 621355968000000000 ) / TimeSpan.TicksPerSecond;
    }

    /// <summary>
    /// ��ǰʱ��ת���ɱ���ʱ��
    /// </summary>
    /// <param name="local_dateTime"></param>
    /// <returns></returns>
    public static DateTime Convert2BeijingDateTime( DateTime local_dateTime )
    {
        return local_dateTime.ToUniversalTime( ).AddHours( 8 );
    }

    /// <summary>
    /// ��ȡ����0��0��0��
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
