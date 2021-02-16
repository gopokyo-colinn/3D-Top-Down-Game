using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyGameObject : MonoBehaviour
{
    public float fDeactivateAfter = 1f;
    public bool bDestroyIt;

    private bool bCoroutineRunning;
    // Start is called before the first frame update
    void Start()
    {
        if (bDestroyIt)
            Destroy(gameObject, fDeactivateAfter);
        else
        {
            bCoroutineRunning = true;
            StartCoroutine(HelpUtils.WaitForSeconds(delegate { bCoroutineRunning = false; gameObject.SetActive(false); }, fDeactivateAfter));
        }
    }
    private void Update()
    {
        if (!bDestroyIt)
        {
            if (gameObject.activeSelf && !bCoroutineRunning)
            {
                bCoroutineRunning = true;
                StartCoroutine(HelpUtils.WaitForSeconds(delegate { bCoroutineRunning = false; gameObject.SetActive(false); }, fDeactivateAfter));
            }
        }
    }

}
