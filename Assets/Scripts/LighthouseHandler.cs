using UnityEngine;

public class LighthouseHandler : MonoBehaviour {
    [Tooltip("Referência para o GameObject do sol")]
    [SerializeField] private GameObject _sol;

    [Tooltip("Referência para o GameObject da maquete")]
    [SerializeField] private GameObject _maquete;

    [Tooltip("Rotação do sol")]
    [SerializeField] private float _rotation;

    [Tooltip("Ângulo para o dia (de -360 a 360 graus)")]
    [Range(-360f, 360f)]
    [SerializeField] private float _dia = 47;

    [Tooltip("Ângulo para a noite (de -360 a 360 graus)")]
    [Range(-360f, 360f)]
    [SerializeField] private float _noite = 216;

    [Tooltip("Referência para o componente OVRPassthroughLayer")]
    [SerializeField] private OVRPassthroughLayer _passthroughLayer;

    private float _passtrhoughBrightness;

    [Tooltip("Modificador de intensidade da luz")]
    [Range(0, 10)]
    [SerializeField] private int _lightModifier;

    [Tooltip("Referência para a luz global")]
    [SerializeField] private Light _globalLight;

    [HideInInspector] public bool Active = false;

    void Update() {
        if (!Active) return;

        _globalLight.enabled = !Active;

        _passtrhoughBrightness = _sol.transform.rotation.z / _lightModifier;
        _passthroughLayer.SetColorMapControls(0, _passtrhoughBrightness);
        _rotation = _sol.transform.eulerAngles.z;
        _maquete.SetActive(_rotation < _noite && _rotation > _dia);
    }
}
