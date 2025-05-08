using UnityEngine;
using DG.Tweening;

public class QuizCardManager : MonoBehaviour
{
    [SerializeField] private RectTransform cardRect;
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeStrength = 30f;
    [SerializeField] private int shakeVibrato = 10;
    [SerializeField] private float slideDistance = 2000f;
    [SerializeField] private float slideDuration = 0.5f;

    public void ShakeCard()
    {
        cardRect.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato);
    }

    public void SlideOut()
    {
        cardRect.DOAnchorPosX(slideDistance, slideDuration)
            .SetEase(Ease.InOutQuad);
    }

    public void SlideIn()
    {
        cardRect.anchoredPosition = new Vector2(-slideDistance, cardRect.anchoredPosition.y);
        cardRect.DOAnchorPosX(0, slideDuration)
            .SetEase(Ease.InOutQuad);
    }

    public void ResetPosition()
    {
        cardRect.anchoredPosition = Vector2.zero;
    }
}