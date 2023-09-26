using DG.Tweening; //Importa a biblioteca DOTween
using System;
using System.Collections;
using UnityEngine;

public class BubbleHandler : MonoBehaviour {

    private AudioSource _audioSource; //AudioSource que tocará os áudios
    private SphereCollider _colisorBolha; //Colisor da bolha
    private BubbleTransitionHandler _bubbleTransitionHandler;


    [Header("Referência dos objetos da cena")] //Cabeçalho para melhor organização no inspector
    [Tooltip("Aqui você deve adicionar a referência do GameObject da maquete para essa variável")]
    //Tooltip é o texto que aparece quando o mouse fica em cima da variável
    [SerializeField] private GameObject _maquete; //GameObject da maquete

    [Tooltip("Aqui você deve adicionar a referência do GameObject da bolha para essa variável")]
    [SerializeField] private GameObject _bolhaParent; //GameObject da bolha


    [Header("Tamanho da bolha")]
    [Tooltip("Dita o quão rápido a bolha cresce e volta pro tamanho original (em segundos)")]
    [SerializeField] private float _tamanhoTransicao;

    [Tooltip("Ditará qual tamanho a bolha irá se expandir")]
    [SerializeField][Range(0, 5)] private float _tamanhoExpandida; //Tamanho da bolha ao expandir. Range diz
                                                                   //o valor mínimo e máximo que pode ser
                                                                   //atribuído

    [Tooltip("Mostrará na cena o tamanho visualmente do tamanho")]
    [SerializeField] private bool _gizmosTamBolhaExp; //Mostrará na cena o tamanho visualmente do tamanho

    private float _tamanhoBolhaNormal; //Tamanho da bolha ao iniciar a cena


    [Header("Audio da bolha")]
    [Tooltip("Quanto tempo demorará para o áudio ficar no volume máximo ou mínimo, vale tanto para os " +
        "audios que irão tocar quanto para os sons ambientes (em segundos)")]
    [SerializeField] private float _audioTransicao;

    [Tooltip("Adicionar o áudio que deverá ser tocado ao se aproximar da bolha")]
    public AudioClip[] Audio; //Array de áudios

    [Tooltip("Volume dos áudios")]
    [SerializeField][Range(0, 100)] private float _volumesAudio; //Array de volumes de cada áudio

    private bool _primeiraInteracao = false;

    private void Awake() {
        _colisorBolha = GetComponent<SphereCollider>(); //Pega o componente SphereCollider do objeto
        _audioSource = GetComponent<AudioSource>(); //Pega o componente AudioSource do objeto
        _bubbleTransitionHandler = GetComponent<BubbleTransitionHandler>();
        _bubbleTransitionHandler.Maquete = _maquete;

        _primeiraInteracao = false;

        _tamanhoBolhaNormal = 1; //Atribui o tamanho normal da bolha
    }

    private void OnTriggerEnter(Collider other) { //Quando um objeto entrar no colisor

        if (!other.CompareTag("MainCamera")) { //Caso o objeto não seja a câmera
            return; //Só ignora
        }

        if (_maquete != null && _maquete.GetComponent<LighthouseHandler>() != null) {
            _maquete.GetComponent<LighthouseHandler>().Active = true;
        }

        _audioSource.DOFade(_volumesAudio, _audioTransicao);

        _bubbleTransitionHandler.RiverAnimation();

        float newScale = _tamanhoExpandida * 2.5f;
        _bolhaParent.transform.DOScale(newScale, _tamanhoTransicao);

        if (_primeiraInteracao) return;

        _primeiraInteracao = true;

        if (!_audioSource.isPlaying) {
            if (Audio.Length != 0) { //Se o AudioSource não estiver tocando e tiver audio
                PlayAudio(Audio[0]);

                _colisorBolha.radius = 1; //Atribui o tamanho da bolha ao colisor
            } else {
                if (!_bubbleTransitionHandler.IsBubblePopped()) {
                    StartCoroutine(PopBubble(_audioSource.clip != null ? _audioSource.clip.length : 0.1f));
                }
            }
        }
    }

    private void PlayAudio(AudioClip audioClip) { //Função para tocar um áudio
        _audioSource.clip = audioClip; //Atribui o áudio
        _audioSource.Play(); //Toca o áudio
        if (Audio.Length > 1) {
            StartCoroutine(PlayNextAudio(1)); //Começa a corrotina para tocar o próximo áudio
        } else {
            if (!_bubbleTransitionHandler.IsBubblePopped()) {
                StartCoroutine(PopBubble(_audioSource.clip != null ? _audioSource.clip.length : 0.1f));
            }
        }
    }

    private IEnumerator PlayNextAudio(int index) { //Corrotina para tocar o próximo áudio
        yield return new WaitForSeconds(_audioSource.clip.length); //Espera o tempo do áudio atual acabar
        PlayAudio(Audio[index]); //Toca o próximo áudio
        if (Audio.Length > index + 1) { //Se o próximo áudio existir
            StartCoroutine(PlayNextAudio(index + 1)); //Começa a corrotina para tocar o próximo áudio
        } else {
            if (!_bubbleTransitionHandler.IsBubblePopped()) {
                yield return new WaitForSeconds(_audioSource.clip.length); //Espera o tempo do áudio atual acabar
                StartCoroutine(PopBubble(_audioSource.clip != null ? _audioSource.clip.length : 0.1f));
            }
        }
    }

    private IEnumerator PopBubble(float animDelay) {
        yield return new WaitForSeconds(animDelay);
        _bolhaParent.transform.DOScale(_tamanhoBolhaNormal, _tamanhoTransicao);
        _bubbleTransitionHandler.StartPopBubble();
    }

    private void OnTriggerExit(Collider other) { //Quando um objeto sair do colisor
        if (!other.CompareTag("MainCamera")) { //Caso o objeto não seja a câmera
            return; //Igonora
        }

        _bolhaParent.transform.DOScale(_tamanhoBolhaNormal, _tamanhoTransicao);

        _colisorBolha.radius = 0.3f; //Atribui o tamanho normal da bolha ao colisor

        _audioSource.DOFade(0.05f, 2);
    }

    void OnDrawGizmosSelected() { //Função para desenhar no editor
        if (!_gizmosTamBolhaExp) { //Se a variável GizmosTamBolhaExp for falsa
            return; //Só ignora
        }

        Gizmos.color = Color.yellow; //Atribui a cor amarela ao Gizmos
        Gizmos.DrawWireSphere(transform.position, _tamanhoExpandida); //Desenha uma esfera com o
                                                                      //tamanho da bolha expandida
    }
}