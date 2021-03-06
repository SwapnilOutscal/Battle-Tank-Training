﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BTScriptableObject;
using Common;
using System;
using UI;

namespace AchievementM
{
    public class AchievementManager : Singleton<AchievementManager>
    {
        [SerializeField]
        private AchievementScriptable achievementScriptable;

        private AchievementController hiScoreController;
        private AchievementController gamesPlayedController;
        private AchievementController enemiesKilledController;

        public event Action<AchievementType, int> AchievementCheck;
        public event Action<string> AchievementUI;

        // Use this for initialization
        void Start()
        {
            GameUI.InstanceClass.ScoreIncreased += ScoreIncreased;
            Enemy.EnemyManager.Instance.EnemyDestroyed += EnemyKilled;
            BTManager.GameManager.Instance.GameStarted += GamesPlayed;
            SaveLoad.SaveLoadManager.Instance.AchievementUnlocked += CallAchievementUI;


            if (achievementScriptable != null)
            {
                hiScoreController       = new AchievementController(achievementScriptable, AchievementType.hiScore);
                gamesPlayedController   = new AchievementController(achievementScriptable, AchievementType.gamesPlayed);
                enemiesKilledController = new AchievementController(achievementScriptable, AchievementType.enemyKilled);
            }
            else 
            {
                Debug.Log("[Achievement Manager] missing scriptable object reference");
            }

        }

        private void ScoreIncreased()
        {
            hiScoreController.CheckAchievement(UIManager.Instance.playerScore);
        }

        private void EnemyKilled()
        {
            enemiesKilledController.CheckAchievement(Enemy.EnemyManager.Instance.enemiesKilled);
        }

        private void GamesPlayed()
        {
            gamesPlayedController.CheckAchievement(BTManager.GameManager.Instance.gamesPlayed);
        }

        public void CheckAchievement(AchievementType achievementType, int achievementIndex)
        {
            AchievementCheck?.Invoke(achievementType, achievementIndex);
        }

        private void CallAchievementUI(AchievementType achievementType, int achievementIndex)
        {
            for (int i = 0; i < achievementScriptable.achievements.Count; i++)
            {
                if(achievementScriptable.achievements[i].achievementType == achievementType)
                {
                    string value = "Unlocked " + achievementType.ToString() + " Achievement. Title " +
                                          achievementScriptable.achievements[i].achievementInfo[achievementIndex].achievementTitle;
                            AchievementUI?.Invoke(value);
                }
            }
        }

    }
}