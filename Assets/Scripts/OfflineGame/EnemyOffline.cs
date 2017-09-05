using System;

using UnityEngine;


[Serializable]
public abstract class EnemyOffline : MonoBehaviour
{
    public string TypeEnemy;
    public float Speed;
    public float Damage;
    public float DamageFinish;
    public int Health;
    public GameObject DestroyExplosion;
    public Transform TargetPosition;
    public GameObject BulletPrefab;
    public Transform SpawnBullet1;
    public Transform SpawnBullet2;
    public bool CanFire = true;
    public GameObject TargetPlayer;
    public bool isDestroy = false;

     
    public virtual void Destroyed()
    {
        
    }

    public virtual void TakeDamage(GunsType gun)
    {
        
    } 

   
}