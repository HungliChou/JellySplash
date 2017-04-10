using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum JellyTypes{Red,Blue,Green}
public enum GameState{Stop, Action, Verify}

public class GameManager : MonoBehaviour {

    private static GameManager instance;                                //Singleton private instance
    public static GameManager GetInstance{get{return instance;}}        //Singleton instance getter
    public Controller controller;

    public int rowNumber = 2;
    public int columnNumber = 6;
    public int ColumnNumber{get{return columnNumber;}}


    public List<Jelly> jellyPrefab = new List<Jelly>();                 //Jelly prefabs
    public List<Juice> juicePrefab = new List<Juice>();                 //Juice prefabs
    public GameObject glowingJelly;
    public GameObject glowingJuice;
    public GameObject jellyParent;                                      //Jellies' parent
    //音效获取
    public AudioClip mathThreeClip;
    public AudioClip SwapClip;
    public AudioClip explosionClip;
    public AudioClip SwapWrog;

    public Text score;

    public GameState currentState;                                      //Current State of the game

    private const int maxRow = 10;

    public SecArray[] jellyArray = new SecArray[maxRow];

    private ArrayList jellyArrayList;
    private ArrayList matchesArrayList;
  
    public int allScore = 0;

    void Awake()
    {
        instance = this;
        //DontDestroyOnLoad(gameObject);
    }
        

    void Start () {
        SpawnJellyWave(3,8);
        currentState = GameState.Action;
    }

    void Update()
    {
        
    }

    private void SpawnJellyWave(int totalType, int juiceQuota)
    {
        controller.JellyTypes_lvl.Clear();

        int type = 0;

        for(int i = 0; i<jellyPrefab.Count; i++)
        {
            controller.JellyTypes_lvl.Add((JellyTypes)i);
        }
        controller.AssignJuiceQuota(totalType, juiceQuota);


        for (int rowIndex = 0; rowIndex < rowNumber; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < columnNumber; columnIndex++)
            {
                //create crisscross jellies
                type = ((rowIndex+columnIndex+1)%totalType==0) ? 0: type+1;

                Jelly obj = AddJelly(type,rowIndex, columnIndex, 1, 1,false);

                SetJelly(rowIndex,columnIndex,obj);
            }
        }
    }

    //Instantiate Jelly to designated position and give size
    public Jelly AddJelly(int type,int rowIndex, int columnIndex, int width, int height, bool hasPower)
    {
        //实例化预置件是对象类型
        Jelly obj = Instantiate(jellyPrefab[type]) as Jelly;

        //设置其父容器为GameController
        obj.transform.parent = jellyParent.transform;
        obj.rowIndex = rowIndex;
        obj.columnIndex = columnIndex;
        obj.width = width;
        obj.height = height;
        obj.type = type;
        obj.gameController = this;
        obj.InitializePosition();
        obj.hasPower = hasPower;
        if(hasPower)
        {
            GameObject glow = Instantiate(glowingJelly);
            glow.transform.parent = obj.transform;
            glow.transform.localPosition = Vector3.zero;
            obj.sr_glow = glow.GetComponent<SpriteRenderer>();
        }

        return obj;
    }

    int FindSpawnJellyPositionY(int _step)
    {
        int topRow = -1;
        for (int rowIndex = 0; rowIndex < maxRow; rowIndex++)
        {
            if(jellyArray[rowIndex].Column[_step]!= null)
            {
                topRow = rowIndex;
            }
        }

        return topRow;
    }

    public void SpawnJuice(int step, JellyTypes type, bool hasPower)
    {
        Juice juice = Instantiate(juicePrefab[(int)type]) as Juice;
        juice.transform.position = controller.transform.position - new Vector3(0,1,0);
        if(hasPower)
        {
            GameObject glow = Instantiate(glowingJuice) as GameObject;
            glow.transform.parent = juice.transform;
            glow.transform.localPosition = Vector3.zero;
        }
        int topRow = FindSpawnJellyPositionY(step);
        Jelly topJellp = jellyArray[topRow].Column[step];
        juice.ActivateMovement(topJellp.transform.position.y);
        jellyArray[topRow + 1].Column[step] = juice.AddJelly((int)type, step, topRow + 1, hasPower);
    }

    //根据行列索引得到糖果对象
    private Jelly GetJelly(int rowIndex,int columnIndex)
    {
        Jelly c = jellyArray[rowIndex].Column[columnIndex];
        return c;
    }

    //根据行列索引设置糖果的位置
    private void SetJelly(int _rowIndex,int _columnIndex,Jelly c)
    {
        jellyArray[_rowIndex].Column[_columnIndex] = c;
    }

//    //选中糖果
//    public void SelectJelly(Jelly c)
//    {
//        //Removejelly(c);return;
//        if (currentjelly == null)
//        {
//            //第一次点击，将其存入当前糖果中
//            currentjelly = c;
//            currentjelly.selected = true;
//        }
//        else
//        {
//            //如果点击的是相邻的糖果，则进行交换
//            if (Mathf.Abs(c.rowIndex-currentjelly.rowIndex)
//                +Mathf.Abs(c.columnIndex-currentjelly.columnIndex)==1)
//            {
//                StartCoroutine(ExchangeJelly2(currentjelly,c));
//            }
//            //置空第一次选中的对象
//            currentjelly.selected = false;
//            currentjelly = null;
//        }
//    }
        
    //添加爆炸效果
    private void AddEffect(Vector3 position)
    {
        //Instantiate(Resources.Load("Prefabs/Explosion2"), position, Quaternion.identity);
//        //添加摄像头摇动范围
//        CameraShake.shakeFor(0.1f, 0.1f);
    }
    //调用销毁方法删除糖果
    private void RemoveJelly(Jelly c)
    {
        AddEffect(c.transform.position);
        //播放爆炸音效
        GetComponent<AudioSource>().PlayOneShot(explosionClip);
        //移除自己
        c.DisposeJelly();
        //得到被移除糖果上面的糖果
        int columnIndex = c.columnIndex;
        for (int rowIndex = c.rowIndex + 1; rowIndex < rowNumber; rowIndex++)
        {
            Jelly c1 = GetJelly(rowIndex, columnIndex);
            //往下移一位
            c1.rowIndex--;
            //c1.UpdatePosition();
            //c1.ITweenToPosition();
            //保存其位置
            SetJelly(rowIndex-1, columnIndex, c1);
        }
    }

    //返回布尔值，显示是否能够交换
    private bool CheckMatches()
    {
        return CheckHotizontalMatches() || ChecVerticalMatches();
    }

    //检查水平方向是否有可以消除的
    private bool CheckHotizontalMatches()
    {

        bool result = false;
        for (int rowIndex = 0; rowIndex < rowNumber; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < columnNumber - 2; columnIndex++)
            {
                if ((GetJelly(rowIndex, columnIndex).type == GetJelly(rowIndex, columnIndex + 1).type) &&
                    (GetJelly(rowIndex, columnIndex + 1).type == GetJelly(rowIndex, columnIndex + 2).type))
                {
                    //播放匹配音效
                    GetComponent<AudioSource>().PlayOneShot(mathThreeClip);
                    result = true;
                    Debug.Log(columnIndex + "" + columnIndex + 1 + "" + columnIndex + 2);
                    AddMatches(GetJelly(rowIndex, columnIndex));
                    AddMatches(GetJelly(rowIndex, columnIndex + 1));
                    AddMatches(GetJelly(rowIndex, columnIndex + 2));
                }
            }
        }

        return result;
    }

    //检查垂直是否有可以消除的
    private bool ChecVerticalMatches()
    {
        bool result = false;
        for (int columnIndex = 0; columnIndex < columnNumber; columnIndex++)
        {
            for (int rowIndex = 0; rowIndex < rowNumber - 2; rowIndex++)
            {
                if ((GetJelly(rowIndex, columnIndex).type == GetJelly(rowIndex+1, columnIndex).type) &&
                    (GetJelly(rowIndex+1, columnIndex).type == GetJelly(rowIndex+2, columnIndex).type))
                {

                    //播放匹配音效
                    GetComponent<AudioSource>().PlayOneShot(mathThreeClip);
                    result = true;
                    AddMatches(GetJelly(rowIndex, columnIndex));
                    AddMatches(GetJelly(rowIndex+1, columnIndex));
                    AddMatches(GetJelly(rowIndex+2, columnIndex));
                }
            }
        }

        return result;
    }

    //生成匹配数组
    private void AddMatches(Jelly c)
    {
        if (matchesArrayList == null)
        {
            //第一次进行创建
            matchesArrayList = new ArrayList();
        }
        int index = matchesArrayList.IndexOf(c);
        //如果当前对象不在匹配数组中，就将之添加进去
        if (index == -1)
        {
            matchesArrayList.Add(c);
        }
    }

    //移除匹配数组中的元素
    private void RemoveMatches()
    {        
        Jelly temp;
        for (int index = 0; index < matchesArrayList.Count; index++)
        {
            temp = matchesArrayList[index] as Jelly;
            RemoveJelly(temp);
        }

        //得到分数
        GetScore(matchesArrayList);

        matchesArrayList = new ArrayList();
        StartCoroutine(WaitAndCheck());


    }

    //等待一会儿后进行检查
    IEnumerator WaitAndCheck()
    {
        yield return new WaitForSeconds(0.5f);
        //如果消除完成后还有一起的糖果，消除
        if (CheckMatches())
        {
            RemoveMatches();
        }
    }

    //得到分数
    private void GetScore(ArrayList al){

        allScore += 100 * al.Count;
        score.text = "SCORE : " + allScore;
    }
}
