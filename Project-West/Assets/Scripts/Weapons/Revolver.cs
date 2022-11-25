using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    int RoundsInGun;
    [SerializeField]
    int RoundsRemain;

    [SerializeField]
    float MuzzleVelocity;

    [SerializeField]
    TextMeshProUGUI InGun;
    [SerializeField]
    TextMeshProUGUI Total;


    void Awake()
    {
        RoundsInGun = MagazineSize;
    }


    void Update()
    {
        if (Input.GetButtonDown("Shoot"))
        {
            if(RoundsInGun > 0)
            {
                RoundsInGun--;
                Fire();
            }
        }

        InGun.text = RoundsInGun.ToString();
        Total.text = RoundsRemain.ToString();

        if (Input.GetButtonDown("Reload"))
        {
            if(RoundsRemain > 0)
            {
                if(RoundsRemain >= MagazineSize - RoundsInGun)
                {
                    RoundsRemain = RoundsRemain - (MagazineSize - RoundsInGun);
                    RoundsInGun = MagazineSize;
                }

                if(RoundsRemain < MagazineSize - RoundsInGun)
                {
                    RoundsInGun += RoundsRemain;
                    RoundsRemain = 0;
                }
            }

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
