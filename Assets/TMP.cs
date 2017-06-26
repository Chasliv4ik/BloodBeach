using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class TMP : MonoBehaviour
{
    // Use this for initialization
    public Transform TargeTransform;
    public float Speed;
    public Vector2 StartPosMouse;
    public Vector2 dir;
    public Text Text1;
    public Text Text2;

    private bool _canMove;


    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0) // && Input.mousePosition.x <= Screen.width/2)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                StartPosMouse = Input.GetTouch(0).position;
            }
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                var del = Input.GetTouch(0).deltaPosition;
                Text1.text = del.x + " " + del.y;

                TargeTransform.position = new Vector3(Mathf.Clamp(TargeTransform.position.x + (del.x*Speed), -90, 480),
                    Mathf.Clamp(TargeTransform.position.y + (del.y*Speed), -80, 160), GetZ());
            }
            if (Input.GetTouch(0).phase == TouchPhase.Stationary)
            {
                StartPosMouse = Input.GetTouch(0).position;
            }
        }
    }

  

    public float GetZ()
    {
        var tmp = Mathf.Sqrt((300*300) -
                             (TargeTransform.position.x - transform.position.x)*
                             (TargeTransform.position.x - transform.position.x) -
                             ((TargeTransform.position.y - transform.position.y)*
                              (TargeTransform.position.y - transform.position.y)));

        if (!float.IsNaN(tmp))
        {
            return tmp;
        }

        return TargeTransform.position.z;
    }
}