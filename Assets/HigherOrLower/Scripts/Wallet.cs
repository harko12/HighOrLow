using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Wallet
{
    public float Time;
    public int Tokens;
    public int Coins;

    public Wallet()
    {

    }

    public void Wipe()
    {
        Time = Tokens = Coins = 0;
    }

    public override string ToString()
    {
        return String.Format("Tokens: {0}, Coins: {1}, Time: {2}, ", Tokens, Coins, Time);
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void Load(string json)
    {
        JsonUtility.FromJsonOverwrite(json, this);
    }

    public static Wallet operator+ (Wallet w1, Wallet w2)
    {
        w1.Time += w2.Time;
        w1.Tokens += w2.Tokens;
        w1.Coins += w2.Coins;
        return w1;
    }
}
