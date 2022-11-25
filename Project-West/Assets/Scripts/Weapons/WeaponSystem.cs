using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSystem : MonoBehaviour
{
    ThrowKnives throwKnives;
    public Image throwKnivesIcon;
    Revolver revolver;
    public Image revolverIcon;

    Vector2 offSize;
    Vector2 onSize;

    // Start is called before the first frame update
    void Start()
    {
        throwKnives = GetComponent<ThrowKnives>();
        revolver = GetComponent<Revolver>();

        throwKnives.enabled = false;
        revolver.enabled = true;

        offSize = new Vector2(60, 60);
        onSize = new Vector2(80, 80);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("z"))
        {
            if (throwKnives.enabled == true)
            {
                throwKnives.enabled = false;
                revolver.enabled = true;
            }
            else if (throwKnives.enabled == false)
            {
                throwKnives.enabled = true;
                revolver.enabled = false;
            }
        }

        if (throwKnives.enabled)
        {
            throwKnivesIcon.rectTransform.sizeDelta = onSize;
        }
        else
        {
            throwKnivesIcon.rectTransform.sizeDelta = offSize;
        }

        if (revolver.enabled)
        {
            revolverIcon.rectTransform.sizeDelta = onSize;
        }
        else
        {
            revolverIcon.rectTransform.sizeDelta = offSize;
        }
    }
}
