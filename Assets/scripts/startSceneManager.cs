using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class startSceneManager : MonoBehaviour
{
    bool isSelectLevel;
    public Text hintText;
    public RectTransform leftPoint, rightPoint;
    public Image bunnyImage;
    string selectStr = "- Arrow to Select Level-";
    string startStr = "- Space to Start -";
    public AudioClip left, right, select;
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        isSelectLevel = false;
        hintText.text = selectStr;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isSelectLevel)
        {
            audioSource.PlayOneShot(select);
            UnityEngine.SceneManagement.SceneManager.LoadScene("gameScene");
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            setArrowKey(KeyCode.LeftArrow);

        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            setArrowKey(KeyCode.RightArrow);
        }
    }
    void setArrowKey(KeyCode key)
    {
        audioSource.PlayOneShot(key == KeyCode.RightArrow ? right : left);
        GameManager.isCubeMode = (key == KeyCode.RightArrow) ? true : false;
        bunnyImage.rectTransform.position = (key == KeyCode.RightArrow) ? rightPoint.position : leftPoint.position;
        hintText.text = startStr;
        isSelectLevel = true;
    }
}
