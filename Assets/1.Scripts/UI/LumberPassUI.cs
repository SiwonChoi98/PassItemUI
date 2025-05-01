using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

public class LumberPassUI : MonoBehaviour
{
    [SerializeField] private ItemLumberPass _baseItemLumberPassPrefab;
    [SerializeField] private ScrollRect _scrollRect;

    private List<ItemLumberPass> _itemList;
    private float _offset;
    private float _itemHeight;
    private void Start()
    {
        SpawnItem();
        SetContentHeight();
        
        _scrollRect.onValueChanged.AddListener(_ => OnScrollChanged());
    }
    private void SetData(ItemLumberPass item, int key)
    {
        item.SetData(SpecDataManager.Instance.GetPassInfoData(key));
    }
    private void SpawnItem()
    {
        _itemHeight = _baseItemLumberPassPrefab.GetComponent<RectTransform>().rect.height;
        
        RectTransform scrollRect = _scrollRect.GetComponent<RectTransform>();
        _itemList = new List<ItemLumberPass>();

        int itemCount = (int)(scrollRect.rect.height / _itemHeight) + 1 + 2;

        for (int i = 0; i < itemCount; i++)
        {
            ItemLumberPass item = Instantiate(_baseItemLumberPassPrefab, _scrollRect.content);
            item.transform.localPosition = new Vector3(0, -i * _itemHeight);
            
            _itemList.Add(item);
            SetData(item, i+1);
        }

        _offset = _itemList.Count * _itemHeight;
    }

    private void SetContentHeight()
    {
        _scrollRect.content.sizeDelta = new Vector2(_scrollRect.content.sizeDelta.x, SpecDataManager.Instance.PassInfoCount * _itemHeight);
    }

    private bool RelocationItem(ItemLumberPass item, float contentY, float scrollHeight)
    {
        if (item.transform.localPosition.y + contentY > (_itemHeight * 2f) - 10f)
        {
            item.transform.localPosition -= new Vector3(0, _offset);
            return true;
        }
        else if (item.transform.localPosition.y + contentY < -scrollHeight - _itemHeight)
        {
            item.transform.localPosition += new Vector3(0, _offset);
            return true;
        }

        return false;
    }
    private void OnScrollChanged()
    {
        RectTransform scrollRect = _scrollRect.GetComponent<RectTransform>();
        float scrollHeight = scrollRect.rect.height;
        float contentY = _scrollRect.content.anchoredPosition.y;

        foreach (var item in _itemList)
        {
            bool changed = RelocationItem(item, contentY, scrollHeight);
            if (changed)
            {
                int idx = (int)(-item.transform.localPosition.y / _itemHeight);
                SetData(item, idx + 1);
            }
        }
    }
}
