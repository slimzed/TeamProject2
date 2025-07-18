using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonLevelLoader : MonoBehaviour
{
    private GameObject levelLoader;
    void Start()
    {
        levelLoader = GameObject.Find("ScriptCalls");
    }

    public void OnClick()
    {
        if (levelLoader != null)
        {
            levelLoader.GetComponent<LevelLoader>().LoadNextScene();
        }
    }


}
