using UnityEngine;

public class LighthouseHandler : MonoBehaviour {
    [Tooltip("Refer�ncia para o GameObject do sol")]
    [SerializeField] private GameObject _sol;

    [Tooltip("Refer�ncia para o GameObject da maquete")]
    [SerializeField] private GameObject _maquete;

    [Tooltip("Rota��o do sol")]
    [SerializeField] private float _rotation;

    [Tooltip("�ngulo para o dia (de -360 a 360 graus)")]
    [Range(-360f, 360f)]
    [SerializeField] private float _dia = 47;

    [Tooltip("�ngulo para a noite (de -360 a 360 graus)")]
    [Range(-360f, 360f)]
    [SerializeField] private float _noite = 216;

    [Tooltip("Refer�ncia para o componente OVRPassthroughLayer")]
    [SerializeField] private OVRPassthroughLayer _passthroughLayer;

    private float _passtrhoughBrightness;

    [Tooltip("Modificador de intensidade da luz")]
    [Range(0, 10)]
    [SerializeField] private int _lightModifier;

    [Tooltip("Refer�ncia para a luz global")]
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
