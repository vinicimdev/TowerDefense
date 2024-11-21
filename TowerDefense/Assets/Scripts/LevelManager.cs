using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    public Transform startPoint;
    public Transform[] path;

    public int gold;

    private void Awake()
    {
        main = this;
    }

    private void Start()
    {
        gold = 200;
    }

    public void IncreaseGold(int amount)
    {
        gold += amount;
    }

    public bool SpendGold(int amount)
    {
        if(amount <= gold)
        {
            gold -= amount;
            return true;
        }
        else
        {
            Debug.Log("Bro u broke af");
            return false;
        }
    }
}
