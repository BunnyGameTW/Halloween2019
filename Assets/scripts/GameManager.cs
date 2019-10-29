using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    
    enum GameState
    {
        SELECT_MODEL, DECIDE_SCALE, DECIDE_DIRCTION, DECIDE_POSITION, PLACE_MODEL, FINISH
    }
    [Serializable]
    public struct DirectionToKeyMap
    {
        public Transform trans;
        public KeyCode key;
    }
    GameState gameState;

    float SCALE_RATIO = 0.5f;
    float SCALE_LIMIT = 3.0f;
    float MOVE_SPEED = 1.0f;
    float PUMPKIN_MOVE_SPEED = 5.0f;
    float WAITING_TIME = 2.0f;
    float START_SCALE = 1.0f;
    KeyCode CONFIRM_KEY = KeyCode.Space;
    GameObject selectObject;
    Vector3 selectObjectRotation;
    KeyCode selectDirectionKey;
    int selectModelIndex;
    int decideScaleCounter, placeModelCounter;
    float scaleCounter, waitingCounter;
    bool isWating, isFirst;
    public GameObject[] models;
    public DirectionToKeyMap[] directionMaps;
    public Text hintText;
    public TMPro.TextMeshProUGUI endText;
    public GameObject pumpkin, startNode;
    string[] hints = {
        "左右選擇你的模型\n空白鍵確定",
        "設定長寬高的尺寸\n空白鍵確定",
        "方向鍵選擇要從哪插入\n空白鍵確定",
        "方向鍵微調你要插入的位置\n空白鍵確定",
        "第一次空白鍵開始插入\n第二次空白鍵停止",
        "做得好，南瓜君覺得舒服\n若覺得你的作品完成了\n可以隨時按ESC結束、R重來\n否則會持續到模型全用完為止",
    };

    // Start is called before the first frame update
    void Start()
    {
        isFirst = true;
        endText.enabled = false;
        init();
        showHint();
    }

    // Update is called once per frame
    void Update()
    {
        playerControl();
        if (Input.GetKeyDown(KeyCode.Escape) && gameState != GameState.FINISH)
        {
            onGameFinish();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("gameScene");
        } 
    }

    void init()
    {
        gameState = GameState.SELECT_MODEL;
        selectDirectionKey = KeyCode.Escape;
        selectModelIndex = 0;
        decideScaleCounter = 0;
        placeModelCounter = 0;
        scaleCounter = START_SCALE;
        waitingCounter = 0.0f;
        isWating = false;
        selectObjectRotation = Vector3.zero;
    }

    void playerControl()
    {
        switch (gameState)
        {
            case GameState.SELECT_MODEL:
                selectModel();
                break;
            case GameState.DECIDE_SCALE:
                decideScale();
                break;
            case GameState.DECIDE_DIRCTION:
                decideDirection();
                break;
            case GameState.DECIDE_POSITION:
                decidePosition();
                break;
            case GameState.PLACE_MODEL:
                placeModel();
                break;
            case GameState.FINISH:
                gameFinish();
                break;
        }
        
    }
    void selectModel()
    {

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //TODO FIRST UI呈現 畫面更換模型
            selectModelIndex = (selectModelIndex - 1 < 0) ? models.Length - 1 : selectModelIndex - 1;

        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            selectModelIndex = (selectModelIndex + 1 == models.Length) ? 0 : selectModelIndex + 1;
        }
        else if (Input.GetKeyDown(CONFIRM_KEY))
        {
            
            selectObject = models[selectModelIndex];
            selectObject.transform.localPosition = Vector3.zero;
            gameState = GameState.DECIDE_SCALE;
            showHint();
        }
        Debug.Log("selectModelIndex" + selectModelIndex);
    }

    void decideScale()
    {
        scaleCounter += Time.deltaTime * SCALE_RATIO;
        if(scaleCounter > SCALE_LIMIT || scaleCounter < -SCALE_LIMIT)
        {
            SCALE_RATIO *= -1;
        }

        selectObject.transform.localScale = new Vector3(
            (decideScaleCounter == 0) ? scaleCounter : selectObject.transform.localScale.x,
            (decideScaleCounter == 1) ? scaleCounter : selectObject.transform.localScale.y,
            (decideScaleCounter == 2) ? scaleCounter : selectObject.transform.localScale.z
        );

        if (Input.GetKeyDown(CONFIRM_KEY))
        {
            scaleCounter = START_SCALE;
            decideScaleCounter++;
            if(decideScaleCounter == 3)
            {
                gameState = GameState.DECIDE_DIRCTION;
                showHint();
                selectObjectRotation = selectObject.transform.eulerAngles;
            }
        }
    }
    void decideDirection()
    {
        foreach (var item in directionMaps)
        {
            if (Input.GetKeyDown(CONFIRM_KEY))
            {
                if (selectDirectionKey == KeyCode.Escape)
                {
                    setSelectObjectTransform(KeyCode.UpArrow, getDirectionTranform(KeyCode.UpArrow));
                }
                gameState = GameState.DECIDE_POSITION;
                showHint();
            }
            else if (Input.GetKeyDown(item.key))
            {
                //TODO 做動畫 從中間飛到那個地方
                setSelectObjectTransform(item.key, item.trans);
            }
        }
    }

    void setSelectObjectTransform(KeyCode key, Transform parent)
    {
        selectDirectionKey = key;
        selectObject.transform.SetParent(parent);
        selectObject.transform.localPosition = new Vector3(0, 0, 0);
        selectObject.transform.localEulerAngles = selectObjectRotation;
    }
    Transform getDirectionTranform(KeyCode key)
    {
        foreach (var item in directionMaps)
        {
            if(item.key == key)
            {
                return item.trans;
            }
        }
        return null;
    }
    void decidePosition()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            selectObject.transform.position = new Vector3(
                selectObject.transform.position.x,
                selectObject.transform.position.y + MOVE_SPEED * Time.deltaTime,
                selectObject.transform.position.z
            );
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            selectObject.transform.position = new Vector3(
               selectObject.transform.position.x,
               selectObject.transform.position.y + -MOVE_SPEED * Time.deltaTime,
               selectObject.transform.position.z
           );
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            selectObject.transform.position = new Vector3(
               selectObject.transform.position.x + ((selectDirectionKey == KeyCode.LeftArrow|| selectDirectionKey == KeyCode.RightArrow) ? 0 : -MOVE_SPEED * Time.deltaTime),
               selectObject.transform.position.y,
               selectObject.transform.position.z + ((selectDirectionKey == KeyCode.LeftArrow || selectDirectionKey == KeyCode.RightArrow) ? -MOVE_SPEED * Time.deltaTime : 0)
           );
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            selectObject.transform.position = new Vector3(
              selectObject.transform.position.x + ((selectDirectionKey == KeyCode.LeftArrow || selectDirectionKey == KeyCode.RightArrow) ? 0 : MOVE_SPEED * Time.deltaTime),
               selectObject.transform.position.y,
               selectObject.transform.position.z + ((selectDirectionKey == KeyCode.LeftArrow || selectDirectionKey == KeyCode.RightArrow) ? MOVE_SPEED * Time.deltaTime : 0)
           );
        }
        if (Input.GetKeyDown(CONFIRM_KEY))
        {
            gameState = GameState.PLACE_MODEL;
            showHint();
            Color materialColor = selectObject.transform.GetChild(0).GetComponent<MeshRenderer>().materials[0].GetColor("_Color");
            selectObject.transform.GetChild(0).GetComponent<MeshRenderer>().materials[0].SetColor("_Color", new Color(materialColor.r, materialColor.g, materialColor.b, 0.8f));
        }
    }
    void placeModel()
    {
        if (!isWating)
        {
            if (Input.GetKeyDown(CONFIRM_KEY))
            {
                placeModelCounter++;
            }
            if (placeModelCounter == 1)
            {
                Vector3 direction = Vector3.zero;
                switch (selectDirectionKey)
                {
                    case KeyCode.LeftArrow:
                        direction = Vector3.left;
                        break;
                    case KeyCode.RightArrow:
                        direction = Vector3.right;
                        break;
                    case KeyCode.UpArrow:
                        direction = Vector3.forward;
                        break;
                    case KeyCode.DownArrow:
                        direction = Vector3.back;
                        break;
                    default:
                        break;
                }
                selectObject.transform.Translate(direction * Time.deltaTime * MOVE_SPEED, Space.World);
            }
            else if (placeModelCounter == 2)
            {
                Destroy(selectObject.transform.GetChild(0).gameObject);
                isWating = true;
                if (isFirst)
                {
                    hintText.text = hints[5];
                    hintText.GetComponent<Animator>().SetTrigger("Show");
                }
            }
        }
        else {
            waitingCounter += Time.deltaTime;
            if (waitingCounter >= WAITING_TIME)
            {  
                resizeArray();
                if (models.Length == 0)
                {
                    onGameFinish();
                }
                else
                {
                    isFirst = false;
                    Debug.Log("繼續");
                    init(); 
                }
            }
        }
    }
    void onGameFinish()
    {
        foreach (var item in directionMaps)
        {
            int count = item.trans.childCount;
            for (int i = 0; i < count; i++)
            {
                item.trans.GetChild(0).SetParent(pumpkin.transform);
            }
        }
        startNode.SetActive(false);
        endText.enabled = true;
        gameState = GameState.FINISH;
    }

    void gameFinish()
    {
        //TODO 粒子特效 或文字有特效
        pumpkin.transform.eulerAngles = new Vector3(
            pumpkin.transform.eulerAngles.x,
            pumpkin.transform.eulerAngles.y,
            pumpkin.transform.eulerAngles.z + Time.deltaTime * PUMPKIN_MOVE_SPEED
        );
        Debug.Log("game finish");
    }
    void resizeArray()
    {

        bool needSwap = false;
        for (int i = 0; i <= models.Length - 1; i++)
        {
            if (needSwap)
            {
                GameObject tmpObj = models[i - 1];
                models[i - 1] = models[i];
                models[i] = tmpObj;
            }
            if (models[i] == selectObject)
            {
                needSwap = true;
            }
           
        }
        Array.Resize(ref models, models.Length - 1);
    }
    void showHint()
    {
        if (isFirst)
        {
            switch (gameState)
            {
                case GameState.SELECT_MODEL:
                    hintText.text = hints[0];
                    break;
                case GameState.DECIDE_SCALE:
                    hintText.text = hints[1];
                    break;
                case GameState.DECIDE_DIRCTION:
                    hintText.text = hints[2];
                    break;
                case GameState.DECIDE_POSITION:
                    hintText.text = hints[3];
                    break;
                case GameState.PLACE_MODEL:
                    hintText.text = hints[4];
                    break;
                default:
                    break;
            }
            hintText.GetComponent<Animator>().SetTrigger("Show");
        }
    }
}
