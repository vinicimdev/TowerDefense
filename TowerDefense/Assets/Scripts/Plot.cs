using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Color hoverColor;
    [SerializeField] private BoxCollider2D hitbox;

    private GameObject tower;
    private Color startColor;
    private bool showRange = false;

    private void Start()
    {
        startColor = sr.color;
    }

    private void OnMouseEnter()
    {
        if(tower != null)
            return;
        else
            OnHoverColor();
    }

    private void OnMouseExit()
    {
        sr.color = startColor;
    }

    private void OnHoverColor()
    {
        sr.color = hoverColor;
    }

    private void OnMouseDown()
    {
        if (tower != null)
        {
            hitbox.enabled = false;
            return;
        }
        else
        {
            hitbox.enabled = true;
        }

        Tower towerToBuild = BuildManager.main.GetSelectedTower();

        if (towerToBuild.cost > LevelManager.main.gold)
        {
            Debug.Log("Pobre");
            return;
        }

        LevelManager.main.SpendGold(towerToBuild.cost);
        
        if(tower == null)
            tower = Instantiate(towerToBuild.prefab, transform.position, Quaternion.identity);
        else
            return;

        sr.color = startColor;
    }
}
