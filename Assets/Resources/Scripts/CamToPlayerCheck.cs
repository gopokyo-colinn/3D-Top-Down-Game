using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamToPlayerCheck : MonoBehaviour
{
    PlayerController player;
    LayerMask playerLayer;
    Vector3 playerHeadOffset;
    Vector3 direction;
    bool bIsVisible;
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerController.Instance;
        playerLayer = LayerMask.NameToLayer("Player");
        playerHeadOffset = new Vector3(0, 1.2f, 0);
    }

    // Update is called once per frame
    RaycastHit hit;
    void Update()
    {
        direction = ((player.transform.position + playerHeadOffset) - transform.position).normalized;
        Debug.DrawRay(transform.position, direction * 100f, Color.red);
        if (Physics.Raycast(transform.position, direction, out hit, 100f))
        {
            if (hit.collider.gameObject.layer == playerLayer)
            {
                if (!bIsVisible)
                {
                    bIsVisible = true;
                    player.SwitchMaterial("default");
                }
                return;
            }
            else
            {
                if (bIsVisible)
                {
                    player.SwitchMaterial("switch");
                    bIsVisible = false;
                }
            }
        }
    }
}
