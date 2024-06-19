using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lasergun : Gun
{
    private GameObject effect;
    private LineRenderer laser;
    private bool isShooting;

    protected override void Start()
    {
        base.Start();
        laser = muzzlePos.GetComponent<LineRenderer>();
        effect = transform.Find("Effect").gameObject;
    }

    protected override void Shoot()
    {
        direction = (mousePos - new Vector2(transform.position.x, transform.position.y)).normalized;
        transform.right = direction;

        if (Input.GetButtonDown("Fire1"))
        {
            isShooting = true;
            laser.enabled = true;
            effect.SetActive(true);
        }
        if (Input.GetButtonUp("Fire1"))
        {
            isShooting = false;
            laser.enabled = false;
            effect.SetActive(false);
        }
        animator.SetBool("Shoot", isShooting);

        if (isShooting)
        {
            Fire();
        }
    }

    protected override void Fire()
    {
        RaycastHit2D hit2D = Physics2D.Raycast(muzzlePos.position, direction, 30);

        // Debug.DrawLine(muzzlePos.position, hit2D.point);
        laser.SetPosition(0, muzzlePos.position);
        laser.SetPosition(1, hit2D.point);

        effect.transform.position = hit2D.point;
        effect.transform.forward = -direction;
    }
}
