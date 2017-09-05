
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public Image FillProcess;
    public Text InfoGun;

    public void ViewInfo(GunsType gun)
    {
        if (gun.TypeGunn != "GunDShK")
        {
           
            Debug.Log(gun.ReloadSize);
            FillProcess.fillAmount = gun.ReloadSize*1f / gun._reloadSize;
            InfoGun.text = gun.ReloadSize + "/" + gun.MagazinSize;
        }
        else
        {
            FillProcess.fillAmount = 1;
            InfoGun.text ="-/-";
        }
    }

  

    public void ReloadProcess(float time)
    {

        FillProcess.fillAmount =time;
    }

}

