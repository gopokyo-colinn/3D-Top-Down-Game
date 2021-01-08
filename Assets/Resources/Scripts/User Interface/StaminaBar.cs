using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    PlayerController player;
    public Image staminaFillAmount;

    float fLerpSpeed = 5f;
    // Start is called before the first frame update
    void Start()
    {
        /// Right now the health fill amount only works for 100 health, do it for any health value.
        player = GameController.Instance.player;
        staminaFillAmount.fillAmount = player.fCurrentStamina / player.fMaxStamina;
    }
    public void ChangeStaminaUI()
    {
        StartCoroutine(StaminaLerp());
    }
    IEnumerator StaminaLerp()
    {
       // while (staminaFillAmount.fillAmount != ((int)player.fCurrentStamina / (int)player.fMaxStamina))
        {
            //Debug.Log("I m stamina in while");
            staminaFillAmount.fillAmount = Mathf.Lerp(staminaFillAmount.fillAmount, player.fCurrentStamina / player.fMaxStamina, fLerpSpeed * Time.deltaTime);
            yield return null;
        }
       // StopAllCoroutines();
    }
}
