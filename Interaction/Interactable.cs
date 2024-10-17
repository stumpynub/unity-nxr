using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Interactable : MonoBehaviour
{
    public string GrabAction = "GripPressed"; 
    public string DropAction = "GripPressed"; 
    public NXRInteractor PrimaryInteractor { get; set; }
    public NXRInteractor SecondaryInteractor { get; set; }
    
    
    public event OnGrabbed onGrabbed; 
    public event OnDropped onDropped; 


    public delegate void OnGrabbed(NXRInteractor interactor); 
    public delegate void OnDropped(NXRInteractor interactor); 


    public void Grab(NXRInteractor interactor) { 
        if (PrimaryInteractor == null) { 
            PrimaryInteractor = interactor; 
        } else { 
            SecondaryInteractor = interactor; 
        }

        onGrabbed?.Invoke(interactor);
    }


    public void Drop(NXRInteractor interactor) { 
        // invoke before so we can compare to our Primary and Secondary interactors
        onDropped?.Invoke(interactor);

        if (interactor == PrimaryInteractor) PrimaryInteractor = null; 
        if (interactor == SecondaryInteractor) SecondaryInteractor = null; 

    }


    public bool IsTwoHanded() { 
        return PrimaryInteractor != null && SecondaryInteractor != null; 
    }

    public bool IsGrabbed() { 
         return PrimaryInteractor != null || SecondaryInteractor != null; 
    }
}
