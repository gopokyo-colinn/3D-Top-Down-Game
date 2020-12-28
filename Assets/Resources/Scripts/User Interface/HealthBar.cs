using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    PlayerController player;
    public Image healthFillAmount;

    float fLerpSpeed = 5f;
    // Start is called before the first frame update
    void Start()
    {
        /// Right now the health fill amount only works for 100 health, do it for any health value.
        player = GameController.Instance.player;
        healthFillAmount.fillAmount = player.iCurrentHitPoints / 100f;
    }
    public void ChangeHealthUI()
    {
            StartCoroutine(HealthLerp());
    }
    IEnumerator HealthLerp()
    {
        while (healthFillAmount.fillAmount != player.iCurrentHitPoints / 100f)
        {
            Debug.Log("I m running in while");
            healthFillAmount.fillAmount = Mathf.Lerp(healthFillAmount.fillAmount, player.iCurrentHitPoints / 100f, fLerpSpeed * Time.deltaTime);
            yield return null;
        }
    }

}
