using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHpBar : MonoBehaviour
{
    public Image enemyHpBarForeground;
    private RectTransform hpBarRectTransform;
    private float initialWidth;

    void Start()
    {
        hpBarRectTransform = enemyHpBarForeground.GetComponent<RectTransform>();
        initialWidth = hpBarRectTransform.sizeDelta.x;

        hpBarRectTransform.anchorMin = new Vector2(0, 0.5f);
        hpBarRectTransform.anchorMax = new Vector2(0, 0.5f);
        hpBarRectTransform.pivot = new Vector2(0, 0.5f);

        UpdateHealthBar(1f);
    }

    public void UpdateHealthBar(float healthPercentage)
    {
        hpBarRectTransform.sizeDelta = new Vector2(initialWidth * healthPercentage, hpBarRectTransform.sizeDelta.y);
    }

}
