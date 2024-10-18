using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private Mode mode;
    public enum Mode
    {
        LookAt,
        LookAtInverted,
        CameraLookForward,
        CameraLookForwardInverted
    }
    private void LateUpdate()
    {
        switch(mode)
        {
            case Mode.LookAt: 
                transform.LookAt(Camera.main.transform);
                break;

            case Mode.LookAtInverted:

                Vector3 dirFromCamera = transform.position - Camera.main.transform.position;
                transform.LookAt(transform.position + dirFromCamera);
                break;

            case Mode.CameraLookForward:
                transform.forward = Camera.main.transform.forward;
                break;

            case Mode.CameraLookForwardInverted:
                transform.forward = -Camera.main.transform.forward;
                break;
        }
        
    }
}
