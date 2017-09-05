
using System.Collections;

using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using GameController = GameController;
using Random = UnityEngine.Random;

public class ShipControllerOffline : EnemyOffline
{
    private bool _canMove;
    public Transform CarSpawnTransform;
    public GameObject CarPrefab,TankPrefab;
    public int CountCar = 2;
    public int CountTank = 2;
    public float TimeSpawnCar = 4;
    private GameObject[] _carTargets;
    public GameObject Gun1, Gun2;
    private GameControllerOffline _gameController;
 

    void Start()
    {
        _gameController = FindObjectOfType<GameControllerOffline>();
        var shipInfo = LoadDataManager.Data.Enemys[1];
        Health = shipInfo.Health;
        Damage = shipInfo.Damage;
        Speed = shipInfo.Speed;
       
        //Health = 100;
        //Damage = 5;
        //Speed = Random.Range(20, 40);
        _carTargets = GameObject.FindGameObjectsWithTag(Tags.SpawnTag);
        TargetPlayer = UnityEngine.Camera.main.gameObject;
    }


    void Update()
    {
        if (!isDestroy)
        {
            if (_canMove)
            {
                if (transform.position == TargetPosition.position)
                {
                    GetComponent<Animator>().SetTrigger(Tags.StopeMoveTrigger);
                    _canMove = false;
                    StartCoroutine(SpawnEnemy());
                }
                transform.position = Vector3.MoveTowards(transform.position, TargetPosition.position,
                    Speed*Time.deltaTime);
            }
            if (CanFire)
            {
                StartCoroutine(Fire());
            }

            Gun1.transform.LookAt(TargetPlayer.transform.position);
            Gun2.transform.LookAt(TargetPlayer.transform.position);
        }
    }

    public void MoveTo(Transform target)
    {
        TargetPosition = target;
        _canMove = true;
        var dir = (TargetPosition.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(dir);
    }

    public override void Destroyed()
    {
        if (!isDestroy)
        {
            var expl = Instantiate(DestroyExplosion, transform);
            Destroy(expl, 2);
            GetComponent<Animator>().SetTrigger(Tags.DestroyTrigger);
            Destroy(transform.gameObject, 5f);
            _gameController.TargetTransform.FirstOrDefault(x => x.transform == TargetPosition).IsEmpty = true;
            _gameController.InstantiateShip();
            _gameController.HeathSlider.SetActive(false);
            isDestroy = true;
        }
    }
    public override void TakeDamage(GunsType gun)
    {
        _gameController.HeathSlider.SetActive(true);
        Health -= gun.DamageBoat;
        _gameController.HeathSlider.transform.GetChild(0).GetComponent<Image>().fillAmount = Health / 100f;
        if (Health <= 0)
        {
            Destroyed();
            _gameController.HeathSlider.SetActive(false);
        }
    }

    IEnumerator SpawnEnemy()
    {
        while (CountCar > 0)
        {
            var car = Instantiate(CarPrefab, CarSpawnTransform.position,CarPrefab.transform.rotation);
            car.GetComponent<CarControllerOffline>().TargetPosition = _carTargets[Random.Range(0, _carTargets.Length - 1)].transform;
            car.GetComponent<CarControllerOffline>().StartMove(CarSpawnTransform.GetChild(0).transform.position);
            yield return new WaitForSeconds(TimeSpawnCar);
            CountCar--;
        }
        while (CountTank>0)
        {
            var tank = Instantiate(TankPrefab, CarSpawnTransform.position, TankPrefab.transform.rotation);
            tank.GetComponent<TankControllerOffline>().TargetPosition = _carTargets[Random.Range(0, _carTargets.Length - 1)].transform;
            tank.GetComponent<TankControllerOffline>().StartMove(CarSpawnTransform.GetChild(0).transform.position);
            yield return new WaitForSeconds(TimeSpawnCar*3);
            CountTank--;
        }
    }

    IEnumerator Fire()
    {
        CanFire = false;
        for (int i = 0; i < 5; i++)
        {
        //    SpawnBullet1.gameObject.SetActive(true);
            GameObject bullet = Instantiate(BulletPrefab, SpawnBullet1.position, SpawnBullet1.rotation);
            bullet.GetComponent<BulletCarColliderControllOffline>().SetDamage(Damage);
            bullet.transform.localScale /= 2f;
            bullet.transform.localEulerAngles = new Vector3(bullet.transform.localEulerAngles.x-90, bullet.transform.localEulerAngles.y);
            bullet.GetComponent<Rigidbody>().velocity = -bullet.transform.up*300;

            yield return new WaitForSeconds(0.13f);
        //    SpawnBullet1.gameObject.SetActive(false);
            Destroy(bullet, 8);
        }
        yield return new WaitForSeconds(3f);
        for (int i = 0; i < 5; i++)
        {
          
        //    SpawnBullet2.gameObject.SetActive(true);
            GameObject bullet2 = Instantiate(BulletPrefab, SpawnBullet2.position, SpawnBullet2.rotation);
            bullet2.GetComponent<BulletCarColliderControllOffline>().SetDamage(Damage);
            bullet2.transform.localScale /= 2f;
            bullet2.transform.localEulerAngles = new Vector3(bullet2.transform.localEulerAngles.x - 90, bullet2.transform.localEulerAngles.y);
            bullet2.GetComponent<Rigidbody>().velocity = -bullet2.transform.up * 300;

            yield return new WaitForSeconds(0.13f);
        //    SpawnBullet2.gameObject.SetActive(false);
            Destroy(bullet2, 8);
         
        }
        yield return new WaitForSeconds(3f);
        CanFire = true;
    }


  
}