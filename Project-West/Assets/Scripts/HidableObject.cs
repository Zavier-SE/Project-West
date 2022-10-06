using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidableObject : MonoBehaviour
{
    public bool ishideable;
    bool playerInRange;
    GameObject player;

    [SerializeField]
    GameObject InteractionIcon;
    // Start is called before the first frame update
    void Start()
    {
        playerInRange = false;
        ishideable = true;
        InteractionIcon.GetComponent<SpriteRenderer>().enabled = false;
    }

    private void Update()
    {
        bool playerCanHide = player.GetComponent<PlayerController>().CanHideInObj;
        if (playerCanHide && playerInRange && ishideable)
        {
            InteractionIcon.GetComponent<SpriteRenderer>().enabled = true;
        }

        if (!playerCanHide || !playerInRange || !ishideable)
        {
            InteractionIcon.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("in");
            playerInRange = true;
            player = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("out");
            playerInRange = false;
        }
    }
}
