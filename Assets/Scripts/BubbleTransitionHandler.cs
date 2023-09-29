using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class BubbleTransitionHandler : MonoBehaviour {
    private SphereCollider _sphereCollider;
    private MeshRenderer _sphereMeshRenderer;
    private Transform _sphereTransform;

    public bool IsFirstBubble = false;
    private bool _didItPop = false;
    private bool _riverFlow = false;

    [HideInInspector] public GameObject Maquete; // GameObject da maquete

    [Header("Efeitos na cena para esta bolha")]
    [Tooltip("Referência para o sistema de partículas da bolha")]
    [SerializeField] private ParticleSystem _bubblePop;

    [Tooltip("Referência para o sistema de partículas da bolha")]
    public ParticleSystem BubbleSpawner;

    [Tooltip("Referência para o efeito visual do rio")]
    [SerializeField] private VisualEffect _Rio;

    [Tooltip("Referência para a animação de aumento da bolha")]
    [HideInInspector] public Animator RiseAnimation;

    [Tooltip("Quanto a bolha subirá")]
    [Range(0, 5)] public float RiseAmount = 2;
    [SerializeField] bool GizmosAlturaBolha;

    [Tooltip("Objetos que serão mostrados")]
    [SerializeField] private GameObject[] _scenery;

    [Header("Características do rio")]
    [Tooltip("Curva de animação do rio")]
    [SerializeField] private AnimationCurve _riverShowBehavior;

    [Tooltip("Parar a animação do rio após o estouro da bolha")]
    [SerializeField] private bool _stopRiverAnimationAfterBubble;

    [Header("Próxima bolha")]
    [Tooltip("Referência para a próxima bolha")]
    public GameObject NextBubble;

    private void OnEnable() {
        Initializer();

        if (IsFirstBubble) {
            StartCoroutine(FirstBubble());
        }

        if (_Rio != null) {
            _Rio.Stop();
        }

        StartCoroutine(StartBubble());

        foreach (var obj in _scenery) {
            obj.SetActive(false);
        }
    }

    private IEnumerator FirstBubble() {
        BubbleSpawner.Play();
        RiseAnimation.Play("Rise");
        NextBubble.GetComponentInChildren<BubbleTransitionHandler>().NextBubble.SetActive(false);
        NextBubble.SetActive(false);
        yield return new WaitForSecondsRealtime(3f);
        BubbleSpawner.Stop();
    }

    private void Initializer() {
        RiseAnimation = GetComponent<Animator>();
        _sphereCollider = GetComponentInChildren<SphereCollider>();
        _sphereMeshRenderer = GetComponentInChildren<MeshRenderer>();

        Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();
        _sphereTransform = transforms[1];
    }

    private IEnumerator StartBubble() {
        if (Maquete != null) Maquete.SetActive(false);
        yield return new WaitForSecondsRealtime(3f);
        _sphereMeshRenderer.enabled = true;
        if (Maquete != null) Maquete.SetActive(true);
        yield return new WaitForSecondsRealtime(5f);
        BubbleSpawner.Stop();
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
    }

    public void RiverAnimation() {
        if (_Rio != null && !_riverFlow) {
            _riverFlow = true;
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

            if (NextBubble != null) {
                NextBubble.SetActive(true);
                NextBubble.GetComponentInChildren<BubbleTransitionHandler>().RiseAnimation.Play("Rise");
                NextBubble.GetComponentInChildren<BubbleTransitionHandler>().BubbleSpawner.Play();
            }

            if (_stopRiverAnimationAfterBubble) {
                yield return new WaitForSecondsRealtime(3);
                _Rio.Stop();
            }
        }
    }

    public bool IsBubblePopped() {
        return _didItPop;
    }

    void OnDrawGizmosSelected() {
        if (!GizmosAlturaBolha) {
            return;
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(gameObject.transform.position + new Vector3(0, RiseAmount, 0), .5f);
    }
}
