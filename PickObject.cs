using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;



public class PickObject : MonoBehaviour
{
    Vector3 initialPos;
    Vector3 initialRot;
    Vector3 initialScale;
    Transform rack;
    public GameObject canvas;
    GameObject canvasClone;
    public Transform vrCamera;
    float radius = 10f;
    float radiusObj = 1.5f;
    public Transform parent;
    public Transform dest;
    public GameObject cart;
    //Shader shader1;
    //Shader shader2;
    //Renderer rend;
    UDPScript udp;
    
    public string currentData;

    public bool selected = false;

    public bool isChild = false;

    DateTime enterTime;

    DateTime pickTime;

    //public GameObject handObject;
    //public GameObject rightArmObject;
    //public GameObject leftArmObject;

    void Start()
    {

        initialPos = transform.position;
        initialRot = transform.eulerAngles;
        initialScale = transform.localScale;
        rack = transform.parent;
        GetComponent<Rigidbody>().maxDepenetrationVelocity = 0.1f;
        EventTrigger trigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((eventData) => { Enter(); });
        trigger.triggers.Add(entry);
        EventTrigger.Entry entry1 = new EventTrigger.Entry();
        entry1.eventID = EventTriggerType.PointerExit;
        entry1.callback.AddListener((eventData) => { Exit(); });
        trigger.triggers.Add(entry1);
        udp = GameObject.Find("dummy").transform.GetComponent<UDPScript>();

        //shader1 = Shader.Find("Standard");
        //shader2 = Shader.Find("Outlined/Custom");
        //if (transform.childCount == 0)
        //{
        //    rend = GetComponent<Renderer>();
        //}
        //else
        //{
        //    if(transform.GetChild(0).name == "Head_and_shoulders")
        //    {
        //        rend = transform.GetChild(0).GetChild(1).GetComponent<Renderer>();
        //    }
        //    else if(transform.GetChild(0).name == "Box002")
        //    {
        //        rend = transform.GetChild(1).GetComponent<Renderer>();
        //    }
        //    else
        //    {
        //       rend = transform.GetChild(0).GetComponent<Renderer>();
        //    }

        //}


    }

    void Enter()
    {
        selected = true;
        enterTime = DateTime.Now;
    }

    void Exit()
    {
        selected = false;
    }

    // Update is called once per frame
    void Update()
    {
        currentData = udp.getText();
        pickTime = udp.getPickTime();
        if (Mathf.Abs(parent.transform.position.x - GetComponent<Rigidbody>().position.x) < 7.5 &&
          Mathf.Abs(parent.transform.position.y - GetComponent<Rigidbody>().position.y) < 7.5 &&
          Mathf.Abs(parent.transform.position.z - GetComponent<Rigidbody>().position.z) < 7.5)
        {

            //GetComponent<Outline>().enabled = true;
            //rend.material.shader = shader2;
        }
        //else
        //{

        //    GetComponent<Outline>().enabled = false;
        //    //rend.material.shader = shader1;
        //}
        //if (transform.parent == dest.transform)
        //{

        //    GetComponent<Outline>().enabled = false;
        //    //rend.material.shader = shader1;
        //}
        //if (transform.parent == cart.transform)
        //{

        //    GetComponent<Outline>().enabled = false;
        //    //rend.material.shader = shader1;
        //}

        if (transform.parent == dest.transform)
        {
            isChild = true;
        }
        else
        {
            isChild = false;
        }



        //if (currentData.Equals("pick") && dest.childCount == 0 && selected && (enterTime<pickTime))
        //{
        //    OnDown();
        //    currentData = "init";
        //}

        var data = currentData.Split('|');

        //if (data[0].Equals("place"))
        //{
        //    handObject.SetActive(false);
        //}

        if (data[0].Equals("place") && isChild)
        {
            Place();
            currentData = "init";
        }
        if (currentData.Equals("replace") && isChild)
        {
            Replace();
            currentData = "init";
        }
        
    }


    public void OnDown()
    {
        //print("picked");
        //print(transform.name);
        
        //if ((Mathf.Abs(parent.transform.position.x - GetComponent<Rigidbody>().position.x) < 10 &&
        //    Mathf.Abs(parent.transform.position.y - GetComponent<Rigidbody>().position.y) < 10 &&
        //    Mathf.Abs(parent.transform.position.z - GetComponent<Rigidbody>().position.z) < 10 &&
        //    !cart.GetComponent<RotateCart>().isFull())  && dest.childCount==0 || transform.parent == cart.transform)
        
        if(!cart.GetComponent<RotateCart>().isFull() && dest.childCount==0 && transform.parent != cart.transform)
        {
            transform.position = dest.transform.position;
            transform.parent = dest.transform;
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;


            float a = vrCamera.eulerAngles.y;
            double b = (a * (Math.PI)) / 180;
            Vector3 newPos = new Vector3(parent.position.x + radius * (float)Math.Sin(b), 0.5f, parent.position.z + (radius * (float)Math.Cos(b) + 0.2f));
            Vector3 newRot = new Vector3(transform.eulerAngles.x, vrCamera.eulerAngles.y, 0f);

            print("Object: " + transform.name + " is picked");

            //canvasClone = Instantiate(canvas, parent);

            ////canvas.SetActive(true);


            ////canvas.GetComponent<RectTransform>().anchoredPosition = newPos;
            //Transform btn1 = canvasClone.transform.GetChild(0).GetChild(2);
            //btn1.GetComponent<Button>().onClick.AddListener(Place);
            //Transform btn2 = canvasClone.transform.GetChild(0).GetChild(3);
            //btn2.GetComponent<Button>().onClick.AddListener(Replace);

            //canvasClone.transform.eulerAngles = newRot;
            //canvasClone.transform.position = newPos;
            //canvasClone.transform.localScale = Vector3.one;
        }
        else
        {
            print("Object: " + transform.name + " is not in range");
        }
    }



    public void Place()
    {
        
        GameObject child = dest.GetChild(0).gameObject;
        if (Mathf.Abs(cart.transform.position.x - child.transform.position.x) < 5 &&
            Mathf.Abs(cart.transform.position.y - child.transform.position.y) < 5 &&
            Mathf.Abs(cart.transform.position.z - child.transform.position.z) < 5)
        {
            if (child)
            {
                //Destroy(canvasClone);
                child.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                child.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;

                child.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
                float a = vrCamera.eulerAngles.y;
                double b = (a * (Math.PI)) / 180;

                Vector3 newPos = new Vector3(parent.position.x + radiusObj * (float)Math.Sin(b), 0.3f, parent.position.z + radiusObj * (float)Math.Cos(b));
                child.transform.position = new Vector3(cart.transform.position.x, 0.3f, cart.transform.position.z);

                child.transform.parent = cart.transform;
                
                //child.transform.position = new Vector3(cart.transform.position.x, cart.transform.position.y, cart.transform.position.z);
                //child.transform.eulerAngles = new Vector3(transform.eulerAngles.x, vrCamera.eulerAngles.y - 90, transform.eulerAngles.z);
                

                child.GetComponent<Rigidbody>().useGravity = true;
                //child.GetComponent<Rigidbody>().velocity = Vector3.zero;
                //child.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                //canvas.SetActive(false);
                print("placed object: " + transform.name);
            }
        }
    }


    public void Replace()
    {
        print("replaced object: " + transform.name);
        //Destroy(canvasClone);
        GameObject child = dest.GetChild(0).gameObject;
        child.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        child.transform.parent = rack;
        child.transform.position = initialPos;
        child.transform.eulerAngles = initialRot;
        child.transform.localScale = initialScale;
        //print(child.GetComponent<PickObject>().getInitial());

        child.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        //canvas.SetActive(false);

    }

   
}