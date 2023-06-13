using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking.Types;

public class Test : MonoBehaviour {

    //ISSO É UM SCRIPT DE TESTE, NÃO É O SCRIPT DA APLICAÇÃO, NÃO O UTILIZAR ALÉM DESSE PROTÓTIPO!!!

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
        Debug.LogWarning($"Essa aplicação está usando um script de teste no objeto {gameObject.name}. Não o utilizar além desse protótipo.");

        if (CenaDentroBolha == null) {
            print("O objeto CenaDentroBolha não foi atribuído.");
        }

        if (ObjetosDoCenario.Length == 0) {
            print("O array ObjetosDoCenario está vazio.");
        }

        if (Bolha == null) {
            print("O objeto Bolha não foi atribuído.");
        }

        if (SonsAmbiente.Length == 0) {
            print("O array SonsAmbiente está vazio.");
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
        if (!other.CompareTag("MainCamera")) {
            return;
        }

        StartCoroutine(LerpScale(new Vector3(4f, 4f, 4f)));

        colisorBolha.radius = 2.5f;

        if (!audioSource.isPlaying) {
            audioSource.clip = Audio[0];
            audioSource.volume = VolumesAudio[0] * 0.01f;
            audioSource.loop = LoopAudio[0];
            audioSource.Play();
            StartCoroutine(PlayNextAudio(1));
        }
    }

    private IEnumerator LerpScale(Vector3 targetSize) {
        while (Bolha.transform.localScale.x != targetSize.x) {
            Bolha.transform.localScale = Vector3.Lerp(Bolha.transform.localScale, targetSize, Time.deltaTime * 5);
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
        if (!other.CompareTag("MainCamera")) {
            return;
        }

        StopAllCoroutines();

        StartCoroutine(LerpScale(new Vector3(1f, 1f, 1f)));

        colisorBolha.radius = 1.5f;

        audioSource.Stop();
        audioSource.clip = null;
    }
}
