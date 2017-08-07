using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


public class CarControllerOffline : EnemyOffline
{
    [SerializeField] public Vector3 Data;

    public GameObject Gun;
    
    public bool CheckFinish = false;
    


    private NavMeshAgent _navMeshAgent;
    private int _line = 1;
    private List<GameObject> WayLines1;
    private List<GameObject> WayLines2;
    private List<GameObject> WayLines3;
    void Awake()
    {
        var carInfo = LoadDataManager.Data.Enemys[0];
        Health = carInfo.Health;
      
        Damage = carInfo.Damage;
        DamageFinish = carInfo.DamageFinish;
        Speed = carInfo.Speed;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.speed = Speed;
        WayLines1 = GameObject.FindGameObjectsWithTag("WayLine1").ToList();
        WayLines2 = GameObject.FindGameObjectsWithTag("WayLine2").ToList();
        WayLines3 = GameObject.FindGameObjectsWithTag("WayLine3").ToList();
        TargetPlayer = UnityEngine.Camera.main.gameObject;
    } 

    void Start()
    {
    }

    void Update()
    {
       
        if (CheckFinish&&_navMeshAgent.remainingDistance < 2)
        {
            var player = FindObjectOfType<PlayerControllerOffline>();
            player.Health -= DamageFinish;
            //    player.HealthSlider.value = player.Health;
            Destroy(gameObject);
        }

        if (!CheckFinish&& _navMeshAgent.remainingDistance < 3)
        {
            MoveToNextPoint(_line);
        }

        if (CanFire)
        {
            StartCoroutine(Fire());
        }
        Gun.transform.LookAt(TargetPlayer.transform.position);
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
        SpawnBullet1.gameObject.SetActive(true);
        GameObject bullet = Instantiate(BulletPrefab, SpawnBullet1.position, SpawnBullet1.rotation);
        bullet.GetComponent<BulletCarColliderControllOffline>().SetDamage(Damage);
        bullet.transform.localScale /= 2f;
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.up*400;
        yield return new WaitForSeconds(0.13f);
        SpawnBullet1.gameObject.SetActive(false);
        Destroy(bullet, 2);
        yield return new WaitForSeconds(3f);
        CanFire = true;
    }

    public void StartMove()
    {
        MoveToNextPoint(_line);
    }
}