using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Enemy : MonoBehaviour
{
    public string TypeEnemy;
    public float Speed;
    public float Damage;
    public float DamageFinish;
    public int Health;

    public Transform Target;

    public GameObject BulletPrefab;

    public Transform SpawnBullet1;

    public Transform SpawnBullet2;

    public bool CanFire = true;

    public GameObject Camera;
}