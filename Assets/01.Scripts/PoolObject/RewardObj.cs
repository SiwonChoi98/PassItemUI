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
    
    public void Initialize(Sprite sprite, Vector3 endPosition)
    {
        SetSprite(sprite);
        GetComponent<RectTransform>()
            .DOMove(endPosition, 1f)
            .SetEase(Ease.InCubic)
            .OnComplete(ReturnToPool);
    }
}
