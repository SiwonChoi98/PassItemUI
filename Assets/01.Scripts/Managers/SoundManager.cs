using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private List<AudioSource> _sfx_Audio;
    
    public void Play_SFX(SoundType soundType, float volume)
    {
        AudioSource emptySource = _sfx_Audio.Find(source => !source.isPlaying);

        AudioClip clip = ResourceManager.Instance.SoundDatas.SoundDataDic[soundType];
        if (!clip)
            return;
        
        if(emptySource != null)
        {
            emptySource.volume = volume;
            emptySource.PlayOneShot(clip);
        }
        else
        {
            _sfx_Audio[0].PlayOneShot(clip);
        }
    }
    
}