﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class TankControllerOffline : EnemyOffline {
    public GameObject Gun;

    public bool CheckFinish = false;


    private NavMeshAgent _navMeshAgent;
    private int _line = 1;
    private List<GameObject> WayLines1;
    private List<GameObject> WayLines2;
    private List<GameObject> WayLines3;
    void Awake()
    {
        var tankInfo = LoadDataManager.Data.Enemys[2];
        Health = tankInfo.Health;
        Damage = tankInfo.Damage;
        DamageFinish = tankInfo.DamageFinish;
        Speed = tankInfo.Speed;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.speed = Speed;
      
        WayLines1 = GameObject.FindGameObjectsWithTag("WayLine1").ToList();
        WayLines2 = GameObject.FindGameObjectsWithTag("WayLine2").ToList();
        WayLines3 = GameObject.FindGameObjectsWithTag("WayLine3").ToList();
        TargetPlayer = GameObject.FindWithTag("Player");
    }

    void Start()
    {
    }

    void Update()
    {
       
        Gun.transform.LookAt(TargetPlayer.transform.position);
       Gun.transform.localEulerAngles = new Vector3(0, Gun.transform.localEulerAngles.y, 0);
        Gun.transform.GetChild(0).LookAt(TargetPlayer.transform.position);
       Gun.transform.GetChild(0).localEulerAngles = new Vector3(Gun.transform.GetChild(0).localEulerAngles.x, 0, 0);
        if (CheckFinish && _navMeshAgent.remainingDistance < 2)
        {
            var player = FindObjectOfType<PlayerControllerOffline>();
            player.Health -= DamageFinish;
            //    player.HealthSlider.value = player.Health;
            Destroy(gameObject);
        }

        if (!CheckFinish && _navMeshAgent.remainingDistance < 3)
        {
            MoveToNextPoint(_line);
        }

        if (CanFire)
        {
           StartCoroutine(Fire());
        }
       
    }

    public void MoveToNextPoint(int line) 
    {
        switch (line)
        {
            case 1:
                _navMeshAgent.SetDestination(WayLines1[Random.Range(0, WayLines1.Count)].transform.position);
                break;
            case 2:
                _navMeshAgent.SetDestination(WayLines2[Random.Range(0, WayLines2.Count)].transform.position);
                break;
            case 3:
                _navMeshAgent.SetDestination(WayLines3[Random.Range(0, WayLines3.Count)].transform.position);
                break;
            case 4:
                _navMeshAgent.SetDestination(TargetPosition.position);
                CheckFinish = true;
                break;
        }
        _line++;
    }

    IEnumerator Fire()
    {
        CanFire = false;
      //  _navMeshAgent.speed = 0;
        SpawnBullet1.gameObject.SetActive(true);
        GameObject bullet = Instantiate(BulletPrefab, SpawnBullet1.position, SpawnBullet1.rotation);
        bullet.GetComponent<BulletCarColliderControllOffline>().SetDamage(Damage);
        bullet.transform.localScale = new Vector3(1,4,1);
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.right * 400;
        bullet.transform.localEulerAngles = new Vector3(-90,0,0);
      //  GetComponent<Animator>().SetTrigger("OnFire");
        Destroy(bullet, 2);
        yield return new WaitForSeconds(Random.Range(5,10));
        SpawnBullet1.gameObject.SetActive(false);
        CanFire = true;
    }

    public void StartMove()
    {
        MoveToNextPoint(_line);
    }
}