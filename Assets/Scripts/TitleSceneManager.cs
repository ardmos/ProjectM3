using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleSceneManager : MonoBehaviour
{
    public Button StartButton;

    // Start is called before the first frame update
    void Start()
    {
        StartButton.onClick.AddListener(()=>LoadSceneManager.Instance.Load(LoadSceneManager.Scene.StageSelectScene));
    }

}
