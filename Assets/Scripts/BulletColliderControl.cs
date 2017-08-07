using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BulletColliderControl : MonoBehaviour
{
    public GameController GC;
    public GunsType GunsType;
    public GameObject PrefabParticleTerrain;

    void Start()
    {
        
        GunsType = FindObjectOfType<Gun>().Guns;
    }
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == Tags.TerrainTag)
        {
            var expl = Instantiate(PrefabParticleTerrain, new Vector3(transform.position.x + 5, transform.position.y + 5, transform.position.z), Quaternion.identity);
            Destroy(expl, 2);
        }
        if (collider.tag == Tags.ShipTag)
        {
           
            var expl = Instantiate(PrefabParticleTerrain, transform.position, Quaternion.identity);
            Destroy(expl, 2);
            var EC = collider.transform.parent.gameObject.GetComponent<EnemyController>();
            GC.HeathSlider.SetActive(true);      
             EC.Health -= GunsType.DamageBoat;
            GC.HeathSlider.transform.GetChild(0).GetComponent<Image>().fillAmount =
             EC.Health / 100f;
            if (EC.Health <= 0)
            {
                EC.Destroyed();
                GC.TargetTransform.FirstOrDefault(x => x.transform == EC.TargetPosition).IsEmpty = true;
                GC.InstantiateShip();
                GC.HeathSlider.SetActive(false);              
                collider.enabled = false;
            }
            Destroy(gameObject);
           
        }

        if (collider.tag == Tags.CarTag)
        {
           
            var expl = Instantiate(PrefabParticleTerrain, transform.position, Quaternion.identity);
            Destroy(expl, 2);
            var EC = collider.GetComponent<CarController>();
            GC.HeathSlider.SetActive(true);
            EC.Health -= GunsType.DamageCar;
            GC.HeathSlider.transform.GetChild(0).GetComponent<Image>().fillAmount =
             EC.Health / 60f;
            if (EC.Health <= 0)
            {
                Destroy(EC.transform.gameObject);
                GC.HeathSlider.SetActive(false);
                collider.enabled = false;
            }
            Destroy(gameObject);
        }
    }
}
