using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractablePhysicsGrab : MonoBehaviour
{

    private Interactable _interatable;
    private Rigidbody _rb; 

    
    void Start()
    {
        _interatable = GetComponent<Interactable>();
        _rb = GetComponent<Rigidbody>();
    }


    void FixedUpdate()
    {
        if (_interatable.PrimaryInteractor != null) { 
            Quaternion current = transform.rotation; 
            Quaternion inverse = Quaternion.Inverse(_interatable.PrimaryInteractor.transform.rotation); 
            Quaternion newRot = current * inverse; 

            Vector3 dir = _interatable.PrimaryInteractor.transform.position - transform.position; 
            _rb.velocity = dir / Time.deltaTime;  
            _rb.angularVelocity = newRot.eulerAngles / Time.deltaTime; 
        }
    }
}
