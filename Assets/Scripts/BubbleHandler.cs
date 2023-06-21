using System.Collections;
using UnityEngine;

public class BubbleHandler : MonoBehaviour {

    //ESSE SCRIPT AINDA EST� EM TESTE, N�O � O SCRIPT DA APLICA��O, N�O O UTILIZAR AL�M DESSE PROT�TIPO!!!

    private AudioSource audioSource; //AudioSource que tocar� os �udios
    private SphereCollider colisorBolha; //Colisor da bolha

    [Header("Refer�ncia dos objetos da cena")] //Cabe�alho para melhor organiza��o no inspector

    [Tooltip("Aqui voc� deve adicionar a refer�ncia do GameObject da maquete para essa vari�vel")] //Tooltip � o texto que aparece quando o
                                                                                                   //mouse fica em cima da vari�vel
    [SerializeField] GameObject Maquete; //GameObject da maquete

    [Tooltip("Aqui voc� deve adicionar a refer�ncia do GameObject da bolha para essa vari�vel")]
    [SerializeField] GameObject Bolha; //GameObject da bolha

    [Tooltip("Ditar� qual tamanho a bolha ir� se expandir")]
    [SerializeField][Range(0, 5)] float TamanhoDaBolhaExpandida; //Tamanho da bolha ao expandir. Range diz o valor m�nimo e m�ximo que pode ser atribu�do

    [Tooltip("Mostrar� na cena o tamanho visualmente do tamanho")]
    [SerializeField] bool GizmosTamBolhaExp; //Mostrar� na cena o tamanho visualmente do tamanho
    private float tamanhoBolhaNormal; //Tamanho da bolha ao iniciar a cena

    [Header("Audio de voz")]
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
        Debug.LogWarning($"Essa aplica��o est� usando um script de teste no objeto {gameObject.name}. N�o o utilizar al�m desse prot�tipo.");

        if (Maquete == null) { //Se a vari�vel Maquete n�o estiver atribu�da
            Debug.LogWarning("O objeto CenaDentroBolha n�o foi atribu�do.");
        }

        if (Bolha == null) { //Se a vari�vel Bolha n�o estiver atribu�da
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
            var CenaBolhaAudioSource = Maquete.AddComponent<AudioSource>(); //Adiciona um AudioSource no objeto Maquete
            CenaBolhaAudioSource.clip = clip; //Atribui o �udio
            CenaBolhaAudioSource.volume = VolumesAmbiente[VolumesAmbienteIndex] * 0.01f; //Atribui o volume multiplicado por 0.01f para ficar entre 0 e 1
                                                                                         //pois atributo volume do AudioSource vai de 0 a 1
            CenaBolhaAudioSource.loop = LoopAmbiente[VolumesAmbienteIndex]; //Atribui se o �udio estar� em loop
            VolumesAmbienteIndex++; //Adiciona 1 ao index
        }
    }

    private void OnTriggerEnter(Collider other) { //Quando um objeto entrar no colisor
        if (!other.CompareTag("MainCamera")) { //Caso o objeto n�o seja a c�mera
            return; //S� ignora
        }

        StopAllCoroutines(); //Para todas as corrotinas. 
        //Corrotinas s�o fun��es que podem ser pausadas e continuadas depois de um tempo determinado ou at� que uma
        //condi��o seja atingida fora do void Update() ou void FixedUpdate() que s�o fun��es que s�o chamadas a cada frame.

        StartCoroutine(LerpScale(new Vector3(TamanhoDaBolhaExpandida * 2.5f, TamanhoDaBolhaExpandida * 2.5f, TamanhoDaBolhaExpandida * 2.5f))); 
        //Come�a a corrotina para expandir a bolha

        colisorBolha.radius = TamanhoDaBolhaExpandida; //Atribui o tamanho da bolha ao colisor

        if (!audioSource.isPlaying) { //Se o AudioSource n�o estiver tocando
            PlayAudio(Audio[0], VolumesAudio[0], LoopAudio[0]);
            StartCoroutine(PlayNextAudio(1)); //Come�a a corrotina para tocar o pr�ximo �udio
        }
    }

    private void PlayAudio(AudioClip audioClip, float volume, bool loop) { //Fun��o para tocar um �udio
        audioSource.clip = audioClip; //Atribui o �udio
        audioSource.volume = volume * 0.01f; //Atribui o volume multiplicado por 0.01f para ficar entre 0 e 1
                                             //pois atributo volume do AudioSource vai de 0 a 1
        audioSource.loop = loop; //Atribui se o �udio estar� em loop
        audioSource.Play(); //Toca o �udio
    }

    private IEnumerator LerpScale(Vector3 targetSize) { //Corrotina para expandir a bolha
        while (Bolha.transform.localScale.x != targetSize.x) { //Enquanto o tamanho da bolha n�o for o tamanho desejado
            Bolha.transform.localScale = Vector3.Lerp(Bolha.transform.localScale, targetSize, Time.deltaTime * 5); //Expande a bolha
            yield return null; //Retorna null para continuar a corrotina no pr�ximo frame
        } //Quando o tamanho da bolha for o tamanho desejado, a corrotina para
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

        StartCoroutine(LerpScale(new Vector3(tamanhoBolhaNormal, tamanhoBolhaNormal, tamanhoBolhaNormal))); //Come�a a corrotina para diminuir a bolha

        colisorBolha.radius = tamanhoBolhaNormal; //Atribui o tamanho normal da bolha ao colisor

        audioSource.Stop(); //Para o �udio
        audioSource.clip = null; //Atribui null ao �udio
    }

    void OnDrawGizmosSelected() { //Fun��o para desenhar a bolha no editor
        if (!GizmosTamBolhaExp) { //Se a vari�vel GizmosTamBolhaExp for falsa
            return; //S� ignora
        }

        Gizmos.color = Color.yellow; //Atribui a cor amarela ao Gizmos
        Gizmos.DrawWireSphere(transform.position, TamanhoDaBolhaExpandida); //Desenha uma esfera com o tamanho da bolha expandida
    }
}