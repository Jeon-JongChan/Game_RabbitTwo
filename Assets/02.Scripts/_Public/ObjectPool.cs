using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 간단한 오브젝트 풀
public class ObjectPool : MonoBehaviour
{
    // 풀링할 오브젝트 배열
    [SerializeField] private GameObject[] _prefabs;
    // 오브젝트 풀 갯수
    [SerializeField] private int _countInPool;

    [SerializeField] bool _useSizeByWeight = false;

    [SerializeField] Vector2 _weightMinMax = Vector2.zero;


    // 오브젝트 풀 리스트
    //private List<GameObject> _pool = new List<GameObject> ();
    Queue<GameObject> _pool = new Queue<GameObject>();
    public GameObject Pool
    {
        get
        {   
            return _pool.Count > 0 ? _pool.Dequeue(): null;
        }
        set
        {
            if (value is GameObject)
            {
                value.transform.position = transform.position;
                _pool.Enqueue(value);
            }
        }
    }
    public int CountInPool
    {
        get { return _countInPool; }
    }

    void Start ()
    {
        // 지정한 풀 크기만큼 오브젝트를 추가함
        AddToPool(_countInPool);
    }

    void RandomSizeByWeight(GameObject tempObj, Rigidbody rb, Vector2 minMax)
    {
        float _sizeByWeightRatio = 10;
        rb.mass = Random.Range(minMax.x, minMax.y);
        Vector3 temp = Vector3.one * (rb.mass / _sizeByWeightRatio);
        tempObj.transform.localScale = temp;
    }
    public void ReturnPool(object obj)
    {
        Pool = obj as GameObject;
    }
    // 오브젝트풀에 아이템을 추가함
    private void AddToPool (int numberInPool)
    {
        Rigidbody rb = null;
        PooledObj pooled = null;
        for (int i = 0; i < numberInPool; i++)
        {
            // 풀링할 오브젝트 프리팹 인덱스를 선택함
            int randomIndex = Random.Range(0, _prefabs.Length);

            // 지정한 프리팹을 오브젝트로 생성함
            GameObject instance = Instantiate(_prefabs[randomIndex]);

            pooled = instance.GetComponent<PooledObj>();
            if (pooled == null) pooled = instance.AddComponent<PooledObj>();

            pooled.RetrunPool += ReturnPool;

            // 생성한 오브젝트를 풀링 오브젝트의 자식으로 연결함
            instance.transform.parent = transform;
            // 오브젝트를 비활성화 함
            instance.SetActive(false);

            rb = instance.GetComponent<Rigidbody>();
            if (rb == null) rb = instance.AddComponent<Rigidbody>();

            if(_useSizeByWeight) RandomSizeByWeight(instance, rb, _weightMinMax);

            // 오브젝트 풀에 생성한 오브젝트를 추가함
            //_pool.Add (instance);
            Pool = instance;
        }
    }

}