using UnityEngine;
using System.Collections;

public class Jelly : MonoBehaviour {

    //set Index and Offset
    public int rowIndex = 0;
    public int columnIndex = 0;
    public int width = 1;
    public int height = 1;
    private const float rangeFactor = 0.2f;
    private const float xOff = 1.6f;
    private const float yOff = 1.6f;

    public bool hasPower = false;

    public int type;
    public int level;

    public GameManager gameController;
    public SpriteRenderer sr;
    public SpriteRenderer sr_glow;

    public void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
        
    public void InitializePosition()
    {
        transform.localPosition = new Vector3(columnIndex * xOff, rowIndex * yOff, 0f);
    }

    public int GetSplashRange()
    {
        float range = (width*height-1)*rangeFactor;
        return (Mathf.CeilToInt(range) + 1);
    }

    public void DisposeJelly()
    {
        gameController = null;
        //Destroy(bg.gameObject);
        Destroy(this.gameObject);
    }

    public void ScaleSize()
    {
        transform.localScale = new Vector3(transform.localScale.x * width, transform.localScale.y* height, 1);
        transform.localPosition += new Vector3(0.5f*xOff*(width-1), 0.5f*xOff*(height-1), 0);
    }
}
