using UnityEngine;

public class Tween
{
    public static float GetTime( float time, float nDuration )
    {
        return time > nDuration ? nDuration : time;
    }

    public static float Linear( float time, float nBegin, float nChange, float nDuration )
    {
        time = GetTime( time, nDuration );
        return nChange * time / nDuration + nBegin;
    }

    public class Quad
    {
        public static float easeIn( float time, float nBegin, float nChange, float nDuration )
        {
            time = GetTime( time, nDuration );
            time = time / nDuration;
            return nChange * time * time + nBegin;
        }

        public static float easeOut( float time, float nBegin, float nChange, float nDuration )
        {
            time = GetTime( time, nDuration );
            time = time / nDuration;
            return -nChange * time * ( time - 2 ) + nBegin;
        }

        public static float easeInOut( float time, float nBegin, float nChange, float nDuration )
        {
            time = GetTime( time, nDuration );
            time = time / ( nDuration / 2 );
            if ( time < 1 )
            {
                return nChange / 2 * time * time + nBegin;
            }
            time--;
            return -nChange / 2 * ( time * ( time - 2 ) - 1 ) + nBegin;
        }
    }

    public class Cubic
    {
        public static float easeIn( float time, float nBegin, float nChange, float nDuration )
        {
            time = GetTime( time, nDuration );
            time = time / nDuration;
            return nChange * time * time * time + nBegin;
        }

        public static float easeOut( float time, float nBegin, float nChange, float nDuration )
        {
            time = GetTime( time, nDuration );
            time = time / nDuration - 1;
            return nChange * ( time * time * time + 1 ) + nBegin;
        }

        public static float easeInOut( float time, float nBegin, float nChange, float nDuration )
        {
            time = GetTime( time, nDuration );
            time = time / ( nDuration / 2 );
            if ( time < 1 )
            {
                return nChange / 2 * time * time * time + nBegin;
            }
            time -= 2;
            return nChange / 2 * ( time * time * time + 2 ) + nBegin;
        }
    }

    public class Quart
    {
        public static float easeIn( float time, float nBegin, float nChange, float nDuration )
        {
            time = GetTime( time, nDuration );
            time = time / nDuration;
            return nChange * time * time * time * time + nBegin;
        }

        public static float easeOut( float time, float nBegin, float nChange, float nDuration )
        {
            time = GetTime( time, nDuration );
            time = time / nDuration - 1;
            return -nChange * ( time * time * time * time - 1 ) + nBegin;
        }

        public static float easeInOut( float time, float nBegin, float nChange, float nDuration )
        {
            time = GetTime( time, nDuration );
            time = time / ( nDuration / 2 );
            if ( time < 1 )
            {
                return nChange / 2 * time * time * time * time + nBegin;
            }

            time -= 2;
            return -nChange / 2 * ( time * time * time * time - 2 ) + nBegin;
        }
    }

    public class Quint
    {
        public static float easeIn( float time, float nBegin, float nChange, float nDuration )
        {
            time = GetTime( time, nDuration );
            time = time / nDuration;
            return nChange * time * time * time * time * time + nBegin;
        }

        public static float easeOut( float time, float nBegin, float nChange, float nDuration )
        {
            time = GetTime( time, nDuration );
            time = time / nDuration - 1;
            return nChange * ( time * time * time * time * time + 1 ) + nBegin;
        }

        public static float easeInOut( float time, float nBegin, float nChange, float nDuration )
        {
            time = GetTime( time, nDuration );
            time = time / ( nDuration / 2 );
            if ( time < 1 )
            {
                return nChange / 2 * time * time * time * time * time + nBegin;
            }

            time -= 2;
            return nChange / 2 * ( time * time * time * time * time + 2 ) + nBegin;
        }
    }

    public class Sine
    {
        public static float easeIn( float time, float nBegin, float nChange, float nDuration )
        {
            time = GetTime( time, nDuration );
            return -nChange * Mathf.Cos( time / nDuration * ( Mathf.PI / 2 ) ) + nChange + nBegin;
        }

        public static float easeOut( float time, float nBegin, float nChange, float nDuration )
        {
            time = GetTime( time, nDuration );
            return nChange * Mathf.Sin( time / nDuration * ( Mathf.PI / 2 ) ) + nBegin;
        }

        public static float easeInOut( float time, float nBegin, float nChange, float nDuration )
        {
            time = GetTime( time, nDuration );
            return -nChange / 2 * ( Mathf.Cos( Mathf.PI * time / nDuration ) - 1 ) + nBegin;
        }
    }

    public class Expo
    {
        public static float easeIn( float time, float nBegin, float nChange, float nDuration )
        {
            time = GetTime( time, nDuration );
            if ( time == 0 )
            {
                return nBegin;
            }
            else
            {
                return nChange * Mathf.Pow( 2, 10 * ( time / nDuration - 1 ) ) + nBegin;
            }
        }

        public static float easeOut( float time, float nBegin, float nChange, float nDuration )
        {
            time = GetTime( time, nDuration );
            if ( time == nDuration )
            {
                return nBegin + nChange;
            }
            else
            {
                return nChange * ( -Mathf.Pow( 2, -10 * time / nDuration ) + 1 ) + nBegin;
            }
        }

        public static float easeInOut( float time, float nBegin, float nChange, float nDuration )
        {
            time = GetTime( time, nDuration );
            if ( time == 0 )
            {
                return nBegin;
            }
            if ( time == nDuration )
            {
                return nBegin + nChange;
            }

            time = time / ( nDuration / 2 );
            if ( time < 1 )
            {
                return nChange / 2 * Mathf.Pow( 2, 10 * ( time - 1 ) ) + nBegin;
            }
            time--;
            return nChange / 2 * ( -Mathf.Pow( 2, -10 * time ) + 2 ) + nBegin;
        }
    }

    public class Circ
    {
        public static float easeIn( float time, float nBegin, float nChange, float nDuration )
        {
            time = GetTime( time, nDuration );
            time = time / nDuration;
            return -nChange * ( Mathf.Sqrt( 1 - time * time ) - 1 ) + nBegin;
        }

        public static float easeOut( float time, float nBegin, float nChange, float nDuration )
        {
            time = GetTime( time, nDuration );
            time = time / nDuration - 1;
            return nChange * Mathf.Sqrt( 1 - time * time ) + nBegin;
        }

        public static float easeInOut( float time, float nBegin, float nChange, float nDuration )
        {
            time = GetTime( time, nDuration );
            time = time / ( nDuration / 2 );
            if ( time < 1 )
            {
                return -nChange / 2 * ( Mathf.Sqrt( 1 - time * time ) - 1 ) + nBegin;
            }
            time -= 2;
            return nChange / 2 * ( Mathf.Sqrt( 1 - time * time ) + 1 ) + nBegin;
        }
    }

    public class Elastic
    {
        public static float easeIn( float time, float nBegin, float nChange, float nDuration, float a = 0, float p = 0 )
        {
            time = GetTime( time, nDuration );
            if ( time == 0 )
            {
                return nBegin;
            }
            time = time / nDuration;
            if ( time == 1 )
            {
                return nBegin + nChange;
            }
            if ( p == 0 )
            {
                p = nDuration * 0.3f;
            }

            float s;
            if ( a == 0 || a < Mathf.Abs( nChange ) )
            {
                a = nChange;
                s = p / 4;
            }
            else
            {
                s = p / ( 2 * Mathf.PI ) * Mathf.Asin( nChange / a );
            }
            time -= 1;
            return -( a * Mathf.Pow( 2, 10 * time ) * Mathf.Sin( ( time * nDuration - s ) * ( 2 * Mathf.PI ) / p ) ) + nBegin;
        }
        public static float easeOut( float time, float nBegin, float nChange, float nDuration, float a = 0, float p = 0 )
        {
            time = GetTime( time, nDuration );
            if ( time == 0 )
            {
                return nBegin;
            }

            time = time / nDuration;
            if ( time == 1 )
            {
                return nBegin + nChange;
            }

            if ( p == 0 )
            {
                p = nDuration * 0.3f;
            }

            float s;
            if ( a == 0 || a < Mathf.Abs( nChange ) )
            {
                a = nChange;
                s = p / 4;
            }
            else
            {
                s = p / ( 2 * Mathf.PI ) * Mathf.Asin( nChange / a );
            }
            return ( a * Mathf.Pow( 2, -10 * time ) * Mathf.Sin( ( time * nDuration - s ) * ( 2 * Mathf.PI ) / p ) + nChange + nBegin );
        }
        public static float easeInOut( float time, float nBegin, float nChange, float nDuration, float a = 0, float p = 0 )
        {
            time = GetTime( time, nDuration );
            if ( time == 0 )
            {
                return nBegin;
            }

            time = time / ( nDuration / 2 );
            if ( time == 2 )
            {
                return nBegin + nChange;
            }

            if ( p == 0 )
            {
                p = nDuration * ( 0.3f * 1.5f );
            }

            float s;
            if ( a == 0 || a < Mathf.Abs( nChange ) )
            {
                a = nChange;
                s = p / 4;
            }
            else
            {
                s = p / ( 2 * Mathf.PI ) * Mathf.Asin( nChange / a );
            }

            if ( time < 1 )
            {
                time -= 1;
                return -0.5f * ( a * Mathf.Pow( 2, 10 * time ) * Mathf.Sin( ( time * nDuration - s ) * ( 2 * Mathf.PI ) / p ) ) + nBegin;
            }
            time -= 1;
            return a * Mathf.Pow( 2, -10 * time ) * Mathf.Sin( ( time * nDuration - s ) * ( 2 * Mathf.PI ) / p ) * 0.5f + nChange + nBegin;
        }
    }

    public class Back
    {
        public static float easeIn( float time, float nBegin, float nChange, float nDuration, float s = 0 )
        {
            time = GetTime( time, nDuration );
            if ( s == 0 )
            {
                s = 1.70158f;
            }
            time = time / nDuration;
            return nChange * time * time * ( ( s + 1 ) * time - s ) + nBegin;
        }

        public static float easeOut( float time, float nBegin, float nChange, float nDuration, float s = 0 )
        {
            time = GetTime( time, nDuration );
            if ( s == 0 )
            {
                s = 1.70158f;
            }
            time = time / nDuration - 1;
            return nChange * ( time * time * ( ( s + 1 ) * time + s ) + 1 ) + nBegin;
        }

        public static float easeInOut( float time, float nBegin, float nChange, float nDuration, float s = 0 )
        {
            time = GetTime( time, nDuration );
            if ( s == 0 )
            {
                s = 1.70158f;
            }

            time = time / ( nDuration / 2 );
            if ( time < 1 )
            {
                return nChange / 2 * ( time * time * ( ( ( s *= 1.525f ) + 1 ) * time - s ) ) + nBegin;
            }
            time -= 2;
            return nChange / 2 * ( time * time * ( ( ( s *= 1.525f ) + 1 ) * time + s ) + 2 ) + nBegin;
        }
    }

    public class Bounce
    {
        public static float easeIn( float time, float nBegin, float nChange, float nDuration )
        {
            time = GetTime( time, nDuration );
            return nChange - easeOut( nDuration - time, 0, nChange, nDuration ) + nBegin;
        }

        public static float easeOut( float time, float nBegin, float nChange, float nDuration )
        {
            time = GetTime( time, nDuration );

            time = time / nDuration;
            if ( time < ( 1 / 2.75f ) )
            {
                return nChange * ( 7.5625f * time * time ) + nBegin;
            }
            else if ( time < ( 2 / 2.75f ) )
            {
                time -= 1.5f / 2.75f;
                return nChange * ( 7.5625f * time * time + 0.75f ) + nBegin;
            }
            else if ( time < ( 2.5f / 2.75f ) )
            {
                time -= 2.25f / 2.75f;
                return nChange * ( 7.5625f * time * time + 0.9375f ) + nBegin;
            }
            else
            {
                time -= 2.625f / 2.75f;
                return nChange * ( 7.5625f * time * time + 0.984375f ) + nBegin;
            }
        }

        public static float easeInOut( float time, float nBegin, float nChange, float nDuration )
        {
            time = GetTime( time, nDuration );
            if ( time < nDuration / 2 )
            {
                return easeIn( time * 2, 0, nChange, nDuration ) * 0.5f + nBegin;
            }
            else return easeOut( time * 2 - nDuration, 0, nChange, nDuration ) * 0.5f + nChange * 0.5f + nBegin;
        }
    }
}
