using DG.Tweening; //Importa a biblioteca DOTween
using System;
using System.Collections;
using UnityEngine;

public class BubbleHandler : MonoBehaviour {

    private AudioSource _audioSource; //AudioSource que tocar� os �udios
    private SphereCollider _colisorBolha; //Colisor da bolha
    private BubbleTransitionHandler _bubbleTransitionHandler;


    [Header("Refer�ncia dos objetos da cena")] //Cabe�alho para melhor organiza��o no inspector
    [Tooltip("Aqui voc� deve adicionar a refer�ncia do GameObject da maquete para essa vari�vel")]
    //Tooltip � o texto que aparece quando o mouse fica em cima da vari�vel
    [SerializeField] private GameObject _maquete; //GameObject da maquete

    [Tooltip("Aqui voc� deve adicionar a refer�ncia do GameObject da bolha para essa vari�vel")]
    [SerializeField] private GameObject _bolhaParent; //GameObject da bolha


    [Header("Tamanho da bolha")]
    [Tooltip("Dita o qu�o r�pido a bolha cresce e volta pro tamanho original (em segundos)")]
    [SerializeField] private float _tamanhoTransicao;

    [Tooltip("Ditar� qual tamanho a bolha ir� se expandir")]
    [SerializeField][Range(0, 5)] private float _tamanhoExpandida; //Tamanho da bolha ao expandir. Range diz
                                                                   //o valor m�nimo e m�ximo que pode ser
                                                                   //atribu�do

    [Tooltip("Mostrar� na cena o tamanho visualmente do tamanho")]
    [SerializeField] private bool _gizmosTamBolhaExp; //Mostrar� na cena o tamanho visualmente do tamanho

    private float _tamanhoBolhaNormal; //Tamanho da bolha ao iniciar a cena


    [Header("Audio da bolha")]
    [Tooltip("Quanto tempo demorar� para o �udio ficar no volume m�ximo ou m�nimo, vale tanto para os " +
        "audios que ir�o tocar quanto para os sons ambientes (em segundos)")]
    [SerializeField] private float _audioTransicao;

    [Tooltip("Adicionar o �udio que dever� ser tocado ao se aproximar da bolha")]
    public AudioClip[] Audio; //Array de �udios

    [Tooltip("Volume dos �udios")]
    [SerializeField][Range(0, 100)] private float _volumesAudio; //Array de volumes de cada �udio

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

        if (!other.CompareTag("MainCamera")) { //Caso o objeto n�o seja a c�mera
            return; //S� ignora
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
            if (Audio.Length != 0) { //Se o AudioSource n�o estiver tocando e tiver audio
                PlayAudio(Audio[0]);

                _colisorBolha.radius = 1; //Atribui o tamanho da bolha ao colisor
            } else {
                if (!_bubbleTransitionHandler.IsBubblePopped()) {
                    StartCoroutine(PopBubble(_audioSource.clip != null ? _audioSource.clip.length : 0.1f));
                }
            }
        }
    }

    private void PlayAudio(AudioClip audioClip) { //Fun��o para tocar um �udio
        _audioSource.clip = audioClip; //Atribui o �udio
        _audioSource.Play(); //Toca o �udio
        if (Audio.Length > 1) {
            StartCoroutine(PlayNextAudio(1)); //Come�a a corrotina para tocar o pr�ximo �udio
        } else {
            if (!_bubbleTransitionHandler.IsBubblePopped()) {
                StartCoroutine(PopBubble(_audioSource.clip != null ? _audioSource.clip.length : 0.1f));
            }
        }
    }

    private IEnumerator PlayNextAudio(int index) { //Corrotina para tocar o pr�ximo �udio
        yield return new WaitForSeconds(_audioSource.clip.length); //Espera o tempo do �udio atual acabar
        PlayAudio(Audio[index]); //Toca o pr�ximo �udio
        if (Audio.Length > index + 1) { //Se o pr�ximo �udio existir
            StartCoroutine(PlayNextAudio(index + 1)); //Come�a a corrotina para tocar o pr�ximo �udio
        } else {
            if (!_bubbleTransitionHandler.IsBubblePopped()) {
                yield return new WaitForSeconds(_audioSource.clip.length); //Espera o tempo do �udio atual acabar
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
        if (!other.CompareTag("MainCamera")) { //Caso o objeto n�o seja a c�mera
            return; //Igonora
        }

        _bolhaParent.transform.DOScale(_tamanhoBolhaNormal, _tamanhoTransicao);

        _colisorBolha.radius = 0.3f; //Atribui o tamanho normal da bolha ao colisor

        _audioSource.DOFade(0.05f, 2);
    }

    void OnDrawGizmosSelected() { //Fun��o para desenhar no editor
        if (!_gizmosTamBolhaExp) { //Se a vari�vel GizmosTamBolhaExp for falsa
            return; //S� ignora
        }

        Gizmos.color = Color.yellow; //Atribui a cor amarela ao Gizmos
        Gizmos.DrawWireSphere(transform.position, _tamanhoExpandida); //Desenha uma esfera com o
                                                                      //tamanho da bolha expandida
    }
}