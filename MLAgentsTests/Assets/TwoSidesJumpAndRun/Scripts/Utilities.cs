using System;
using UnityEngine;
using System.Collections.Generic;
using System.Security.Cryptography;

public static class Utilities 
{

    //Shuffle list function;
    public static void Shuffle<T>(this IList<T> list)
    {
        RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
        int n = list.Count;
        while (n > 1)
        {
            byte[] box = new byte[1];
            do provider.GetBytes(box);
            while (!(box[0] < n * (Byte.MaxValue / n)));
            int k = (box[0] % n);
            n--;
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    //Player prefs bool implementation;
    public static void SetBool(string name, bool booleanValue)
    {
        PlayerPrefs.SetInt(name, booleanValue ? 1 : 0);
    }
    public static bool GetBool(string name)
    {
        return PlayerPrefs.GetInt(name) == 1 ? true : false;
    }

    //Play single audio functions;
    public static void PlaySFX(AudioSource source, AudioClip clip, bool loop = false)
    {
        source.loop = loop;
        if(clip)
        {
            source.clip = clip;
            source.Play();
        }
    }

    //Play random audio from array;
    public static void PlaySFX(AudioSource source, AudioClip[] clip, bool loop = false)
    {
        source.loop = loop;
        if (clip.Length > 0)
        {
            int random = UnityEngine.Random.Range(0, clip.Length);
            source.clip = clip[random];
            source.Play();
        }
    }

    //Stop audio playing;
    public static void StopSFX(AudioSource source)
    {
        if(source.isPlaying)
            source.Stop();
    }
}
