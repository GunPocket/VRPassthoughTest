using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SplineHandler))] //Diz que esse editor � para o script BubbleHandler
public class SplineCustomEditor : Editor {
    public override void OnInspectorGUI() { //Esse m�todo � chamado toda vez que o editor � aberto

        SplineHandler splineHandler = (SplineHandler)target; //Pega o script BubbleHandler do objeto que est� sendo editado

        DrawDefaultInspector(); //Desenha o inspector padr�o do objeto

        /*if (splineHandler.NodeReference.Length != splineHandler.splineTangentIn.Length) {
            splineHandler.splineTangentIn = new float[splineHandler.NodeReference.Length];
        }

        if (splineHandler.NodeReference.Length != splineHandler.splineTangentOut.Length) {
            splineHandler.splineTangentOut = new float[splineHandler.NodeReference.Length];
        }*/
    }
}
