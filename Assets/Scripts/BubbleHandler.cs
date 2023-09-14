using DG.Tweening; //Importa a biblioteca DOTween
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

    [Tooltip("Volume de cada �udio")]
    [Range(0, 100)] public float[] VolumesAudio; //Array de volumes de cada �udio

    private void Awake() {
        _colisorBolha = GetComponent<SphereCollider>(); //Pega o componente SphereCollider do objeto
        _audioSource = GetComponent<AudioSource>(); //Pega o componente AudioSource do objeto
        _bubbleTransitionHandler = GetComponent<BubbleTransitionHandler>();

        if (_maquete == null) { //Se a vari�vel Maquete n�o estiver atribu�da
            Debug.LogWarning("O objeto Maquete n�o foi atribu�do.", gameObject);
        }

        if (_bolhaParent == null) { //Se a vari�vel Bolha n�o estiver atribu�da
            Debug.LogWarning("O objeto Bolha n�o foi atribu�do.", gameObject);
        }

        if (Audio.Length == 0 || Audio == null) { //Se o array Audio estiver vazio
            Debug.LogWarning("O array Audio est� vazio.", gameObject);
        }

        _tamanhoBolhaNormal = _colisorBolha.radius; //Atribui o tamanho normal da bolha
    }

    private void OnTriggerEnter(Collider other) { //Quando um objeto entrar no colisor
        if (!other.CompareTag("MainCamera")) { //Caso o objeto n�o seja a c�mera
            return; //S� ignora
        }

        //StopAllCoroutines(); //Para todas as corrotinas. 
        //Corrotinas s�o fun��es que podem ser pausadas e continuadas depois de um tempo determinado ou
        //at� que uma condi��o seja atingida fora do void Update() ou void FixedUpdate() que s�o fun��es
        //que s�o chamadas a cada frame

        if (!_audioSource.isPlaying) {
            if (Audio.Length != 0) { //Se o AudioSource n�o estiver tocando e tiver audio
                PlayAudio(Audio[0], VolumesAudio[0]);

                float newScale = _tamanhoExpandida * 2.5f;
                _bolhaParent.transform.DOScale(newScale, _tamanhoTransicao);

                _colisorBolha.radius = _tamanhoExpandida; //Atribui o tamanho da bolha ao colisor
            } else {
                if (!_bubbleTransitionHandler.IsBubblePopped()) {
                    StartCoroutine(PopBubble(_audioSource.clip != null ? _audioSource.clip.length : 0.1f));
                }
            }
        }
    }

    private void PlayAudio(AudioClip audioClip, float volume) { //Fun��o para tocar um �udio
        _audioSource.clip = audioClip; //Atribui o �udio
        _audioSource.DOFade(volume * 0.01f, _audioTransicao); //Aumenta o volume do audio de 0 at� o volume
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
        PlayAudio(Audio[index], VolumesAudio[index]); //Toca o pr�ximo �udio
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

    /*private void OnTriggerExit(Collider other) { //Quando um objeto sair do colisor
        if (!other.CompareTag("MainCamera")) { //Caso o objeto n�o seja a c�mera
            return; //Igonora
        }

        StopAllCoroutines(); //Para todas as corrotinas

        bolhaParent.transform.DOScale(tamanhoBolhaNormal, tamanhoTransicao);

        colisorBolha.radius = tamanhoBolhaNormal; //Atribui o tamanho normal da bolha ao colisor

        StartCoroutine(PauseAudio());
    }*/

    private IEnumerator PauseAudio() {
        _audioSource.DOFade(0, 1);
        yield return new WaitForSeconds(1);
        _audioSource.Stop(); //Para o �udio
        _audioSource.clip = null; //Atribui null ao �udio
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