using System.Collections;
using UnityEngine;

namespace BulletHell
{
    public class PlayerMovement : MonoBehaviour
    {
        public float speed;
        public TrailRenderer trailRenderer;
        private Vector2 input;
        private Vector2 mousePos;
        private Animator animator;
        private Rigidbody2D rigidbody;

        //时间有限,暂时用几个变量来写,后续有时间改成有限状态机来做代码会跟清晰后续也好维护
        private bool canDash = true, isDashing = false;
        private float dashingVelocity = 15f;
        private float dashintTime = 0.2f;
        private float dashInterval = 1.0f;

        //private Vector3 oldPosition;

        void Start( )
        {
            animator = GetComponent<Animator>( );
            rigidbody = GetComponent<Rigidbody2D>( );
        }

        void Update( )
        {
            //冲刺时 阻塞移动操作
            if ( isDashing )
            {
                return;
            }

            input.x = Input.GetAxisRaw( "Horizontal" );
            input.y = Input.GetAxisRaw( "Vertical" );

            rigidbody.velocity = input.normalized * speed;
            mousePos = Camera.main.ScreenToWorldPoint( Input.mousePosition );

            if ( mousePos.x > transform.position.x )
            {
                transform.rotation = Quaternion.Euler( new Vector3( 0, 0, 0 ) );
            }
            else
            {
                transform.rotation = Quaternion.Euler( new Vector3( 0, 180, 0 ) );
            }

            if ( input != Vector2.zero )
            {
                animator.SetBool( "isMoving", true );
            }
            else
            {
                animator.SetBool( "isMoving", false );
            }

            if ( canDash && Input.GetKeyDown( KeyCode.LeftShift ) )
            {
                canDash = false;
                isDashing = true;
                StartCoroutine( Dash( ) );
            }
        }

        //private void LateUpdate( )
        //{
        //    if ( oldPosition != transform.position )
        //    {
        //        oldPosition = transform.position;
        //        GameWatcher.DispathPlayerPositionEvent( oldPosition );
        //    }
        //}

        IEnumerator Dash( )
        {
            trailRenderer.enabled = true;
            trailRenderer.Clear();
            Vector2 mousePos = Camera.main.ScreenToWorldPoint( Input.mousePosition );
            var dir = ( mousePos - new Vector2( transform.position.x, transform.position.y ) ).normalized;
            rigidbody.velocity = dir.normalized * dashingVelocity;
            yield return new WaitForSeconds( dashintTime );
            isDashing = false;
            trailRenderer.enabled = false;
            yield return new WaitForSeconds( dashInterval );
            canDash = true;
        }

    }
}