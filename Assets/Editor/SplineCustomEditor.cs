using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SplineHandler))] //Diz que esse editor � para o script BubbleHandler
public class SplineCustomEditor : Editor {
    public override void OnInspectorGUI() { //Esse m�todo � chamado toda vez que o editor � aberto

        SplineHandler splineHandler = (SplineHandler)target; //Pega o script BubbleHandler do objeto que est� sendo editado

        DrawDefaultInspector(); //Desenha o inspector padr�o do objeto

        if (splineHandler.PositionReference.Length != splineHandler.SplineTangentIn.Length) {
            splineHandler.SplineTangentIn = new float[splineHandler.PositionReference.Length];
        }

        if (splineHandler.PositionReference.Length != splineHandler.SplineTangentOut.Length) {
            splineHandler.SplineTangentOut = new float[splineHandler.PositionReference.Length];
        }
    }
}
