using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjSpawner : MonoBehaviour {

    [SerializeField] bool _isStart = true;
    [Tooltip("true 일 경우 충돌체가 spawner에 존재할 경우 생성하지 않습니다.")]
    [SerializeField] bool _isSetColider = false;
    [Tooltip("무한 생성")]
    public int limitCount = 0;
    [SerializeField] float _generateCycle = 1f;
    [SerializeField] GameObject _obj;

    [SerializeField] bool _useObjectPool = false;

    ObjectPool _pool = null;
    Collider _col = null;

    [SerializeField]bool _spawnState = true;
    public bool SpawnState
    {
        get { return _spawnState; }
        set { _spawnState = value; }
    }

    bool _isCollision = false;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!_isSetColider && (_col = gameObject.GetComponent<Collider>()) == null)
        {
            _col = gameObject.AddComponent<CapsuleCollider>();
            _col.isTrigger = true;
        }

        if(_useObjectPool && (_pool = gameObject.GetComponent<ObjectPool>()) == null)
        {
            _pool = gameObject.AddComponent<ObjectPool>();
        }

    }
#endif

    private void OnEnable()
    {
        if(_isStart) StartSpawn();
        if (_pool == null) _pool = GetComponent<ObjectPool>();
    }

    IEnumerator SpawnEnemy()
    {
        WaitForSeconds wsSpawnDelay = new WaitForSeconds(_generateCycle);
        int limit = limitCount;
        bool limitTrigger = false;
        if (limitCount == 0) limitTrigger = true;

        if (_isSetColider)
        {
            //limit 이 0보다 클 경우 하나씩 줄여가며 카운트, 0이면 limitTrigger가 true가 되면서 무한생성
            while(_spawnState && (limitTrigger || limit-- > 0))
            {
                yield return wsSpawnDelay;
                if(!_isCollision) SpawnFunc();
            }
        }
        else
        {
            while (_spawnState && (limitTrigger || limit-- > 0))
            {
                yield return wsSpawnDelay;
                SpawnFunc();
            }
        }
    }

    void SpawnFunc()
    {
        if(_useObjectPool)
        {
            if (_pool.CountInPool > 0)
            {
                GameObject temp;
                temp = _pool.Pool;
                if(temp != null) temp.SetActive(true);              
            }
        }
        else
        {
            Instantiate(_obj, transform.position, Quaternion.identity, transform);
        }
    }

    public void StartSpawn()
    {
        //StopCoroutine("SpawnEnemy");
        StartCoroutine("SpawnEnemy");
    }
    public void StopSpawn()
    {
        StopCoroutine("SpawnEnemy");
    }

    private void OnTriggerEnter(Collider other)
    {
        _isCollision = true;
    }
    private void OnTriggerExit(Collider other)
    {
        _isCollision = false;
    }
}

