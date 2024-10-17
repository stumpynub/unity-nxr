using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using ExitGames.Client.Photon.StructWrapping;
using System.Threading;

public class NXRInteractor : MonoBehaviour
{


    public InputActionReference InteractAction;
    public Interactable GrabbedInteractable;
    public NXRController Controller { get; private set; }

    private float _sortThrottleTime = 5;
    private float _sortThrottle = 0;
    private Dictionary<String, List<Interactable>> _hoveredInteractables = new();


    public event Grabbed OnGrabbed;
    public event Dropped OnDropped;


    public delegate void Grabbed(Interactable interactable);
    public delegate void Dropped(Interactable interactable);


    void Start()
    {
        Controller = GetComponent<NXRController>();
    }

    void TryInteract(InputAction.CallbackContext ctx)
    {
        if (GrabbedInteractable != null) return;

        foreach (String key in GetSortedInteractables().Keys)
        {
            if (ctx.action.ToString().Contains(key))
            {
                GrabbedInteractable = GetSortedInteractables()[key].First();
                GrabbedInteractable.Grab(this);
                Controller.HandActionMap.FindAction(GrabbedInteractable.DropAction).canceled += TryDrop;
                OnGrabbed?.Invoke(GrabbedInteractable);
            }
        }
    }


    void TryDrop(InputAction.CallbackContext ctx)
    {
        if (GrabbedInteractable == null) return;
        OnDropped?.Invoke(GrabbedInteractable);

        GrabbedInteractable.Drop(this);
        Controller.HandActionMap.FindAction(GrabbedInteractable.DropAction).canceled -= TryDrop;
        GrabbedInteractable = null;
    }


    Dictionary<string, List<Interactable>> GetSortedInteractables()
    {
        Dictionary<String, List<Interactable>> sorted = new(); 
        if (_hoveredInteractables.Count <= 0) return  sorted;


        foreach (String key in _hoveredInteractables.Keys)
        {
            Vector3 pos = transform.position;
            List<Interactable> interactableList = _hoveredInteractables[key].OrderBy(x => Vector3.Distance(x.gameObject.transform.position, pos)).ToList();
            sorted[key] = interactableList; 
        }
        
        _hoveredInteractables = sorted; 
        
        return _hoveredInteractables;
    }


    void OnTriggerEnter(Collider other)
    {

        Interactable interactable = other.GetComponent<Interactable>();

        if (interactable == null) return;
        if (_hoveredInteractables.ContainsKey(interactable.GrabAction))
        {
            if (!_hoveredInteractables[interactable.GrabAction].Contains(interactable))
                _hoveredInteractables[interactable.GrabAction].Add(interactable);
        }
        else
        {
            List<Interactable> interactableList = new();
            interactableList.Add(interactable);
            _hoveredInteractables.Add(interactable.GrabAction, interactableList);
        }

        Controller.HandActionMap.FindAction(interactable.GrabAction).performed += TryInteract;
    }


    void OnTriggerExit(Collider other)
    {
        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable == null) return;

        if (_hoveredInteractables.ContainsKey(interactable.GrabAction))
        {
            if (_hoveredInteractables[interactable.GrabAction].Contains(interactable))
            {
                _hoveredInteractables[interactable.GrabAction].Remove(interactable);
            }

            if (_hoveredInteractables[interactable.GrabAction].Count() <= 0)
            {
                _hoveredInteractables.Remove(interactable.GrabAction);
            }
        }
    }
}
