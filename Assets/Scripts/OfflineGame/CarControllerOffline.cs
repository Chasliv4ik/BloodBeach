using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
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
    private GameControllerOffline _gameController;

    void Awake()
    {
          
        _gameController = FindObjectOfType<GameControllerOffline>();
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
        if (!isDestroy)
        {
            if (_navMeshAgent.hasPath && CheckFinish && _navMeshAgent.remainingDistance < 5 && _line > 4)
            {
                var player = FindObjectOfType<PlayerControllerOffline>();
                player.Health -= DamageFinish;
                Debug.Log("DestroyCar");
                //    player.HealthSlider.value = player.Health;
                Destroy(gameObject);
            }

            if (!CheckFinish && _navMeshAgent.remainingDistance < 5)
            {
                MoveToNextPoint(_line);
            }

            if (CanFire)
            {
                StartCoroutine(Fire());
            }
            Gun.transform.LookAt(TargetPlayer.transform.position);
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

    private void SetDestination()
    {
        
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

    public override void Destroyed()
    {
        if (!isDestroy)
        {
            isDestroy = true;
            var expl = Instantiate(DestroyExplosion, transform);
            Destroy(expl, 2);
            CanFire = false;
            GetComponent<NavMeshAgent>().speed = 0;
            GetComponent<Animator>().enabled = true;
            GetComponent<Animator>().SetTrigger("OnDestroy");
            Destroy(transform.gameObject, 6);
        }
    }

    public override void TakeDamage(GunsType gun)
    {
        _gameController.HeathSlider.SetActive(true);
        Health -= gun.DamageCar;
        _gameController.HeathSlider.transform.GetChild(0).GetComponent<Image>().fillAmount = Health/60f;
        if (Health <= 0)
        {
            Destroyed();
            _gameController.HeathSlider.SetActive(false);
        }
    }

    public void StartMove(Vector3 startDestination)
    {
        _navMeshAgent.SetDestination(startDestination);
    }
}