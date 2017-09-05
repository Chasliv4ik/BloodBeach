using System;
using System.Collections;

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
    public float DelayShoot;
    public int DamageBoat;
    public int DamageCar;  
    public string TypeGunn;
    public UIManager ManagerUI;
    public bool isFire = true;
   // public Color GunColor;

    public  int _reloadSize;

    public GunsType(GunsType gunsType)
    {
        ManagerUI = GameObject.FindObjectOfType<UIManager>();
        DamageBoat = gunsType.DamageBoat;
        DamageCar = gunsType.DamageCar;
        MagazinSize = gunsType.MagazinSize;
        ReloadSize = gunsType.ReloadSize;
        TypeGunn = gunsType.TypeGunn;
        ReloadTime = gunsType.ReloadTime;
        DelayShoot = gunsType.DelayShoot;
        _reloadSize = ReloadSize;
       

    }

    public bool IsShooting()
    {
       
        if (TypeGunn != "GunDShK")
        {
            ReloadSize--;
            if (ReloadSize <= 0)
            {
                return false;
            }
            MagazinSize--;
        }
        ManagerUI.ViewInfo(this);
        return true;
    }

    public IEnumerator DelayAfteShoot()
    {
        yield return new WaitForSeconds(DelayShoot);
        isFire = true;
    }

    public IEnumerator ReloadGun()
    {
        isFire = false;
        ManagerUI.ViewInfo(this);
        float timeReload = 0.001f;
        while (timeReload<ReloadTime)
        {
            timeReload += Time.deltaTime;
            ManagerUI.ReloadProcess(timeReload/ReloadTime);
            yield return null;
        }
        isFire = true;
        ReloadSize = _reloadSize;
        ManagerUI.ViewInfo(this);
    }

  
    public static GunsType CreateGunFromJson(string jsonStr)
    {
        return JsonUtility.FromJson<GunsType>(jsonStr);
    }

 
}