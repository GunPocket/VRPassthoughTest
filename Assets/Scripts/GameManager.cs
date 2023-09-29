using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [Tooltip("Refer�ncia para o GameObject da primeira bolha")]
    [SerializeField] GameObject FirstBubble;

    private void Awake() {
        // Desative a primeira bolha no in�cio do jogo
        FirstBubble.SetActive(false);

        // Inicie uma corrotina para ativar a primeira bolha ap�s um atraso
        StartCoroutine(StartFirstBubble());
    }

    private IEnumerator StartFirstBubble() {
        // Aguarde um atraso de 0.5 segundos (tempo real)
        yield return new WaitForSecondsRealtime(0.5f);

        // Ative a primeira bolha ap�s o atraso
        FirstBubble.SetActive(true);
    }
}
