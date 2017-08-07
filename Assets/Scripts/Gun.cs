using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityStandardAssets.Cameras;

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
    public Transform MovingPart;

    #endregion
 

    private void Start()
    {
     
    }
  





}