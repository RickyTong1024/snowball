using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class SoundManager : MonoBehaviour
{
    private AudioSource m_sound_source;
    private AudioSource m_play_mus;
    private string m_mus_name;
    private string m_last_mus_name = "";
    private bool m_stop_mus = false;
    private bool is_play_mus = true;
    private bool is_play_eff = true;
    private bool is_pause = false;

    public void pause_sound()
    {
        is_pause = true;
    }

    public void continue_sound()
    {
        is_pause = false;
    }

    public void play_sound_ex(AudioClip clip)
    {
        if (Is_play_eff)
        {
            if (m_sound_source == null)
            {
                m_sound_source = transform.gameObject.AddComponent<AudioSource>();
            }

            m_sound_source.clip = clip;
            m_sound_source.Play();
        }
    }

    public void play_sound(AudioClip clip)
    {
        if (Is_play_eff)
        {
            if (transform.gameObject.GetComponents<AudioSource>().Length > 10)
            {
                return;
            }

            AudioSource _source = transform.gameObject.AddComponent<AudioSource>();
            _source.clip = clip;
            _source.Play();

            UnityEngine.Object.Destroy(_source, clip.length + 1.0f);
        }
    }

    public void play_3dsound(AudioClip clip, GameObject target)
    {
        if (Is_play_eff)
        {
            AudioSource _source = target.AddComponent<AudioSource>();
            _source.spatialBlend = 1.0f;
            _source.minDistance = 2;
            _source.maxDistance = 12;
            _source.rolloffMode = AudioRolloffMode.Logarithmic;
            _source.clip = clip;
            _source.Play();

            UnityEngine.Object.Destroy(_source, clip.length + 1.0f);
        }
    }

    public void play_mus(string name)
    {
        m_mus_name = name;
        m_stop_mus = true;
    }

    private AudioClip m_clip;
    const int m_samplingRate = 8000;

    public bool Is_play_mus
    {
        get
        {
            return is_play_mus && !is_pause;
        }

        set
        {
            if(value && m_last_mus_name != "")
            {
                play_mus(m_last_mus_name);
            }
            is_play_mus = value;
        }
    }

    public bool Is_play_eff
    {
        get
        {
            return is_play_eff && !is_pause;
        }

        set
        {
            is_play_eff = value;
        }
    }

    void Update()
    {
        if (!Is_play_mus)
        {
            if (m_play_mus.volume > 0)
            {
                m_play_mus.volume -= Time.deltaTime;
                if (m_play_mus.volume <= 0)
                {
                    m_play_mus.volume = 0.0f;
                }
            }
            return;
        }
        if (m_play_mus == null)
        {
            m_play_mus = transform.gameObject.AddComponent<AudioSource>();
        }
        if (m_stop_mus == true)
        {
            if (m_play_mus.volume > 0)
            {
                m_play_mus.volume -= Time.deltaTime;
            }

            if (m_play_mus.volume <= 0)
            {
                m_stop_mus = false;
                m_play_mus.volume = 0.0f;
                m_play_mus.Stop();

                if (m_mus_name.Length > 0)
                {
                    AudioClip _clip = Resources.Load<AudioClip>("music/" + m_mus_name);
                    if (_clip != null)
                    {
                        m_play_mus.clip = _clip;
                        m_play_mus.loop = true;
                        m_play_mus.Play();
                        m_last_mus_name = m_mus_name;
                    }
                }
            }
        }
        else if (m_play_mus.isPlaying && m_play_mus.volume < 0.8f)
        {
            m_play_mus.volume += Time.deltaTime;
        }
    }
}