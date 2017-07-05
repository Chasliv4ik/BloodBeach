using System.Collections;
using System.Collections.Generic;
using System.Linq;
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


    public void SpawnBullet(float velocity, int playerId)
    {
        PlayerController player = GameObject.FindGameObjectsWithTag("Player")
            .ToList()
            .Find(_pl => _pl.GetComponent<PlayerController>().connectionToServer.connectionId == playerId)
            .GetComponent<PlayerController>();

        GameObject bullet = Instantiate(_bulletPrefab, player.CurrentGun.BulletSpawnTransform.position,
            player.CurrentGun.BulletSpawnTransform.rotation);

        bullet.transform.localRotation = player.CurrentGun.BulletSpawnTransform.rotation;
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * velocity;

        NetworkServer.Spawn(bullet);
    }

    public void SpawnBullet(ObjectKey objectKey, Vector3 pos, Quaternion rot)
    {
        GameObject objectToSpawn = Instantiate(ObjectPool[objectKey], pos, rot);
        NetworkServer.Spawn(objectToSpawn);
    }
}
