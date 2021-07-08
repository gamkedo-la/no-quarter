using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HurtScreen : MonoBehaviour
{
    private Image image;
    private bool isDamageScreenOn = true;

    [SerializeField] float hurtScreenDuration = 1.0f;
    [SerializeField] float hurtScreenFadeSpeed = 1.0f;

    // Start is called before the first frame update
    void Awake()
    {
        image = GetComponent<Image>();
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
    }

    private void Update() {
        if (isDamageScreenOn || image.color.a <= 0) { return; }

        image.color = new Color(
            image.color.r,
            image.color.g,
            image.color.b,
            image.color.a - hurtScreenFadeSpeed * Time.deltaTime
            );
    }

    public void ShowHurtScreen()
    {
        StartCoroutine(DisplayHurtScreen());
    }

    private IEnumerator DisplayHurtScreen()
    {
        isDamageScreenOn = true;
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
        
        yield return new WaitForSeconds(hurtScreenDuration);

        isDamageScreenOn = false;        
    }
}
