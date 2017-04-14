using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Controller : MonoBehaviour {

    private Transform myTransform;

    private Vector3 currentSpawnerPos;                                //Current position of spawner
    public int currentStep;
    public int maxStep;
    private const float stepX = 1.6f;
    private Vector3 currentPosition;

    public List<JellyTypes> jellyTypes_lvl = new List<JellyTypes>();    //List of jelly types that will be used in the level
    public List<JellyTypes> JellyTypes_lvl {get {return jellyTypes_lvl;}}
    public JellyTypes currentJuice;                                    //Current Type of juice we are using

    public List<int> jellyJuiceQuota = new List<int>();

    public bool isPowering;
    public int powerQuota = 0;
    public float power_current = 0;
    public float power_max;
    private float power_basicAdd = 5;
    private float spawnerCD = 0;

    public RectTransform juiceSelction;
    public Text powerQuotaText;
    public Slider powerSlide;
    public List<Text> juiceQuotaText = new List<Text>();
    public Image powerSign;

    void Awake()
    {
        myTransform = transform;
        currentPosition = myTransform.position;
        power_max = 20;
    }

	// Use this for initialization
	void Start () {
        maxStep = GameManager.GetInstance.ColumnNumber - 1;
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.A))
        {
            if(currentStep > 0)
            {
                currentPosition = new Vector3(currentPosition.x - stepX, currentPosition.y, 0); 
                myTransform.position = currentPosition;
                currentStep--;
            }
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            if(currentStep < maxStep)
            {
                currentPosition = new Vector3(currentPosition.x + stepX, currentPosition.y, 0);
                myTransform.position = currentPosition;
                currentStep++;
            }
        }

        if(Input.GetKeyDown(KeyCode.W))
        {
            currentJuice = ((int)currentJuice<(jellyTypes_lvl.Count-1)) ? currentJuice + 1 : 0;
            if(currentJuice==0)
                juiceSelction.localPosition -=  new Vector3(0, (jellyTypes_lvl.Count-1) * 30, 0);
            else
                juiceSelction.localPosition +=  new Vector3(0, 30, 0);
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            currentJuice = ((int)currentJuice>0) ? (JellyTypes)(currentJuice-1) : (JellyTypes)(jellyTypes_lvl.Count-1);
            if((int)currentJuice==(jellyTypes_lvl.Count-1))
                juiceSelction.localPosition +=  new Vector3(0, (jellyTypes_lvl.Count-1) * 30, 0);
            else
                juiceSelction.localPosition -=  new Vector3(0, 30, 0);
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            if(!isPowering)
                EnablePowering(true);
            else
                EnablePowering(false);
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(jellyJuiceQuota[(int)currentJuice]<=0 || spawnerCD!=0)
                return;
                
            spawnerCD = 1;

            JellyTypes juice = currentJuice;
            GameManager.GetInstance.SpawnJuice(currentStep,juice, isPowering);
            jellyJuiceQuota[(int)juice]--;
            juiceQuotaText[(int)juice].text = jellyJuiceQuota[(int)juice].ToString();

            if(isPowering)
            {
                powerQuota--;
                if(powerQuota==0)
                    EnablePowering(false);
            }
            else
                AddPower(power_basicAdd);
        }

        if(spawnerCD>0)
        {
            spawnerCD -= Time.deltaTime;
        }
        else
        {
            spawnerCD = 0;
        }

        powerQuotaText.text = powerQuota.ToString();
    }

    void EnablePowering(bool enable)
    {
        if(enable)
        {
            if(!isPowering)
            {
                if(powerQuota>0)
                {
                    isPowering = true;
                    powerSign.color = Color.white;
                }
            }
        }
        else
        {
            if(isPowering)
            {
                isPowering = false;
                powerSign.color = Color.black;
            }
        }
    }

    public void AddPower(float power)
    {
        power_current += power;
        CheckPowerQuota();
    }

    void CheckPowerQuota()
    {
        while(power_current >= power_max)
        {
            powerQuota++;
            power_current -= power_max;
            if(power_current < power_max)
                break;
        }
        powerSlide.value = power_current / power_max;
    }

    public void AssignJuiceQuota(int juiceCount,int quota)
    {
        for(int i = 0; i< juiceCount; i++)
        {
            jellyJuiceQuota.Add(quota);
            juiceQuotaText[i].text = jellyJuiceQuota[i].ToString();
        }
    }
}
