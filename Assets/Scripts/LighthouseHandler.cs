using UnityEngine;

public class LighthouseHandler : MonoBehaviour {
    [SerializeField] private GameObject _sol;
    [SerializeField] private GameObject _maquete;
    [SerializeField] private float _rotation;
    [Range(-360f, 360f)][SerializeField] private float _dia = 47;
    [Range(-360f, 360f)][SerializeField] private float _noite = 216;

    [SerializeField] private OVRPassthroughLayer _passthroughLayer;

    private float _passtrhoughBrightness;

    [Range(0, 10)][SerializeField] private int _lightModifier;

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