using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum ObjectKey
{
    Bullet,
    Boat,
    Car
}

public class NetworkSpawnController : NetworkBehaviour
{
    public static NetworkSpawnController Instance;

    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private GameObject _boatPrefab;
    [SerializeField] private GameObject _carPrefab;

    private Dictionary<ObjectKey, GameObject> ObjectPool;

	void Start ()
	{
	    Instance = this;

        ObjectPool = new Dictionary<ObjectKey, GameObject>();

        ObjectPool.Add(ObjectKey.Bullet, _bulletPrefab);
        ObjectPool.Add(ObjectKey.Boat, _boatPrefab);
        ObjectPool.Add(ObjectKey.Car, _carPrefab);
	}


    public void SpawnObject(string objectName, float velocity)
    {
        switch (objectName)
        {
            case "Player Bullet":

                PlayerController localPlayer = GameObject.Find("Local Player").GetComponent<PlayerController>();

                GameObject bullet = Instantiate(_bulletPrefab, localPlayer.Gun.BulletSpawnTransform.position,
                    localPlayer.Gun.BulletSpawnTransform.rotation);

                bullet.transform.localRotation = localPlayer.Gun.BulletSpawnTransform.rotation;
                bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * velocity;

                NetworkServer.Spawn(bullet);
                break;
        }


    }

    public void SpawnObject(ObjectKey objectKey, Vector3 pos, Quaternion rot)
    {
        GameObject objectToSpawn = Instantiate(ObjectPool[objectKey], pos, rot);
        NetworkServer.Spawn(objectToSpawn);
    }
}
