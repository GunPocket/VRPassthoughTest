using DG.Tweening; //Importa a biblioteca DOTween
using System.Collections;
using UnityEngine;

public class BubbleHandler : MonoBehaviour {

    //ESSE SCRIPT AINDA ESTÁ EM TESTE, NÃO É O SCRIPT DA APLICAÇÃO, NÃO O UTILIZAR ALÉM DESSE PROTÓTIPO!!!

    private AudioSource audioSource; //AudioSource que tocará os áudios
    private SphereCollider colisorBolha; //Colisor da bolha


    [Header("Referência dos objetos da cena")] //Cabeçalho para melhor organização no inspector

    [Tooltip("Aqui você deve adicionar a referência do GameObject da maquete para essa variável")]
    //Tooltip é o texto que aparece quando o mouse fica em cima da variável
    [SerializeField] GameObject maquete; //GameObject da maquete

    [Tooltip("Aqui você deve adicionar a referência do GameObject da bolha para essa variável")]
    [SerializeField] GameObject bolha; //GameObject da bolha


    [Header("Tamanho da bolha")]

    [Tooltip("Dita o quão rápido a bolha cresce e volta pro tamanho original (em segundos)")]
    [SerializeField] private float tamanhoTransicao;

    [Tooltip("Ditará qual tamanho a bolha irá se expandir")]
    [SerializeField][Range(0, 5)] float tamanhoDaBolhaExpandida; //Tamanho da bolha ao expandir. Range diz
                                                                 //o valor mínimo e máximo que pode ser
                                                                 //atribuído

    [Tooltip("Mostrará na cena o tamanho visualmente do tamanho")]
    [SerializeField] bool GizmosTamBolhaExp; //Mostrará na cena o tamanho visualmente do tamanho

    private float tamanhoBolhaNormal; //Tamanho da bolha ao iniciar a cena


    [Header("Audio da bolha")]
    [Tooltip("Quanto tempo demorará para o áudio ficar no volume máximo ou mínimo, vale tanto para os " +
        "audios que irão tocar quanto para os sons ambientes (em segundos)")]
    [SerializeField] float audioTransicao;
    
    [Tooltip("Adicionar o áudio que deverá ser tocado ao se aproximar da bolha")]
    public AudioClip[] Audio; //Array de áudios
    
    [Tooltip("Volume de cada áudio")]
    [Range(0, 100)] public float[] VolumesAudio; //Array de volumes de cada áudio
    
    [Tooltip("Se cada áudio deverá tocar em loop")]
    public bool[] LoopAudio; //Array de se cada áudio deverá tocar em loop


    [Header("Sons ambiente")]
    
    [Tooltip("Adicionar os sons ambientes que tocarão ao se aproximar da bolha")]
    public AudioClip[] SonsAmbiente; //Array de sons ambiente
    
    [Tooltip("Volume de cada som ambiente")]
    [Range(0, 100)] public float[] VolumesAmbiente; //Array de volumes de cada som ambiente
    
    [Tooltip("Se cada som ambiente estará em loop")]
    public bool[] LoopAmbiente; //Array de se cada som ambiente estará em loop

    private void Awake() {
        Debug.LogWarning($"Essa aplicação está usando um script de teste no objeto {gameObject.name}." +
            $" Não o utilizar além desse protótipo.");

        if (maquete == null) { //Se a variável Maquete não estiver atribuída
            Debug.LogWarning("O objeto CenaDentroBolha não foi atribuído.");
        }

        if (bolha == null) { //Se a variável Bolha não estiver atribuída
            Debug.LogWarning("O objeto Bolha não foi atribuído.");
        }

        if (Audio.Length == 0 || Audio == null) { //Se o array Audio estiver vazio
            Debug.LogWarning("O array Audio está vazio.");
        }

        if (SonsAmbiente.Length == 0 || SonsAmbiente == null) { //Se o array SonsAmbiente estiver vazio
            Debug.LogWarning("O array SonsAmbiente está vazio.");
        }

        colisorBolha = GetComponent<SphereCollider>(); //Pega o componente SphereCollider do objeto
        audioSource = GetComponent<AudioSource>(); //Pega o componente AudioSource do objeto

        tamanhoBolhaNormal = colisorBolha.radius; //Atribui o tamanho normal da bolha

        int VolumesAmbienteIndex = 0; //Index para o array de volumes de cada som ambiente
        foreach (AudioClip clip in SonsAmbiente) { //Para cada som ambiente no array SonsAmbiente
            var CenaBolhaAudioSource = maquete.AddComponent<AudioSource>(); //Adiciona um AudioSource
                                                                            //no objeto Maquete
            CenaBolhaAudioSource.clip = clip; //Atribui o áudio
            CenaBolhaAudioSource.DOFade(VolumesAmbiente[VolumesAmbienteIndex] * 0.01f, audioTransicao); 
            //Atribui o volume multiplicado por 0.01f para ficar entre 0 e 1 pois atributo volume do
            //AudioSource vai de 0 a 1
            CenaBolhaAudioSource.loop = LoopAmbiente[VolumesAmbienteIndex]; //Atribui se o áudio estará
                                                                            //em loop
            VolumesAmbienteIndex++; //Adiciona 1 ao index
        }
    }

    private void OnTriggerEnter(Collider other) { //Quando um objeto entrar no colisor
        if (!other.CompareTag("MainCamera")) { //Caso o objeto não seja a câmera
            return; //Só ignora
        }       

        StopAllCoroutines(); //Para todas as corrotinas. 
        //Corrotinas são funções que podem ser pausadas e continuadas depois de um tempo determinado ou
        //até que uma condição seja atingida fora do void Update() ou void FixedUpdate() que são funções
        //que são chamadas a cada frame

        float newScale = tamanhoDaBolhaExpandida * 2.5f;
        bolha.transform.DOScale(newScale, tamanhoTransicao);

        colisorBolha.radius = tamanhoDaBolhaExpandida; //Atribui o tamanho da bolha ao colisor

        if (!audioSource.isPlaying) { //Se o AudioSource não estiver tocando
            PlayAudio(Audio[0], VolumesAudio[0], LoopAudio[0]);
            StartCoroutine(PlayNextAudio(1)); //Começa a corrotina para tocar o próximo áudio
        }
    }

    private void PlayAudio(AudioClip audioClip, float volume, bool loop) { //Função para tocar um áudio
        audioSource.clip = audioClip; //Atribui o áudio
        audioSource.DOFade(volume * 0.01f, audioTransicao); //Aumenta o volume do audio de 0 até o volume
        audioSource.loop = loop; //Atribui se o áudio estará em loop
        audioSource.Play(); //Toca o áudio
    }

    private IEnumerator PlayNextAudio(int index) { //Corrotina para tocar o próximo áudio
        yield return new WaitForSeconds(audioSource.clip.length); //Espera o tempo do áudio atual acabar
        PlayAudio(Audio[index], VolumesAudio[index], LoopAudio[index]); //Toca o próximo áudio
        if (Audio.Length > index + 1) { //Se o próximo áudio existir
            StartCoroutine(PlayNextAudio(index + 1)); //Começa a corrotina para tocar o próximo áudio
        }
    }

    private void OnTriggerExit(Collider other) { //Quando um objeto sair do colisor
        if (!other.CompareTag("MainCamera")) { //Caso o objeto não seja a câmera
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
        audioSource.Stop(); //Para o áudio
        audioSource.clip = null; //Atribui null ao áudio
    }

    void OnDrawGizmosSelected() { //Função para desenhar no editor
        if (!GizmosTamBolhaExp) { //Se a variável GizmosTamBolhaExp for falsa
            return; //Só ignora
        }

        Gizmos.color = Color.yellow; //Atribui a cor amarela ao Gizmos
        Gizmos.DrawWireSphere(transform.position, tamanhoDaBolhaExpandida); //Desenha uma esfera com o
                                                                            //tamanho da bolha expandida
    }
}