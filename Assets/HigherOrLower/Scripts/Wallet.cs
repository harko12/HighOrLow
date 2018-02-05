using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Wallet
{
    public int Points;
    public int Tokens;
    public int Coins;

    public Wallet()
    {

    }

    public void Wipe()
    {
        Points = Tokens = Coins = 0;
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void Load(string json)
    {
        JsonUtility.FromJsonOverwrite(json, this);
    }

}
