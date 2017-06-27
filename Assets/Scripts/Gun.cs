using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityStandardAssets.Cameras;
using Debug = UnityEngine.Debug;

public class Gun : MonoBehaviour
{
    #region public variables

    public GameObject Trunk,Camera;
    [SerializeField]
    public GunsType Guns;
    public GunsType.TypeGun GunType;
    public GameObject  BulletPrefab;
    public Transform BulletSpawnTransform,MainCameraTransform,TargetTransform;
    public float BulletSpeed = 400;
    public LookatTarget LookAt;

    #endregion

 

    private void Start()
    {
      Guns = new GunsType(LoadDataManager.Data.Guns.FirstOrDefault(x=>x.TypeGunn == GunType.ToString()));
    }
  





}