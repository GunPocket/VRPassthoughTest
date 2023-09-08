using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class BubbleTransitionHandler : MonoBehaviour {

    private SphereCollider sphereCollider;
    private MeshRenderer sphereMeshRenderer;

    public bool IsFirstBubble = false;
    private bool firstInteraction = true;

    [Header("Efeitos na cena para esta bolha")]
    [SerializeField] private ParticleSystem BubbleBurst;
    public ParticleSystem BubbleSpawner;
    [SerializeField] private VisualEffect Rio;
    public Animator riseAnimation;

    [Header("Objetos que irão ser mostrados")]
    [SerializeField] private GameObject[] Scenery;

    [Header("Velocidade que o rio aparecerá")]
    [SerializeField] private AnimationCurve RiverShowBehavior;

    [Header("Próxima bolha")]
    public GameObject NextBubble;

    private void OnEnable() {
        riseAnimation = GetComponent<Animator>();
        sphereCollider = GetComponentInChildren<SphereCollider>();
        sphereMeshRenderer = GetComponentInChildren<MeshRenderer>();

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

        if (NextBubble != null) {
            
        }
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

        if (Rio != null) {
            StartCoroutine(StartRiver());
        }
        BubbleSpawner.Stop();

        if (IsFirstBubble) {
            BubbleBurst.Play();
            sphereMeshRenderer.enabled = false;
            sphereCollider.enabled = false;
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
}
