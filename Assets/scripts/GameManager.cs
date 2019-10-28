using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] models;
    enum GameState
    {
        SELECT_MODEL, DECIDE_SCALE, DECIDE_DIRCTION, DECIDE_PSITION, PLACE_MODEL, FINISH
    }
    // Start is called before the first frame update
    void Start()
    {
        //
    }

    // Update is called once per frame
    void Update()
    {
        playerControl();
    }
    void playerControl()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {

        }
    }
}
