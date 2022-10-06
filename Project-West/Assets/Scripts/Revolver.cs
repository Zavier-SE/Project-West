using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Revolver : MonoBehaviour
{
    public Transform muzzlePoint;

    [SerializeField]
    GameObject bulletpf;
    [SerializeField]
    Transform Aim;

    [SerializeField]
    int MagazineSize;
    [SerializeField]
    int RoundsRemain;

    [SerializeField]
    float MuzzleVelocity;

    void Awake()
    {
        RoundsRemain = MagazineSize;
    }


    void Update()
    {
        if (Input.GetButtonDown("Shoot"))
        {
            if(RoundsRemain > 0)
            {
                RoundsRemain--;
                Fire();
            }
        }

        if (Input.GetButtonDown("Reload"))
        {
            RoundsRemain = MagazineSize;
        }
    }

    void Fire()
    {
        Vector2 targetDir = (Aim.position - muzzlePoint.position).normalized;
        float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        GameObject bullet = Instantiate(bulletpf, muzzlePoint.position, rotation);
        bullet.GetComponent<Projectile>().Shoot(targetDir * MuzzleVelocity);
    }
}
