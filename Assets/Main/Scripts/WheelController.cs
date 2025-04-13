using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    [SerializeField] GameObject[] wheels;
    [SerializeField] float rottationSpeed;

    Animator anim;

    float accelerationInput;
    float turnInput;

    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        Turn();
        //Rotate();
    }
    void Turn()
    {
        if (turnInput > 0)
        {
            anim.SetBool("goingLeft", false);
            anim.SetBool("goingRight", true);
        }
        else if (turnInput < 0)
        {
            anim.SetBool("goingLeft", true);
            anim.SetBool("goingRight", false);
        }
        else
        {
            anim.SetBool("goingLeft", false);
            anim.SetBool("goingRight", false);
        }
    }

    void Rotate()
    {
        foreach(var wheel in wheels)
        {
            wheel.transform.Rotate(new Vector3(Time.deltaTime * rottationSpeed * accelerationInput, 0, 0), Space.Self);
        }
    }

    public void SetInputVector(Vector2 inputVector)
    {
        accelerationInput = inputVector.y;
        turnInput = inputVector.x;

    }
}
