using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowKnives : MonoBehaviour
{
    public Transform throwingPoint;

    [SerializeField]
    GameObject knifepf;
    [SerializeField]
    Transform Aim;

    [SerializeField]
    int TotalKnives;
    [SerializeField]
    int KnivesRemain;

    [SerializeField]
    float ThrowingVelocity;

    void Awake()
    {
        KnivesRemain = TotalKnives;
    }

    void Update()
    {
        if (Input.GetButtonDown("Shoot"))
        {
            if (KnivesRemain > 0)
            {
                KnivesRemain--;
                Fire();
            }
        }
    }

    void Fire()
    {
        Vector2 targetDir = (Aim.position - throwingPoint.position).normalized;
        float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        GameObject bullet = Instantiate(knifepf, throwingPoint.position, rotation);
        bullet.GetComponent<Projectile>().Shoot(targetDir * ThrowingVelocity);
    }

    public void GetKnife()
    {
        if(KnivesRemain < TotalKnives)
        {
            KnivesRemain++;
        }
    }
}
