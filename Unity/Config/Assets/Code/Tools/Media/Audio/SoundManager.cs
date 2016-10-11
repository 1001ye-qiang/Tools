using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : SingletonMono<SoundManager>
{
    public delegate bool IsDisable();
    public static IsDisable disableSE;
    public static IsDisable disableBG;


    // 音量
    private SoundVolume volume = new SoundVolume();

    private static readonly float _pitch = 1.0f;
    public float pitch
    {
        get { return _pitch; }
    }

    public int numberOfConcurrentSE = 10;
    public int numberOfConcurrentENV = 3;
    public float fadeTime = 0.4f;

    //公用音
    public AudioClip[] SAC;

    // === AudioSource ===
    // BGM
    private AudioSource BGMsource;
    // SE
    private LinkedList<AudioSE> enabledSEsources;
    private Queue<AudioSE> cachedSEsources;
    // 環境音
    private LinkedList<AudioSource> enabledENVsources;
    private Queue<AudioSource> cachedENVsources;

    // === AudioClip ===
    // BGM
    public AudioClip[] BGM;
    // SE
    public AudioClip[] SE;
    // 環境音
    public AudioClip[] ENV;


    // === AudioList ===
    // **List<T>はArrayListよりlow memoryだそう
    // BGM
    public List<string> BGM_list;
    // SE
    public List<string> SE_list;
    // 環境音
    public List<string> ENV_list;

    private GameObject audioSources;
    private static readonly String audioSourcesParentGameObjectName = "Audio Sources";
    private static readonly String audioSourcesChildGameObjectName = "SE";
    private static readonly String audioSourcesChild2GameObjectName = "BGM";
    private static readonly String audioSourcesChild3GameObjectName = "ENV";

    void Awake()
    {

        DontDestroyOnLoad(gameObject);

        // 全てのAudioSourceコンポーネントを追加する
        audioSources = new GameObject(audioSourcesParentGameObjectName);
        audioSources.transform.parent = transform;

        // BGM AudioSource
        GameObject bgmObj = new GameObject(audioSourcesChild2GameObjectName);
        bgmObj.transform.parent = audioSources.transform;
        BGMsource = bgmObj.AddComponent<AudioSource>();
        // BGMはループを有効にする
        BGMsource.loop = true;
        BGMsource.pitch = pitch;

        // 音量初期化 
        volume.BGM = PlayerPrefs.GetFloat("YINYUESHENGLIANG", 0.7f);
        volume.SE = PlayerPrefs.GetFloat("YINXIAOSHENGLIANG", 0.8f);
        volume.VOICE = PlayerPrefs.GetFloat("voice_value", 1f);

        // SE AudioSource
        enabledSEsources = new LinkedList<AudioSE>();
        cachedSEsources = new Queue<AudioSE>();

        // 環境音 AudioSource
        enabledENVsources = new LinkedList<AudioSource>();
        cachedENVsources = new Queue<AudioSource>();

        // 音源をリストに追加する
        for (int i = 0, il = BGM.Length; i < il; i++)
        {
            BGM_list.Add(BGM[i].name);
        }
        for (int i = 0, il = SE.Length; i < il; i++)
        {
            SE_list.Add(SE[i].name);
        }
        for (int i = 0, il = ENV.Length; i < il; i++)
        {
            ENV_list.Add(ENV[i].name);
        }
    }

    void Update()
    {
        if (curClip != null && disableBG != null && !disableBG())
        {
            PlayBGM(curClip);
        }
        else if (curClip == null && disableBG != null && disableBG())
        {
            curClip = BGMsource.clip;
            BGMsource.clip = null;
            StopBGM();
        }
    }

    // ***** BGM再生 *****
    // BGM再生
    [Obsolete("PlayBGM(AudioClip, bool) のほうを使用する")]
    public void PlayBGM(string name, bool loop = true)
    {
        int index = BGM_list.IndexOf(name);
        if (index >= 0)
        {
            PlayBGM(index);
            BGMsource.loop = loop;
            BGMsource.pitch = pitch;
        }
        else
        {
            // Fetch the audio if not existing
            FetchAddtionalBGM(name, (audio) =>
            {
                // Set into audio list
                int i = BGM.Length;
                BGM.SetValue(audio, i);
                BGM_list.Add(audio.name);
                // Play
                PlayBGM(i);
                BGMsource.loop = loop;
                BGMsource.pitch = pitch;
            });
        }
    }

    [Obsolete("PlayBGM(AudioClip, bool) のほうを使用する")]
    public void PlayBGM(int index)
    {
        if (0 > index || BGM.Length <= index)
        {
            return;
        }
        PlayBGM(BGM[index]);
    }

    public string GetPlayingClipName()
    {
        if (BGMsource != null && BGMsource.clip != null && !string.IsNullOrEmpty(BGMsource.clip.name)) return BGMsource.clip.name;
        return null;
    }

    AudioClip curClip = null;
    public void PlayBGM(AudioClip clip, bool useFade = true, bool loop = true)
    {
        if (disableBG != null && disableBG())
        {
            curClip = clip;
            return;
        }
        else
        {
            curClip = null;
        }

        BGMsource.loop = loop;
        BGMsource.pitch = pitch;

        // 同じBGMの場合は何もしない
        if (BGMsource.clip != null && clip != null && BGMsource.clip.name == clip.name)
        {
            return;
        }

        if (VOLUME.BGM == 0f)
        {
            BGMsource.clip = clip;
            return;
        }

        if (useFade)
        {
            StartCoroutine(Fadeout(fadeTime, BGMsource, () =>
            {
                BGMsource.clip = clip;
                BGMsource.volume = VOLUME.BGM;
                BGMsource.Play();
            }));
        }
        else
        {
            BGMsource.Stop();
            BGMsource.volume = VOLUME.BGM;
            BGMsource.clip = clip;
            BGMsource.Play();
        }
    }

    // BGM停止
    public void StopBGM(bool useFade = true)
    {

        if (VOLUME.BGM == 0f)
        {
            BGMsource.clip = null;
            return;
        }

        if (useFade)
        {
            StartCoroutine(Fadeout(fadeTime, BGMsource, null));
        }
        else
        {
            BGMsource.Stop();
            BGMsource.clip = null;
        }
    }


    // ***** SE再生 *****
    // SE再生
    [Obsolete("PlaySE(AudioClip, float, bool) のほうを使用する")]
    public void PlaySE(string name)
    {

        int index = SE_list.IndexOf(name);
        if (index >= 0)
        {
            PlaySE(index);
        }
        else
        {
            // Fetch the audio if not existing
            FetchAddtionalSE(name, (audio) =>
            {
                // Set into audio list
                int i = SE.Length;
                SE.SetValue(audio, i);
                SE_list.Add(audio.name);
                // Play
                PlaySE(i);
            });
        }
    }

    [Obsolete("PlaySE(AudioClip, float, bool) のほうを使用する")]
    public void PlaySE(int index)
    {
        if (0 > index || SE.Length <= index)
        {
            return;
        }

        PlaySE(SE[index]);
    }

    public AudioSE PlaySE(AudioClip clip, float volume = 1f, bool isVoice = false, bool doLoop = false)
    {
        if (disableSE != null && disableSE())
        {
            return null;
        }

        float seVolume = isVoice ? this.volume.VOICE : this.volume.SE;
        if (seVolume == 0f) return null;

        AudioSE se;
        if (enabledSEsources.Count >= numberOfConcurrentSE)
        {
            // 制限数を超えて再生中ならば、最も古い音を強制的に終了する
            se = enabledSEsources.First.Value;
            enabledSEsources.RemoveFirst();
        }
        else
        {
            if (cachedSEsources.Count > 0)
            {
                se = cachedSEsources.Dequeue();
                se.enabled = true;
            }
            else
            {
                GameObject seObj = new GameObject(audioSourcesChildGameObjectName);
                seObj.transform.parent = audioSources.transform;
                se = seObj.AddComponent<AudioSE>();

                se.enableCache = true;
            }
        }
        enabledSEsources.AddLast(se);
        se.Init(clip, volume, isVoice);
        se.Play(this.volume.Mute, this.volume.SE, this.volume.VOICE, doLoop);
        return se;
    }


    public void PlaySE(AudioSE se)
    {
        float seVolume = se.isVoice ? this.volume.VOICE : this.volume.SE;
        if (seVolume == 0f) return;

        if (enabledSEsources.Count >= numberOfConcurrentSE)
        {
            // 制限数を超えて再生中ならば、最も古い音を強制的に終了する
            se = enabledSEsources.First.Value;
            enabledSEsources.RemoveFirst();
            se.Stop();
            if (se.enableCache) cachedSEsources.Enqueue(se);
        }

        enabledSEsources.AddLast(se);
        se.Play(this.volume.Mute, this.volume.SE, this.volume.VOICE);
    }

    // SE停止
    public void StopSE()
    {
        // 全てのSE用のAudioSouceを停止する
        foreach (AudioSE observer in enabledSEsources)
        {
            cachedSEsources.Enqueue(observer);
            AudioSource source = observer.GetComponent<AudioSource>();
            source.Stop();
            source.clip = null;
        }
        enabledSEsources.Clear();

        GameObject AudioSourcesParent = GameObject.Find("CommonSceneObjects/SoundManager/" + audioSourcesParentGameObjectName);
        if (AudioSourcesParent != null)
        {
            foreach (Transform child in AudioSourcesParent.transform)
            {
                if (String.Equals(child.gameObject.name, audioSourcesChildGameObjectName))
                {
                    AudioSource audio = child.GetComponent<AudioSource>();
                    audio.volume = 0;
                }
            }
        }
    }

    public AudioClip GetSAC(int index)
    {
        if (index < 0 || index >= SAC.Length)
            return null;
        return SAC[index];
    }

    // 名前を指定してAudioClipを取得
    public AudioClip GetSE(string name)
    {
        int index = SE_list.IndexOf(name);
        if (index >= 0)
        {
            return SE[index];
        }
        return null;
    }

    // ***** 環境音再生 *****
    // 環境音再生
    public void PlayENV(string name)
    {
        int index = ENV_list.IndexOf(name);
        if (index >= 0)
        {
            PlayENV(index);
        }
        else
        {
            // Fetch the audio if not existing
            FetchAddtionalENV(name, (audio) =>
            {
                // Set into audio list
                int i = ENV.Length;
                ENV.SetValue(audio, i);
                ENV_list.Add(audio.name);
                // Play
                PlayENV(i);
            });
        }
    }

    public void PlayENV(int index)
    {
        if (0 > index || ENV.Length <= index)
        {
            return;
        }
        PlayENV(ENV[index]);
    }

    public void PlayENV(AudioClip clip)
    {
        AudioSource source;
        if (enabledENVsources.Count >= numberOfConcurrentENV)
        {
            // 制限数を超えて再生中ならば、最も古い音を強制的に終了する
            source = enabledENVsources.First.Value;
            source.Stop();
            enabledENVsources.RemoveFirst();
        }
        else
        {
            if (cachedENVsources.Count > 0)
            {
                source = cachedENVsources.Dequeue();
            }
            else
            {
                GameObject envObj = new GameObject(audioSourcesChild3GameObjectName);
                envObj.transform.parent = audioSources.transform;
                envObj.AddComponent<AudioSE>();
                source = envObj.GetComponent<AudioSource>();
                
                source.loop = true;
            }
            source.mute = this.volume.Mute;
            source.volume = this.volume.ENV;
        }
        enabledENVsources.AddLast(source);
        source.clip = clip;
        source.pitch = pitch;
        source.Play();
    }

    // 環境音停止
    public void StopENV()
    {
        // 全ての環境音用のAudioSouceを停止する
        foreach (AudioSource source in enabledENVsources)
        {
            source.Stop();
            source.clip = null;
            cachedENVsources.Enqueue(source);
        }
        enabledENVsources.Clear();
    }

    public void StopENV(AudioClip clip)
    {
        foreach (AudioSource source in enabledENVsources)
        {
            if (source.clip != clip) continue;
            enabledENVsources.Remove(source);
            source.clip = null;
            cachedENVsources.Enqueue(source);
            break;
        }
    }

    public void StopAllSound(bool useFade = true)
    {
        StopBGM(useFade);
        StopSE();
        StopENV();
    }

    // Fade out
    IEnumerator Fadeout(float duration, AudioSource audio, Action callback)
    {
        StopCoroutine("Fadeout");
        float currentTime = 0.0f;
        float firstVol = audio.volume;
        while (duration > currentTime)
        {
            currentTime += Time.fixedDeltaTime;
            audio.volume = Mathf.Lerp(firstVol, 0.0f, currentTime / duration);
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        if (callback != null)
        {
            callback();
        }
    }

    // Property
    public SoundVolume VOLUME
    {
        get
        {
            return this.volume;
        }
        set
        {
            this.volume = value;

            // ミュート・ボリューム設定
            // BGM
            this.BGMsource.mute = this.volume.Mute;
            this.BGMsource.volume = this.volume.BGM;
            this.BGMsource.pitch = pitch;

            if (this.volume.BGM == 0f)
            {
                this.BGMsource.Stop();
            }
            else if (!this.BGMsource.isPlaying)
            {
                this.BGMsource.Play();
            }

            foreach (AudioSE se in enabledSEsources)
            {
                // SE
                se.OnChangeVolume(this.volume.Mute, this.volume.SE, this.volume.VOICE);
            }
            foreach (AudioSource source in enabledENVsources)
            {
                // ENV
                source.mute = this.volume.Mute;
                source.volume = this.volume.ENV;
                source.pitch = pitch;
            }
        }
    }


    // === TODO === //
    // Load audios dynamically

    // 新しい音源をローカルストレージから追加する
    // 想定使用ケース:Awakeで使えるものとして登録
    public void AddAdditionalBGM()
    {
        // BGM.Add
        // BGM_list.Add
    }
    public void AddAdditionalSE()
    {
        // SE.Add
        // SE_list.Add
    }
    public void AddAdditionalENV()
    {
        // ENV.Add
        // ENV_list.Add
    }

    // 新しい音源をWWWで取ってくる (非同期)
    // 想定使用ケース:Playする時、対象音源がない時
    public void FetchAddtionalBGM(string name, Action<AudioClip> callback)
    {
        // Get audio from local/web
        // ★AudioClip could be taken from www.andioClip
        // After audio loaded, callback is called
    }
    public void FetchAddtionalSE(string name, Action<AudioClip> callback)
    {
        // Get audio from local/web
        // ★AudioClip could be taken from www.andioClip
        // After audio loaded, callback is called
    }
    public void FetchAddtionalENV(string name, Action<AudioClip> callback)
    {
        // Get audio from local/web
        // ★AudioClip could be taken from www.andioClip
        // After audio loaded, callback is called
    }

    // ======== //

    public void OnEnableAudioListener(AudioListener listener)
    {
        audioSources.transform.parent = listener.transform;
    }

    public void OnDisableAudioListener(AudioListener listener)
    {
        if (audioSources.transform.parent != listener.transform) return;
        audioSources.transform.parent = transform.parent;
    }

    public void OnStopSE(AudioSE se)
    {
        enabledSEsources.Remove(se);
        if (se.enableCache) cachedSEsources.Enqueue(se);
    }

    public void ResetPitchAllSound()
    {
        //BGM
        BGMsource.pitch = pitch;

        //SE
        foreach (AudioSE observer in enabledSEsources)
            observer.GetComponent<AudioSource>().pitch = pitch;
        GameObject AudioSourcesParent = GameObject.Find("CommonSceneObjects/SoundManager/" + audioSourcesParentGameObjectName);
        if (AudioSourcesParent != null)
            foreach (Transform child in AudioSourcesParent.transform)
                if (String.Equals(child.gameObject.name, audioSourcesChildGameObjectName))
                {
                    AudioSource audio = child.GetComponent<AudioSource>();
                    audio.pitch = pitch;
                }

        //ENV
        foreach (AudioSource source in enabledENVsources)
            source.GetComponent<AudioSource>().pitch = pitch;
    }

}

// 音量クラス
[Serializable]
public class SoundVolume
{
    public float BGM = 1.0f;
    public float ENV = 1.0f;
    public float SE = 1.0f;
    public float VOICE = 1.0f;
    public bool Mute = false;

    public void Init()
    {
        BGM = 1.0f;
        ENV = 1.0f;
        SE = 1.0f;
        Mute = false;
    }
}

