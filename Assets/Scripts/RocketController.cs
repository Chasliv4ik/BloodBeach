
using UnityEngine;

using Debug = UnityEngine.Debug;

public class RocketController : MonoBehaviour
{
    // Use this for initialization
    private bool _isTarget = false;
    private Transform _enemyTransform;
    private float _speed;
    private GunsType _gun;
   
    private Vector3 _targetPosition;
  

    // Update is called once per frame
    void Update()
    {
        if (_isTarget)
        {
            if(_enemyTransform == null)
                Destroy(gameObject);
            else { 
            _targetPosition = new Vector3(_enemyTransform.position.x, _enemyTransform.position.y + 2,
                _enemyTransform.transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _speed*Time.deltaTime);
            _speed += Time.deltaTime*15;
            transform.LookAt(_targetPosition);
}
        }
    }

    public void SetTarget(Transform target, GunsType gun)
    {
        _gun = gun;
        _speed = 25;
        _enemyTransform = target;
        _isTarget = true;
    }

    void OnCollisionEnter(Collision enemyCollision)
    {
        Debug.Log("Collision");
        if (enemyCollision.collider.tag == _enemyTransform.tag)
        {
            var enemy = _enemyTransform.GetComponent<EnemyOffline>();
            enemy.TakeDamage(_gun);
            Destroy(gameObject);
        }
    }
}