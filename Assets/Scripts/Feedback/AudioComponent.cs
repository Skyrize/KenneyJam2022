using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Audio
{
    public string name;
    public AudioClip clip;
    public bool loop;
    public float maxVolume;
    public float minVolume;
    public float maxPitch;
    public float minPitch;
}

[RequireComponent(typeof(AudioSource))]
public class AudioComponent : MonoBehaviour
{
    [SerializeField]
    private List<Audio> m_audios = new List<Audio>();

    bool m_isOnCooldown = true;
    AudioSource m_source = null;

    private void Awake()
    {
        m_source = GetComponent<AudioSource>();
    }

    private void Play(Audio _audio)
    {
        m_source.pitch = Random.Range(_audio.minPitch, _audio.maxPitch);
        m_source.volume = Random.Range(_audio.minVolume, _audio.maxVolume);
        m_source.clip = _audio.clip;
        if (_audio.loop)
        {
            m_source.Play();
        }
        else
        {
            m_source.PlayOneShot(_audio.clip);
        }

    }

    public void Play(string _audioName)
    {
        Audio audio = m_audios.Find((a) => a.name == _audioName);

        if (audio.name == null)
        {
            Debug.LogException(new System.Exception("Can't find audio named " + _audioName));
            return;
        }
        Play(audio);
    }

    IEnumerator TriggerCooldown(float _duration)
    {
        m_isOnCooldown = true;
        yield return new WaitForSeconds(_duration);
        m_isOnCooldown = false;
    }

    public void PlayWithCooldown(string _audioName)
    {
        if (!m_isOnCooldown)
            return;
        Audio audio = m_audios.Find((a) => a.name == _audioName);

        if (audio.name == null)
        {
            Debug.LogException(new System.Exception("Can't find audio named " + _audioName));
            return;
        }
        Play(audio);
        StartCoroutine(TriggerCooldown(audio.clip.length));
    }

    public float GetCurrentClipLenght()
    {
        return m_source.clip.length;
    }

    public Audio GetAudio(string clipName)
    {
        Audio audio = m_audios.Find((a) => a.name == clipName);

        if (audio.name == null)
        {
            Debug.LogException(new System.Exception("Can't find audio named " + clipName));
        }
        return audio;
    }

}
