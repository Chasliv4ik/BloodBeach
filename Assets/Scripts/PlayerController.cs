using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityStandardAssets.Cameras;

public class PlayerController : NetworkBehaviour {

    #region public variables

    public Gun Gun;
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

    [SyncVar] private Quaternion syncPos;
    private float _lerpRate = 15;

    private void Start()
    {

        _gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        //     MagazinData.Add(Gun.Guns.Type, Gun.Guns.MagazinSize);
        //  Guns = FindObjectsOfType<Gun>();
        //ViewInfoAboutGun(Gun.Guns);
    }

    void Update()
    {
        if (!isLocalPlayer) return;

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
    
    private void FixedUpdate()
    {
        //Cmd_SendPositionToServer(transform.position);

        if (isLocalPlayer)
        {
            SendPosition(Gun.MovingPart.localRotation);
        }

        if (!isLocalPlayer) //Lerping if not local player
        {
            Gun.MovingPart.localRotation = Quaternion.Lerp(Gun.MovingPart.localRotation, syncPos, Time.deltaTime * _lerpRate);
            //Gun.MovingPart.localRotation = Quaternion.Euler(Vector3.Lerp(Gun.MovingPart.eulerAngles, syncPos, Time.deltaTime * _lerpRate));
        }
    }

    [Command]
    void Cmd_SendPositionToServer(Quaternion pos)
    {
        syncPos = pos;
    }

    [ClientCallback]
    private void SendPosition(Quaternion pos)
    {
        Cmd_SendPositionToServer(pos);
    }

    public override void OnStartClient()
    {
        //#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                Gun.LookAt.enabled = false;
                rotationSpeed = 50f; //2 for Android
        //#endif

        //Gun.GetComponentInChildren<NetworkAnimator>().SetParameterAutoSend(0, true);

    }

    public override void OnStartLocalPlayer()
    {
        //base.OnStartLocalPlayer();

        foreach (var gun in Guns)
        {
          gun.Guns = new GunsType(LoadDataManager.Data.Guns.FirstOrDefault(x => x.TypeGunn == gun.GunType.ToString()));
        }

        #if UNITY_ANDROID && !UNITY_EDITOR
            Gun.LookAt.enabled = true;
            transform.Find("TargetRotator").parent = null;  
        #endif

        name = "Local Player";
        SetTypeGun(0);
        //Gun.GetComponentInChildren<NetworkAnimator>().SetParameterAutoSend(0, true);

        EventTrigger.Entry startFire = new EventTrigger.Entry();
        startFire.eventID = EventTriggerType.PointerDown;
        startFire.callback.AddListener((eventData) => { OnFire(); });
        GameObject.Find("Button Fire").GetComponent<EventTrigger>().triggers.Add(startFire);

        EventTrigger.Entry endFire = new EventTrigger.Entry();
        endFire.eventID = EventTriggerType.PointerUp;
        endFire.callback.AddListener((eventData) => { OffFire(); });
        GameObject.Find("Button Fire").GetComponent<EventTrigger>().triggers.Add(endFire);
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

    //void FixedUpdate()
    //{
        //if (Input.touchCount > 0) // && Input.mousePosition.x <= Screen.width/2)
        //{
        //    if (Input.GetTouch(0).phase == TouchPhase.Began)
        //        TmpMousePos = Input.GetTouch(0).position;
        //    if (Input.GetTouch(0).phase == TouchPhase.Moved)
        //    {

        //        MyAngle = Sensitivity * ((Input.GetTouch(0).position.x - TmpMousePos.x) / Screen.width);
        //        Gun.TargetPlayer.transform.RotateAround(Gun.TargetPlayer.transform.position, Gun.TargetPlayer.transform.up, MyAngle);

        //        MyAngle = Sensitivity * ((Input.GetTouch(0).position.y - TmpMousePos.y) / Screen.height);
        //        Gun.TargetPlayer.transform.RotateAround(Gun.TargetPlayer.transform.position, Gun.TargetPlayer.transform.right, -MyAngle);

        //        var tmpy = GetRangeHorizontal(Gun.TargetPlayer.transform.localEulerAngles.y);
        //        var tmpx = GetRangeVertical(Gun.TargetPlayer.transform.localEulerAngles.x);
        //        Gun.TargetPlayer.transform.localEulerAngles = new Vector3(tmpx,
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
        if (Gun.Guns.MagazinSize > 0 || Gun.Guns.TypeGunn == "GunDShK")
        {
            if (isFire && Gun.Guns.IsShooting())
            {
                Cmd_PlayFireAnimation();

                
                GameObject bullet = Instantiate(Gun.BulletPrefab, Gun.BulletSpawnTransform.position,
                    Gun.BulletSpawnTransform.rotation);
                
                //NetworkSpawnController.Instance.SpawnObject(ObjectKey.Bullet, Gun.BulletSpawnTransform.position, Gun.BulletSpawnTransform.rotation);

                //NetworkSpawnController.Instance.SpawnObject(bullet);

                //bullet.transform.localRotation = Gun.BulletSpawnTransform.rotation;
                //bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * Gun.BulletSpeed;

                Cmd_SpawnObject("Player Bullet", Gun.BulletSpeed);




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

    [Command]
    private void Cmd_PlayFireAnimation()
    {
        Rpc_PlayFireAnimation();
    }

    [ClientRpc]
    private void Rpc_PlayFireAnimation()
    {
        Gun.GetComponentInChildren<Animator>().SetTrigger("Fire");
    }

    [Command]
    private void Cmd_SpawnObject(string objectName, float velocity)
    {
        NetworkSpawnController.Instance.SpawnObject(objectName, velocity);
    }



    IEnumerator ReloadGun()
    {
        yield return new WaitForSeconds(Gun.Guns.ReloadTime);
        Gun.Guns.SetDefaultReloadSize();
        //ViewInfoAboutGun(Gun.Guns);
        isFire = true;
    }

    public void SetTypeGun(int type)
    {
        //Camera.main.transform.SetParent(transform);
        Quaternion tmpCamera = Gun.Camera.transform.rotation;
        Gun.gameObject.SetActive(false); 
        Gun = Guns.Find(gun => gun.GunType == ((GunsType.TypeGun) type));
        Gun.Camera.transform.rotation = tmpCamera;
        Gun.gameObject.SetActive(true);
        Camera.main.transform.position = Gun.MainCameraTransform.position;
        Camera.main.transform.SetParent(Gun.MainCameraTransform);
        //ViewInfoAboutGun(Gun.Guns);
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
                    //Handheld.Vibrate();
                    EC.Destroyed();
                    _gameController.TargetTransform.FirstOrDefault(x => x.transform == EC.TargetPosition).IsEmpty = true;
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
                    //Handheld.Vibrate();
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
