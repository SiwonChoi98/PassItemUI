using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundData", menuName = "Scriptable Object Asset/SoundData")]
public class SoundDatas : ScriptableObject
{
    [SerializedDictionary("SoundType", "Clip")]
    public SerializedDictionary<SoundType, AudioClip> SoundDataDic;
}
