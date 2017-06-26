using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string TypeEnemy;
    public float Speed,Damage,DamageFinish;
    public int Health;
    public Transform Target;
    public GameObject BulletPrefab;
    public Transform SpawnBullet1, SpawnBullet2;
    public bool CanFire = true;
    public GameObject Camera;

    public static Enemy CreatEnemyFromJson(string strJson)
    {
        return JsonUtility.FromJson<Enemy>(strJson);
    }
}