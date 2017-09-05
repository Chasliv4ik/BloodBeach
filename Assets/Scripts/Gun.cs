
using Assets.Scripts.Interfaces;
using UnityEngine;

using UnityStandardAssets.Cameras;

public class Gun : MonoBehaviour
{
    #region public variables

    public GameObject Trunk,CameraRotatorGroup;
    [SerializeField]
    public GunsType Guns;
    public GunsType.TypeGun GunType;
    public GameObject  BulletPrefab;
    public Transform BulletSpawnTransform, BulletSpawnTransform2, MainCameraTransform,TargetTransform;
    public float BulletSpeed = 400;
    public LookatTarget LookAt;
    public Transform MovingPart;

    #endregion
 

    private void Start()
    {
     
    }

    public Transform GetBulletSpawn()
    {
        if (Guns.ReloadSize%2 == 0)
        {
            return BulletSpawnTransform;
        }
        return BulletSpawnTransform2;
    }
    
    public float GetDamage()
    {
        return 0;
    }
}