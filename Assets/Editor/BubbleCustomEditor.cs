using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BubbleHandler))] //Diz que esse editor � para o script BubbleHandler
public class BubbleCustomEditor : Editor {
    public override void OnInspectorGUI() { //Esse m�todo � chamado toda vez que o editor � aberto
       
        BubbleHandler bubbleHandler = (BubbleHandler)target; //Pega o script BubbleHandler do objeto que est� sendo editado

        DrawDefaultInspector(); //Desenha o inspector padr�o do objeto

        if (bubbleHandler.Audio.Length != bubbleHandler.VolumesAudio.Length) { //Se o tamanho do array Audio for diferente do tamanho do array VolumesAudio
            bubbleHandler.VolumesAudio = new float[bubbleHandler.Audio.Length]; //Atribui o tamanho do array VolumesAudio para o tamanho do array Audio
        }
    }
}
