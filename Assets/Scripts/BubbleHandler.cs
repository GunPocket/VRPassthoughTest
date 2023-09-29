using DG.Tweening;
using System.Collections;
using UnityEngine;

public class BubbleHandler : MonoBehaviour {
    private AudioSource _audioSource;
    private SphereCollider _colisorBolha;
    private BubbleTransitionHandler _bubbleTransitionHandler;

    [Header("Refer�ncia dos objetos da cena")]
    [Tooltip("Aqui voc� deve adicionar a refer�ncia do GameObject da maquete para essa vari�vel")]
    [SerializeField] private GameObject _maquete;

    [Tooltip("Aqui voc� deve adicionar a refer�ncia do GameObject da bolha para essa vari�vel")]
    [SerializeField] private GameObject _bolhaParent;

    [Header("Tamanho da bolha")]
    [Tooltip("Dita o qu�o r�pido a bolha cresce e volta pro tamanho original (em segundos)")]
    [SerializeField] private float _tamanhoTransicao = 1.0f;

    [Tooltip("Ditar� qual tamanho a bolha ir� se expandir")]
    [SerializeField, Range(0, 5)] private float _tamanhoExpandida = 1.0f;

    [Tooltip("Mostrar� na cena o tamanho visualmente do tamanho")]
    [SerializeField] private bool _gizmosTamBolhaExp;

    private float _tamanhoBolhaNormal = 1.0f;

    [Header("Audio da bolha")]
    [Tooltip("Quanto tempo demorar� para o �udio ficar no volume m�ximo ou m�nimo, vale tanto para os audios que ir�o tocar quanto para os sons ambientes (em segundos)")]
    [SerializeField] private float _audioTransicao = 1.0f;

    [Tooltip("Adicionar o �udio que dever� ser tocado ao se aproximar da bolha")]
    [SerializeField] private AudioClip[] _audioClips;

    [Tooltip("Volume dos �udios")]
    [SerializeField, Range(0, 100)] private float _volumesAudio = 100.0f;

    private bool _primeiraInteracao = false;

    private void Awake() {
        // Inicializa��o de componentes e atributos
        _colisorBolha = GetComponent<SphereCollider>();
        _audioSource = GetComponent<AudioSource>();
        _bubbleTransitionHandler = GetComponent<BubbleTransitionHandler>();
        _bubbleTransitionHandler.Maquete = _maquete;

        _primeiraInteracao = false;

        _tamanhoBolhaNormal = 1.0f;
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("MainCamera")) {
            return; // Ignorar se n�o for a c�mera principal
        }

        // Ativar elementos relacionados � c�mera
        if (_maquete != null && _maquete.GetComponent<LighthouseHandler>() != null) {
            _maquete.GetComponent<LighthouseHandler>().Active = true;
        }

        // Transi��o do volume do �udio
        _audioSource.DOFade(_volumesAudio, _audioTransicao);

        // Anima��o da bolha
        _bubbleTransitionHandler.RiverAnimation();

        // Aumentar o tamanho da bolha
        float newScale = _tamanhoExpandida * 2.5f;
        _bolhaParent.transform.DOScale(newScale, _tamanhoTransicao);

        if (_primeiraInteracao) return;

        _primeiraInteracao = true;

        if (!_audioSource.isPlaying) {
            if (_audioClips.Length > 0) {
                // Tocar o primeiro �udio
                PlayAudio(_audioClips[0]);
                _colisorBolha.radius = 1;
            } else {
                if (!_bubbleTransitionHandler.IsBubblePopped()) {
                    // Iniciar a anima��o de "estourar" da bolha
                    StartCoroutine(PopBubble(_audioSource.clip != null ? _audioSource.clip.length : 0.1f));
                }
            }
        }
    }

    private void PlayAudio(AudioClip audioClip) {
        _audioSource.clip = audioClip;
        _audioSource.Play();
        if (_audioClips.Length > 1) {
            // Tocar os pr�ximos �udios em sequ�ncia
            StartCoroutine(PlayNextAudio(1));
        } else {
            if (!_bubbleTransitionHandler.IsBubblePopped()) {
                // Iniciar a anima��o de "estourar" da bolha
                StartCoroutine(PopBubble(_audioSource.clip != null ? _audioSource.clip.length : 0.1f));
            }
        }
    }

    private IEnumerator PlayNextAudio(int index) {
        yield return new WaitForSeconds(_audioSource.clip.length);
        PlayAudio(_audioClips[index]);
        if (_audioClips.Length > index + 1) {
            // Continuar tocando os pr�ximos �udios
            StartCoroutine(PlayNextAudio(index + 1));
        } else {
            if (!_bubbleTransitionHandler.IsBubblePopped()) {
                yield return new WaitForSeconds(_audioSource.clip.length);
                // Iniciar a anima��o de "estourar" da bolha
                StartCoroutine(PopBubble(_audioSource.clip != null ? _audioSource.clip.length : 0.1f));
            }
        }
    }

    private IEnumerator PopBubble(float animDelay) {
        yield return new WaitForSeconds(animDelay);
        // Reduzir o tamanho da bolha de volta ao tamanho normal
        _bolhaParent.transform.DOScale(_tamanhoBolhaNormal, _tamanhoTransicao);
        _bubbleTransitionHandler.StartPopBubble();
    }

    private void OnTriggerExit(Collider other) {
        if (!other.CompareTag("MainCamera")) {
            return; // Ignorar se n�o for a c�mera principal
        }

        // Reduzir o tamanho da bolha de volta ao tamanho normal
        _bolhaParent.transform.DOScale(_tamanhoBolhaNormal, _tamanhoTransicao);

        _colisorBolha.radius = 0.3f;

        // Transi��o do volume do �udio
        _audioSource.DOFade(0.05f, 2);
    }

    private void OnDrawGizmosSelected() {
        if (!_gizmosTamBolhaExp) {
            return; // N�o desenhar gizmos se a op��o estiver desativada
        }

        // Desenhar uma esfera de gizmo para representar o tamanho expandido da bolha
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _tamanhoExpandida);
    }
}
