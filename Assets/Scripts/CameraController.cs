using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera[] cameras;
    private int currentCamera = 0;

    public void SwitchCameras()
    {
        cameras[currentCamera].gameObject.SetActive(false);

        if (currentCamera == cameras.Length - 1)
        {
            cameras[0].gameObject.SetActive(true);
            currentCamera = 0;
        }
        else
        {
            cameras[currentCamera + 1].gameObject.SetActive(true);
            currentCamera++;
        }
    }
}
