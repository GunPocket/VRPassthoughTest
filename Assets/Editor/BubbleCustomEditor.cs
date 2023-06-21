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
                
        if (bubbleHandler.Audio.Length != bubbleHandler.LoopAudio.Length) { //Se o tamanho do array Audio for diferente do tamanho do array LoopAudio
            bubbleHandler.LoopAudio = new bool[bubbleHandler.Audio.Length]; //Atribui o tamanho do array LoopAudio para o tamanho do array Audio
        }
                
        if (bubbleHandler.SonsAmbiente.Length != bubbleHandler.VolumesAmbiente.Length) { //Se o tamanho do array SonsAmbiente for diferente do tamanho do array VolumesAmbiente
            bubbleHandler.VolumesAmbiente = new float[bubbleHandler.SonsAmbiente.Length]; //Atribui o tamanho do array VolumesAmbiente para o tamanho do array SonsAmbiente
        }
                
        if (bubbleHandler.SonsAmbiente.Length != bubbleHandler.LoopAmbiente.Length) { //Se o tamanho do array SonsAmbiente for diferente do tamanho do array LoopAmbiente
            bubbleHandler.LoopAmbiente = new bool[bubbleHandler.SonsAmbiente.Length]; //Atribui o tamanho do array LoopAmbiente para o tamanho do array SonsAmbiente
        }
    }
}
