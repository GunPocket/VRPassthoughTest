using DG.Tweening; //Importa a biblioteca DOTween
using System.Collections;
using UnityEngine;

public class BubbleHandler : MonoBehaviour {

    //ESSE SCRIPT AINDA EST� EM TESTE, N�O � O SCRIPT DA APLICA��O, N�O O UTILIZAR AL�M DESSE PROT�TIPO!!!

    private AudioSource audioSource; //AudioSource que tocar� os �udios
    private SphereCollider colisorBolha; //Colisor da bolha


    [Header("Refer�ncia dos objetos da cena")] //Cabe�alho para melhor organiza��o no inspector

    [Tooltip("Aqui voc� deve adicionar a refer�ncia do GameObject da maquete para essa vari�vel")]
    //Tooltip � o texto que aparece quando o mouse fica em cima da vari�vel
    [SerializeField] GameObject maquete; //GameObject da maquete

    [Tooltip("Aqui voc� deve adicionar a refer�ncia do GameObject da bolha para essa vari�vel")]
    [SerializeField] GameObject bolha; //GameObject da bolha


    [Header("Tamanho da bolha")]

    [Tooltip("Dita o qu�o r�pido a bolha cresce e volta pro tamanho original (em segundos)")]
    [SerializeField] private float tamanhoTransicao;

    [Tooltip("Ditar� qual tamanho a bolha ir� se expandir")]
    [SerializeField][Range(0, 5)] float tamanhoDaBolhaExpandida; //Tamanho da bolha ao expandir. Range diz
                                                                 //o valor m�nimo e m�ximo que pode ser
                                                                 //atribu�do

    [Tooltip("Mostrar� na cena o tamanho visualmente do tamanho")]
    [SerializeField] bool GizmosTamBolhaExp; //Mostrar� na cena o tamanho visualmente do tamanho

    private float tamanhoBolhaNormal; //Tamanho da bolha ao iniciar a cena


    [Header("Audio da bolha")]
    [Tooltip("Quanto tempo demorar� para o �udio ficar no volume m�ximo ou m�nimo, vale tanto para os " +
        "audios que ir�o tocar quanto para os sons ambientes (em segundos)")]
    [SerializeField] float audioTransicao;
    
    [Tooltip("Adicionar o �udio que dever� ser tocado ao se aproximar da bolha")]
    public AudioClip[] Audio; //Array de �udios
    
    [Tooltip("Volume de cada �udio")]
    [Range(0, 100)] public float[] VolumesAudio; //Array de volumes de cada �udio
    
    [Tooltip("Se cada �udio dever� tocar em loop")]
    public bool[] LoopAudio; //Array de se cada �udio dever� tocar em loop


    [Header("Sons ambiente")]
    
    [Tooltip("Adicionar os sons ambientes que tocar�o ao se aproximar da bolha")]
    public AudioClip[] SonsAmbiente; //Array de sons ambiente
    
    [Tooltip("Volume de cada som ambiente")]
    [Range(0, 100)] public float[] VolumesAmbiente; //Array de volumes de cada som ambiente
    
    [Tooltip("Se cada som ambiente estar� em loop")]
    public bool[] LoopAmbiente; //Array de se cada som ambiente estar� em loop

    private void Awake() {
        Debug.LogWarning($"Essa aplica��o est� usando um script de teste no objeto {gameObject.name}." +
            $" N�o o utilizar al�m desse prot�tipo.");

        if (maquete == null) { //Se a vari�vel Maquete n�o estiver atribu�da
            Debug.LogWarning("O objeto CenaDentroBolha n�o foi atribu�do.");
        }

        if (bolha == null) { //Se a vari�vel Bolha n�o estiver atribu�da
            Debug.LogWarning("O objeto Bolha n�o foi atribu�do.");
        }

        if (Audio.Length == 0 || Audio == null) { //Se o array Audio estiver vazio
            Debug.LogWarning("O array Audio est� vazio.");
        }

        if (SonsAmbiente.Length == 0 || SonsAmbiente == null) { //Se o array SonsAmbiente estiver vazio
            Debug.LogWarning("O array SonsAmbiente est� vazio.");
        }

        colisorBolha = GetComponent<SphereCollider>(); //Pega o componente SphereCollider do objeto
        audioSource = GetComponent<AudioSource>(); //Pega o componente AudioSource do objeto

        tamanhoBolhaNormal = colisorBolha.radius; //Atribui o tamanho normal da bolha

        int VolumesAmbienteIndex = 0; //Index para o array de volumes de cada som ambiente
        foreach (AudioClip clip in SonsAmbiente) { //Para cada som ambiente no array SonsAmbiente
            var CenaBolhaAudioSource = maquete.AddComponent<AudioSource>(); //Adiciona um AudioSource
                                                                            //no objeto Maquete
            CenaBolhaAudioSource.clip = clip; //Atribui o �udio
            CenaBolhaAudioSource.DOFade(VolumesAmbiente[VolumesAmbienteIndex] * 0.01f, audioTransicao); 
            //Atribui o volume multiplicado por 0.01f para ficar entre 0 e 1 pois atributo volume do
            //AudioSource vai de 0 a 1
            CenaBolhaAudioSource.loop = LoopAmbiente[VolumesAmbienteIndex]; //Atribui se o �udio estar�
                                                                            //em loop
            VolumesAmbienteIndex++; //Adiciona 1 ao index
        }
    }

    private void OnTriggerEnter(Collider other) { //Quando um objeto entrar no colisor
        if (!other.CompareTag("MainCamera")) { //Caso o objeto n�o seja a c�mera
            return; //S� ignora
        }       

        StopAllCoroutines(); //Para todas as corrotinas. 
        //Corrotinas s�o fun��es que podem ser pausadas e continuadas depois de um tempo determinado ou
        //at� que uma condi��o seja atingida fora do void Update() ou void FixedUpdate() que s�o fun��es
        //que s�o chamadas a cada frame

        float newScale = tamanhoDaBolhaExpandida * 2.5f;
        bolha.transform.DOScale(newScale, tamanhoTransicao);

        colisorBolha.radius = tamanhoDaBolhaExpandida; //Atribui o tamanho da bolha ao colisor

        if (!audioSource.isPlaying) { //Se o AudioSource n�o estiver tocando
            PlayAudio(Audio[0], VolumesAudio[0], LoopAudio[0]);
            StartCoroutine(PlayNextAudio(1)); //Come�a a corrotina para tocar o pr�ximo �udio
        }
    }

    private void PlayAudio(AudioClip audioClip, float volume, bool loop) { //Fun��o para tocar um �udio
        audioSource.clip = audioClip; //Atribui o �udio
        audioSource.DOFade(volume * 0.01f, audioTransicao); //Aumenta o volume do audio de 0 at� o volume
        audioSource.loop = loop; //Atribui se o �udio estar� em loop
        audioSource.Play(); //Toca o �udio
    }

    private IEnumerator PlayNextAudio(int index) { //Corrotina para tocar o pr�ximo �udio
        yield return new WaitForSeconds(audioSource.clip.length); //Espera o tempo do �udio atual acabar
        PlayAudio(Audio[index], VolumesAudio[index], LoopAudio[index]); //Toca o pr�ximo �udio
        if (Audio.Length > index + 1) { //Se o pr�ximo �udio existir
            StartCoroutine(PlayNextAudio(index + 1)); //Come�a a corrotina para tocar o pr�ximo �udio
        }
    }

    private void OnTriggerExit(Collider other) { //Quando um objeto sair do colisor
        if (!other.CompareTag("MainCamera")) { //Caso o objeto n�o seja a c�mera
            return; //Igonora
        }

        StopAllCoroutines(); //Para todas as corrotinas

        bolha.transform.DOScale(tamanhoBolhaNormal, tamanhoTransicao);

        colisorBolha.radius = tamanhoBolhaNormal; //Atribui o tamanho normal da bolha ao colisor

        StartCoroutine(PauseAudio());
    }

    private IEnumerator PauseAudio() {
        audioSource.DOFade(0, 1);
        yield return new WaitForSeconds(1);
        audioSource.Stop(); //Para o �udio
        audioSource.clip = null; //Atribui null ao �udio
    }

    void OnDrawGizmosSelected() { //Fun��o para desenhar no editor
        if (!GizmosTamBolhaExp) { //Se a vari�vel GizmosTamBolhaExp for falsa
            return; //S� ignora
        }

        Gizmos.color = Color.yellow; //Atribui a cor amarela ao Gizmos
        Gizmos.DrawWireSphere(transform.position, tamanhoDaBolhaExpandida); //Desenha uma esfera com o
                                                                            //tamanho da bolha expandida
    }
}