using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking.Types;

public class Test : MonoBehaviour {

    //ISSO � UM SCRIPT DE TESTE, N�O � O SCRIPT DA APLICA��O, N�O O UTILIZAR AL�M DESSE PROT�TIPO!!!

    private AudioSource audioSource;
    private SphereCollider colisorBolha;

    [Header("Arrumar depois")]
    [SerializeField] GameObject CenaDentroBolha;
    [SerializeField] GameObject[] ObjetosDoCenario;
    [SerializeField] GameObject Bolha;

    [Header("Audio de voz")]
    [SerializeField] AudioClip[] Audio;
    [SerializeField][Range(0, 100)] float[] VolumesAudio;
    [SerializeField] bool[] LoopAudio;

    [Header("Sons ambiente")]
    [SerializeField] AudioClip[] SonsAmbiente;
    [SerializeField][Range(0, 100)] float[] VolumesAmbiente;
    [SerializeField] bool[] LoopAmbiente;


    private void Awake() {
        Debug.LogWarning($"Essa aplica��o est� usando um script de teste no objeto {gameObject.name}. N�o o utilizar al�m desse prot�tipo.");

        if (CenaDentroBolha == null) {
            print("O objeto CenaDentroBolha n�o foi atribu�do.");
        }

        if (ObjetosDoCenario.Length == 0) {
            print("O array ObjetosDoCenario est� vazio.");
        }

        if (Bolha == null) {
            print("O objeto Bolha n�o foi atribu�do.");
        }

        if (SonsAmbiente.Length == 0) {
            print("O array SonsAmbiente est� vazio.");
        }


        Bolha.SetActive(true);

        colisorBolha = GetComponent<SphereCollider>();
        audioSource = GetComponent<AudioSource>();

        int VolumesAmbienteIndex = 0;
        foreach (AudioClip clip in SonsAmbiente) {
            var CenaBolhaAudioSource = CenaDentroBolha.AddComponent<AudioSource>();
            CenaBolhaAudioSource.clip = clip;
            CenaBolhaAudioSource.volume = VolumesAmbiente[VolumesAmbienteIndex] * 0.01f;
            CenaBolhaAudioSource.loop = LoopAmbiente[VolumesAmbienteIndex];
            VolumesAmbienteIndex++;
        }
    }

    private void OnTriggerEnter(Collider other) {
        print(other.name);

        StartCoroutine(LerpScale(new Vector3(0.5f, 0.5f, 0.5f)));

        foreach (GameObject gameObject in ObjetosDoCenario) {
            gameObject.SetActive(false);
        }

        Bolha.SetActive(false);

        colisorBolha.radius = 5;

        if (!audioSource.isPlaying) {
            audioSource.clip = Audio[0];
            audioSource.volume = VolumesAudio[0] * 0.01f;
            audioSource.loop = LoopAudio[0];
            audioSource.Play();
            StartCoroutine(PlayNextAudio(1));
        }
    }
    
    private IEnumerator LerpScale(Vector3 targetSize) {
        while (CenaDentroBolha.transform.localScale != targetSize) {
            CenaDentroBolha.transform.localScale = Vector3.Lerp(CenaDentroBolha.transform.localScale, targetSize, Time.deltaTime * 10);
            yield return null;
        }
    }

    private IEnumerator PlayNextAudio(int index) {
        yield return new WaitForSeconds(audioSource.clip.length);
        audioSource.clip = Audio[index];
        audioSource.volume = VolumesAudio[index] * 0.01f;
        audioSource.loop = LoopAudio[index];
        audioSource.Play();
        if (Audio.Length > index + 1) {
            StartCoroutine(PlayNextAudio(index + 1));
        }
    }

    private void OnTriggerExit(Collider other) {
        foreach (GameObject gameObject in ObjetosDoCenario) {
            gameObject.SetActive(true);
        }

        Bolha.SetActive(true);
        StartCoroutine(LerpScale(new Vector3(0.1f, 0.1f, 0.1f)));

        StopCoroutine("PlayNextAudio");

        colisorBolha.radius = 1.5f;

        audioSource.Stop();
        audioSource.clip = null;
    }
}
