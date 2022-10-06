using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private GameObject player;

    private float radius;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        this.transform.localPosition = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, 0));
        radius = 5;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.x = Mathf.Clamp(mousePos.x, player.transform.localPosition.x - radius, player.transform.localPosition.x + radius);
        mousePos.y = Mathf.Clamp(mousePos.y, player.transform.localPosition.y - radius, player.transform.localPosition.y + radius);
        mousePos.z = 0f;
        this.transform.localPosition = mousePos;
    }
}
