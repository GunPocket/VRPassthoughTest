using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class RiverHandler : MonoBehaviour {
    [SerializeField] private float delay;

    [SerializeField] private float minVal;
    [SerializeField] private float maxVal;


    private VisualEffect River;

    private void OnEnable() {
        River = GetComponent<VisualEffect>();
        StartCoroutine(RandomRiver());
        
    }

    private IEnumerator RandomRiver() {
        float[] newX = new float[2];
        float[] newZ = new float[2];

        for (int i = 0; i < 2; i++) {
            newX[i] = Random.Range(minVal, maxVal);
            newZ[i] = Random.Range(minVal, maxVal);
        }

        River.SetVector3("p2", new Vector3(newX[0], 0, newZ[0]));
        River.SetVector3("p3", new Vector3(newX[1], 0, newZ[1]));

        yield return new WaitForSeconds(delay);
        StartCoroutine(RandomRiver());
    }
}