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
    [SyncVar] public int PlayerId = -1;
    private GameObject _shootHelper;

    private void Start()
    {

        _gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        _shootHelper = GameObject.Find("Shoot Helper");
        //     MagazinData.Add(CurrentGun.Guns.Type, CurrentGun.Guns.MagazinSize);
        //  Guns = FindObjectsOfType<CurrentGun>();
        //ViewInfoAboutGun(CurrentGun.Guns);
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
            SendPosition(CurrentGun.MovingPart.localRotation);
        }

        if (!isLocalPlayer) //Lerping if not local player
        {
            CurrentGun.MovingPart.localRotation = Quaternion.Lerp(CurrentGun.MovingPart.localRotation, syncPos, Time.deltaTime * _lerpRate);
            //CurrentGun.MovingPart.localRotation = Quaternion.Euler(Vector3.Lerp(CurrentGun.MovingPart.eulerAngles, syncPos, Time.deltaTime * _lerpRate));
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
                CurrentGun.LookAt.enabled = false;
                rotationSpeed = 50f; //2 for Android
                                     //#endif

        //CurrentGun.GetComponentInChildren<NetworkAnimator>().SetParameterAutoSend(0, true);
        //PlayerId = connectionToServer.connectionId;
        
    }

    public override void OnStartLocalPlayer()
    {
        //base.OnStartLocalPlayer();
        //Debug.Log("ID: " + connectionToServer.connectionId);


        foreach (var gun in Guns)
        {
          gun.Guns = new GunsType(LoadDataManager.Data.Guns.FirstOrDefault(x => x.TypeGunn == gun.GunType.ToString()));
        }

        #if UNITY_ANDROID && !UNITY_EDITOR
            CurrentGun.LookAt.enabled = true;
            transform.Find("TargetRotator").parent = null;  
        #endif

        name = "Local Player";
        SetTypeGun(0);

        EventTrigger.Entry startFire = new EventTrigger.Entry();
        startFire.eventID = EventTriggerType.PointerDown;
        startFire.callback.AddListener((eventData) => { OnFire(); });
        GameObject.Find("Button Fire").GetComponent<EventTrigger>().triggers.Add(startFire);

        EventTrigger.Entry endFire = new EventTrigger.Entry();
        endFire.eventID = EventTriggerType.PointerUp;
        endFire.callback.AddListener((eventData) => { OffFire(); });
        GameObject.Find("Button Fire").GetComponent<EventTrigger>().triggers.Add(endFire);

        GameScreen.Instance.SetupReadyButtons();
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
                Cmd_PlayFireAnimation();
                Cmd_ShootBullet(CurrentGun.BulletSpeed, connectionToServer.connectionId);

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
        if (!NetworkManagerCustom.Instance.GameIsRunning) return;

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
        CurrentGun.GetComponentInChildren<Animator>().SetTrigger("Fire");
    }

    [Command]
    private void Cmd_ShootBullet(float velocity, int playerId)
    {
        NetworkSpawnController.Instance.SpawnBullet(velocity, playerId);
    }

    [Command]
    public void Cmd_SetPlayerReadyness(bool isReady)
    {
        NetworkManagerCustom.Instance.SetPlayerReadiness(connectionToClient, isReady);
        Rpc_SendPlayerReadiness(connectionToClient.connectionId, isReady);
    }

    [ClientRpc]
    public void Rpc_SendPlayerReadiness(int connectionId, bool isReady)
    {
        GameScreen.Instance.SetPlayerReadiness(connectionId, isReady);
    }

    [ClientRpc]
    public void Rpc_StartGame()
    {
        if (!isLocalPlayer) return;
        GameScreen.Instance.StartGame();
    }

    IEnumerator ReloadGun()
    {
        yield return new WaitForSeconds(CurrentGun.Guns.ReloadTime);
        CurrentGun.Guns.SetDefaultReloadSize();
        //ViewInfoAboutGun(CurrentGun.Guns);
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
        //ViewInfoAboutGun(CurrentGun.Guns);
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
                expl = Instantiate(PrefabParticleTerrain, hit.point, Quaternion.identity);
                Destroy(expl, 2);
                CarController CC = hit.collider.GetComponent<CarController>();
                _gameController.HeathSlider.SetActive(true);
                CC.Health -= CurrentGun.Guns.DamageCar;
                _gameController.HeathSlider.transform.GetChild(0).GetComponent<Image>().fillAmount = CC.Health / 60f;

                if (CC.Health <= 0)
                {
                    //Handheld.Vibrate();
                    Destroy(CC.transform.gameObject);
                    _gameController.HeathSlider.SetActive(false);
                    hit.collider.enabled = false;
                }
                break;
            case "Terrain":
                Cmd_SpawnExplosion(hit.point.x, hit.point.y, hit.point.z);
                //expl = Instantiate(PrefabParticleTerrain, hit.point, Quaternion.identity);
                //Destroy(expl, 2);
                break;
        }
    }

    [Command]
    private void Cmd_SpawnExplosion(float x, float y, float z)
    {
        NetworkSpawnController.Instance.SpawnExplosion(new Vector3(x, y, z));
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
