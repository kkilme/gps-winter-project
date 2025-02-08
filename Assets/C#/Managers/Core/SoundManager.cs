using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
    private AudioSource[] _audioSources = new AudioSource[(int)GlobalEnums.SoundType.MaxCount];
    private Dictionary<string, AudioClip> _audioClipDic = new Dictionary<string, AudioClip>();

    public void Init()
    {
        GameObject root = GameObject.Find("@Sound");
        if (root == null)
        {
            root = new GameObject { name = "@Sound" };
            Object.DontDestroyOnLoad(root);

            string[] soundNames = System.Enum.GetNames(typeof(GlobalEnums.SoundType));
            for (int i = 0; i < soundNames.Length - 1; i++)
            {
                GameObject go = new GameObject { name = soundNames[i] };
                _audioSources[i] = go.AddComponent<AudioSource>();
                go.transform.parent = root.transform;
            }

            _audioSources[(int)GlobalEnums.SoundType.Bgm].loop = true;
        }
    }

    // path 위치의 음원 재생
    public void Play(string path, GlobalEnums.SoundType type = GlobalEnums.SoundType.Effect, float pitch = 1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type, pitch);
    }

    // 재생 타입은 type, 재생 속도는 pitch로 설정하여 audioClip 재생
    public void Play(AudioClip audioClip, GlobalEnums.SoundType type = GlobalEnums.SoundType.Effect, float pitch = 1.0f)
    {
        if (audioClip == null)
            return;

        if (type == GlobalEnums.SoundType.Bgm)
        {
            AudioSource audioSource = _audioSources[(int)type];
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else
        {
            AudioSource audioSource = _audioSources[(int)GlobalEnums.SoundType.Effect];
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
        }
    }

    // path 위치의 음원파일 로드 후 반환
    private AudioClip GetOrAddAudioClip(string path, GlobalEnums.SoundType type = GlobalEnums.SoundType.Effect)
    {
        if (path.Contains("Sounds/") == false)
            path = $"Sounds/{path}";

        AudioClip audioClip;

        if (type == GlobalEnums.SoundType.Bgm)
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
