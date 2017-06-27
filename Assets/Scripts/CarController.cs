using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


public class CarController : Enemy
{
    [SerializeField] public Vector3 Data;

    public GameObject Gun;

    public bool CanMove = false;

  
    private NavMeshAgent _navMeshAgent;

    void Awake()
    {
        var carInfo = LoadDataManager.Data.Enemys[0];
        Health = carInfo.Health;
        Damage = carInfo.Damage;
        DamageFinish = carInfo.DamageFinish;
        Speed = carInfo.Speed;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        Camera = UnityEngine.Camera.main.gameObject;
    }

    void Start()
    {
    }

    void Update()
    {
        if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance && _navMeshAgent.remainingDistance > 0)
        {
            var player = FindObjectOfType<PlayerController>();
            player.Health -= DamageFinish;
            player.HealthSlider.value = player.Health;
            Destroy(gameObject);
        }


        if (CanFire)
        {
            StartCoroutine(Fire());
        }
        Gun.transform.LookAt(Camera.transform.position);
        //   Data = _navMeshAgent.nextPosition.normalized;
        // transform.LookAt(Data);
    }

    IEnumerator Fire()
    {
        CanFire = false;
        SpawnBullet1.gameObject.SetActive(true);
        GameObject bullet = Instantiate(BulletPrefab, SpawnBullet1.position, SpawnBullet1.rotation);
        bullet.GetComponent<BulletCarColliderControll>().SetDamage(Damage);
        bullet.transform.localScale /= 2f;
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.up*400;
        yield return new WaitForSeconds(0.13f);
        SpawnBullet1.gameObject.SetActive(false);
        Destroy(bullet, 2);
        yield return new WaitForSeconds(2.5f);
        SpawnBullet2.gameObject.SetActive(true);
        GameObject bullet2 = Instantiate(BulletPrefab, SpawnBullet2.position, SpawnBullet2.rotation);
        bullet2.GetComponent<BulletCarColliderControll>().SetDamage(Damage);
        bullet2.transform.localScale /= 2f;
        yield return new WaitForSeconds(0.13f);
        SpawnBullet2.gameObject.SetActive(false);
        Destroy(bullet2, 2);
        yield return new WaitForSeconds(2.5f);
        CanFire = true;
    }

    public void StartMove()
    {
        _navMeshAgent.SetDestination(Target.position);
    }
}