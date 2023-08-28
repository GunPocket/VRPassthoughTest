using UnityEngine;
using UnityEngine.Splines;

public class SplineHandler : MonoBehaviour {
    [Header("Referencia do Spline")]
    public SplineContainer SplineContainer;

    [Header("Referencias para os Nodes do Spline")]
    public Transform[] PositionReference;

    [Header("Como deve ser feito a edição das tangentes das splines")]
    public EditTypeEnum EditType;

    public enum EditTypeEnum {
        Manual,
        Automatico
    };

    [Header("Magnitude das tangentes")]
    public float[] SplineTangentIn;
    public float[] SplineTangentOut;

    [Header("Comportamento da tangente")]
    public TangentMode TangentMode;

    

    private Vector3 tangentIn;
    private Vector3 tangentOut;
    

    private void Awake() {
        Debug.LogWarning("Refatorar isso aqui pf");

        if (!SplineContainer) {
            SplineContainer = GetComponent<SplineContainer>();
        }
    }

    private void LateUpdate() {
        if (EditType == EditTypeEnum.Automatico) {
            for (int i = 0; i < PositionReference.Length; i++) {
                if (0 == i) {
                    tangentIn = Vector3.zero;
                    tangentOut = PositionReference[i + 1].position - PositionReference[i].position;
                } else if (i == PositionReference.Length - 1) {
                    tangentIn = PositionReference[i].position - PositionReference[i - 1].position;
                    tangentOut = Vector3.zero;
                } else {
                    tangentIn = PositionReference[i - 1].position - PositionReference[i + 1].position;
                    tangentOut = PositionReference[i + 1].position - PositionReference[i - 1].position;
                }


                BezierKnot bezierKnotPosition = new BezierKnot(PositionReference[i].position,
                                                Vector3.Normalize(tangentIn) * SplineTangentIn[i],
                                                Vector3.Normalize(tangentOut) * SplineTangentOut[i]);

                SplineContainer.Spline.SetKnot(i, bezierKnotPosition);
                SplineContainer.Spline.SetTangentMode(TangentMode);
            }
        }
    }
}
