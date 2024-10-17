using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq; 
using UnityEngine;
using UnityEngine.InputSystem;

public class NXRInteractor : MonoBehaviour
{
 

    public InputActionReference InteractAction; 
    public Interactable GrabbedInteractable; 
    public NXRController Controller {  get; private set; }

    private List<Interactable> _hoveredInteractables = new(); 


    public event Grabbed OnGrabbed; 
    public event Dropped OnDropped; 


    public delegate void Grabbed(Interactable interactable); 
    public delegate void Dropped(Interactable interactable); 


    void Start()
    {
        Controller = GetComponent<NXRController>();
    }


    void TryInteract(InputAction.CallbackContext ctx) { 
        if (_hoveredInteractables.Count <= 0) return; 
        
        GrabbedInteractable = GetSortedInteractables()[0]; 
        GrabbedInteractable.Grab(this); 

        Controller.HandActionMap.FindAction(GrabbedInteractable.GrabAction).canceled += TryDrop; 
        OnGrabbed?.Invoke(GrabbedInteractable); 
    }

    
    void TryDrop(InputAction.CallbackContext ctx) { 
        if (GrabbedInteractable == null) return; 
        OnDropped?.Invoke(GrabbedInteractable); 
        
        GrabbedInteractable.Drop(this); 
        Controller.HandActionMap.FindAction(GrabbedInteractable.DropAction).canceled -= TryDrop; 
        GrabbedInteractable = null; 
    }


    List<Interactable> GetSortedInteractables() { 
        if (_hoveredInteractables.Count <= 0) return new List<Interactable>(); 

        Vector3 pos = transform.position; 
        return _hoveredInteractables.OrderBy(x => Vector3.Distance(x.gameObject.transform.position, pos)).ToList(); 
    }
    

    void OnTriggerEnter(Collider other) { 

        Interactable interactable = other.GetComponent<Interactable>();

        if (interactable == null) return;  
        if (_hoveredInteractables.Contains(interactable)) return;

    
        _hoveredInteractables.Add(interactable); 
        Controller.HandActionMap.FindAction(interactable.GrabAction).performed += TryInteract;
    }


    void OnTriggerExit(Collider other) { 
        Interactable interactable = other.GetComponent<Interactable>();
        if (_hoveredInteractables.Contains(interactable)) {
            Controller.HandActionMap.FindAction(interactable.GrabAction).performed -= TryInteract; 
            _hoveredInteractables.Remove(interactable);
        }
    }


    void OnCollisionEnter(Collision other) { 
        Interactable interactable = other.gameObject.GetComponent<Interactable>();

        if (interactable == null) return;  
        if (_hoveredInteractables.Contains(interactable)) return;


        _hoveredInteractables.Add(interactable); 
    }


    void OnCollisionExit(Collision other) { 
        Interactable interactable = other.gameObject.GetComponent<Interactable>();
        if (_hoveredInteractables.Contains(interactable)) {
            _hoveredInteractables.Remove(interactable);
        }
    }
}   
