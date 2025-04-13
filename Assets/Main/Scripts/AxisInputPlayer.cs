using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisInputPlayer : MonoBehaviour
{
    ModCarController modCarController;
    //WheelController wheelController;
    

    void Awake()
    {
        modCarController = GetComponent<ModCarController>();
        //wheelController = GetComponent<WheelController>();
    }

    void Update()
    {
        Vector2 inputVector = Vector2.zero;

        inputVector.x = Input.GetAxisRaw("Horizontal");
        inputVector.y = Input.GetAxisRaw("Vertical");

        modCarController.SetInputVector(inputVector);
        //wheelController.SetInputVector(inputVector);
    }
}
