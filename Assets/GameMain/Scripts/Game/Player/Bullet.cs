using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public float bulletHarm = 60;
    public float critRate = 0.3f;
    public GameObject explosionPrefab;
    new private Rigidbody2D rigidbody;

    void Awake( )
    {
        rigidbody = GetComponent<Rigidbody2D>( );
    }

    public void SetSpeed( Vector2 direction )
    {
        rigidbody.velocity = direction * speed;
    }

    private void OnTriggerEnter2D( Collider2D other )
    {
        // Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        GameObject exp = ObjectPool.Instance.GetObject( explosionPrefab );
        exp.transform.position = transform.position;

        // Destroy(gameObject);
        ObjectPool.Instance.PushObject( gameObject );


        if ( other.TryGetComponent<Creature>( out var creature ) )
        {
            float critHarm = Random.value < critRate ? 2 : 1;
            float realHarm = bulletHarm * critHarm * creature.defense;
            creature.Hp -= realHarm;
            MsgFire.Event(
                GameEventName.ON_FLY_HARMTEXT,
                creature.transform.position, critHarm > 1 ? HarmText.HarmType.crit : HarmText.HarmType.physics,
                ( int ) realHarm );
        }
    }
}
