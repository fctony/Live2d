using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using live2d;

public class GameManager : MonoBehaviour {

    //单例
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }

    //游戏的有关判断
    public bool gameOver;
    public Live2dSampleModel badBoyScript;
    public GameObject badBoyTalkLine;
    public GameObject gameOverBtns;

    //玩家属性
    public int gold;
    public int favor;
    public int leftDays;

    public Text goldText;
    public Text favorText;
    public Text dateText;

    public LAppModelProxy lAppModelProxy;

    public GameObject actionBtns;

    public GameObject talkLine;
    public Text talkLineText;

    //天黑天亮属性
    public Image mask;
    public bool toAnotherDay;
    public bool toBeDay;
    private float timeVal;

    //工作
    public GameObject workBtns;
    public Sprite[] workSprites;
    public Image workImage;
    public GameObject workUI;

    //聊天
    public GameObject chatUI;

    

    //约会
    public SpriteRenderer bgImage;
    public Sprite[] dateSprites;

    //其他
    public GameObject clickEffect;
    public Canvas canvas;
    public Texture2D missCuiNewCloth;

    //音乐播放
    private AudioSource audioSource;
    public AudioClip[] audioClips;


    private void Awake()
    {
        _instance = this;
        gold = favor = 0;
        leftDays = 20;
        UpdateUI();
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.clip = audioClips[0];
        audioSource.Play();
    }

    // Update is called once per frame
    void Update () {
        //产生鼠标点击特效
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Vector2.one;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out mousePos);
            GameObject go = Instantiate(clickEffect);
            go.transform.SetParent(canvas.transform);
            go.transform.localPosition = mousePos;
        }

        //游戏结束逻辑
        if (gameOver)
        {
            talkLine.SetActive(true);
            gameOverBtns.SetActive(true);
            actionBtns.SetActive(false);
            if (favor>=1500)
            {
                talkLineText.text = "墨子终于追到了村儿里最漂酿的女孩--雨涵" + "\n"
                    + "最后他们幸福的在一起了！";
            }
            else if(leftDays!=0&&favor<1500)
            {
                talkLineText.text = "雨涵收到欺负的时候墨子没能保护她，" + "\n" + "从此他们决裂了！";
            }
            else
            {
                talkLineText.text = "墨子在雨涵出国前没能获取她的芳心," + "\n" + "雨涵出国留学了，他们最终没有在一起。";
            }
        }


        //是否过渡到另外一天
        if (toAnotherDay)
        {
            if (toBeDay)
            {
                //天亮
                if (timeVal>=2)
                {
                    timeVal = 0;
                    ToDay();
                }
                else
                {
                    timeVal += Time.deltaTime;
                }
            }
            else
            {
                //天黑
                ToDark();
            }
        }
	}

    //即将天黑
    public void ToBeDark()
    {
        toAnotherDay = true;
    }

    //天黑
    private void ToDark()
    {
        mask.color += new Color(0,0,0,Mathf.Lerp(0,1,0.1f));
        if (mask.color.a>=0.8f)
        {
            mask.color = new Color(0, 0, 0, 1);
            toBeDay = true;
            ResetUI();
            UpdateUI();
        }
    }

    //天亮
    private void ToDay()
    {
        mask.color -= new Color(0,0,0,Mathf.Lerp(1,0,0.1f));
        if (mask.color.a<=0.2f)
        {
            mask.color = new Color(0,0,0,0);
            toAnotherDay = false;
            toBeDay = false;
            if (leftDays!=5)
            {
                audioSource.clip = audioClips[0];
                audioSource.Play();
            }
        }
    }

    /// <summary>
    /// 打工
    /// </summary>
    public void ClickWorkBtn()
    {
        actionBtns.SetActive(false);
        workBtns.SetActive(true);
        lAppModelProxy.SetVisible(false);
        PlayButtonSound();
        audioSource.clip = audioClips[2];
        audioSource.Play();
    }

    public void GetMoney(int workIndex)
    {
        audioSource.PlayOneShot(audioClips[6]);
        workBtns.SetActive(false);
        ChangeGold((4-workIndex)*20);
        workImage.sprite = workSprites[workIndex];
        workUI.SetActive(true);
        talkLine.SetActive(true);
        talkLineText.text = "劳动最关荣!经过劳动得到"+ ((4 - workIndex) * 20).ToString()+"的金币。";
    }

    /// <summary>
    /// 聊天
    /// </summary>
    public void ClickChatBtn()
    {
        actionBtns.SetActive(false);
        chatUI.SetActive(true);
        audioSource.clip = audioClips[1];
        audioSource.Play();
        if (favor>=100)
        {
            lAppModelProxy.GetModel().StartMotion("tap_body", 1, 2);
        }
        else
        {
            lAppModelProxy.GetModel().StartMotion("tap_body",0,2);
        }
    }

    public void GetFavor(int chatIndex)
    {
        chatUI.SetActive(false);
        talkLine.SetActive(true);
        switch (chatIndex)
        {
            case 0:
                if (favor>20)
                {
                    ChangeFavor(10);
                    talkLineText.text = "谢谢啊，墨子，你也很帅。。。";
                    audioSource.PlayOneShot(audioClips[7]);

                }
                else
                {
                    ChangeFavor(2);
                    talkLineText.text = "哦，谢谢";
                    lAppModelProxy.GetModel().SetExpression("f08");
                    audioSource.PlayOneShot(audioClips[7]);
                }
                break;
            case 1:
                if (favor>60)
                {
                    ChangeFavor(20);
                    talkLineText.text = "啊。。哦，不好意思，谢谢哈。。。";
                    lAppModelProxy.GetModel().SetExpression("f07");
                    audioSource.PlayOneShot(audioClips[7]);
                }
                else
                {
                    ChangeFavor(-20);
                    talkLineText.text = "你看错了，那是残留的纸巾！" +"\n"+
                        "刚才我擦汗不小心留下的，你手拿开，真不礼貌！";
                    lAppModelProxy.GetModel().SetExpression("f03");
                }
                break;
            case 2:
                if (favor>100)
                {
                    ChangeFavor(40);
                    talkLineText.text = "那。。。咱们一起去吃饭，下午去哪玩?";
                    lAppModelProxy.GetModel().SetExpression("f05");
                    audioSource.PlayOneShot(audioClips[7]);
                }
                else
                {
                    ChangeFavor(-40);
                    talkLineText.text = "你这人说话怎么这样啊，我又没得罪你。";
                    lAppModelProxy.GetModel().SetExpression("f04");
                }
                break;
            default:
                break;
        }
    }


    /// <summary>
    /// 约会
    /// </summary>
    public void ClickDateBtn()
    {
        actionBtns.SetActive(false);
        talkLine.SetActive(true);
        int randomNum = Random.Range(1, 4);
        bool hasEnoughGold = false;
        bgImage.sprite = dateSprites[randomNum];
        switch (randomNum)
        {
            case 1:
                if (gold>=50)
                {
                    ChangeGold(-50);
                    ChangeFavor(150);
                    talkLineText.text = "学校门口原来有这么好玩的地方，" + "\n" +
                        "今天谢谢你了，墨子。";
                    hasEnoughGold = true;
                }
                else
                {
                    talkLineText.text = "没事，不用在意，我最近零花钱比较多。";
                    ChangeFavor(-50);
                }
                break;
            case 2:
                if (gold>=150)
                {
                    ChangeGold(-150);
                    ChangeFavor(300);
                    talkLineText.text = "蟹黄汤包，烤鸭还有其他的甜品真的都太好吃了！" + "\n"
                        + "谢谢招待！";
                    hasEnoughGold = true;
                }
                else
                {
                    talkLineText.text = "下次有机会我再请你吃饭吧。";
                    ChangeFavor(-150);
                }
                break;
            case 3:
                if (gold>=300)
                {
                    ChangeGold(-300);
                    ChangeFavor(500);
                    talkLineText.text = "今天真的很开心," + "\n" +
                        "还有，谢谢你送的礼物，你人真好。。。";
                    hasEnoughGold = true;
                }
                else
                {
                    talkLineText.text = "那个娃娃真的好可爱哦，好想要。。。";
                    ChangeFavor(-300);
                }
                break;
            default:
                break;
        }
        if (hasEnoughGold)
        {
            lAppModelProxy.GetModel().StartMotion("pinch_in",0,2);
        }
        else
        {
            lAppModelProxy.GetModel().StartMotion("flick_head", 0, 2);
        }
        audioSource.clip = audioClips[3];
        audioSource.Play();
        PlayButtonSound();
    }

    /// <summary>
    /// 表白
    /// </summary>
    public void ClickLoveBtn()
    {
        actionBtns.SetActive(false);
        talkLine.SetActive(true);
        audioSource.clip = audioClips[4];
        audioSource.Play();
        if (favor>=1500)//表白成功
        {
            talkLineText.text = "谢谢你啊，墨子，其实，我也喜欢你很久了," + "\n" +
             "自己喜欢的那个人正好也喜欢着自己，" + "\n" +
             "真好，希望你可以让我永远陪着你。";
            lAppModelProxy.GetModel().StartMotion("pinch_out", 0, 2);
            lAppModelProxy.GetModel().SetExpression("f07");
            gameOver = true;
        }
        else
        {
            talkLineText.text = "墨子，你。。。你，" + "\n" +
                "突然的表白吓我一跳" + "\n" +
                "你真的喜欢我是吗？" + "\n" +
                "可是。。。我们还不够了解彼此。。。";
            lAppModelProxy.GetModel().StartMotion("shake", 0, 2);
            lAppModelProxy.GetModel().SetExpression("f04");
        }
        PlayButtonSound();
    }

    //更新玩家属性UI显示
    private void UpdateUI()
    {
        goldText.text = gold.ToString();
        favorText.text = favor.ToString();
        dateText.text = leftDays.ToString();
    }
    //金边数额的变化方法
    public void ChangeGold(int goldValue)
    {
        gold += goldValue;
        if (gold<=0)
        {
            gold = 0;
        }
        UpdateUI();
    }
    //好感度数额变化的方法
    public void ChangeFavor(int favorValue)
    {
        favor += favorValue;
        if (favor<=0)
        {
            favor = 0;
        }
        UpdateUI();
    }

    //重置所有UI
    private void ResetUI()
    {
        workUI.SetActive(false);
        talkLine.SetActive(false);
        actionBtns.SetActive(true);
        lAppModelProxy.SetVisible(true);
        lAppModelProxy.GetModel().SetExpression("f01");
        bgImage.sprite = dateSprites[0];
        leftDays--;
        if (leftDays==5)
        {
            CreateBadBoy();
        }
        else if (leftDays==10)
        {
            Live2DModelUnity live2DModelUnity = lAppModelProxy.GetModel().GetLive2DModelUnity();
            live2DModelUnity.setTexture(2, missCuiNewCloth);
        }
        else if (leftDays==0)
        {
            gameOver = true;
        }
    }

    //产生坏男孩
    private void CreateBadBoy()
    {
        lAppModelProxy.isRunningAway = true;
        badBoyScript.gameObject.SetActive(true);
        lAppModelProxy.GetModel().SetExpression("f04");
        actionBtns.SetActive(false);
        badBoyTalkLine.SetActive(true);
        audioSource.clip = audioClips[5];
        audioSource.Play();
    }

    public void CloseBadBoyTalkLine()
    {
        badBoyTalkLine.SetActive(false);
    }

    public void DefeatBadBoy()
    {
        lAppModelProxy.GetModel().StartMotion("shake",0,2);
        talkLine.SetActive(true);
        talkLineText.text = "刚才吓死我了，谢谢你，墨子" + "\n" +
            "要不是你及时救我，我就。。。" + "\n" +
            "你人好还勇敢，真帅。。。";
        ChangeFavor(300);
    }

    public void LoadScene(int sceneNum)
    {
        SceneManager.LoadScene(sceneNum);
    }

    public void PlayButtonSound()
    {
        audioSource.PlayOneShot(audioClips[8]);
    }
}
