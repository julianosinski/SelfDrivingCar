using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModCarController : MonoBehaviour
{
    [Header("Car parts")]
    [SerializeField] Rigidbody sphereRB;
    [SerializeField] Rigidbody carRb;

    [SerializeField] Transform partsContainer;
    [Header("Rest...")]
    [SerializeField] LayerMask groundLayer;
    public float maxFwdSpeed = 20;
    [SerializeField] float maxTurnSpeed = 10;
    //[SerializeField] AnimationCurve turnCurve;
    [SerializeField] float maxRevSpeed = 10;
    [SerializeField] float airDrag = 0.1f;
    [SerializeField] float groundDrag = 4;

    [SerializeField] float dirftFactor = 0.95f;

    [Header("Grand check related")]
    [SerializeField] bool isAlignToGroundEffectedBySpeed = true;
    [SerializeField] float alignToGroundTime = 4;
    


    float accelerationInput;
    float turnInput;
    public float currentSpeed;
    

    void Start()
    {
        if(partsContainer!= null)
        {
            sphereRB.transform.parent = partsContainer;
            carRb.transform.parent = partsContainer;
        }
        else
        {
            sphereRB.transform.parent = null;
            if (carRb!=null)
            {
                carRb.transform.parent = null;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = sphereRB.transform.position;

        ApplySteering();   
    }

    void ApplyEngineForce()
    {
        currentSpeed = Vector3.Dot(transform.forward, sphereRB.linearVelocity);

        if (currentSpeed > maxFwdSpeed && accelerationInput > 0)
        {
            return;
        }
        if (currentSpeed < -maxRevSpeed && accelerationInput < 0)
        {
            return;
        }
        if (sphereRB.linearVelocity.sqrMagnitude > maxFwdSpeed * maxFwdSpeed && accelerationInput > 0)
        {
            return;
        }

        if (accelerationInput == 0 || (accelerationInput < 0 && currentSpeed > 0))
        {
            //sphereRB.drag = Mathf.Lerp(sphereRB.drag, 3.0f, Time.fixedDeltaTime * 3);
            sphereRB.linearDamping = Mathf.Lerp(sphereRB.linearDamping, groundDrag, Time.fixedDeltaTime * groundDrag);
        }
        else
        {
            sphereRB.linearDamping = 0;
        }

        //Vector3 engineForceVector = transform.forward * accelerationInput * accelerationFactor;

        //sphereRB.AddForce(engineForceVector, ForceMode.Force);
        
        float newAcceleration = accelerationInput;
        newAcceleration *= accelerationInput > 0 ? maxFwdSpeed : maxRevSpeed; 
        sphereRB.AddForce(transform.forward * newAcceleration, ForceMode.Acceleration);
        
    }


    void FixedUpdate()
    {
        if (AlignToGround())
        {
            ApplyEngineForce();
            KillOrthogonalVelocity();
        }
        else
        {
            sphereRB.linearDamping = airDrag;
            sphereRB.AddForce(transform.up * -10f);
        }
        RotateCarColider();
    }

    void ApplySteering()
    {
        //float current = Mathf.Lerp(0, maxTurnSpeed, turnInput );
        //print("current: " + current + " turnInput: " + turnInput);
        //float newRotation = turnInput * maxTurnSpeed * Time.deltaTime * (1-(currentSpeed / (accelerationInput > 0 ? maxFwdSpeed : maxRevSpeed)*0.5f));
        //float rotationDifficulty = turnCurve.Evaluate(Time.fixedDeltaTime*3) * (1 - (currentSpeed / (accelerationInput > 0 ? maxFwdSpeed : maxRevSpeed) * 0.5f));
        

        float newRotation = turnInput * maxTurnSpeed * Time.deltaTime * (Mathf.Abs(currentSpeed) < 5 ? 0 : 1);
        //print(newRotation);
        
        transform.Rotate(0, newRotation, 0);
    }

    bool AlignToGround()
    {
        RaycastHit hit;

        bool isGrounded = Physics.Raycast(transform.position, -transform.up, out hit, 1f, groundLayer);

        // badz prostopadle do ziemi

        // now wersja jest bardziej plynna 
        Quaternion toRotateTo = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, toRotateTo, Time.deltaTime * (isAlignToGroundEffectedBySpeed ? Mathf.Abs(currentSpeed) : alignToGroundTime));
        // stara wersja 
        //transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;

        return isGrounded;
    }

    void KillOrthogonalVelocity()
    {
        Vector3 forwardVelocity = transform.forward * Vector3.Dot(sphereRB.linearVelocity, transform.forward);
        Vector3 rightVelocity = transform.right * Vector3.Dot(sphereRB.linearVelocity, transform.right);

        sphereRB.linearVelocity = forwardVelocity + rightVelocity * dirftFactor;
    }

    void RotateCarColider()
    {
        if (carRb != null)
        {
            carRb.constraints = RigidbodyConstraints.FreezeRotation;
            carRb.MoveRotation(transform.rotation);
        }
    }

    public void SetInputVector(Vector2 inputVector)
    {
        accelerationInput = inputVector.y;
        turnInput = inputVector.x;
        
    }
}
