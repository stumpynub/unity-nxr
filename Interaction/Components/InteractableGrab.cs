using UnityEngine;
using UnityEditor.Animations;
using System.Data.Common;
using Unity.VisualScripting;

public class InteractableGrab : MonoBehaviour
{

    public bool Reparent = true;
    public bool Persice = false; 

    public bool UpdatePosition = true; 
    public bool UpdateRotation = true; 


    public Vector3 RotationOffset;


    private Vector3 _prevPos; 
    private Quaternion _prevRot; 
    private Vector3 _prevPrimaryPos; 
    private Quaternion _prevPrimaryRot; 

    
    Interactable _interactable;


    void Start()
    {
        _interactable = GetComponent<Interactable>();
        _interactable.onGrabbed += OnGrabbed;
        _interactable.onDropped += OnDropped;
    }

    void Update()
    {

        Vector3 newPos = _interactable.transform.position; 
        Quaternion newRot = _interactable.transform.rotation; 


        // update position and rotation when primary grabber found 
        if (_interactable.PrimaryInteractor != null)
        { 
            newRot = _interactable.PrimaryInteractor.transform.rotation; 
            newPos = _interactable.PrimaryInteractor.transform.position; 

            _interactable.PrimaryInteractor.Controller.HandActionMap.FindAction("TriggerPressed").performed += ctx => { 
                RotationOffset -= Vector3.left * -20f; 
            }; 
        }


        // do look rotation if two handed
        if (_interactable.IsTwoHanded())
        {
            Vector3 dir = _interactable.SecondaryInteractor.transform.position - _interactable.PrimaryInteractor.transform.position;
            Vector3 up = _interactable.PrimaryInteractor.transform.up; // expexting OpenXR aim pose
            newRot.SetLookRotation(dir, up);
        }


        // update transform if grabbed 
        if (_interactable.IsGrabbed() && !Reparent && !Persice) {    
            
            _interactable.transform.SetPositionAndRotation(newPos, newRot * Quaternion.Euler(RotationOffset));
        }

        if (_interactable.IsGrabbed() && !Reparent && Persice) { 
            PreciseGrab(); 
        }
    }

    void OnGrabbed(NXRInteractor interactor)
    {
        if (interactor == _interactable.PrimaryInteractor)
        {
            _prevPos = transform.position; 
            _prevRot = transform.rotation; 
            _prevPrimaryPos = _interactable.PrimaryInteractor.transform.position; 
            _prevPrimaryRot = interactor.transform.rotation; 

            ReparentGrab(interactor);
        }
    }

    void OnDropped(NXRInteractor interactor)
    {
        if (interactor == _interactable.PrimaryInteractor)
        {
            if (Reparent) transform.parent = null; 
        }
    }

    void PreciseGrab()  { 
        NXRInteractor interactor = _interactable.PrimaryInteractor; 
        Vector3 pos = _prevPos + (interactor.transform.position -_prevPrimaryPos); 
        Quaternion rot = _prevPrimaryRot * interactor.transform.rotation; 

        if (UpdatePosition) _interactable.transform.position = pos; 
        if (UpdateRotation) _interactable.transform.rotation = Quaternion.Inverse(_prevPrimaryRot) * interactor.transform.rotation * _prevRot; 
    }


    void ReparentGrab(NXRInteractor interactor) { 
        if (Reparent) transform.parent = interactor.transform;

        if (!Persice) {
            transform.position = interactor.transform.position; 
            transform.rotation = interactor.transform.rotation;
        }
    }

    void FollowGrab() { 

    }
}

