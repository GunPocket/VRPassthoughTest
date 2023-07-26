using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothHandler : MonoBehaviour
{
    public Cloth cloth;

    private void LateUpdate() {
        cloth.capsuleColliders = FindObjectsOfType<CapsuleCollider>();        
    }
}
