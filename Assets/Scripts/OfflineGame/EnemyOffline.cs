using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class EnemyOffline : MonoBehaviour
{
    public string TypeEnemy;
    public float Speed;
    public float Damage;
    public float DamageFinish;
    public int Health;

    public Transform TargetPosition;
    public GameObject BulletPrefab;
    public Transform SpawnBullet1;
    public Transform SpawnBullet2;
    public bool CanFire = true;
    public GameObject TargetPlayer;


    public void TakeDamage(int amount)
    {
        
    }

   
}