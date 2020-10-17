using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject {
    public Transform Transform;

    public bool isUsed;

    public PoolObject(Transform t) {
        Transform = t;
    }

    public void Use(){
        isUsed = true;
    }

    public void Dispose() {
        isUsed = false;
        Transform.position = Vector3.one * 1000; 
    }
}