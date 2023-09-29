using DG.Tweening;
using System.Collections;
using UnityEngine;

public class BubbleHandler : MonoBehaviour {
    private AudioSource _audioSource;
    private SphereCollider _colisorBolha;
    private BubbleTransitionHandler _bubbleTransitionHandler;

    [Header("Referência dos objetos da cena")]
    [Tooltip("Aqui você deve adicionar a referência do GameObject da maquete para essa variável")]
    [SerializeField] private GameObject _maquete;

    [Tooltip("Aqui você deve adicionar a referência do GameObject da bolha para essa variável")]
    [SerializeField] private GameObject _bolhaParent;

    [Header("Tamanho da bolha")]
    [Tooltip("Dita o quão rápido a bolha cresce e volta pro tamanho original (em segundos)")]
    [SerializeField] private float _tamanhoTransicao = 1.0f;

    [Tooltip("Ditará qual tamanho a bolha irá se expandir")]
    [SerializeField, Range(0, 5)] private float _tamanhoExpandida = 1.0f;

    [Tooltip("Mostrará na cena o tamanho visualmente do tamanho")]
    [SerializeField] private bool _gizmosTamBolhaExp;

    private float _tamanhoBolhaNormal = 1.0f;

    [Header("Audio da bolha")]
    [Tooltip("Quanto tempo demorará para o áudio ficar no volume máximo ou mínimo, vale tanto para os audios que irão tocar quanto para os sons ambientes (em segundos)")]
    [SerializeField] private float _audioTransicao = 1.0f;

    [Tooltip("Adicionar o áudio que deverá ser tocado ao se aproximar da bolha")]
    [SerializeField] private AudioClip[] _audioClips;

    [Tooltip("Volume dos áudios")]
    [SerializeField, Range(0, 100)] private float _volumesAudio = 100.0f;

    private bool _primeiraInteracao = false;

    private void Awake() {
        // Inicialização de componentes e atributos
        _colisorBolha = GetComponent<SphereCollider>();
        _audioSource = GetComponent<AudioSource>();
        _bubbleTransitionHandler = GetComponent<BubbleTransitionHandler>();
        _bubbleTransitionHandler.Maquete = _maquete;

        _primeiraInteracao = false;

        _tamanhoBolhaNormal = 1.0f;
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("MainCamera")) {
            return; // Ignorar se não for a câmera principal
        }

        // Ativar elementos relacionados à câmera
        if (_maquete != null && _maquete.GetComponent<LighthouseHandler>() != null) {
            _maquete.GetComponent<LighthouseHandler>().Active = true;
        }

        // Transição do volume do áudio
        _audioSource.DOFade(_volumesAudio, _audioTransicao);

        // Animação da bolha
        _bubbleTransitionHandler.RiverAnimation();

        // Aumentar o tamanho da bolha
        float newScale = _tamanhoExpandida * 2.5f;
        _bolhaParent.transform.DOScale(newScale, _tamanhoTransicao);

        if (_primeiraInteracao) return;

        _primeiraInteracao = true;

        if (!_audioSource.isPlaying) {
            if (_audioClips.Length > 0) {
                // Tocar o primeiro áudio
                PlayAudio(_audioClips[0]);
                _colisorBolha.radius = 1;
            } else {
                if (!_bubbleTransitionHandler.IsBubblePopped()) {
                    // Iniciar a animação de "estourar" da bolha
                    StartCoroutine(PopBubble(_audioSource.clip != null ? _audioSource.clip.length : 0.1f));
                }
            }
        }
    }

    private void PlayAudio(AudioClip audioClip) {
        _audioSource.clip = audioClip;
        _audioSource.Play();
        if (_audioClips.Length > 1) {
            // Tocar os próximos áudios em sequência
            StartCoroutine(PlayNextAudio(1));
        } else {
            if (!_bubbleTransitionHandler.IsBubblePopped()) {
                // Iniciar a animação de "estourar" da bolha
                StartCoroutine(PopBubble(_audioSource.clip != null ? _audioSource.clip.length : 0.1f));
            }
        }
    }

    private IEnumerator PlayNextAudio(int index) {
        yield return new WaitForSeconds(_audioSource.clip.length);
        PlayAudio(_audioClips[index]);
        if (_audioClips.Length > index + 1) {
            // Continuar tocando os próximos áudios
            StartCoroutine(PlayNextAudio(index + 1));
        } else {
            if (!_bubbleTransitionHandler.IsBubblePopped()) {
                yield return new WaitForSeconds(_audioSource.clip.length);
                // Iniciar a animação de "estourar" da bolha
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
            return; // Ignorar se não for a câmera principal
        }

        // Reduzir o tamanho da bolha de volta ao tamanho normal
        _bolhaParent.transform.DOScale(_tamanhoBolhaNormal, _tamanhoTransicao);

        _colisorBolha.radius = 0.3f;

        // Transição do volume do áudio
        _audioSource.DOFade(0.05f, 2);
    }

    private void OnDrawGizmosSelected() {
        if (!_gizmosTamBolhaExp) {
            return; // Não desenhar gizmos se a opção estiver desativada
        }

        // Desenhar uma esfera de gizmo para representar o tamanho expandido da bolha
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _tamanhoExpandida);
    }
}
