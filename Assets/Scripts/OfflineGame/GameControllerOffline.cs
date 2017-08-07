using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class GameControllerOffline : MonoBehaviour
{

    public List<TargetController> TargetTransform;
    public Vector3 SpawnPosistion = new Vector3(-2000,0f,0);
    float deltaTime = 0.0f;
    public GameObject HeathSlider;
    public GameObject ShipPrefab;
    public int MaxCountShip = 3;
    public float AfterTimeInstantiate = 5f;
    public GameObject PausePanel;
	void Start ()
	{
	    TargetTransform = new List<TargetController>(FindObjectsOfType<TargetController>());
	    StartCoroutine(InstantiateShip(MaxCountShip));
	}

    void Update()
    {
        if(Time.timeScale>0)
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        if (Input.GetKey(KeyCode.Escape))
        {
            PausePanel.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void SetOk()
    {
        Time.timeScale = 1; 
        PausePanel.SetActive(false);
    }

    void OnGUI()
    {
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUILayout.Label(text);
    }
    public IEnumerator InstantiateShip(int countship)
    {
        while (countship > 0)
        {
            var ship =  Instantiate(ShipPrefab,new Vector3(Random.Range(100, 500), SpawnPosistion.y,SpawnPosistion.z), Quaternion.identity);
            ship.GetComponent<ShipControllerOffline>().MoveTo(GetTarget().transform);
             countship--;
            yield return new WaitForSeconds(AfterTimeInstantiate);
        }
    }

    public void OnOffZoom()
    {
        Camera mCamera = Camera.main;
        if (mCamera.fieldOfView == 60)
        {
            mCamera.fieldOfView = 30;
        }
        else
            mCamera.fieldOfView = 60;
    }
    TargetController GetTarget()
    {
        var target = TargetTransform[Random.Range(0, TargetTransform.Count)];
        if (target.IsEmpty)
        {
            target.IsEmpty = false;
            return target;
        }
        else
        {
            return GetTarget();
        }
    }

    public void InstantiateShip()
    {
        StartCoroutine(InstantiateShip(1));
    }
	// Update is called once per frame
	
    

}
