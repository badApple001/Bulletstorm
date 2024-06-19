using System;
using System.Globalization;
using System.IO;
using UnityEngine;


/// <summary>
/// 通用时间 将所有时间转换成北京时间来处理
/// </summary>
public struct TDateTime
{
    /// <summary>
    /// 北京的时间戳
    /// </summary>
    private long timeStamps;

    public long TimeStamps { get { return timeStamps; } }

    public TDateTime( DateTime dateTime ) : this( )
    {
        DateTime beijing = dateTime.ToUniversalTime( ).AddHours( 8 );
        Refresh( beijing );
    }

    public TDateTime( long timeStamp ) : this( )
    {
        Refresh( new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc ).AddSeconds( timeStamp ) );
    }

    public TDateTime( string dateTimeString ) : this( )
    {
        DateTime beijing = DateTime.ParseExact( dateTimeString, DateTimeUtils.defaultDateTimeFormat, CultureInfo.InvariantCulture ).ToUniversalTime( ).AddHours( 8 );
        Refresh( beijing );
    }

    private void Refresh( DateTime beijing )
    {
        timeStamps = ( beijing.Ticks - 621355968000000000 ) / 10000000;
        Year = beijing.Year;
        Month = beijing.Month;
        Day = beijing.Day;
        Hour = beijing.Hour;
        Minute = beijing.Minute;
        Second = beijing.Second;
        Millisecond = beijing.Millisecond;
    }

    public static TDateTime Now
    {
        get
        {
            return new TDateTime( DateTime.Now );
        }
    }

    public DateTime ToDateTime( )
    {
        return new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc ).AddSeconds( timeStamps );
    }

    public override string ToString( )
    {
        return ToDateTime( ).ToString( DateTimeUtils.defaultDateTimeFormat, CultureInfo.InvariantCulture );
    }

    public byte[] Serialize( )
    {
        using ( MemoryStream ms = new MemoryStream( ) )
        using ( BinaryWriter writer = new BinaryWriter( ms ) )
        {
            Int32 flag = 0;
            flag |= ( byte ) 99 << 14;
            flag |= ( byte ) 106 << 7;
            flag |= ( byte ) 99;
            writer.Write( flag );
            writer.Write( timeStamps );
            return ms.ToArray( );
        }
    }

    public static TDateTime Deserialize( byte[] bytes )
    {
        using ( MemoryStream ms = new MemoryStream( bytes ) )
        using ( BinaryReader reader = new BinaryReader( ms ) )
        {
            Int32 flag = reader.ReadInt32( );
            Debug.Assert( flag == 1635683, "TDateTime data format error." );
            long timestamps = reader.ReadInt64( );
            return new TDateTime( timestamps );
        }
    }


    public int Year { private set; get; }
    public int Month { private set; get; }
    public int Day { private set; get; }
    public int Hour { private set; get; }
    public int Minute { private set; get; }
    public int Second { private set; get; }
    public int Millisecond { private set; get; }

    public void AddYears( int year )
    {
        Refresh( ToDateTime( ).AddYears( year ) );
    }

    public void AddMonths( int month )
    {
        Refresh( ToDateTime( ).AddMonths( month ) );
    }

    public void AddDays( int day )
    {
        //timeStamps += day * 24 * 60 * 60;
        Refresh( ToDateTime( ).AddDays( day ) );
    }

    public void AddHours( int hour )
    {
        //timeStamps += hour * 60 * 60;
        Refresh( ToDateTime( ).AddHours( hour ) );
    }

    public void AddMinutes( int minute )
    {
        //timeStamps += minute * 60;
        Refresh( ToDateTime( ).AddMinutes( minute ) );
    }

    public void AddSeconds( int second )
    {
        //timeStamps += second;
        Refresh( ToDateTime( ).AddSeconds( second ) );
    }

    public void AddMilliseconds( int milliseconds )
    {
        //timeStamps += milliseconds / 1000;
        Refresh( ToDateTime( ).AddMilliseconds( milliseconds ) );
    }

    /// <summary>
    /// 获取当前时间的0点0分
    /// </summary>
    /// <returns></returns>
    public TDateTime GetToday0Hour0Minute0Seconds( )
    {
        DateTime dateTime = ToDateTime( );
        return new TDateTime( dateTime.AddHours( -dateTime.Hour ).AddMinutes( -dateTime.Minute ).AddSeconds( -dateTime.Second ).AddMilliseconds( -dateTime.Millisecond ) );
    }


    public static TDateTime operator +( TDateTime d, TimeSpan t )
    {
        return new TDateTime( d.ToDateTime( ) + t );
    }

    public static TimeSpan operator -( TDateTime d1, TDateTime d2 )
    {
        return d1.ToDateTime( ) - d2.ToDateTime( );
    }

    public static TDateTime operator -( TDateTime d1, TimeSpan t )
    {
        return new TDateTime( d1.ToDateTime( ) - t );
    }

    public static bool operator ==( TDateTime d1, TDateTime d2 )
    {
        return d1.timeStamps == d2.timeStamps;
    }

    public static bool operator !=( TDateTime d1, TDateTime d2 )
    {
        return d1.timeStamps != d2.timeStamps;
    }

    public static bool operator <( TDateTime d1, TDateTime d2 )
    {
        return d1.timeStamps < d2.timeStamps;
    }

    public static bool operator >( TDateTime d1, TDateTime d2 )
    {
        return d1.timeStamps > d2.timeStamps;
    }

    public static bool operator <=( TDateTime d1, TDateTime d2 )
    {
        return d1.timeStamps <= d2.timeStamps;
    }

    public static bool operator >=( TDateTime d1, TDateTime d2 )
    {
        return d1.timeStamps >= d2.timeStamps;
    }

    public override bool Equals( object obj )
    {
        if ( obj is TDateTime tdateTime )
        {
            return tdateTime.timeStamps == this.timeStamps;
        }
        else
        {
            return base.Equals( obj );
        }
    }

    public override int GetHashCode( )
    {
        return base.GetHashCode( );
    }
}
