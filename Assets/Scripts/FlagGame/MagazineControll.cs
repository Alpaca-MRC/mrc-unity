using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagazineControll : MonoBehaviour
{
    public GameManager _gameManager;
    private int _myMagazine;

    void Start() {
        if (gameObject.name.Equals("Magazine_ONE"))
        {
            _myMagazine = 1;
        }
        else if (gameObject.name.Equals("Magazine_TWO"))
        {
            _myMagazine = 2;
        }
        else
        {
            _myMagazine = 3;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 박은 것이 플레이어라면
        if (other.gameObject.name.Equals("Friendly_Car") && other.gameObject.layer == 6)
        {
            Debug.Log("내 번호는: " + _myMagazine);
            other.gameObject.GetComponent<FriendlyShootingCar>().SetCurrentMagazineToFull();
            _gameManager.ReinstantiateMagazine(_myMagazine);
        }
        // 박은 것이 적이라면
        else if (other.gameObject.name.Equals("Enemy_Car") && other.gameObject.layer == 7)
        {
            other.gameObject.GetComponent<EnemyShootingCar>().SetCurrentMagazineToFull();
            _gameManager.ReinstantiateMagazine(_myMagazine);
        }
    }
}
