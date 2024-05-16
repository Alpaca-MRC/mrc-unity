using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(ARPlaneManager))]
public class ARSceneController : MonoBehaviour
{
    [SerializeField]
    private InputActionReference _togglePlanesAction;
    [SerializeField]
    private InputActionReference _leftActivateAction;
    [SerializeField]
    private XRRayInteractor _leftRayInteractor;
    [SerializeField]
    private GameObject _grabbableCube;
    [SerializeField]
    private GameObject _prefab;
    private ARPlaneManager _planeManager;
    private ARAnchorManager _anchorManager;
    private bool _isVisible = false;
    private int _numPlanesAddedOccurred = 0;

    private List<ARAnchor> _anchors = new();

    // 이미 카트가 생겼는지 판단할 플래그
    private bool _isCartExist = false;

    void Start()
    {
        _planeManager = GetComponent<ARPlaneManager>();
        
        if (_planeManager is null) {
            Debug.LogError("--> 'ARPlaneManager'를 찾을 수 없음");
        }

        _anchorManager = GetComponent<ARAnchorManager>();

        if (_anchorManager is null) {
            Debug.LogError("--> 'ARAnchorManager'를 찾을 수 없음...");
        }

        _togglePlanesAction.action.performed += OnTogglePlanesAction;
        _planeManager.planesChanged += OnPlanesChanged;
        _anchorManager.anchorsChanged += OnAnchorsChanged;
        _leftActivateAction.action.performed += OnLeftActivateAction;
    }

    private void OnAnchorsChanged(ARAnchorsChangedEventArgs args)
    {
        // 통제 범위 밖에서 anchor가 지워졌다면 우리 리스트에서도 지워주기
        foreach (var removeAnchor in args.removed) {
            _anchors.Remove(removeAnchor);
            Destroy(removeAnchor.gameObject);
        }
    }

    private void OnLeftActivateAction(InputAction.CallbackContext context)
    {
        CheckIfRayHitsCollider();
    }

    private void CheckIfRayHitsCollider() {
        // 만약 Ray가 Collider를 친다면
        if (_leftRayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit)) {
            // 닿은 물체가 바닥일때만 기능 및 카드가 존재하지 않을때 기능
            ARPlane hitPlane = hit.transform.GetComponent<ARPlane>();
            if (hitPlane != null && hitPlane.classification == PlaneClassification.Floor && !_isCartExist) {
                // 물체 생성
                // 회전각 결정
                Quaternion rotation = Quaternion.Euler(0, 0, 0);
                GameObject instance = Instantiate(_prefab, hit.point, rotation);
                _isCartExist = true;

                // RC카의 이동을 위해 anchor를 제거
                // if (instance.GetComponent<ARAnchor>() == null) {
                //     ARAnchor anchor = instance.AddComponent<ARAnchor>();

                //     if (anchor != null) {
                //         _anchors.Add(anchor);
                //     }
                //     else {
                //         Debug.LogError("Anchor가 없어요...");
                //     }
                // }
            }
        } else {
            Debug.Log("--> 닿은게 없음");
        }
    }

    void Update(){}

    private void OnTogglePlanesAction(InputAction.CallbackContext obj) {
        //_isVisible = !_isVisible; // 토글 기능 꺼두기
        float fillAlpha = _isVisible ? 0.3f : 0f;
        float lineAlpha = _isVisible ? 1.0f : 0f;

        foreach (var plane in _planeManager.trackables) {
            SetPlaneAlpha(plane, fillAlpha, lineAlpha);
        }
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

    // private void OnPlanesChanged(ARPlanesChangedEventArgs args) {
    //     if (args.added.Count > 0 ) {
    //         _numPlanesAddedOccurred++;

    //         foreach (var plane in _planeManager.trackables) {
    //             PrintPlaneLabel(plane);
    //         }
    //     }
    // }
    
    private void OnPlanesChanged(ARPlanesChangedEventArgs args) {
        if (args.added.Count > 0 ) {
            _numPlanesAddedOccurred++;

            foreach (var plane in args.added) {
                // 새로운 plane에 대해서 alpha 값을 0으로 설정
                SetPlaneAlpha(plane, 0f, 0f);
            }
        }
    }

    void OnDestroy() {
        _togglePlanesAction.action.performed -= OnTogglePlanesAction;
        _planeManager.planesChanged -= OnPlanesChanged;
        _anchorManager.anchorsChanged -= OnAnchorsChanged;
        _leftActivateAction.action.performed -= OnLeftActivateAction;
    }

    // 큐브 생성 코드 -> 이후 랜덤 아이템 생성 위해 코드 남겨둠
    // private void SpawnGrabbableCube() {
    //     Vector3 spawnPosition;

    //     // 장면(Scene)속 인식된 각 물체를 순회
    //     foreach (var plane in _planeManager.trackables) {
    //         // 만약 해당 물체가 책상(Table)로 인식된다면, 큐브를 소환
    //         if (plane.classification == PlaneClassification.Table) {
    //             spawnPosition = plane.transform.position;
    //             spawnPosition.y += 0.3f;
    //             // 해당 위치(spawnPosition)에 큐브를 소환
    //             Instantiate(_grabbableCube, spawnPosition, Quaternion.identity);
    //         }
    //     }
    // }
}
