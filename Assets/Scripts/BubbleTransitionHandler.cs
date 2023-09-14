using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class BubbleTransitionHandler : MonoBehaviour {

    private SphereCollider _sphereCollider;
    private MeshRenderer _sphereMeshRenderer;
    private Transform _sphereTransform;

    public bool IsFirstBubble = false;
    private bool _firstInteraction = true;
    private bool _didItPop = false;

    [Header("Efeitos na cena para esta bolha")]
    [SerializeField] private ParticleSystem _bubblePop;
    public ParticleSystem BubbleSpawner;
    [SerializeField] private VisualEffect _Rio;
    [HideInInspector] public Animator RiseAnimation;
    [Range(0, 5)] public float RiseAmount = 2;

    [Header("Objetos que irão ser mostrados")]
    [SerializeField] private GameObject[] _scenery;

    [Header("Características do rio")]
    [SerializeField] private AnimationCurve _riverShowBehavior;
    [SerializeField] private bool _stopRiverAnimationAfterBubble;

    [Header("Próxima bolha")]
    public GameObject NextBubble;

    [SerializeField] bool GizmosAlturaBolha;


    private void OnEnable() {
        Initializer();

        if (IsFirstBubble) {
            BubbleSpawner.Play();
            RiseAnimation.Play("Rise");
            NextBubble.GetComponentInChildren<BubbleTransitionHandler>().NextBubble.SetActive(false);
            NextBubble.SetActive(false);
        }

        if (_Rio != null) {
            _Rio.Stop();
        }

        StartCoroutine(StartBubble());

        foreach (var obj in _scenery) {
            obj.SetActive(false);
        }
    }

    private void Initializer() {
        RiseAnimation = GetComponent<Animator>();
        _sphereCollider = GetComponentInChildren<SphereCollider>();
        _sphereMeshRenderer = GetComponentInChildren<MeshRenderer>();

        Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();
        _sphereTransform = transforms[1];
    }

    private IEnumerator StartBubble() {
        yield return new WaitForSecondsRealtime(3f);
        _sphereMeshRenderer.enabled = true;
        yield return new WaitForSecondsRealtime(5f);
        BubbleSpawner.Stop();
    }

    public void OnTriggerEnter(Collider other) {
        if (!_firstInteraction) {
            return;
        }

        _firstInteraction = false;
    }

    public void StartPopBubble() {
        StartCoroutine(PopBubble());
    }

    private IEnumerator PopBubble() {

        BubbleSpawner.Stop();

        _sphereTransform.transform.DOMoveY(_sphereTransform.position.y + 2, 3);
        yield return new WaitForSecondsRealtime(3);
        _bubblePop.Play();
        _sphereMeshRenderer.enabled = false;
        _didItPop = true;
        _sphereCollider.enabled = false;
        if (_Rio != null) {
            StartCoroutine(StartRiver());
        }
    }

    private IEnumerator StartRiver() {
        _Rio.Play();
        float x = 0;
        while (x < 100) {
            yield return new WaitForSecondsRealtime(0.05f);
            x++;
            float value = _riverShowBehavior.Evaluate(x / 100f);
            _Rio.playRate = value;
        }
        if (x == 100) {
            foreach (var obj in _scenery) {
                obj.SetActive(true);
            }

            NextBubble.SetActive(true);
            NextBubble.GetComponentInChildren<BubbleTransitionHandler>().RiseAnimation.Play("Rise");
            NextBubble.GetComponentInChildren<BubbleTransitionHandler>().BubbleSpawner.Play();

            if (_stopRiverAnimationAfterBubble) {
                yield return new WaitForSecondsRealtime(3);
                _Rio.Stop();
            }
        }
    }

    public bool IsBubblePopped() {
        return _didItPop;
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
