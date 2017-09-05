using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : Enemy
{
    private bool _canMove;
    public Transform CarSpawnTransform;
    public GameObject CarPrefab;
    public int CountCar = 2;
    public float TimeSpawnCar = 4;
    private GameObject[] _carTargets;
    public GameObject Gun1, Gun2;


    void Start()
    {
        var shipInfo = LoadDataManager.Data.Enemys[1];
        Health = shipInfo.Health;
        Damage = shipInfo.Damage;
        Speed = shipInfo.Speed;
        _carTargets = GameObject.FindGameObjectsWithTag(Tags.SpawnTag);
        TargetPlayer = UnityEngine.Camera.main.gameObject;
    }


    void Update()
    {
        if (_canMove)
        {
            if (transform.position == TargetPosition.position)
            {
                GetComponent<Animator>().SetTrigger(Tags.StopeMoveTrigger);
                _canMove = false;
                StartCoroutine(SpawnEnemyCar());
            }
            transform.position = Vector3.MoveTowards(transform.position, TargetPosition.position, Speed*Time.deltaTime);
        }
        if (CanFire)
        {
            StartCoroutine(Fire());
        }

        Gun1.transform.LookAt(TargetPlayer.transform.position);
        Gun2.transform.LookAt(TargetPlayer.transform.position);
    }

    public void MoveTo(Transform target)
    {
        TargetPosition = target;
        _canMove = true;
        var dir = (TargetPosition.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(dir);
    }

    public void Destroyed()
    {
        GetComponent<Animator>().SetTrigger(Tags.DestroyTrigger);
        Destroy(transform.gameObject, 5f);
    }

    IEnumerator SpawnEnemyCar()
    {
        while (CountCar > 0)
        {
            var car = Instantiate(CarPrefab, CarSpawnTransform.position, Quaternion.identity);
            car.GetComponent<CarController>().TargetPosition = _carTargets[Random.Range(0, _carTargets.Length - 1)].transform;
            car.GetComponent<CarController>().StartMove();
            yield return new WaitForSeconds(TimeSpawnCar);
            CountCar--;
        }
    }

    IEnumerator Fire()
    {
        CanFire = false;
        for (int i = 0; i < 5; i++)
        {
            SpawnBullet1.gameObject.SetActive(true);
            GameObject bullet = Instantiate(BulletPrefab, SpawnBullet1.position, SpawnBullet1.rotation);
            bullet.GetComponent<BulletCarColliderControll>().SetDamage(Damage);
            bullet.transform.localScale /= 3f;
            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.up*300;

            yield return new WaitForSeconds(0.13f);
            SpawnBullet1.gameObject.SetActive(false);
            Destroy(bullet, 8);
        }
        yield return new WaitForSeconds(3f);
        for (int i = 0; i < 5; i++)
        {
          
            SpawnBullet2.gameObject.SetActive(true);
            GameObject bullet2 = Instantiate(BulletPrefab, SpawnBullet2.position, SpawnBullet2.rotation);
            bullet2.GetComponent<BulletCarColliderControll>().SetDamage(Damage);
            bullet2.transform.localScale /= 3f;
            bullet2.GetComponent<Rigidbody>().velocity = bullet2.transform.up*300;
            yield return new WaitForSeconds(0.13f);
            SpawnBullet2.gameObject.SetActive(false);
            Destroy(bullet2, 8);
         
        }
        yield return new WaitForSeconds(3f);
        CanFire = true;
    }
}