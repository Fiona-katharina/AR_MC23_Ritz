using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Android;

public class Dice : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI NumberDisplay;
    [SerializeField] TextMeshProUGUI NumberDisplayPlayer;
    [SerializeField] GameObject Settings;
    public double t;
    public double time;

    private double deltaTime;
    private double dif;
    private Vector3 lastPos;
    private int maxValue;

    private float accThreshold;
    private float accelerationMagnitude;
    private float LastShake;
    // Start is called before the first frame update
    void Start()
    {
        deltaTime = 0.0;
        lastPos= transform.position;
        accThreshold = 4f;
        maxValue = 20;
    }

    void Update()
    {
        //trigger shake if marker is shaken - very error prone!
        deltaTime += Time.deltaTime;
        if (deltaTime > 0.003)
        {
            if (dif > 0.2) shake();
            deltaTime = 0.0;
            dif = 0.0;
        }
        dif += Vector3.Distance(transform.position, lastPos);
        lastPos = transform.position;
        t= dif;
        //trigger shake if handheld device is shaken
        time = deltaTime;
        accelerationMagnitude = Input.acceleration.sqrMagnitude;
        if(Input.acceleration.sqrMagnitude != accelerationMagnitude) Debug.Log(Input.acceleration.sqrMagnitude.ToString());
        if(accelerationMagnitude > accThreshold && Time.unscaledTime-LastShake>2) 
        {
            shake();
            Handheld.Vibrate();
        }
        if(Input.GetKeyDown(KeyCode.Backspace)) { shake(); }
    }
    private void shake() 
    {
        int n=Random.Range(1, maxValue);
        LastShake = Time.unscaledTime;
        //update Dice info UI
        NumberDisplayPlayer.text = n.ToString();
        Settings.GetComponent<DwarfInit>().SetDicePoints(n);
        NumberDisplay.text = n.ToString();
    }
    public void setToDice(int kindOf)
    {
        switch (kindOf) {
            case 20:
                maxValue = 21;
                break;
            case 10: 
                maxValue = 11; 
                break;
            case 8:
                maxValue = 9;
                break;
            case 4: 
                maxValue = 5; 
                break;
            default: 
                maxValue = 21; 
                break;
        }
    }
}
