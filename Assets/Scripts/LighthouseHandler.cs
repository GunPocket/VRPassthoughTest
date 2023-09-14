using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LighthouseHandler : MonoBehaviour {
    [SerializeField] private GameObject _sol;
    [SerializeField] private GameObject _maquete;
    private float _rotation;
    [Range(-360f, 360f)][SerializeField] private float _dia = 47;
    [Range(-360f, 360f)][SerializeField] private float _noite = 216;

    // Update is called once per frame
    void Update() {
        _rotation = _sol.transform.eulerAngles.z;
        _maquete.SetActive(_rotation < _noite && _rotation > _dia);
    }
}
