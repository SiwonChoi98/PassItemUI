using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemLumberPass : MonoBehaviour
{
    [SerializeField] private Text _levelText;
    [SerializeField] private Text _rewardValueText;

    public void SetData(PassInfoData data)
    {
        _levelText.text = data.pass_level.ToString();
        _rewardValueText.text = data.reward_value.ToString();
    }
}
