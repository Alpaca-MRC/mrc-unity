using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARPlaneManager))]
public class ARSceneController : MonoBehaviour
{
    private ARPlaneManager _planeManager;

    void Start()
    {
        _planeManager = GetComponent<ARPlaneManager>();
        _planeManager.planesChanged += OnPlanesChanged;
    }

    private void SetPlaneAlpha(ARPlane plane, float fillAlpha, float lineAlpha) {
        var meshRenderer = plane.GetComponentInChildren<MeshRenderer>();
        var lineRenderer = plane.GetComponentInChildren<LineRenderer>();

        if (meshRenderer != null) {
            Color color = meshRenderer.material.color;
            color.a = fillAlpha;
            meshRenderer.material.color = color;
        }

        if (lineRenderer != null) {
            // 현재의 startColor 및 endColor 가져오기
            Color startColor = lineRenderer.startColor;
            Color endColor = lineRenderer.endColor;

            // alpha 컴포넌트 설정
            startColor.a = lineAlpha;
            endColor.a = lineAlpha;

            // 업데이트된 alpha로 새로운 color 적용하기
            lineRenderer.startColor = startColor;
            lineRenderer.endColor = endColor;
        }
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs args) {
        if (args.added.Count > 0 ) {

            foreach (var plane in args.added) {
                // 새로운 plane에 대해서 alpha 값을 0으로 설정
                SetPlaneAlpha(plane, 0f, 0f);
            }
        }
    }

    void OnDestroy() {
        _planeManager.planesChanged -= OnPlanesChanged;
    }
}
