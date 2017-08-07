using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class PlayerControllerOffline : MonoBehaviour {

    #region public variables

    public Gun CurrentGun;
    public List<Gun> Guns;
    [SerializeField]
    public float  MyAngle, AngleHRange;
    public Vector2 RangeLeftRight;
    public float Sensitivity = 1f;
    public Vector3 TmpMousePos;
    public float Health = 10000;
    public Slider HealthSlider;
    public float distance = 100;
    public float rotationSpeed = 2;
    public Text InfoGun;
    public Transform TargetTransform;
    public GameObject PrefabParticleTerrain,PrefabParticleExplosion, BulletPrefab;
    public Slider SliderSpeed, SliderSmooth;
    #endregion

    #region private variables

    private Vector3 dir;
    private Vector3 targetPoint;
    private bool isClick = false;
    private bool isFire = true;
    private float tmpAngle;
    private GameControllerOffline _gameController;

    [SerializeField]
    //  private Dictionary<GunsType.TypeGun, int> MagazinData = new Dictionary<GunsType.TypeGun, int>();

    #endregion

 
    private GameObject _shootHelper;

    private void Start()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        CurrentGun.LookAt.enabled = false;
        rotationSpeed = 20;
#endif
        _gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerOffline>();
        _shootHelper = GameObject.Find("Shoot Helper");
        foreach (var gun in Guns)
        {
            gun.Guns = new GunsType(LoadDataManager.Data.Guns.FirstOrDefault(x => x.TypeGunn == gun.GunType.ToString()));
        }
        //     MagazinData.Add(CurrentGun.Guns.Type, CurrentGun.Guns.MagazinSize);
        //  Guns = FindObjectsOfType<CurrentGun>();
        ViewInfoAboutGun(CurrentGun.Guns);
    }

    void Update()
    {
       

#if UNITY_EDITOR || UNITY_STANDALONE_WIN

        if (Input.GetMouseButtonDown(0))
        {
            CastRayToWorld();
        }
        if (isClick)
        {
            LookAtThis(targetPoint);
        }
#endif

#if UNITY_ANDROID
        if (Input.touchCount > 0) // && Input.mousePosition.x <= Screen.width/2)
        {

            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                var del = Input.GetTouch(0).deltaPosition;
                TargetTransform.position = new Vector3(Mathf.Clamp(TargetTransform.position.x + (del.x * rotationSpeed), -90, 480),
                    Mathf.Clamp(TargetTransform.position.y + (del.y * rotationSpeed), -80, 160), GetZ());
            }else if (Input.GetTouch(1).phase == TouchPhase.Moved)
            {
                var del = Input.GetTouch(1).deltaPosition;
                TargetTransform.position = new Vector3(Mathf.Clamp(TargetTransform.position.x + (del.x * rotationSpeed), -90, 480),
                    Mathf.Clamp(TargetTransform.position.y + (del.y * rotationSpeed), -80, 160), GetZ());
            }
        }
#endif
    }

  

   

    public float GetZ()
    {
        var tmp = Mathf.Sqrt((300 * 300) -
                             (TargetTransform.position.x - transform.position.x) *
                             (TargetTransform.position.x - transform.position.x) -
                             ((TargetTransform.position.y - transform.position.y) *
                              (TargetTransform.position.y - transform.position.y)));

        if (!float.IsNaN(tmp))
        {
            return tmp;
        }

        return TargetTransform.position.z; 
    }

    #region OldControll
    private float CalculateAngle(Vector3 temp)
    {
        dir = new Vector3(temp.x, temp.y, temp.z) - CurrentGun.Camera.transform.position;
        return Vector3.Angle(dir, transform.forward);
    }

    private void LookAtThis(Vector3 target)
    {
        if (CalculateAngle(target) > 0.1f)
        {
            CurrentGun.Camera.transform.rotation = Quaternion.RotateTowards(CurrentGun.Camera.transform.rotation,
                Quaternion.LookRotation(dir), rotationSpeed * Time.deltaTime);
            var tmpy = GetRangeHorizontal(CurrentGun.Camera.transform.localEulerAngles.y);
            var tmpx = GetRangeVertical(CurrentGun.Camera.transform.localEulerAngles.x);
            CurrentGun.Camera.transform.localEulerAngles = new Vector3(tmpx, tmpy, 0);
        }
    }

    void CastRayToWorld()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        bool isUIObject = false; 
        foreach (var r in results)
        {
            if (r.gameObject.layer == 5)
            {
                isUIObject = true;
                break;
            }
        }
        if (!isUIObject)
        {
            targetPoint = ray.origin + (ray.direction * distance);
            isClick = true;
        }
    }

    public void ChangeSliderSpeedSmooth()
    {
        CurrentGun.LookAt.m_FollowSpeed = SliderSmooth.value;
    }

    public void ChangeSliderSpeed()
    {
        rotationSpeed = SliderSpeed.value;
    }

    //void FixedUpdate()
    //{
        //if (Input.touchCount > 0) // && Input.mousePosition.x <= Screen.width/2)
        //{
        //    if (Input.GetTouch(0).phase == TouchPhase.Began)
        //        TmpMousePos = Input.GetTouch(0).position;
        //    if (Input.GetTouch(0).phase == TouchPhase.Moved)
        //    {

        //        MyAngle = Sensitivity * ((Input.GetTouch(0).position.x - TmpMousePos.x) / Screen.width);
        //        CurrentGun.TargetPlayer.transform.RotateAround(CurrentGun.TargetPlayer.transform.position, CurrentGun.TargetPlayer.transform.up, MyAngle);

        //        MyAngle = Sensitivity * ((Input.GetTouch(0).position.y - TmpMousePos.y) / Screen.height);
        //        CurrentGun.TargetPlayer.transform.RotateAround(CurrentGun.TargetPlayer.transform.position, CurrentGun.TargetPlayer.transform.right, -MyAngle);

        //        var tmpy = GetRangeHorizontal(CurrentGun.TargetPlayer.transform.localEulerAngles.y);
        //        var tmpx = GetRangeVertical(CurrentGun.TargetPlayer.transform.localEulerAngles.x);
        //        CurrentGun.TargetPlayer.transform.localEulerAngles = new Vector3(tmpx,
        //        tmpy, 0);
                
        //    }
        //}
    //}

   

    public float GetRangeHorizontal(float tmp)
    {
        if (tmp < 360 - RangeLeftRight.x && tmp > 180)
            tmp = 360 - RangeLeftRight.x;
        if (tmp > RangeLeftRight.y && tmp < 180)
            tmp = RangeLeftRight.y;
        return tmp;
    }

    public float GetRangeVertical(float tmp)
    {
        if (tmp < 360 - AngleHRange && tmp > 180)
            tmp = 360 - AngleHRange;
        if (tmp > AngleHRange && tmp < 180)
            tmp = AngleHRange;
        return tmp;
    }
    #endregion

    public void Fire()
    {
        if (CurrentGun.Guns.MagazinSize > 0 || CurrentGun.Guns.TypeGunn == "GunDShK")
        {
            if (isFire && CurrentGun.Guns.IsShooting())
            {
                CurrentGun.GetComponentInChildren<Animator>().SetTrigger("Fire"); 
                GameObject bullet = Instantiate(BulletPrefab, CurrentGun.BulletSpawnTransform.position,
                CurrentGun.BulletSpawnTransform.rotation);

                bullet.transform.localRotation = CurrentGun.BulletSpawnTransform.rotation;
                bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 400;

                _shootHelper.transform.position = CurrentGun.BulletSpawnTransform.position;
                _shootHelper.transform.rotation = CurrentGun.BulletSpawnTransform.rotation;
                _shootHelper.transform.localRotation = CurrentGun.BulletSpawnTransform.rotation;

                RaycastHit hit;
                if (Physics.Raycast(_shootHelper.transform.position, _shootHelper.transform.forward * CurrentGun.BulletSpeed, out hit))
                {
                    OnFallingIntoGoal(hit);
                }
            }
            else
            {
                isFire = false;
                StartCoroutine(ReloadGun());
            }
        }
    }

    public void OnFire()
    {
       

        StartCoroutine("HoldFire");
    }

    public void OffFire()
    {
        StopCoroutine("HoldFire");
    }

    IEnumerator HoldFire()
    {
        while (true)
        {
            Fire();
            yield return new WaitForSeconds(0.2f);
        }
    }

   

    IEnumerator ReloadGun()
    {
        yield return new WaitForSeconds(CurrentGun.Guns.ReloadTime);
        CurrentGun.Guns.SetDefaultReloadSize();
        ViewInfoAboutGun(CurrentGun.Guns);
        isFire = true;
    }

    public void SetTypeGun(int type)
    {
        //Camera.main.transform.SetParent(transform);
        Quaternion tmpCamera = CurrentGun.Camera.transform.rotation;
        CurrentGun.gameObject.SetActive(false); 
        CurrentGun = Guns.Find(gun => gun.GunType == ((GunsType.TypeGun) type));
        CurrentGun.Camera.transform.rotation = tmpCamera;
        CurrentGun.gameObject.SetActive(true);
        Camera.main.transform.position = CurrentGun.MainCameraTransform.position;
        Camera.main.transform.SetParent(CurrentGun.MainCameraTransform);
        ViewInfoAboutGun(CurrentGun.Guns);
    }

    public void OnFallingIntoGoal(RaycastHit hit)
    {
        GameObject expl;

        switch (hit.collider.tag)
        {
            case "Enemy":
                expl = Instantiate(PrefabParticleExplosion, hit.point, Quaternion.identity);
                Destroy(expl, 2);
             ShipControllerOffline EC = hit.collider.transform.parent.gameObject.GetComponent<ShipControllerOffline>();
                _gameController.HeathSlider.SetActive(true);
                EC.Health -= CurrentGun.Guns.DamageBoat;
                _gameController.HeathSlider.transform.GetChild(0).GetComponent<Image>().fillAmount = EC.Health / 100f;

                if (EC.Health <= 0)
                {
                    //Handheld.Vibrate();
                    EC.Destroyed();
                    _gameController.TargetTransform.FirstOrDefault(x => x.transform == EC.TargetPosition).IsEmpty = true;
                    _gameController.InstantiateShip();
                    _gameController.HeathSlider.SetActive(false);
                    hit.collider.enabled = false;
                }
                break;
            case "Car":
                if (CurrentGun.Guns.DamageCar > 0)
                {
                    if (!RandomeRebound(hit))
                    {
                        expl = Instantiate(PrefabParticleExplosion, hit.point, Quaternion.identity);
                        Destroy(expl, 2);
                        CarControllerOffline CC = hit.collider.GetComponent<CarControllerOffline>();
                        _gameController.HeathSlider.SetActive(true);
                        CC.Health -= CurrentGun.Guns.DamageCar;
                        _gameController.HeathSlider.transform.GetChild(0).GetComponent<Image>().fillAmount = CC.Health/
                                                                                                             60f;

                        if (CC.Health <= 0)
                        {
                            //Handheld.Vibrate();
                            CC.CanFire = false;
                            CC.GetComponent<NavMeshAgent>().speed = 0;
                            CC.GetComponent<Animator>().SetTrigger("OnDestroy");
                            Destroy(CC.transform.gameObject,3);
                            _gameController.HeathSlider.SetActive(false);
                            hit.collider.enabled = false;
                        }
                    }
                }
                else
                {
                    GameObject bullet = Instantiate(BulletPrefab,hit.transform.position,
                  Quaternion.identity);
                   bullet.transform.localEulerAngles = new Vector3(Random.Range(-20,-120),bullet.transform.localEulerAngles.y);
                        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 400;
                    Destroy(bullet,2);
                }
                break;
            case "Tank":
                if (CurrentGun.Guns.DamageCar > 0)
                {

                    if (!RandomeRebound(hit))
                    {
                        expl = Instantiate(PrefabParticleExplosion, hit.point, Quaternion.identity);
                    //    expl.GetComponent<ParticleSystem>() = 4;
                        Destroy(expl, 2);
                        TankControllerOffline CC = hit.collider.GetComponent<TankControllerOffline>();
                        _gameController.HeathSlider.SetActive(true);
                         CC.Health -= CurrentGun.Guns.DamageCar;
                        _gameController.HeathSlider.transform.GetChild(0).GetComponent<Image>().fillAmount = CC.Health/60f;
                        
                        if (CC.Health <= 0)
                        {
                            //Handheld.Vibrate();
                            CC.CanFire = false;
                            CC.GetComponent<NavMeshAgent>().speed = 0;
                            CC.GetComponent<Animator>().SetTrigger("OnDestroy");
                            Destroy(CC.transform.gameObject,3);
                            _gameController.HeathSlider.SetActive(false);
                            hit.collider.enabled = false;
                        }
                    }
                }
                else
                {
                  OnRebound(hit);
                }
                break;
            case "Terrain":
                expl = Instantiate(PrefabParticleTerrain, hit.point, Quaternion.identity);
                Destroy(expl, 2);
                break;
        }
    }

    private void OnRebound(RaycastHit hit)
    {
        GameObject bullet = Instantiate(BulletPrefab, hit.transform.position,
                Quaternion.identity);
        bullet.transform.localEulerAngles = new Vector3(Random.Range(-20, -120), bullet.transform.localEulerAngles.y);
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 400;
        Destroy(bullet, 2);
    }

    private bool RandomeRebound(RaycastHit hit)
    {
        var rebound = Random.Range(-50, 20);
        if (rebound > 0)
        {
            OnRebound(hit);
            return true;
        }
        return false;
    }
  
    public void ViewInfoAboutGun(GunsType guns)
    {
        InfoGun.text = "Name: " + guns.TypeGunn + Environment.NewLine +
                       "MagazineSize: " + guns.MagazinSize + Environment.NewLine +
                       "ReloadSize: " + guns.ReloadSize + Environment.NewLine +
                       "ReloadTime: " + guns.ReloadTime + Environment.NewLine +
                       "DamageBoat: " + guns.DamageBoat + Environment.NewLine +
                       "DamageCar: " + guns.DamageCar;
    }
}
