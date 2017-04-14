using UnityEngine;
using System.Collections;

public class Juice : MonoBehaviour {

    private Transform myTransform;
    public int type;
    public int row;
    public int column;
    public float movingSpeed;
    public bool canMove;
    private Vector3 targetPos;
    private const float yOff = 1.8f;
    private Jelly myJelly;
    public Jelly MyJelly{get{return myJelly;}}
    private bool hasPower;
    //public bool HasPower{get{return hasPower;}set{hasPower = value;}}

    void Awake()
    {
        myTransform = transform;
        movingSpeed = 0.5f;
    }

	// Update is called once per frame
	void Update () {
        if(canMove)
        {
            if(myTransform.position.y > targetPos.y)
            {
                myTransform.position -= new Vector3(0,movingSpeed,0);
            }
            else
            {
                myJelly.rowIndex = row;
                myJelly.columnIndex = column;
                myJelly.InitializePosition();
                myJelly.sr.enabled = true;
                if(myJelly.sr_glow)
                    myJelly.sr_glow.enabled = true;
                canMove = false;
                if(hasPower)
                    GameManager.GetInstance.UpdateBoard(column,row,type);
                Destroy(gameObject);
            }
        }
	}

    public void ActivateMovement(float topJellyPosY, bool isPowering)
    {
        targetPos = new Vector3(0,topJellyPosY + yOff, 0);
        hasPower = isPowering;
        canMove = true;
    }
        
    public Jelly AddJelly(int _type, int _column, int _row, bool _hasPower)
    {
        type = _type;
        column = _column;
        row = _row;
        myJelly = GameManager.GetInstance.AddJelly(type,row,column,1,1,_hasPower) as Jelly;
        myJelly.sr.enabled = false;
        if(myJelly.sr_glow)
            myJelly.sr_glow.enabled = false;
        return myJelly;
    }

}
