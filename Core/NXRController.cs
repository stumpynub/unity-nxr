using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class NXRController : MonoBehaviour
{   
    public HandType Hand  = HandType.Left; 
    public NXRActionMap ActionMap { get; set; }


    [HideInInspector]
    public InputActionMap HandActionMap; 
    private TrackedPoseDriver _driver; 


    void OnEnable() { 
        ActionMap = new(); 
        ActionMap.Enable(); 

        if (Hand == HandType.Left) {
            HandActionMap = ActionMap.ControllerLeft; 
        } else { 
            HandActionMap = ActionMap.ControllerRight; 
        }
    }

    void OnDisable() { 
        ActionMap.Disable(); 
    }

    void Start()
    {
        _driver = gameObject.AddComponent<TrackedPoseDriver>(); 
        _driver.positionAction = HandActionMap.FindAction("Position"); 
        _driver.rotationAction = HandActionMap.FindAction("Rotation"); 
    }
}
