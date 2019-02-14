﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;

namespace Enemy
{
    public class PetrolView : EnemyBaseStateView
    {
        [SerializeField] private EnemyView enemyView;

        private List<Vector3> wayPointList;

        private int currentWayPointIndex;

        private Vector3 currentTargetPos;

        [SerializeField]
        private float moveLimit;

        private int replayIndexCOunt = 0;

        protected override void OnEnable()
        {
            base.OnEnable();

            enemyView.ChaseState.enabled = false;

            Debug.Log(this.name + " is in PetrolState");
            wayPointList = new List<Vector3>();

            foreach (var wayPoint in FindObjectsOfType<WayPoints>())
            {
                wayPointList.Add(wayPoint.GetComponent<Transform>().position);
            }

            if (GameManager.Instance.currentState.gameStateType == StateMachine.GameStateType.Game)
            {
                SelectWayPoint();
            }
            else if (GameManager.Instance.currentState.gameStateType == StateMachine.GameStateType.Replay)
            {
                enemyView.Agent.destination = enemyView.GetEnemyController().EnemyData.wayPoints[replayIndexCOunt];
                currentTargetPos = enemyView.GetEnemyController().EnemyData.wayPoints[replayIndexCOunt];
                replayIndexCOunt++;
                if (replayIndexCOunt > enemyView.GetEnemyController().EnemyData.wayPoints.Count)
                    replayIndexCOunt = 0;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        void SelectWayPoint()
        {
            int r = Random.Range(0, wayPointList.Count);
            while (r == currentWayPointIndex)
            {
                r = Random.Range(0, wayPointList.Count);
            }
            currentWayPointIndex = r;
            enemyView.Agent.destination = wayPointList[currentWayPointIndex];
            currentTargetPos = wayPointList[currentWayPointIndex];;
            if (GameManager.Instance.currentState.gameStateType == StateMachine.GameStateType.Game)
            {
                enemyView.SetEnemyData(wayPointList[currentWayPointIndex]);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (GameManager.Instance.currentState.gameStateType == StateMachine.GameStateType.Pause) return;

            if (wayPointList.Count == 0) return;

            if (Vector3.Distance(currentTargetPos, transform.position) < 1f)
            {
                Debug.Log("[PetrolView] Target Changed 1");

                if (GameManager.Instance.currentState.gameStateType == StateMachine.GameStateType.Game)
                {
                    Debug.Log("[PetrolView] Target Changed");
                    SelectWayPoint();
                }
                else if (GameManager.Instance.currentState.gameStateType == StateMachine.GameStateType.Replay)
                {
                    enemyView.Agent.destination = enemyView.GetEnemyController().EnemyData.wayPoints[replayIndexCOunt];
                    currentTargetPos = enemyView.GetEnemyController().EnemyData.wayPoints[replayIndexCOunt];
                    Debug.Log("[PetrolView] Target Changed:" + currentTargetPos);
                    replayIndexCOunt++;
                    if (replayIndexCOunt > enemyView.GetEnemyController().EnemyData.wayPoints.Count)
                        replayIndexCOunt = 0;
                }
            }
        }
    }
}