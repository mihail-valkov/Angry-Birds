using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
   //references to cameras
    [SerializeField] private CinemachineVirtualCamera _idleCam;
    [SerializeField] private CinemachineVirtualCamera _followCam;

    //assign the cameras in awake
    private void Awake()
    {
        SwitchToIdleCam();
    }

    //switch to idle cam
    public void SwitchToIdleCam()
    {
        _idleCam.enabled = true;
        _followCam.enabled = false;
    }

    //switch to follow cam
    public void SwitchToFollowCam(Transform followTransform)

    {
        _idleCam.enabled = false;
        _followCam.enabled = true;
        _followCam.Follow = followTransform;
    }
}
