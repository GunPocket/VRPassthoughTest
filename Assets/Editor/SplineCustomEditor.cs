using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SplineHandler))] //Diz que esse editor é para o script BubbleHandler
public class SplineCustomEditor : Editor {
    public override void OnInspectorGUI() { //Esse método é chamado toda vez que o editor é aberto

        SplineHandler splineHandler = (SplineHandler)target; //Pega o script BubbleHandler do objeto que está sendo editado

        DrawDefaultInspector(); //Desenha o inspector padrão do objeto

        if (splineHandler.NodeReference.Length != splineHandler.SplineTangentIn.Length) {
            splineHandler.SplineTangentIn = new float[splineHandler.NodeReference.Length];
        }

        if (splineHandler.NodeReference.Length != splineHandler.SplineTangentOut.Length) {
            splineHandler.SplineTangentOut = new float[splineHandler.NodeReference.Length];
        }
    }
}
