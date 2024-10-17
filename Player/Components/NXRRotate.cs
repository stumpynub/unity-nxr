using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NXRRotate : MonoBehaviour
{

    public float RotationSpeed = 5.0f; 


    [SerializeField]
    private NXROrigin _origin; 

    private float _rotationValue = 0.0f; 


    void Start()
    {
        print(_origin.GetSecondaryHand()); 
       _origin.GetSecondaryHand().HandActionMap.FindAction("Joy").performed += Rotate; 
       _origin.GetSecondaryHand().HandActionMap.FindAction("Joy").canceled += ctx => _rotationValue = 0.0f; 
    }


    void FixedUpdate() { 
        transform.RotateAround(_origin.CameraObject.transform.position, Vector3.up, _rotationValue); 
    }


    void Rotate(InputAction.CallbackContext ctx) { 
        _rotationValue =  ctx.ReadValue<Vector2>().x * RotationSpeed; 
    }
}
