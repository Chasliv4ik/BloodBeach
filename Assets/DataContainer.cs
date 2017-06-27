using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DataContainer
{
   
    public List<Enemy> Enemys;
 
    public List<GunsType> Guns;

    public static DataContainer CreateFromJson(string json)
    {
        return JsonUtility.FromJson<DataContainer>(json);
    }
}