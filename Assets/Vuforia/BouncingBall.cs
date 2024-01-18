using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class BouncingBall : MonoBehaviour
{
    public GameObject t1;
    public GameObject t2;
    public GameObject t3;
    bool run=false;
    private bool up = true;
    GameObject currentTarget;
    List<GameObject> List=new List<GameObject>();
    private int i = 1;
    // Start is called before the first frame update

    void Start()
    {
        transform.position=t1.transform.position;
        currentTarget = t2;
        if(t1!=null) List.Add(t1);
        if(t2!=null) List.Add(t2);
        if(t3!=null) List.Add(t3);

    }

   
    // Update is called once per frame
    void Update()
    {
        //StartCoroutine("Jump");
        if (Input.GetKeyDown(KeyCode.Space)) { run = true; }
        if (Input.GetKeyDown(KeyCode.Backspace)) {
            run = false;
            transform.position = t1.transform.position;
        }
        if (run == true) {
            transform.position=Vector3.MoveTowards(transform.position, currentTarget.transform.position, Time.deltaTime*0.5f);
            if(Vector3.Distance(transform.position, currentTarget.transform.position) < 0.01f)
            {
                if (i < List.Count - 1) {
                    i++;
                    currentTarget = List[i]; 
                }
                else
                {
                    i = 0;
                    currentTarget = List[0];
                }
            } 
        }

    }
    void Jump()
    {
        if (up)
        {
            if (transform.position.y < 0.12f)
            {
                transform.Translate(0, (0.5f * Time.deltaTime), 0);
            }
            else up = false;
        }
        else
        {
            if (transform.position.y > 0.03f)
            {
                transform.Translate(0, (-0.5f * Time.deltaTime), 0);
            }
            else up = true;
        }
    }
}
