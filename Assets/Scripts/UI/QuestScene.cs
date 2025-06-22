using GIKCore;
using GIKCore.Net;
using GIKCore.Sound;
using GIKCore.UI;
using GIKCore.Utilities;
using pbdson;
using System.Collections;
using System.Collections.Generic;
using UIEngine.UIPool;
using UnityEngine;
using UnityEngine.UI;

public class QuestProps
{
    public int questId;
    public long start;
    public long end;
    public string name;
    public string desc;
    public string icon;
    public string subIcon;
    public string link;
    public Sprite sprite;
}
public class QuestScene : GameListener
{
    // Fields
    [SerializeField] private HorizontalPoolGroup m_Pool;

    // Values
    public List<QuestProps> lstQuests = new List<QuestProps>();
    private string currentLink;

    // Methods
    // Start is called before the first frame update
    public override bool ProcessSocketData(int id, byte[] data)
    {
        if (base.ProcessSocketData(id, data))
            return true;

        switch (id)
        {
            case IService.GET_QUESTS:
                {
                    CommonVector cv = ISocket.Parse<CommonVector>(data);
                    int BLOCK_LONG = 3;
                    int START_LONG = 1;
                    int COUNT_LONG = (cv.aLong.Count - START_LONG) / BLOCK_LONG;

                    int BLOCK_STRING = 5;
                    int START_STRING = 0;
                    GameData.main.isFirstQuest = cv.aLong[0] == 1;
                    lstQuests.Clear();

                    for(int i = 0; i < COUNT_LONG; i++)
                    {
                        QuestProps qp = new QuestProps();
                        qp.questId = (int)cv.aLong[START_LONG + i * BLOCK_LONG + 0];
                        qp.start = cv.aLong[START_LONG + i * BLOCK_LONG + 1];
                        qp.end = cv.aLong[START_LONG + i * BLOCK_LONG + 2];

                        qp.name = cv.aString[START_STRING + i * BLOCK_STRING + 0];
                        qp.desc = cv.aString[START_STRING + i * BLOCK_STRING + 1];
                        qp.icon = cv.aString[START_STRING + i * BLOCK_STRING + 2];
                        qp.subIcon = cv.aString[START_STRING + i * BLOCK_STRING + 3];
                        qp.link = cv.aString[START_STRING + i * BLOCK_STRING + 4];
                        lstQuests.Add(qp);
                    }
                    m_Pool.SetAdapter(lstQuests);
                    break;
                }
            case IService.GET_ACCESSTOKEN:
                {
                    CommonVector cv = ISocket.Parse<CommonVector>(data);
                    IUtil.OpenURL(currentLink + "?token=" + cv.aString[0]);
                    //Application.OpenURL(currentLink + "?access_token=" + cv.aString[0]);
                    break;
                }
        }

        return false;
    }
    protected override void Awake()
    {
        base.Awake();
        m_Pool.SetCellDataCallback((GameObject go, QuestProps data, int idx) =>
        {
            go.GetComponent<CellQuest>().SetData(data);
            go.GetComponent<ButtonClickEvent>().SetOnClickCallback(() =>
            {
                currentLink = data.link;
                Game.main.socket.GetAccessTokenWebview();
            });
        });
    }
    public void DoClickBack()
    {
        SoundHandler.main.PlaySFX("900_click_4", "sounds");
        Game.main.LoadScene("HomeSceneNew", delay: 0.3f, curtain: true);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
