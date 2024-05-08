using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MovePortal : MonoBehaviour
{
    public GameObject startGameCanvas;
    public XRRayInteractor leftRayInteractor;
    public XRRayInteractor rightRayInteractor;

    void Start()
    {
        if (startGameCanvas != null) {
            startGameCanvas.SetActive(false);
        }

        // 레이저 비활성화
        if (leftRayInteractor != null) leftRayInteractor.gameObject.SetActive(false);
        if (rightRayInteractor != null) rightRayInteractor.gameObject.SetActive(false);
    }

    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            if (startGameCanvas != null) {
                startGameCanvas.SetActive(true);
            }


            // 레이저 활성화
            if (leftRayInteractor != null) leftRayInteractor.gameObject.SetActive(true);
            if (rightRayInteractor != null) rightRayInteractor.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.name == "Player")
        {
            if (startGameCanvas != null) {
                startGameCanvas.SetActive(false);
            }

            // 레이저 비활성화
            if (leftRayInteractor != null) leftRayInteractor.gameObject.SetActive(false);
            if (rightRayInteractor != null) rightRayInteractor.gameObject.SetActive(false);
        }
    }
}
