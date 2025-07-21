using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource audioSource;
    public SongBeatData songBeatData;

    private int nextIndex;
    private float songTime;
    private double songStartTimeDSP;

    public delegate void OnBeatAction(int beatNumber, bool isFirstSpawner, float beatTimeDifference);
    public event OnBeatAction OnBeat;
    public static event Action OnGameVictory;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            enabled = false;
            return;
        }

        if (songBeatData == null || songBeatData.songClip == null)
        {
            enabled = false;
            return;
        }

        audioSource.clip = songBeatData.songClip;
        audioSource.Play();
        songStartTimeDSP = AudioSettings.dspTime;
        songTime = audioSource.clip.length;

        nextIndex = 0;
    }

    private void OnDestroy()
    {
        if (OnGameVictory != null)
        {
            Delegate[] subscribers = OnGameVictory.GetInvocationList();
            foreach (Delegate d in subscribers)
            {
                OnGameVictory -= (Action)d;
            }
        }
    }

    private void Update()
    {
        if (songBeatData == null || songBeatData.beats == null || nextIndex >= songBeatData.beats.Count)
        {
            if (nextIndex >= songBeatData.beats.Count && !IsInvoking("WinGame"))
            {
                Invoke("WinGame", 1f);
            }
            return;
        }

        double currentAudioTime = AudioSettings.dspTime - songStartTimeDSP;

        while (nextIndex < songBeatData.beats.Count && currentAudioTime >= songBeatData.beats[nextIndex].time)
        {
            float beatTimeDifference = 0f;
            if (nextIndex + 1 < songBeatData.beats.Count)
            {
                beatTimeDifference = songBeatData.beats[nextIndex + 1].time - songBeatData.beats[nextIndex].time;
            }
            else
            {
                beatTimeDifference = (float)(songTime / songBeatData.beats.Count);
            }

            bool isFirstSpawner = songBeatData.beats[nextIndex].beatNumber % 2 == 0;
            OnBeat?.Invoke(songBeatData.beats[nextIndex].beatNumber, isFirstSpawner, beatTimeDifference);

            nextIndex++;
        }
    }

    public int GetCurrentBeat()
    {
        if (songBeatData == null || songBeatData.beats == null || songBeatData.beats.Count == 0 || audioSource == null)
        {
            return -1;
        }

        double currentAudioTime = AudioSettings.dspTime - songStartTimeDSP;
        int currentBeat = -1;

        for (int i = 0; i < songBeatData.beats.Count; i++)
        {
            if (currentAudioTime >= songBeatData.beats[i].time)
            {
                currentBeat = songBeatData.beats[i].beatNumber;
            }
            else
            {
                break;
            }
        }
        return currentBeat;
    }

    private void WinGame()
    {
        OnGameVictory?.Invoke();
    }
}