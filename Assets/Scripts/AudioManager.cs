using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource audioSource;
    public SongBeatData songBeatData; // this handles the location of beats n shi

    private int nextIndex;
    private float lastBeatTime;
    private float songTime;

    public delegate void OnBeatAction(int beatNumber, bool isFirstSpawner);
    public static event OnBeatAction OnBeat;
    public static Action OnGameOver;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = songBeatData.songClip;
        audioSource.Play();
        songTime = audioSource.clip.length;

        nextIndex = 0;
    }

    private void Update()
    {
        if (nextIndex >= songBeatData.beats.Count) { OnGameOver?.Invoke(); return; }
        if (Time.time >= songBeatData.beats[nextIndex].time) // checks if the time is greater than the next beat index
        {
            bool isFirstSpawner = nextIndex % 2 == 0; // basically just alternates the bool depending on odd or even beats 
            OnBeat?.Invoke(songBeatData.beats[nextIndex].beatNumber, isFirstSpawner);
            nextIndex++;
        }
            lastBeatTime = Time.time;
    }



    public int GetCurrentBeat()
    {
        int currentbeat = -1;
        if (songBeatData == null || songBeatData.beats == null || songBeatData.beats.Count == 0)
        {
            return currentbeat;
        }
        for (int i = 0; i < songBeatData.beats.Count; i++)
        {
            if (audioSource.time >= songBeatData.beats[i].time)
            {
                currentbeat = songBeatData.beats[i].beatNumber;

            } else
            {
                break; // if the time ends up being less, then we know the beats are sorted and can break out of the loop.
            }
        }
        return currentbeat;
    }


}
