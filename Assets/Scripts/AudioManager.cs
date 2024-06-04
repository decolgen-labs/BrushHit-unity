using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Ins;

    [SerializeField] AudioClip _winSFX;
    [SerializeField] AudioClip _loseSFX;
    [SerializeField] AudioClip _clickSFX;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioMixer _audioMixer;

    bool _isMuted = false;

    private void Awake()
    {
        if (Ins)
        {
            Destroy(Ins);
        }
        Ins = this;
        DontDestroyOnLoad(Ins);
    }

    public void PlayWinSFX()
    {
        _audioSource.PlayOneShot(_winSFX);
    }

    public void PlayLoseSFX()
    {
        _audioSource.PlayOneShot(_loseSFX);
    }

    public void PlayClickSFX()
    {
        _audioSource.PlayOneShot(_clickSFX);
    }

    public void MuteAudio()
    {
        _isMuted = !_isMuted;
        _audioMixer.SetFloat("masterVol", _isMuted ? -80 : 0);
    }
}
