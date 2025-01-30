using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleSceneManager : MonoBehaviour
{
    public CustomClickSoundButton StartButton;

    // Start is called before the first frame update
    void Start()
    {
        StartButton.AddClickListener(()=>LoadSceneManager.Instance.Load(LoadSceneManager.Scene.StageSelectScene));
    }

}
