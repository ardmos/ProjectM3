using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCountManager : MonoBehaviour
{
    [SerializeField] private RectTransform targetGrid;
    [SerializeField] private Target targetPrefab;

    [SerializeField] Sprite[] candySprites;

    void Start()
    {
        //Level1();
    }

    private void Level1()
    {
        int level1MissionCount = 4;
        for(int i = 0; i < level1MissionCount; i++)
        {
            Target targetObject = Instantiate(targetPrefab, targetGrid);
            //targetObject.
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
