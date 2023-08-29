using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class SplineHandler : MonoBehaviour {
    [Header("Referencia do Spline")]
    public SplineContainer SplineContainer;

    [Header("Referencias para os Nodes do Spline")]
    public Transform[] NodeReference;
    //private List<Transform> TangentInReference;
    //private List<Transform> TangentOutReference;

    [Header("Como deve ser feito a edição das tangentes das splines")]
    [Tooltip("Ainda não foi implementado o modo manual para salvar tempo de desenvolvimento")]
    public EditTypeEnum EditType;

    public enum EditTypeEnum {
        /*Manual,*/
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

        for (int i = 0; i < NodeReference.Length; i++) {
            BezierKnot bezierKnotPosition = new BezierKnot(NodeReference[i].position);
            SplineContainer.Spline.SetKnot(i, bezierKnotPosition);
        }

        SplineContainer.Spline.SetTangentMode(TangentMode);


        /*TangentInReference = new List<Transform>();
        TangentOutReference = new List<Transform>();

        foreach (Transform t in NodeReference) {
            int index = TangentInReference.Count;

            TangentInReference.Add(t.GetChild(0));
            var tInReference = SplineContainer.Spline.GetCurve(index > 0 ? index - 1 : index).Tangent1;
            TangentInReference[index].transform.position = tInReference + (float3)t.position;

            TangentOutReference.Add(t.GetChild(1));
            var tOutReference = SplineContainer.Spline.GetCurve(index).Tangent0;
            TangentOutReference[index].transform.position = tOutReference + (float3)t.position;
        }*/
    }

    private void LateUpdate() {
        if (EditType == EditTypeEnum.Automatico) {
            for (int i = 0; i < NodeReference.Length; i++) {
                if (0 == i) {
                    tangentIn = Vector3.zero;
                    tangentOut = NodeReference[i + 1].position - NodeReference[i].position;
                } else if (i == NodeReference.Length - 1) {
                    tangentIn = NodeReference[i].position - NodeReference[i - 1].position;
                    tangentOut = Vector3.zero;
                } else {
                    tangentIn = NodeReference[i - 1].position - NodeReference[i + 1].position;
                    tangentOut = NodeReference[i + 1].position - NodeReference[i - 1].position;
                }

                BezierKnot bezierKnotPosition = new BezierKnot(NodeReference[i].position,
                                                Vector3.Normalize(tangentIn) * SplineTangentIn[i],
                                                Vector3.Normalize(tangentOut) * SplineTangentOut[i]);

                SplineContainer.Spline.SetKnot(i, bezierKnotPosition);

            }
        } else {
           /* for (int i = 0; i < NodeReference.Length; i++) {
                BezierKnot bezierKnotPosition = new BezierKnot(NodeReference[i].transform.position,
                                               TangentInReference[i].transform.position,
                                               TangentOutReference[i].transform.position);

                SplineContainer.Spline.SetKnot(i, bezierKnotPosition);
            }
           */
        }
        SplineContainer.Spline.SetTangentMode(TangentMode);
    }
}
