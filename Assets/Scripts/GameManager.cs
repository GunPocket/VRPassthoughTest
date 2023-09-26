using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject FirstBubble;

    private void Start() {
        FirstBubble.SetActive(false);
        StartCoroutine(StartFirstBubble());
    }

    private IEnumerator StartFirstBubble() { 
        yield return new WaitForSecondsRealtime(1);
        FirstBubble.SetActive(false);
    }
}
