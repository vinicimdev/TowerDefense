using System;
using UnityEngine;

[Serializable]
public class Tower 
{
    public string name;
    public int cost;
    public GameObject prefab;
    public GameObject range;

    public Tower(string _name, int _cost, GameObject _prefab, GameObject _range)
    {
        name = _name;
        cost = _cost;
        prefab = _prefab;
        range = _range;
    }
}
