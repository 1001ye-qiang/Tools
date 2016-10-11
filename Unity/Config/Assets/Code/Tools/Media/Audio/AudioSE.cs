using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioSE : MonoBehaviour
{

    public event Action OnStop;

    [HideInInspector]
    public bool isVoice;

    public bool enableCache
    {
        get { return _enableCache; }
        set { _enableCache = value; }
    }

    public bool autoDestory
    {
        set { _autoDestory = value; }
    }

    private bool _enableCache;
    private bool _autoDestory;
    private float _volume;

    void Update()
    {
        if (GetComponent<AudioSource>().isPlaying) return;
        StopWithNotice();
    }

    public void Init(AudioClip clip, float volume, bool isVoice_ = false)
    {
        Stop();
        GetComponent<AudioSource>().clip = clip;
        GetComponent<AudioSource>().pitch = SoundManager.Instance.pitch;
        _volume = volume;
        isVoice = isVoice_;
    }

    public void Play(bool mute, float seVolume, float voiceVolume, bool doLoop = false)
    {
        enabled = true;
        GetComponent<AudioSource>().enabled = true;
        GetComponent<AudioSource>().mute = mute;
        GetComponent<AudioSource>().volume = isVoice ? voiceVolume : seVolume * _volume;
        GetComponent<AudioSource>().loop = doLoop;
        GetComponent<AudioSource>().pitch = SoundManager.Instance.pitch;
        GetComponent<AudioSource>().Play();
    }

    public void Replay()
    {
        GetComponent<AudioSource>().Play();
    }

    public void OnChangeVolume(bool mute, float seVolume, float voiceVolume)
    {
        GetComponent<AudioSource>().mute = mute;
        GetComponent<AudioSource>().volume = isVoice ? voiceVolume : seVolume * _volume;
    }

    public void Stop()
    {
        enabled = false;
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().enabled = false;
        if (OnStop != null) OnStop();
    }


    public void StopWithNotice()
    {
        Stop();
        SoundManager.Instance.OnStopSE(this);
        if (_autoDestory) Destroy(gameObject);
    }

    public bool IsPlaying()
    {
        return GetComponent<AudioSource>().isPlaying;
    }
}
