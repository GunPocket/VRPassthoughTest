using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.VFX;

public class IntroHandler : MonoBehaviour
{
    public SphereCollider SphereCollider;
    public MeshRenderer SphereMeshRenderer;
    public VisualEffect VisualEffect;
    public ParticleSystem ParticleSystem;
    public AnimationCurve AnimationCurve;

    public GameObject[] gameObjects;

    private void Awake() {
        VisualEffect.Stop();
        foreach (var obj in gameObjects) {
            obj.SetActive(false);
        }
    }

    public void OnTriggerEnter(Collider other) {
        StartCoroutine(StartRiver());
        ParticleSystem.Play();
        SphereMeshRenderer.enabled = false;
        SphereCollider.enabled = false;
        print("Pop~");
    }

    private IEnumerator StartRiver() {
        VisualEffect.Play();
        float x = 0;
        while (x < 100) {
            yield return new WaitForSecondsRealtime(0.05f);
            x++;
            float value = AnimationCurve.Evaluate(x/100f);
            VisualEffect.playRate = value;
        }
        if (x == 100) {
            foreach (var obj in gameObjects) {
                obj.SetActive(true);
            }
        }
    }
}
