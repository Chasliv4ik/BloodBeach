using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using NUnit.Compatibility;
using UnityEngine;

[Serializable]
public class GunsType
{
    [Serializable]
    public enum TypeGun
    {
        GunDShK,
        GunZu,
        GunRPG,
        GunPTRK
    }

    public int MagazinSize;
    public int ReloadSize;
    public float ReloadTime;
    public int DamageBoat;
    public int DamageCar;  
    public TypeGun TypeGunn;
   // public Color GunColor;

    private readonly int _reloadSize;

    //public GunsType(TypeGun e)
    //{
    //    switch (e)
    //    {
    //        case TypeGun.GunDShK:

    //            MagazinSize = 0; //infinity
    //            DamageBoat = 5;
    //            DamageCar = 0;
    //            ReloadSize = 0;
    //            ReloadTime = 0;
    //            GunColor = Color.blue;
    //            break;
    //        case TypeGun.GunZu:
    //            MagazinSize = 100000;
    //            ReloadSize = 10;
    //            ReloadTime = 1.5f;
    //            DamageBoat = 15;
    //            DamageCar = 5;
    //            GunColor = Color.gray;
    //            break;
    //        case TypeGun.GunRPG:
    //            MagazinSize = 100;
    //            ReloadSize = 3;
    //            ReloadTime = 3f;
    //            DamageCar = 100;
    //            DamageBoat = 80;
    //            GunColor = Color.red;
    //            break;
    //        case TypeGun.GunPTRK:
    //            MagazinSize = 50;
    //            ReloadSize = 2;
    //            ReloadTime = 5;
    //            DamageBoat = 100;
    //            DamageCar = 100;
    //            GunColor = Color.cyan;
    //            break;
    //    }
    //    Type = e;
    //    _reloadSize = ReloadSize;
    //}

    public bool IsShooting()
    {
        if (TypeGunn != TypeGun.GunDShK)
        {
            ReloadSize--;
            if (ReloadSize < 0)
            {
                return false;
            }
            MagazinSize--;
        }
        return true;
    }

    public void SetDefaultReloadSize()
    {
        ReloadSize = _reloadSize;
    }

    public static GunsType CreateGunFromJson(string jsonStr)
    {
        return JsonUtility.FromJson<GunsType>(jsonStr);
    }
}