using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BezierSolution
{
    [AddComponentMenu("Bezier Solution/Rail Manager")]
    public class RailManager : MonoBehaviour
    {
        public static RailManager Instance;

        private void Awake()
        {
            Instance = this;
        }

        public GameObject railPrefab;
        public GameObject emptyPrefab;
        private GameObject currentRailA;
        private GameObject currentRailB;
        private bool duo = false;
        public GameObject railWay;

        //public BezierSpline[][] bezierRailList;

        public int railsCount = 10;
        // Start is called before the first frame update
        void Start()
        {
            for (int i = 0; i < railsCount; i++)
            {
                SpawnRail(i, railPrefab, emptyPrefab);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SpawnRail(int i, GameObject railPrefabA, GameObject railPrefabB)
        {
            currentRailA = Instantiate(railPrefabA, currentRailA.transform.GetChild(0).position, Quaternion.identity);
            currentRailA.transform.parent = railWay.transform;
            //bezierRailList[i][0] = currentRailA.GetComponentsInChildren<BezierSpline>()[0];
            Debug.Log("currentRailA.GetComponentsInChildren<BezierSpline>()[0]:" + currentRailA.GetComponentsInChildren<BezierSpline>()[0] + " \n");
            //Debug.Log("bezierRailList:" + bezierRailList + " \n");

            if (!duo)
            {
                currentRailB = Instantiate(railPrefabB, currentRailA.transform.GetChild(0).position, Quaternion.identity);
                duo = true;
            }
            else
            {
                currentRailB = Instantiate(railPrefabB, currentRailB.transform.GetChild(0).position, Quaternion.identity);
            }
            currentRailB.transform.parent = railWay.transform;
            //bezierRailList[i][1] = currentRailB.GetComponentsInChildren<BezierSpline>()[0];
        }

        private GameObject Instantiate(GameObject railPrefabA, Vector3 position, Quaternion identity)
        {
            throw new NotImplementedException();
        }
    }
}