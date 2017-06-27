using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadDataManager : MonoBehaviour
{

    public string SserverUrl;

    [Serializable]
    public class EnemyInfo
    {
        public string TypeEnemy;
        public float Speed;
        public float Damage;
        public float DamageFinish;
        public int Health;
    }
    [Serializable]
    public class DataContainer
    {

        public List<EnemyInfo> Enemys;

        public List<GunsType> Guns;

        public static DataContainer CreateFromJson(string json)
        {
            return JsonUtility.FromJson<DataContainer>(json);
        }
    }

    
 
    public static DataContainer Data;
	void Start ()
	{
        Data = DataContainer.CreateFromJson(Resources.Load<TextAsset>("DataConfig").text);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
