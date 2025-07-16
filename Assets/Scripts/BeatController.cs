using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BeatInfo
{
    public float time;
    public int beatNumber;
}
[CreateAssetMenu(fileName="SongBeatInfo", menuName="Audio/Song Beat Info", order=1)]
public class SongBeatData : ScriptableObject {

    public AudioClip songClip;
    public float bpm;
    public List<BeatInfo> beats;

}
