using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxer : MonoBehaviour
{
    [System.Serializable]
    public struct YSpawnRange{
        public float min;

        public float max;
    }

    public GameObject Prefab;
    public int poolSize;  // how much pipes should respond
    public float shiftSpeed;
    public float spawnRate; // how often does this object spawning
    public Vector3 defaultSpawnPos;
    public bool spawnImmediate; // particle prewarm
    public Vector3 immediateSpawnPos;
    public Vector2 targetAspectRatio; // aspect ratios - works for different screen ratio
    public YSpawnRange ySpawnRange;
    
    float spawnTimer;
    float targetAspect;
    PoolObject[] poolObjects;
    
    GameManager game;

    // initialization
    private void Awake() {
        Configure();
    }

    private void Start()
    {
        // start is called after awake
        // initialize the game
        game = GameManager.Instance;
    }

    private void OnEnable() {
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    private void OnDisable() {
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    private void OnGameOverConfirmed()
    {
        foreach(var po in poolObjects) {
            po.Dispose();
        }

        if (spawnImmediate) {
            SpawnImmediate();
        }
    }
    private void Update() {
        if (game.GameOver) return;

        Shift();
        spawnTimer += Time.deltaTime;
        if (spawnTimer > spawnRate) 
        {
            Spawn();
            spawnTimer = 0;
        }
    }

    void Configure()
    {
        targetAspect = targetAspectRatio.x / targetAspectRatio.y;
        poolObjects = new PoolObject[poolSize];
        for(var i = 0; i < poolObjects.Length; i ++) {
            GameObject go = Instantiate(Prefab) as GameObject;
            Transform t = go.transform;
            t.SetParent(transform);
            t.position = Vector3.one * 1000;
            poolObjects[i] = new PoolObject(t);
        }

        if (spawnImmediate) 
        {
            SpawnImmediate();
        }
    }

    // refresh a new object
    // when the old is out of the screen
    void Spawn() 
    {
        Transform t = GetPoolObject();
        if (t == default) return;
  
        Vector3 pos = Vector3.zero;
        pos.x = defaultSpawnPos.x * Camera.main.aspect / targetAspect;
        pos.y = Random.Range(ySpawnRange.min, ySpawnRange.max);
        t.position = pos;
    }

    // refresh a new object
    // immediately one after another
    void SpawnImmediate()
    {
         Transform t = GetPoolObject();
        if (t == null) {
            return;
        }

        Vector3 pos = Vector3.zero;
        pos.x = immediateSpawnPos.x * Camera.main.aspect / targetAspect;
        pos.y = Random.Range(ySpawnRange.min, ySpawnRange.max);
        t.position = pos;
        Spawn();
    }

    void Shift()
    {
        foreach(var poolObject in poolObjects)
        {
            poolObject.Transform.localPosition += -Vector3.right * shiftSpeed * Time.deltaTime;
            CheckDisposeObject(poolObject);
        }
    }

    void CheckDisposeObject(PoolObject poolObject)
    {
        if (poolObject.Transform.position.x < -defaultSpawnPos.x * Camera.main.aspect / targetAspect) // offscreen
        {
            poolObject.Dispose();
        }
    }
    
    Transform GetPoolObject()
    {
        foreach(var po in poolObjects)
        {
            if (!po.isUsed) {
                po.Use();
                return po.Transform;
            }
        }

        return null;
    }
}
