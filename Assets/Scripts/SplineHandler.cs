using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using static UnityEngine.GraphicsBuffer;

public class SplineHandler : MonoBehaviour {
    public bool EditMode = false;

    [Header("Referencia do Spline")]
    public SplineContainer SplineContainer;

    [Header("Referencias para os Nodes do Spline")]
    public Transform[] NodeReference;
    //private List<Transform> inTangentReference;
    //private List<Transform> outTangentReference;

    [Header("Comportamento das splines")]
    [Tooltip("Ainda não foi implementado o modo manual para salvar tempo de desenvolvimento")]
    public EditTypeEnum EditType;

    public enum EditTypeEnum {
        Automatico,
        Broken,
        Continuous,
        Mirrored
    };

    //[Header("Magnitude das tangentes")]
    private float[] tangentInStrength;
    private float[] tangentOutStrength;

    private TangentMode TangentMode;

    private Vector3 tangentIn;
    private Vector3 tangentOut;


    private void Awake() {
        if (!SplineContainer) {
            SplineContainer = GetComponent<SplineContainer>();
        }

        for (int i = 0; i < NodeReference.Length; i++) {
            BezierKnot bezierKnotPosition = new BezierKnot(NodeReference[i].position);
            SplineContainer.Spline.SetKnot(i, bezierKnotPosition);
        }

        SplineContainer.Spline.SetTangentMode(TangentMode);

        tangentInStrength = new float[NodeReference.Length];
        tangentOutStrength = new float[NodeReference.Length];

        tangentInStrength = TangentSetup(tangentInStrength);
        tangentOutStrength = TangentSetup(tangentOutStrength);

        //BrokenBezierSetup();
    }

    private float[] TangentSetup(float[] tangent) {
        float[] tan = new float[tangent.Length];
        for (int i = 0; i < tan.Length; i++) {
            tan[i] = 1f;
        }
        return tan;
    }

    private void BrokenBezierSetup() {
        ///ESSA PARTE DO CÓDIGO É PARA O SETUP BROKEN TANGENTES

        /*inTangentReference = new List<Transform>();
        outTangentReference = new List<Transform>();

        foreach (Transform t in NodeReference) {
            int index = inTangentReference.Count;

            inTangentReference.Add(t.GetChild(0));
            var tInReference = SplineContainer.Spline.GetCurve(index > 0 ? index - 1 : index).Tangent1;
            inTangentReference[index].transform.position = tInReference + (float3)t.position;

            outTangentReference.Add(t.GetChild(1));
            var tOutReference = SplineContainer.Spline.GetCurve(index).Tangent0;
            outTangentReference[index].transform.position = tOutReference + (float3)t.position;
        }*/
    }

    private void LateUpdate() {
        if (EditMode == false) { return; }

        switch (EditType) {
            case EditTypeEnum.Automatico:
                for (int i = 0; i < NodeReference.Length; i++) {
                    BezierKnot bezierKnotPosition = new BezierKnot(NodeReference[i].position);
                    SplineContainer.Spline.SetKnot(i, bezierKnotPosition);
                }
                TangentMode = TangentMode.AutoSmooth;
                break;

            /*case EditTypeEnum.Broken:
                TangentMode = TangentMode.Broken;
                break;
            */

            case EditTypeEnum.Continuous:
                for (int i = 0; i < NodeReference.Length; i++) {

                    if (0 == i) {
                        tangentIn = Vector3.zero;
                        tangentOut = NodeReference[i + 1].position - NodeReference[i].position;

                        tangentInStrength[i] = 0;
                        tangentOutStrength[i] = Vector3.Distance(NodeReference[i + 1].transform.position,
                                            NodeReference[i].transform.position);

                    } else if (i == NodeReference.Length - 1) {
                        tangentIn = NodeReference[i].position - NodeReference[i - 1].position;
                        tangentOut = Vector3.zero;

                        tangentInStrength[i] = Vector3.Distance(NodeReference[i].transform.position,
                                            NodeReference[i - 1].transform.position);
                        tangentOutStrength[i] = 0;

                    } else {
                        tangentIn = NodeReference[i - 1].position - NodeReference[i + 1].position;
                        tangentOut = NodeReference[i + 1].position - NodeReference[i - 1].position;

                        tangentInStrength[i] = Vector3.Distance(NodeReference[i].transform.position,
                                            NodeReference[i - 1].transform.position); ;
                        tangentOutStrength[i] = Vector3.Distance(NodeReference[i].transform.position,
                                            NodeReference[i + 1].transform.position);

                    }

                    BezierKnot bezierKnotPosition = new BezierKnot(NodeReference[i].position,
                                                    Vector3.Normalize(tangentIn) * tangentInStrength[i],
                                                    Vector3.Normalize(tangentOut) * tangentOutStrength[i]);

                    SplineContainer.Spline.SetKnot(i, bezierKnotPosition);
                }
                TangentMode = TangentMode.Continuous;
                break;

            /*case EditTypeEnum.Mirrored:
                TangentMode = TangentMode.Mirrored;
                break;
            */

            default:
                Debug.LogWarning("Outras formas de editar além de automatico e continuous " +
                    "ainda não implementadas por completo, ajustando para automático");
                TangentMode = TangentMode.AutoSmooth;
                break;
        }

        SplineContainer.Spline.SetTangentMode(TangentMode);
    }
}