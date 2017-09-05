using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class TankControllerOffline : EnemyOffline {
    public GameObject Gun;

    public bool CheckFinish = false;
    public GameObject PrefabGunExplosion;

    private NavMeshAgent _navMeshAgent;
    private int _line = 1;
    private List<GameObject> WayLines1;
    private List<GameObject> WayLines2;
    private List<GameObject> WayLines3;
    private GameControllerOffline _gameController;

    void Awake()
    {
        _gameController = FindObjectOfType<GameControllerOffline>();
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
        TargetPlayer = Camera.main.gameObject;
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
                Debug.Log("DestroyTank");
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
        yield return StartCoroutine(LookAtPlayer());
        SpawnBullet1.gameObject.SetActive(true);
        var expl = Instantiate(PrefabGunExplosion, SpawnBullet1.position, Quaternion.identity);
        Destroy(expl,3);
        GameObject bullet = Instantiate(BulletPrefab, SpawnBullet1.position, SpawnBullet1.localRotation);
        bullet.GetComponent<BulletCarColliderControllOffline>().SetDamage(Damage);
        bullet.transform.SetParent(SpawnBullet1.transform);
        bullet.transform.localScale = new Vector3(1,4,1);
        bullet.transform.localEulerAngles = new Vector3(90, 0, 0);
        bullet.transform.SetParent(null);
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.up * 400;
       
        //  GetComponent<Animator>().SetTrigger("OnFire");
        _navMeshAgent.speed = Speed;
        Destroy(bullet, 2);
        yield return new WaitForSeconds(Random.Range(5,10));
        SpawnBullet1.gameObject.SetActive(false);
        CanFire = true;
     
    }

    IEnumerator LookAtPlayer()
    {
        _navMeshAgent.speed = 0;
        yield return new WaitForSeconds(1f);
         GameObject horizontalRotator = new GameObject()
         {
             transform =
             {
                 position = Gun.transform.position,
                 rotation = Gun.transform.rotation
             }
         };
          horizontalRotator.transform.LookAt(TargetPlayer.transform.position);
          iTween.RotateTo(Gun,new Vector3(0,horizontalRotator.transform.localEulerAngles.y,0),2f);
        GameObject verticalRotator = new GameObject()
        {
            transform =
             {
                 position = Gun.transform.GetChild(0).position
            //     rotation = Gun.transform.GetChild(0).rotation
             }
        };
          verticalRotator.transform.LookAt(TargetPlayer.transform.position);
        //Gun.transform.GetChild(0).localEulerAngles = new Vector3(0,0,0);
        iTween.RotateTo(Gun.transform.GetChild(0).gameObject, new Vector3(verticalRotator.transform.localEulerAngles.x,horizontalRotator.transform.localEulerAngles.y, 0), 2f);
        Destroy(horizontalRotator);
        Destroy(verticalRotator);
        yield return new WaitForSeconds(2f);


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
        _gameController.HeathSlider.transform.GetChild(0).GetComponent<Image>().fillAmount = Health / 100f;
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
