using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ErrorLog : MonoBehaviour
{
    //A textcontainer to display info for the player
    private TextMeshProUGUI text;
    private float start;
    void Start()
    {
        text=this.GetComponent<TextMeshProUGUI>();
    }

    public void setLogMsg(string msg)
    {
        text.SetText(msg);
        start = Time.time;
    }
    void Update()
    {
        if(Time.time-start > 4)
        {
            text.SetText("");
        }
    }
}
