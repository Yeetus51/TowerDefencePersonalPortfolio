using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class UiMoneyDroppedElement : MonoBehaviour
{
    public  TMP_Text moneyText; 
    [SerializeField] float fadeDuration = 1f;
    [SerializeField] float moveUpDistance = 1f;


    public void FadeOutAndMoveUp()
    {
        Color initialColor = moneyText.color;
        Vector3 initialPosition = moneyText.transform.position;

        Tweener colorTweener = moneyText.DOColor(new Color(initialColor.r, initialColor.g, initialColor.b, 0f), fadeDuration);

        Tweener moveTweener = moneyText.transform.DOMoveY(initialPosition.y + moveUpDistance, fadeDuration);

        Sequence sequence = DOTween.Sequence()
            .Join(colorTweener)
            .Join(moveTweener)
            .OnComplete(() =>
            {
                moneyText.color = initialColor;
                moneyText.transform.position = initialPosition;
                this.gameObject.SetActive(false);
            });
    }
}
