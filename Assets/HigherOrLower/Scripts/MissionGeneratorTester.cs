using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighOrLow
{


    [ExecuteInEditMode]
    public class MissionGeneratorTester : MonoBehaviour
    {

        public int playerlevel;
        public GameStage TestStage;
        private GamePlayer testPlayer = new GamePlayer();
        public MissionPanel myMissionPanel;
        public MissionGenerator myGenerator;
        public int missionCount;
        public MissionTypes testMissionType;
        [Range(-10, 10)]
        public float testDifficulty = 0;
        private Mission testMission;
        [SerializeField]
        public Mission difficultyTestMission;
        public float difficultyTestMissionValue;

        public void GenerateMissions()
        {
            testPlayer.Level = playerlevel;
            var missions = myGenerator.GenerateMissions(testPlayer, TestStage, missionCount);
            if (Application.isPlaying)
            {
                myMissionPanel.ClearMissions();
                myMissionPanel.UpdateMissions(testPlayer, missions);
            }
        }

        public void GenerateSingleMission()
        {
            testPlayer.Level = playerlevel;
            testMission = myGenerator.GenerateBaseMission(testMissionType, TestStage);
        }

        public void CalculateMissionDifficulty()
        {
            difficultyTestMission.StageInfo = TestStage;
            difficultyTestMissionValue = difficultyTestMission.Difficulty();
        }
        public float targetDifficulty;
        public void AdjustMissionDifficulty()
        {
            difficultyTestMission.StageInfo = TestStage;
            difficultyTestMission.AdjustDifficulty(targetDifficulty);
            difficultyTestMissionValue = difficultyTestMission.Difficulty();
        }

        private void OnGUI()
        {
            GUI.contentColor = Color.white;
            var message = "waiting..";
            if (testMission != null)
            {
                message = string.Format("Difficulty: {0}\n", testDifficulty);
                message = string.Format("Mission Difficulty: {0}\n", difficultyTestMissionValue);
                message += testMission.Description(true);
            }
            GUI.Label(new Rect(10f, 30f, Screen.width, 200f), message);
        }

    }
}