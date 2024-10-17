using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NXROrigin : MonoBehaviour
{

    public HandType DominantHand = HandType.Left;
    public GameObject CameraObject; 
    public GameObject LeftHandObject; 
    public GameObject RightHandObject;


    public NXRController LeftHand { get; set; }
    public NXRController RightHand { get; set; }


    public void OnEnable() { 
        LeftHand = LeftHandObject.GetComponent<NXRController>();
        RightHand = RightHandObject.GetComponent<NXRController>();
    }


    public NXRController GetDominantHand() { 

        if (DominantHand == HandType.Left) { 
            return LeftHand; 
        }

        return RightHand;
    }


     public NXRController GetSecondaryHand() { 

        if (DominantHand == HandType.Left) { 
            return RightHand; 
        }

        return LeftHand;
    }
}
