using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject FirstBubble;

    private void Awake() {
        FirstBubble.SetActive(false);
        StartCoroutine(StartFirstBubble());
    }

    private IEnumerator StartFirstBubble() { 
        yield return new WaitForSecondsRealtime(0.5f);
        FirstBubble.SetActive(true);
    }
}
