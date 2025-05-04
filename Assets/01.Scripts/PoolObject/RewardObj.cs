using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class RewardObj : BasePoolObject
{
    [SerializeField] private Image _itemImage;
    private void SetSprite(Sprite sprite)
    {
        _itemImage.sprite = sprite;
    }
    
    public void Initialize(Sprite sprite, Vector3 centerPosition)
    {
        SetSprite(sprite);
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.localScale = Vector3.one;
        
        Color color = _itemImage.color;
        color.a = 1f;
        _itemImage.color = color;

        Vector3 endPosition = GetExplodeEndPosition(centerPosition, 150f);

        Sequence seq = DOTween.Sequence();
        seq.Append(rectTransform.DOMove(endPosition, 0.3f).SetEase(Ease.OutCubic));
        seq.Join(rectTransform.DORotate(new Vector3(0, 0, Random.Range(-180f, 180f)), 0.3f));
        seq.Append(rectTransform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack));
        seq.Join(_itemImage.DOFade(0f, 0.3f));
        seq.OnComplete(() =>
        {
            _itemImage.sprite = null;
            ReturnToPool();
        });
    }
    
    private Vector3 GetExplodeEndPosition(Vector3 centerPosition, float distance = 150f)
    {
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        return centerPosition + (Vector3)(randomDir * distance);
    }
}
