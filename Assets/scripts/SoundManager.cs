using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public GameObject bgmPrefab;
    GameObject bgmInstance = null;
    // Use this for initialization
    void Start()
    {
        bgmInstance = GameObject.FindGameObjectWithTag("Sound");
        if (bgmInstance == null)
        {
            bgmInstance = Instantiate(bgmPrefab);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
