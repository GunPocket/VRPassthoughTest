using UnityEngine;

public class ClothWindController : MonoBehaviour
{
    private Cloth _cloth;
    private const float _MIN_ACCEL = 0.1f;
    private const float _MAX_ACCEL = 7.0f;
    private float _accel = _MIN_ACCEL;
    private Vector3 _direction;
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _animator = gameObject.GetComponent<Animator>();
        Debug.Log(_animator);
        _cloth = GetComponent<Cloth>();
        _accel = _cloth.externalAcceleration.z;
        Vector3 vec = transform.up;
        vec =  vec * -7;
        _direction = vec;
    }

    void Update()
    {
        SetAccel(-0.004f);
        CheckInput();
    }

    private void OnMouseDown()
    {
        SetAccel(0.5f);
        _animator.SetBool("hasClicked", true);
    }

    private void CheckInput()
    {
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            CheckHit();
        }
    }

    private void CheckHit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.CompareTag("Cloth"))
            {
                SetAccel(0.5f);
                _animator.SetBool("hasClicked", true);
            }
        }
    }

    private void SetAccel(float amount) 
    {
        if(_accel + amount < _MIN_ACCEL || _accel + amount > _MAX_ACCEL) {
            return;
        }
        _accel += amount;
        _cloth.externalAcceleration = Vector3.ClampMagnitude(_direction,_accel);
    }
}
