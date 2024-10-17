
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class NXRRigidPlayer : MonoBehaviour
{
    
    private NXROrigin _origin; 
    private Rigidbody _body; 
    private CapsuleCollider _collider; 

    // Start is called before the first frame update
    void Start()
    {
        _origin = GetComponent<NXROrigin>();
        _body = GetComponent<Rigidbody>(); 
        _collider = GetComponent<CapsuleCollider>();
    }

    void FixedUpdate()
    {
        if (_origin == null) return; 
        

        Vector3 locCam = _origin.CameraObject.transform.localPosition; 
        Vector3 newCenter = new Vector3(locCam.x, 1, locCam.z); 
        _collider.center = newCenter; 
        

        Vector2 joy = _origin.GetDominantHand().HandActionMap.FindAction("Joy").ReadValue<Vector2>(); 
        Vector3 forward = _origin.CameraObject.transform.forward * joy.y; 
        Vector3 right = _origin.CameraObject.transform.right * joy.x; 
        Vector3 move = Vector3.ProjectOnPlane((forward + right).normalized, Vector3.up); 
        _body.linearVelocity += move; 
    }
}
