using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data model for player, interfaced directly to the playerprefs for now
/// </summary>
public class GamePlayer {
    /// <summary>
    /// for testing
    /// </summary>
    public void Wipe()
    {
        Name = "";
        Level = 1;
        Stage = 1;
        wallet.Wipe();
    }

    public GamePlayer()
    {
        wallet = new Wallet();
    }

    const string KEY_Name = "HoL-Name";
    const string KEY_Level = "HoL-Level";
    const string KEY_Stage = "HoL-Stage";
    const string KEY_Points = "HoL-Points";
    const string KEY_WalletJson = "HoL-Wallet";
    private string mName;
    public string Name
    {
        get
        {
            return mName;
        }
        set
        {
            mName = value;
        }
    }
    private int mLevel;
    public int Level
    {
        get
        {
            return mLevel;
        }
        set
        {
            mLevel = value;
        }
    }
    private int mStage;
    public int Stage
    {
        get
        {
            return mStage;
        }
        set
        {
            mStage = value;
        }
    }

    private string mWalletJson;
    public Wallet wallet { get; set; }

    public void Push()
    {
        PlayerPrefs.SetString(KEY_Name, mName);
        PlayerPrefs.SetInt(KEY_Level, mLevel);
        PlayerPrefs.SetInt(KEY_Stage, mStage);
        mWalletJson = wallet.ToJson();
        PlayerPrefs.SetString(KEY_WalletJson, mWalletJson);
    }

    public void Pull()
    {
        mName = GetValue<string>(KEY_Name);
        mLevel = GetValue<int>(KEY_Level);
        mStage = GetValue<int>(KEY_Stage);
        mWalletJson = GetValue<string>(KEY_WalletJson);
        wallet.Load(mWalletJson);
    }

    private T GetValue<T>(string key)
    {
        if (!PlayerPrefs.HasKey(key))
        {
            return default(T);
        }
        var typeName = typeof(T).Name;
        object o;
        switch (typeName)
        {
            case "String":
                o = PlayerPrefs.GetString(key);
                break;
            case "Int":
            case "Int32":
                o = PlayerPrefs.GetInt(key);
                break;
            case "Float":
                o = PlayerPrefs.GetFloat(key);
                break;
            default:
                throw new System.Exception(string.Format("Invalid Type for PlayerPrefs {0}", typeName));
        }
        if (o == null)
        {
            return default(T);
        }
        else
        {
            return (T)o;
        }
    }
}
