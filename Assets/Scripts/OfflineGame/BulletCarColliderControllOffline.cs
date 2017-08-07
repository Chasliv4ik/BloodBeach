using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletCarColliderControllOffline : MonoBehaviour
{
    private float _damage;

    public void SetDamage(float damage)
    {
        _damage = damage;
    }
   
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == Tags.PlayerTag)
        {
            var player = FindObjectOfType<PlayerControllerOffline>();
            player.Health -= _damage;
           // player.HealthSlider.value = player.Health;
        }
    }
}
