using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    #region public variables

    public Gun Gun;
    public Gun[] Guns;
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
    public GameObject PrefabParticleTerrain;
    public Slider SliderSpeed, SliderSmooth;
    #endregion

    #region private variables

    private Vector3 dir;
    private Vector3 targetPoint;
    private bool isClick = false;
    private bool isFire = true;
    private float tmpAngle;
    private GameController _gameController;
    [SerializeField]
  //  private Dictionary<GunsType.TypeGun, int> MagazinData = new Dictionary<GunsType.TypeGun, int>();

    #endregion

    private void Start()
    {

     
        _gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
   //     MagazinData.Add(Gun.Guns.Type, Gun.Guns.MagazinSize);
      //  Guns = FindObjectsOfType<Gun>();
        ViewInfoAboutGun(Gun.Guns);
#if UNITY_EDITOR
        Gun.LookAt.enabled = false;
        rotationSpeed = 50f;
#endif
    }

    void Update()
    {

#if UNITY_EDITOR
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
        dir = new Vector3(temp.x, temp.y, temp.z) - Gun.Camera.transform.position;
        return Vector3.Angle(dir, transform.forward);
    }

    private void LookAtThis(Vector3 target)
    {
        if (CalculateAngle(target) > 0.1f)
        {
            Gun.Camera.transform.rotation = Quaternion.RotateTowards(Gun.Camera.transform.rotation,
                Quaternion.LookRotation(dir), rotationSpeed * Time.deltaTime);
            var tmpy = GetRangeHorizontal(Gun.Camera.transform.localEulerAngles.y);
            var tmpx = GetRangeVertical(Gun.Camera.transform.localEulerAngles.x);
            Gun.Camera.transform.localEulerAngles = new Vector3(tmpx, tmpy, 0);
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
        Gun.LookAt.m_FollowSpeed = SliderSmooth.value;
    }

    public void ChangeSliderSpeed()
    {
        rotationSpeed = SliderSpeed.value;
    }

    void FixedUpdate()
    {
        //if (Input.touchCount > 0) // && Input.mousePosition.x <= Screen.width/2)
        //{
        //    if (Input.GetTouch(0).phase == TouchPhase.Began)
        //        TmpMousePos = Input.GetTouch(0).position;
        //    if (Input.GetTouch(0).phase == TouchPhase.Moved)
        //    {

        //        MyAngle = Sensitivity * ((Input.GetTouch(0).position.x - TmpMousePos.x) / Screen.width);
        //        Gun.Camera.transform.RotateAround(Gun.Camera.transform.position, Gun.Camera.transform.up, MyAngle);

        //        MyAngle = Sensitivity * ((Input.GetTouch(0).position.y - TmpMousePos.y) / Screen.height);
        //        Gun.Camera.transform.RotateAround(Gun.Camera.transform.position, Gun.Camera.transform.right, -MyAngle);

        //        var tmpy = GetRangeHorizontal(Gun.Camera.transform.localEulerAngles.y);
        //        var tmpx = GetRangeVertical(Gun.Camera.transform.localEulerAngles.x);
        //        Gun.Camera.transform.localEulerAngles = new Vector3(tmpx,
        //        tmpy, 0);
                
        //    }
        //}
    }

   

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
        if (Gun.Guns.MagazinSize > 0 || Gun.Guns.TypeGunn == GunsType.TypeGun.GunDShK)
        {
            if (isFire && Gun.Guns.IsShooting())
            {
                ViewInfoAboutGun(Gun.Guns);
                Gun.GetComponent<Animation>().Play();
                GameObject bullet = Instantiate(Gun.BulletPrefab, Gun.BulletSpawnTransform.position,
                    Gun.BulletSpawnTransform.rotation);
                bullet.transform.localRotation = Gun.BulletSpawnTransform.rotation;
                bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * Gun.BulletSpeed;

                RaycastHit hit;
                if (Physics.Raycast(bullet.transform.position, bullet.GetComponent<Rigidbody>().velocity, out hit))
                {
                    OnFallingIntoGoal(hit);
                }
                Destroy(bullet, 1f);
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
        yield return new WaitForSeconds(Gun.Guns.ReloadTime);
        Gun.Guns.SetDefaultReloadSize();
        ViewInfoAboutGun(Gun.Guns);
        isFire = true;
    }

    public void SetTypeGun(int type)
    {
        Camera.main.transform.SetParent(transform);
        Quaternion tmpCamera = Gun.Camera.transform.rotation;
        Gun.gameObject.SetActive(false);      
        Gun = Guns.FirstOrDefault(x=>x.Guns.TypeGunn == (GunsType.TypeGun) type);
        Gun.Camera.transform.rotation = tmpCamera;
        Gun.gameObject.SetActive(true);
        Camera.main.transform.position = Gun.MainCameraTransform.position;
        Camera.main.transform.SetParent(Gun.MainCameraTransform);
        ViewInfoAboutGun(Gun.Guns);
    }

    public void OnFallingIntoGoal(RaycastHit hit)
    {
        GameObject expl;

        switch (hit.collider.tag)
        {
            case "Enemy":
                expl = Instantiate(PrefabParticleTerrain, hit.point, Quaternion.identity);
                Destroy(expl, 2);
                EnemyController EC = hit.collider.transform.parent.gameObject.GetComponent<EnemyController>();
                _gameController.HeathSlider.SetActive(true);
                EC.Health -= Gun.Guns.DamageBoat;
                _gameController.HeathSlider.transform.GetChild(0).GetComponent<Image>().fillAmount =
                    EC.Health / 100f;
                if (EC.Health <= 0)
                {
                    Handheld.Vibrate();
                    EC.Destroyed();
                    _gameController.TargetTransform.FirstOrDefault(x => x.transform == EC.Target).IsEmpty = true;
                    _gameController.InstantiateShip();
                    _gameController.HeathSlider.SetActive(false);
                    hit.collider.enabled = false;
                }
                break;
            case "Car":
                
                expl = Instantiate(PrefabParticleTerrain, hit.point, Quaternion.identity);
                Destroy(expl, 2);
                CarController CC = hit.collider.GetComponent<CarController>();
                _gameController.HeathSlider.SetActive(true);
                CC.Health -= Gun.Guns.DamageCar;
                _gameController.HeathSlider.transform.GetChild(0).GetComponent<Image>().fillAmount =
                    CC.Health / 60f;
                if (CC.Health <= 0)
                {
                    Handheld.Vibrate();
                    Destroy(CC.transform.gameObject);
                    _gameController.HeathSlider.SetActive(false);
                    hit.collider.enabled = false;
                }
                break;
            case "Terrain":
                expl = Instantiate(PrefabParticleTerrain, hit.point, Quaternion.identity);
                Destroy(expl, 2);
                break;
        }
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
