using UnityEngine;

public class Rifle : Gun
{
    [SerializeField] private float harm = 105;
    [SerializeField] LayerMask checkLayer;
    public float critRate = 0.333f;


    protected override void Fire()
    {
        animator.SetTrigger("Shoot");
        RaycastHit2D hit2D = Physics2D.Raycast(muzzlePos.position, direction, 30, checkLayer );

        GameObject bullet = ObjectPool.Instance.GetObject(bulletPrefab);
        LineRenderer tracer = bullet.GetComponent<LineRenderer>();
        tracer.SetPosition(0, muzzlePos.position);
        tracer.SetPosition(1, hit2D.point);

        GameObject shell = ObjectPool.Instance.GetObject(shellPrefab);
        shell.transform.position = shellPos.position;
        shell.transform.rotation = shellPos.rotation;


        if( hit2D.transform.TryGetComponent<Creature>(out var creature) )
        {
            float critHarm = Random.value < critRate ? 2 : 1;
            float realHarm = harm * critHarm * creature.defense;
            creature.Hp -= realHarm;
            MsgFire.Event(
                GameEventName.ON_FLY_HARMTEXT,
                creature.transform.position, critHarm > 1 ? HarmText.HarmType.crit : HarmText.HarmType.physics,
                ( int ) realHarm );
        }
    }
}
