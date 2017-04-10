using UnityEngine;
using System.Collections;

public class Jelly : MonoBehaviour {

    //set Index and Offset
    public int rowIndex = 0;
    public int columnIndex = 0;
    public int width = 1;
    public int height = 1;
    private const float xOff = 1.6f;
    private const float yOff = 1.6f;

    public bool hasPower = false;

    //控制生成糖果的类型
    //private int jellyTypeNumber = 7;

    //生成糖果材质素材
    //public GameObject[] BGs;
    //private GameObject bg;

    //指示糖果的颜色
    public int type;
    public int level;

    //生成对GameController的引用
    public GameManager gameController;
    public SpriteRenderer sr;
    public SpriteRenderer sr_glow;

    public void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public bool selected
    {
        //值存在于value中
        set {
            if (transform != null)
                sr.color = value ? Color.gray: Color.white;
        }
    }

    public void Powerade()
    {
        //TODO 
        //Instiate glowing object under this
    }

    //点击鼠标时，选择点击的糖果
//    void OnMouseDown()
//    {
//        gameController.SelectJelly(this);
//    }

//    //生成随机颜色的糖果
//    private void AddRandomBG()
//    {
//        //如果糖果已有颜色，就不再生成
//        if (bg != null)
//            return;
//        //随机生成一种颜色的糖果
//        type = Random.Range(0, Mathf.Min(jellyTypeNumber,BGs.Length));
//        bg = (GameObject)Instantiate(BGs[type]);
//        sr = bg.GetComponent<SpriteRenderer>();
//        bg.transform.parent = this.transform;
//    }

    //根据索引值与偏移来更新糖果坐标
    public void InitializePosition()
    {
        transform.localPosition = new Vector3(columnIndex * xOff, rowIndex * yOff, 0f);
    }

//    //实现缓动的效果
//    public void ITweenToPosition()
//    {
//        AddRandomBG();
//        iTween.MoveTo(this.gameObject, iTween.Hash(
//            "x", columnIndex + xOff,
//            "y", rowIndex + yOff,
//            "time",0.3f));
//    }
    //销毁糖果
    public void DisposeJelly()
    {
        gameController = null;
        //Destroy(bg.gameObject);
        Destroy(this.gameObject);
    }
}
