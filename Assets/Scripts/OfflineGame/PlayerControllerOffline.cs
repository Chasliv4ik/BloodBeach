
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class PlayerControllerOffline : MonoBehaviour
{
    #region public variables

    public Gun CurrentGun;
    public List<Gun> Guns;
    [SerializeField] public float MyAngle, AngleHRange;
    public Vector2 RangeLeftRight;
    public float Sensitivity = 1f;
    public Vector3 TmpMousePos;
    public float Health = 10000;
    public Slider HealthSlider;
    public float distance = 100;
    public float rotationSpeed = 2;
    public Text InfoGun;
    public Transform TargetTransform;
    public GameObject ParticleTerrain, ParticleExplosion, ParticleExplosionCliff, ParticleFalling, BulletPrefab;
    public Slider SliderSpeed, SliderSmooth;

    #endregion

    #region private variables

    private Vector3 dir;
    private Vector3 targetPoint;
    private bool isClick = false;
    private float tmpAngle;


    [SerializeField]
    //  private Dictionary<GunsType.TypeGun, int> MagazinData = new Dictionary<GunsType.TypeGun, int>();

    #endregion

    private void Start()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        CurrentGun.LookAt.enabled = false;
        rotationSpeed = 20;
#endif


        foreach (var gun in Guns)
        {
            gun.Guns = new GunsType(LoadDataManager.Data.Guns.FirstOrDefault(x => x.TypeGunn == gun.GunType.ToString()));
        }


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
                TargetTransform.position =
                    new Vector3(Mathf.Clamp(TargetTransform.position.x + (del.x*rotationSpeed), -90, 480),
                        Mathf.Clamp(TargetTransform.position.y + (del.y*rotationSpeed), -80, 160), GetZ());
            }
            else if (Input.GetTouch(1).phase == TouchPhase.Moved)
            {
                var del = Input.GetTouch(1).deltaPosition;
                TargetTransform.position =
                    new Vector3(Mathf.Clamp(TargetTransform.position.x + (del.x*rotationSpeed), -90, 480),
                        Mathf.Clamp(TargetTransform.position.y + (del.y*rotationSpeed), -80, 160), GetZ());
            }
            //var tmpy = GetRangeHorizontal(CurrentGun.CameraRotatorGroup.transform.localEulerAngles.y);
            //var tmpx = GetRangeVertical(CurrentGun.CameraRotatorGroup.transform.localEulerAngles.x);
            //CurrentGun.CameraRotatorGroup.transform.localEulerAngles = new Vector3(tmpx, tmpy, 0);
        }
#endif
    }


    public float GetZ()
    {
        var tmp = Mathf.Sqrt((300*300) -
                             (TargetTransform.position.x - transform.position.x)*
                             (TargetTransform.position.x - transform.position.x) -
                             ((TargetTransform.position.y - transform.position.y)*
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
        dir = new Vector3(temp.x, temp.y, temp.z) - CurrentGun.CameraRotatorGroup.transform.position;
        return Vector3.Angle(dir, transform.forward);
    }

    private void LookAtThis(Vector3 target)
    {
        if (CalculateAngle(target) > 0.1f)
        {
            CurrentGun.CameraRotatorGroup.transform.rotation =
                Quaternion.RotateTowards(CurrentGun.CameraRotatorGroup.transform.rotation,
                    Quaternion.LookRotation(dir), rotationSpeed*Time.deltaTime);
            var tmpy = GetRangeHorizontal(CurrentGun.CameraRotatorGroup.transform.localEulerAngles.y);
            var tmpx = GetRangeVertical(CurrentGun.CameraRotatorGroup.transform.localEulerAngles.x);
            CurrentGun.CameraRotatorGroup.transform.localEulerAngles = new Vector3(tmpx, tmpy, 0);
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
            targetPoint = ray.origin + (ray.direction*distance);
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
            if (CurrentGun.Guns.isFire)
            {
                if (CurrentGun.Guns.IsShooting())
                {
                    CurrentGun.Guns.isFire = false;
                    CurrentGun.GetComponentInChildren<Animator>().SetTrigger("Fire");
                    GameObject bullet;
                    RaycastHit hit;
                    switch (CurrentGun.GunType)
                    {
                        case GunsType.TypeGun.GunDShK:
                            bullet = Instantiate(CurrentGun.BulletPrefab, CurrentGun.BulletSpawnTransform.position,
                        CurrentGun.BulletSpawnTransform.rotation);
                            bullet.transform.LookAt(Camera.main.transform.position + Camera.main.transform.forward*1000f);
                            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward*400;
                            Destroy(bullet, 6);
                          
                            if (Physics.Raycast(Camera.main.transform.position,
                                Camera.main.transform.forward*CurrentGun.BulletSpeed, out hit))
                            {
                                OnFallingIntoGoal(hit);
                            }
                            break;
                        case GunsType.TypeGun.GunZu:
                            bullet = Instantiate(CurrentGun.BulletPrefab, CurrentGun.GetBulletSpawn().position,
                        CurrentGun.BulletSpawnTransform.rotation);
                            bullet.transform.LookAt(Camera.main.transform.position + Camera.main.transform.forward * 1000f);
                            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 400;
                            Destroy(bullet, 6);
                          
                            if (Physics.Raycast(Camera.main.transform.position,
                                Camera.main.transform.forward * CurrentGun.BulletSpeed, out hit))
                            {
                                OnFallingIntoGoal(hit);
                            }
                            break;

                        case GunsType.TypeGun.GunRPG:
                    
                            if (Physics.Raycast(Camera.main.transform.position,
                                Camera.main.transform.forward*CurrentGun.BulletSpeed, out hit))
                            {
                                Debug.Log(hit.collider.tag);
                                if (hit.collider.tag != Tags.TerrainTag && hit.collider.tag != "Untagged" &&
                                    hit.collider.tag != "Cliff"&& hit.collider.tag != "Player")
                                {
                                    bullet = Instantiate(CurrentGun.BulletPrefab, CurrentGun.BulletSpawnTransform.position,
                                   CurrentGun.BulletSpawnTransform.rotation);
                                    bullet.GetComponent<RocketController>().SetTarget(hit.transform, CurrentGun.Guns);
                                }
                                else
                                {
                                    CurrentGun.Guns.ReloadSize++;
                                    CurrentGun.Guns.MagazinSize++;
                                    CurrentGun.Guns.ManagerUI.ViewInfo(CurrentGun.Guns);
                                    CurrentGun.Guns.isFire = true;
                                }
                            }
                            else
                            {
                                CurrentGun.Guns.ReloadSize++;
                                CurrentGun.Guns.MagazinSize++;
                                CurrentGun.Guns.ManagerUI.ViewInfo(CurrentGun.Guns);
                                CurrentGun.Guns.isFire = true;
                            }
                            break;
                    }
                    StartCoroutine(CurrentGun.Guns.DelayAfteShoot());
                }
                else
                {
                    StartCoroutine(CurrentGun.Guns.ReloadGun());
                }
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
            yield return new WaitForSeconds(CurrentGun.Guns.DelayShoot);
        }
    }

  
   

    public void SetTypeGun(int type)
    {
        Camera.main.transform.SetParent(transform);
        Quaternion tmpCamera = CurrentGun.CameraRotatorGroup.transform.rotation;
        CurrentGun.gameObject.SetActive(false);
        CurrentGun = Guns.Find(gun => gun.GunType == ((GunsType.TypeGun) type));
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        CurrentGun.LookAt.enabled = false;
#endif
        CurrentGun.CameraRotatorGroup.transform.localRotation = tmpCamera;
        CurrentGun.gameObject.SetActive(true);
        Camera.main.transform.position = CurrentGun.MainCameraTransform.position;
        Camera.main.transform.SetParent(CurrentGun.MainCameraTransform);
      CurrentGun.Guns.ManagerUI.ViewInfo(CurrentGun.Guns);
    }

    public void OnFallingIntoGoal(RaycastHit hit)
    {
        GameObject expl;
        EnemyOffline enemyOffline = hit.transform.GetComponent<EnemyOffline>();
        if (enemyOffline != null)
            enemyOffline.TakeDamage(CurrentGun.Guns);
        switch (hit.collider.tag)
        {
            case "Terrain":
                expl = Instantiate(ParticleTerrain, hit.point, Quaternion.identity);
                Destroy(expl, 2);
                break;
            case "Cliff":
                expl = Instantiate(ParticleExplosionCliff, hit.point, Quaternion.identity);
                Destroy(expl, 2);
                break; 
            default:
                expl = Instantiate(ParticleFalling, hit.point, Quaternion.identity);
                Destroy(expl, 2);
                break;
        } 
    }

    private void OnRebound(RaycastHit hit) 
    {
        GameObject bullet = Instantiate(BulletPrefab, hit.transform.position,
            Quaternion.identity);
        bullet.transform.localEulerAngles = new Vector3(Random.Range(-20, -120), bullet.transform.localEulerAngles.y);
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward*400;
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

  
}