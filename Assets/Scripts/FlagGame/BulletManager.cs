using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class BulletManager : MonoBehaviour
{
    [SerializeField] protected float speed = 15f; // 발사체 이동 속도
    [SerializeField] protected float hitOffset = 0f; // 충돌 지점 오프셋
    [SerializeField] protected bool UseFirePointRotation; // 발사 지점 회전 사용 여부
    [SerializeField] protected Vector3 rotationOffset = new Vector3(0, 0, 0); // 회전 오프셋
    [SerializeField] protected GameObject hit; // 충돌 이펙트 오브젝트
    [SerializeField] protected ParticleSystem hitPS; // 충돌 이펙트 파티클 시스템
    [SerializeField] protected GameObject flash; // 플래시 이펙트 오브젝트
    [SerializeField] protected Rigidbody rb; // Rigidbody 컴포넌트
    [SerializeField] protected Collider col; // Collider 컴포넌트
    [SerializeField] protected Light lightSourse; // 빛 소스
    [SerializeField] protected GameObject[] Detached; // 분리된 오브젝트 배열
    [SerializeField] protected ParticleSystem projectilePS; // 발사체 파티클 시스템
    private bool startChecker = false; // 시작 체크 변수
    [SerializeField]protected bool notDestroy = false; // 파괴하지 않음 여부

    protected virtual void Start()
    {
        if (!startChecker)
        {
            /*lightSourse = GetComponent<Light>();
            rb = GetComponent<Rigidbody>();
            col = GetComponent<Collider>();
            if (hit != null)
                hitPS = hit.GetComponent<ParticleSystem>();*/
            if (flash != null)
            {
                flash.transform.parent = null; // 부모 설정 해제
            }
        }
        if (notDestroy)
            StartCoroutine(DisableTimer(5)); // 일정 시간 후 비활성화
        else
            Destroy(gameObject, 5); // 일정 시간 후 파괴
        startChecker = true;
    }

    protected virtual IEnumerator DisableTimer(float time)
    {
        yield return new WaitForSeconds(time);
        if(gameObject.activeSelf)
            gameObject.SetActive(false); // 비활성화
        yield break;
    }

    protected virtual void OnEnable()
    {
        if (startChecker)
        {
            if (flash != null)
            {
                flash.transform.parent = null; // 부모 설정 해제
            }
            if (lightSourse != null)
                lightSourse.enabled = true; // 빛 활성화
            col.enabled = true; // 콜라이더 활성화
            rb.constraints = RigidbodyConstraints.None; // Rigidbody 제약 조건 해제
        }
    }

    protected virtual void FixedUpdate()
    {
        if (speed != 0)
        {
            rb.velocity = transform.forward * speed; // 이동
        }
    }

    // 충돌 시 호출되는 메서드
    protected virtual void OnCollisionEnter(Collision collision)
    {
        
        // 총알이 총구를 때려서 터지지 않게
        if (collision.gameObject.name.Equals("FireTool")) {
            return;
        }
        
        // 자기가 자기 자신을 때리지 못하도록 예외처리
        if (collision.collider.name.Equals("Enemy_Car"))
        {
            if (gameObject.name.Equals("Enemy Bullet(Clone)") || gameObject.name.Equals("Enemy Bullet"))
            {
                return;
            }
        }

        // 모든 축의 이동과 회전을 제한
        rb.constraints = RigidbodyConstraints.FreezeAll;
        //speed = 0;
        if (lightSourse != null)
            lightSourse.enabled = false; // 빛 비활성화
        col.enabled = false; // 콜라이더 비활성화
        projectilePS.Stop(); // 발사체 파티클 시스템 정지
        projectilePS.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        ContactPoint contact = collision.contacts[0]; // 충돌 지점 정보
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal); // 충돌 지점의 법선 벡터로 회전 생성
        Vector3 pos = contact.point + contact.normal * hitOffset; // 충돌 지점 계산

        // 충돌 시 이펙트 생성
        if (hit != null)
        {
            hit.transform.rotation = rot;
            hit.transform.position = pos;
            if (UseFirePointRotation) { hit.transform.rotation = gameObject.transform.rotation * Quaternion.Euler(0, 180f, 0); }
            else if (rotationOffset != Vector3.zero) { hit.transform.rotation = Quaternion.Euler(rotationOffset); }
            else { hit.transform.LookAt(contact.point + contact.normal); }
            hitPS.Play(); // 이펙트 재생
        }

        // 충돌 시 발사체의 궤적 제거 또는 부드럽게 제거. 분리된 요소에는 "AutoDestroying script"가 있어야 함
        foreach (var detachedPrefab in Detached)
        {
            if (detachedPrefab != null)
            {
                ParticleSystem detachedPS = detachedPrefab.GetComponent<ParticleSystem>();
                detachedPS.Stop(); // 파티클 시스템 정지
            }
        }
        if (notDestroy)
            StartCoroutine(DisableTimer(hitPS.main.duration)); // 이펙트 지속 시간 후 비활성화
        else
        {
            if (hitPS != null)
            {
                Destroy(gameObject, hitPS.main.duration); // 이펙트 지속 시간 후 파괴
            }
            else
                Destroy(gameObject, 1); // 1초 후 파괴
        }
    }
}
