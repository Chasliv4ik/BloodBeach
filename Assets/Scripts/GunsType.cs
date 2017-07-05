using System;
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
    public string TypeGunn;
   // public Color GunColor;

    private readonly int _reloadSize;

    public GunsType(GunsType gunsType)
    {

        DamageBoat = gunsType.DamageBoat;
        DamageCar = gunsType.DamageCar;
        MagazinSize = gunsType.MagazinSize;
        ReloadSize = gunsType.ReloadSize;
        TypeGunn = gunsType.TypeGunn;
        ReloadTime = gunsType.ReloadTime;
        _reloadSize = ReloadSize;

        //switch (e)
        //{
        //    case TypeGun.GunDShK:

        //        MagazinSize = 0; //infinity
        //        DamageBoat = 5;
        //        DamageCar = 0;
        //        ReloadSize = 0;
        //        ReloadTime = 0;
        //       this.                break;
        //    case TypeGun.GunZu:
        //        MagazinSize = 100000;
        //        ReloadSize = 10;
        //        ReloadTime = 1.5f;
        //        DamageBoat = 15;
        //        DamageCar = 5;

        //        break;
        //    case TypeGun.GunRPG:
        //        MagazinSize = 100;
        //        ReloadSize = 3;
        //        ReloadTime = 3f;
        //        DamageCar = 100;
        //        DamageBoat = 80;

        //        break;
        //    case TypeGun.GunPTRK:
        //        MagazinSize = 50;
        //        ReloadSize = 2;
        //        ReloadTime = 5;
        //        DamageBoat = 100;
        //        DamageCar = 100;

        //        break;
        //}


    }

    public bool IsShooting()
    {
        if (TypeGunn != "GunDShK")
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