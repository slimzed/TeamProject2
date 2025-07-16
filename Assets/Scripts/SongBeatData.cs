using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine;

[System.Serializable]
public class BeatInfo
{
    public float time;
    public int beatNumber;
    public BeatInfo(float time, int beatNumber)
    {
        this.time = time;
        this.beatNumber = beatNumber;
    }
}
[CreateAssetMenu(fileName="SongBeatData", menuName="Audio/Song Beat Info", order=1)]
public class SongBeatData : ScriptableObject {

    public AudioClip songClip;
    public float bpm;
    public List<BeatInfo> beats;

    [ContextMenu("Generate beats from BPM")]
    private void GenerateBeats()
    {
        if (songClip == null)
        {
            Debug.LogWarning("No song clip provided-beats cannot be generated");
            return;
        } else if (bpm <= 0)
        {
            Debug.LogWarning("Beats are provided incorrectly-make sure that they are greater than 0 if you want automatic beat generation.");
            beats.Clear();
            return;
        }

        beats.Clear();

        float bps = 60f / bpm;
        int beatCount = 0;
        float currentTime = 0;


        while (currentTime < songClip.length)
        {
            beats.Add(new BeatInfo(currentTime, beatCount + 1));
            currentTime += bps;
            beatCount++;
        }


        Debug.Log($"Generated {beatCount} beats for {songClip.name}");

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty( this ); // i have 0 clue why this line works but keep it in, if its removed the code doesn't change in scriptableobject
#endif


    }

}

