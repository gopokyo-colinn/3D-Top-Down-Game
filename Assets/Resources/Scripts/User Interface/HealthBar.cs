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
            healthFillAmount.fillAmount = Mathf.Lerp(healthFillAmount.fillAmount, player.iCurrentHitPoints / 100f, fLerpSpeed * Time.deltaTime);
            yield return null;
        }
    }

}
