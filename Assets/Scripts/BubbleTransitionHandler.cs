using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class BubbleTransitionHandler : MonoBehaviour {

    private SphereCollider sphereCollider;
    private MeshRenderer sphereMeshRenderer;
    private Transform sphereTransform;

    public RiverHandler River;
    
    public bool IsFirstBubble = false;
    private bool firstInteraction = true;
    private bool didItPop = false;

    [Header("Efeitos na cena para esta bolha")]
    [SerializeField] private ParticleSystem BubblePop;
    public ParticleSystem BubbleSpawner;
    [SerializeField] private VisualEffect Rio;
    [HideInInspector] public Animator riseAnimation;
    [Range(0, 5)] public float RiseAmount = 2;

    [Header("Objetos que irão ser mostrados")]
    [SerializeField] private GameObject[] Scenery;

    [Header("Velocidade que o rio aparecerá")]
    [SerializeField] private AnimationCurve RiverShowBehavior;

    [Header("Próxima bolha")]
    public GameObject NextBubble;

    [SerializeField] bool GizmosAlturaBolha;


    private void OnEnable() {
        Initializer();
        River?.Initializer(gameObject.transform, NextBubble.transform, Rio);


        if (IsFirstBubble) {
            BubbleSpawner.Play();
            riseAnimation.Play("Rise");
            NextBubble.GetComponentInChildren<BubbleTransitionHandler>().NextBubble.SetActive(false);
            NextBubble.SetActive(false);
        }

        if (Rio != null) {
            Rio.Stop();
        }

        StartCoroutine(StartBubble());

        foreach (var obj in Scenery) {
            obj.SetActive(false);
        }
    }

    private void Initializer() {
        riseAnimation = GetComponent<Animator>();
        sphereCollider = GetComponentInChildren<SphereCollider>();
        sphereMeshRenderer = GetComponentInChildren<MeshRenderer>();

        Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();
        sphereTransform = transforms[1];
    }

    private IEnumerator StartBubble() {
        yield return new WaitForSecondsRealtime(3f);
        sphereMeshRenderer.enabled = true;
        yield return new WaitForSecondsRealtime(5f);
        BubbleSpawner.Stop();
    }

    public void OnTriggerEnter(Collider other) {
        if (!firstInteraction) {
            return;
        }

        firstInteraction = false;
    }

    public void StartPopBubble() {
        StartCoroutine(PopBubble());
    }

    private IEnumerator PopBubble() {

        BubbleSpawner.Stop();

        sphereTransform.transform.DOMoveY(sphereTransform.position.y + 2, 3);
        yield return new WaitForSecondsRealtime(3);
        BubblePop.Play();
        sphereMeshRenderer.enabled = false;
        didItPop = true;
        sphereCollider.enabled = false;
        if (Rio != null) {
            StartCoroutine(StartRiver());
        }
    }

    private IEnumerator StartRiver() {
        Rio.Play();
        float x = 0;
        while (x < 100) {
            yield return new WaitForSecondsRealtime(0.05f);
            x++;
            float value = RiverShowBehavior.Evaluate(x / 100f);
            Rio.playRate = value;
        }
        if (x == 100) {
            foreach (var obj in Scenery) {
                obj.SetActive(true);
            }

            NextBubble.SetActive(true);
            NextBubble.GetComponentInChildren<BubbleTransitionHandler>().riseAnimation.Play("Rise");
            NextBubble.GetComponentInChildren<BubbleTransitionHandler>().BubbleSpawner.Play();

        }
    }

    public bool IsBubblePopped() {
        return didItPop;
    }

    void OnDrawGizmosSelected() { //Função para desenhar no editor
        if (!GizmosAlturaBolha) { //Se a variável GizmosTamBolhaExp for falsa
            return; //Só ignora
        }

        Gizmos.color = Color.blue; //Atribui a cor amarela ao Gizmos
        Gizmos.DrawWireSphere(gameObject.transform.position + new Vector3(0, RiseAmount, 0), .5f); //Desenha uma esfera com o
                                                                                                   //tamanho da bolha expandida
    }
}
