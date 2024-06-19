using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringUtils
{

    public static int[] SplitString2IntArr( string str, params char[] separator )
    {
        return Array.ConvertAll<string, int>( str.Split( separator ), Convert.ToInt32 );
    }

    public static int[] SplitString2IntArr( string str )
    {
        return SplitString2IntArr( str, ',' );
    }
}
