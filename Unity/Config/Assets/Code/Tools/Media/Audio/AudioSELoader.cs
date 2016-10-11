using UnityEngine;
using System.Collections;

public class AudioSELoader : MonoBehaviour
{

    public enum InstanceType
    {
        Child,
        Sibiling,
    }
    //共享音序号
    public int SACIndex = -1;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1f;
    public bool stopOnDisable;
    public bool enablePositionEffect;
    public InstanceType type;
    public GameObject target;
    [HideInInspector]
    public bool isVoice;

    private AudioSE _se;
    private bool _isPlay;

    void Awake()
    {
        if (!enablePositionEffect) return;
        GameObject obj = new GameObject("SE");
        //obj.transform.parent = target;
        _se = obj.AddComponent<AudioSE>();
        //_se = GameObjectUtility.AddChild<AudioSE>("SE", target);
        if (type == InstanceType.Sibiling) _se.transform.parent = target.transform.parent;

        _se.enableCache = false;
        _se.Init(GetClip(), volume);
        _se.GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Linear;
        _se.isVoice = isVoice;
    }

    void OnEnable()
    {
        Play();
    }

    void OnDisable()
    {
        if (!stopOnDisable) return;
        if (_se == null) return;
        _se.StopWithNotice();
    }

    AudioClip GetClip()
    {
        return clip == null ? SoundManager.Instance.GetSAC(SACIndex) : clip;
    }

    void OnDestory()
    {
        if (!enablePositionEffect) return;

        if (_se.GetComponent<AudioSource>().isPlaying) _se.autoDestory = true;
        else Destroy(_se.gameObject);
    }

    public void Play()
    {
        if (_isPlay && isVoice && _se != null)
        {
            _se.Replay();
            return;
        }

        if (enablePositionEffect)
        {
            SoundManager.Instance.PlaySE(_se);
        }
        else
        {
            _se = SoundManager.Instance.PlaySE(GetClip(), volume, isVoice);
        }

        if (_se == null) return;
        _isPlay = true;
        _se.OnStop += OnStop;
    }

    private void OnStop()
    {
        _isPlay = false;

        if (_se == null) return;
        _se.OnStop -= OnStop;
    }

    #region editor methods
    public void PrepareApply()
    {
        gameObject.SetActive(false);
        enabled = true;
    }
    #endregion

    public void Stop()
    {
        if (!_isPlay) return;
        if (_se == null) return;
        _se.StopWithNotice();
    }

    public bool IsPlaying()
    {
        return _isPlay;
    }

}
