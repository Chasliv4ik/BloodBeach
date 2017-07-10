using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class Enemy : NetworkBehaviour
{
    public string TypeEnemy;
    public float Speed;
    public float Damage;
    public float DamageFinish;

    [SyncVar(hook = "ShowSlider")]
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

    public void ShowSlider(int health)
    {
        if (!isServer) return;

        //foreach player
        Rpc_ShowSlider();
    }

    [ClientRpc]
    public void Rpc_ShowSlider()
    {
        
    }
}