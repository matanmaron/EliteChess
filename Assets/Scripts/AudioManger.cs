using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManger : MonoBehaviour
{
    [SerializeField] AudioSource _AudioSource = null;
    [SerializeField] AudioClip Eat = null;
    [SerializeField] AudioClip Move = null;

    private void Play()
    {
        if (!_AudioSource.isPlaying)
        {
            _AudioSource.Play();
        }
    }

    internal void PlayEat()
    {
        _AudioSource.clip = Eat;
        Play();
    }


    internal void PlayMove()
    {
        _AudioSource.clip = Move;
        Play();
    }
}
