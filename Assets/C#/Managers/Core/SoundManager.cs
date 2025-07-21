using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
    private AudioSource[] _audioSources = new AudioSource[(int)SoundType.MaxCount];
    private Dictionary<string, AudioClip> _audioClipDic = new Dictionary<string, AudioClip>(); // 경로와 AudioClip을 매핑하여 재사용

    public void Init()
    {
        GameObject root = GameObject.Find("@Sound");
        if (root == null)
        {
            root = new GameObject { name = "@Sound" };
            Object.DontDestroyOnLoad(root);

            string[] soundNames = System.Enum.GetNames(typeof(SoundType));
            for (int i = 0; i < soundNames.Length - 1; i++)
            {
                GameObject go = new GameObject { name = soundNames[i] };
                _audioSources[i] = go.AddComponent<AudioSource>();
                go.transform.parent = root.transform;
            }

            _audioSources[(int)SoundType.Bgm].loop = true;
        }
    }

    /// <summary>
    /// path 위치의 AudioClip으로 BGM 재생
    /// </summary>
    public void PlayBGM(string path, float pitch = 1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip("BGM/" + path, SoundType.Bgm);
        if(audioClip == null)
        {
            return;
        }
        CoroutineRunner.Instance.StartCoroutine(PlayBGMCoroutine(audioClip, pitch));
    }

    private Tween _bgmFadeTween;

    private IEnumerator PlayBGMCoroutine(AudioClip audioClip, float pitch = 1.0f)
    {
        AudioSource audioSource = _audioSources[(int)SoundType.Bgm];
        if (audioSource.isPlaying && audioSource.volume > 0f)
        {
            // 현재 BGM이 재생 중이면 페이드 아웃
            yield return FadeoutBGM().WaitForCompletion();
        }
        audioSource.Stop();
        audioSource.volume = 0f;

        audioSource.clip = audioClip;
        audioSource.pitch = pitch;
        audioSource.Play();

        _bgmFadeTween = audioSource.DOFade(.5f, 3f);
        yield return _bgmFadeTween.WaitForCompletion(); // 새 BGM 페이드 인
    }

    public Tween FadeoutBGM(float duration = 2f)
    {
        AudioSource audioSource = _audioSources[(int)SoundType.Bgm];

        if(_bgmFadeTween != null && _bgmFadeTween.IsActive() && _bgmFadeTween.IsPlaying())
        {
            _bgmFadeTween.Kill(); // 진행중인 페이드 효과 제거. FadeIn 도중이여도 무조건 FadeOut이 우선순위를 가짐.
        }
        return audioSource.DOFade(0f, duration);
    }

    /// <summary>
    /// 효과음 재생
    /// </summary>
    /// <param name="path">효과음 AudioClip 경로</param>
    /// <param name="stopPlayingSound">기존에 재생중인 효과음 정지할지 여부</param>
    public void PlayEffect(string path, float volume = 1.0f, float pitch = 1.0f, bool stopPlayingSound = false)
    {
        AudioClip audioClip = GetOrAddAudioClip("Effect/" + path, SoundType.Effect);
        if (audioClip == null)
        {
            return;
        }

        AudioSource audioSource = _audioSources[(int)SoundType.Effect];
        if (audioSource.isPlaying && stopPlayingSound)
        {
            audioSource.Stop();
        }
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(audioClip);
        
    }

    public void PlayItemEffect(string path, float volume = 1.0f, float pitch = 1.0f, bool stopPlayingSound = false)
    {
        PlayEffect("Item/" + path, volume, pitch, stopPlayingSound);
    }

    public void PlayGoldSound(float pitch = 1.0f, bool stopPlayingSound = false)
    {
        PlayEffect("Item/Gold", .4f, pitch, stopPlayingSound);
    }

    // path 위치의 음원파일 로드 후 반환
    private AudioClip GetOrAddAudioClip(string path, SoundType type = SoundType.Effect)
    {
        if (path.Contains("Audio/") == false)
            path = $"Audio/{path}";

        AudioClip audioClip;

        if (type == SoundType.Bgm)
        {
            audioClip = Managers.ResourceMng.Load<AudioClip>(path);
        }
        else
        {
            if (_audioClipDic.TryGetValue(path, out audioClip) == false)
            {
                audioClip = Managers.ResourceMng.Load<AudioClip>(path);
                _audioClipDic.Add(path, audioClip);
            }
        }
        
        if (audioClip == null)
            Debug.Log($"Failed to load AudioClip : {path}");

        return audioClip;
    }
    
    public void Clear()
    {
        foreach (AudioSource audioSource in _audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
        
        _audioClipDic.Clear();
    }
}
