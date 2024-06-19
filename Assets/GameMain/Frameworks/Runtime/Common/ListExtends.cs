using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtends
{

    public static List<T> AddRepeat<T>( this List<T> self, T value, int count ) where T : struct
    {
        for ( int i = 0; i < count; i++ )
        {
            self.Add( value );
        }
        return self;
    }
}
