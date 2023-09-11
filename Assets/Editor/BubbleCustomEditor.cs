using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BubbleHandler))] //Diz que esse editor é para o script BubbleHandler
public class BubbleCustomEditor : Editor {
    public override void OnInspectorGUI() { //Esse método é chamado toda vez que o editor é aberto
       
        BubbleHandler bubbleHandler = (BubbleHandler)target; //Pega o script BubbleHandler do objeto que está sendo editado

        DrawDefaultInspector(); //Desenha o inspector padrão do objeto

        if (bubbleHandler.Audio.Length != bubbleHandler.VolumesAudio.Length) { //Se o tamanho do array Audio for diferente do tamanho do array VolumesAudio
            bubbleHandler.VolumesAudio = new float[bubbleHandler.Audio.Length]; //Atribui o tamanho do array VolumesAudio para o tamanho do array Audio
        }
    }
}
