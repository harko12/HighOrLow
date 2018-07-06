﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rand = UnityEngine.Random;
using System.Linq;
using System;

[ExecuteInEditMode]
public class MissionGenerator : MonoBehaviour {
    public MissionSettings missionSettings;

    private static MissionGenerator mInstance;

    public static MissionGenerator GetInstance()
    {
        return mInstance;
    }

    private void Awake()
    {
        mInstance = this;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static Mission[] GenerateMissions_old(GamePlayer p, GameStage stage)
    {
        /* need to come up with some rules, based on player's level and 
         * current score, and such as to what kind of missions to generate. 
         * For now, I'll do one of each
         */
        var m1 = new Mission()
        {
            StageInfo = stage,
            MissionType = MissionTypes.Sprint,
            TotalSeconds = 30f,
            Rounds = 15,
            CanWager = false,
            PrizePurse = new Wallet() { Time = 30, Coins = 1, Tokens = 10 }
        };

        var m2 = new Mission()
        {
            StageInfo = stage,
            MissionType = MissionTypes.Sprint,
            TotalSeconds = 60f,
            Rounds = 20,
            Chances = 1,
            CanWager = false,
            PrizePurse = new Wallet() { Time = 30, Coins = 1, Tokens = 10 }
        };

        var m3 = new Mission()
        {
            StageInfo = stage,
            MissionType = MissionTypes.Survival,
            Rounds = 0,
            TotalSeconds = 5f,
            BaseRecoverySeconds = 1f, // milliseconds, I think
            CanWager = false,
            PrizePurse = new Wallet() { Time = 0, Coins = 0, Tokens = 10 }
        };

        var m4 = new Mission()
        {
            StageInfo = stage,
            MissionType = MissionTypes.ByRound,
            Rounds = 10,
            CanWager = false,
            PrizePurse = new Wallet() { Time = 30, Coins = 1, Tokens = 10 }
        };

        return new Mission[] { m1, m2, m3, m4 };
    }

    public void AdjustValue(ref float value, float percent, float min = 0, float max = 1)
    {
        var minDiff = value - min;
        var maxDiff = max - value;

        var adjustment = maxDiff;

        if (percent < 0)
        {
            adjustment = minDiff * -1;
        }

        adjustment = adjustment * Mathf.Abs(percent);
        value += adjustment;

    }

    public Mission[] GenerateMissions(GamePlayer p, GameStage stage, int totalMissions)
    {
        float levelMultiplier = stage.Level - p.Level;
        var lcv = 0;
        var missionTypePool = new List<MissionTypes>();
        var generatedMissions = new List<Mission>();
        while (lcv < totalMissions)
        {
            if (missionTypePool.Count() < 1)
            {
                missionTypePool = Enum.GetValues(typeof(MissionTypes)).Cast<MissionTypes>().ToList();
            }
            var type = missionTypePool[Rand.Range(0, missionTypePool.Count())];
            bool canWager = Rand.Range(0f, 1f) > missionSettings.WagerChance;

            var m = GenerateBaseMission(type, stage);
            var difficultyAdjust = (Rand.Range(-.75f, .75f));
            if (canWager)
            {
                // for betting, always get harder
                difficultyAdjust = Rand.Range(.5f, .9f);
            }
            m.CanWager = canWager;
            m.AdjustDifficulty(difficultyAdjust);
            // figure out purse
            m.PrizePurse = new Wallet() { Time = 30, Coins = 1, Tokens = 10 };
            lcv++;
            missionTypePool.Remove(type);
            generatedMissions.Add(m);
        }

        return generatedMissions.ToArray();
    }

    public Mission GenerateBaseMission(MissionTypes missionType, GameStage s)
    {
        var settings = missionSettings;
        var query = settings.Scenarios.Where(mission => mission.MissionType == missionType);
        if (!query.Any())
        {
            Debug.LogErrorFormat("Incorrect MissionSettings.  No Scenarios for {0}", missionType);
            return null;
        }

        var count = query.Count();
        var m = query.ElementAt(Rand.Range(0, count));

        var newMission = new Mission()
        {
            StageInfo = s,
            MissionType = m.MissionType,
            TotalSeconds = m.TotalSeconds,
            BaseRecoverySeconds = m.BaseRecoverySeconds,
            Rounds = m.Rounds,
            Chances = m.Chances
        };

        return newMission;
    }
}
