using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothWindController : MonoBehaviour
{
    private Cloth cloth;
    private const float MIN_ACCEL = 0.1f;
    private const float MAX_ACCEL = 7.0f;
    private float accel = MIN_ACCEL;
    private Vector3 direction;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        Debug.Log(animator);
        cloth = GetComponent<Cloth>();
        accel = cloth.externalAcceleration.z;
        //Debug.Log(transform.eulerAngles);
        //Debug.Log(transform.rotation);
        Vector3 vec = transform.up;
        Debug.Log(vec);
        vec =  vec * -7;
        Debug.Log(vec);
        direction = vec;
        //cloth.externalAcceleration = vec;
    }

    // Update is called once per frame
    void Update()
    {
        SetAccel(-0.004f);
        CheckInput();
    }

    private void OnMouseDown()
    {
        SetAccel(0.5f);
        animator.SetBool("hasClicked", true);
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
                animator.SetBool("hasClicked", true);
            }
        }
    }

    private void SetAccel(float amount) 
    {
        if(accel + amount < MIN_ACCEL || accel + amount > MAX_ACCEL) {
            return;
        }
        accel += amount;
        cloth.externalAcceleration = Vector3.ClampMagnitude(direction,accel);
        //Debug.Log(cloth.externalAcceleration);
    }
}
