using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    private float[] positions;
    private Queue<int> positionIndexes;
    [SerializeField] private float intervalBetweenObjects = 17;
    [SerializeField] private GameObject operatorPairPrefab;
    [SerializeField] private GameObject bonusPrefab;
    [SerializeField] private GameObject[] collectibleSetPrefabs;


    private void Awake()
    {
        positions = new float[Mathf.RoundToInt(MainLevelManager.Instance.Spline.CalculateLength() / intervalBetweenObjects)];
        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = 0.05f + i * (0.95f / positions.Length);
        }
        int[] _positionIndexes = new int[positions.Length];
        for (int i = 0; i < _positionIndexes.Length; i++)
        {
            _positionIndexes[i] = i;
        }
        positionIndexes = new Queue<int>(_positionIndexes.OrderBy(x => Random.value).ToList());
    }

    private void Start()
    {
        GenerateOperatorPairs();
        GenerateCollectibleSets();
        GameObject bonusObject = Instantiate(bonusPrefab, MainLevelManager.Instance.Spline.EvaluatePosition(1.0f), Quaternion.LookRotation(MainLevelManager.Instance.Spline.GetPointPosition(MainLevelManager.Instance.Spline.pointCount - 1) - MainLevelManager.Instance.Spline.GetPointPosition(MainLevelManager.Instance.Spline.pointCount - 2)));
        MainLevelManager.Instance.BonusSpline = bonusObject.GetComponent<SplineComputer>();
        MainLevelManager.Instance.Bonus = bonusObject.GetComponent<Bonus>();

    }
    private void GenerateOperatorPairs()
    {
        for (int i = 0; i < positions.Length / 4; i++)
        {
            GenerateOperatorPair(positionIndexes.Dequeue());
        }

    }

    private void GenerateOperatorPair(int positionIndex)
    {
        OperatorPair operatorPair = Instantiate(operatorPairPrefab, Vector3.up * 20, Quaternion.identity).GetComponent<OperatorPair>();
        operatorPair.SplinePositioner.SetPercent(positions[positionIndex]);
    }

    private void GenerateCollectibleSets()
    {
        for (int i = 0; i < positionIndexes.Count; i++)
        {
            GenerateCollectibleSet(positionIndexes.Dequeue());
        }
    }

    private void GenerateCollectibleSet(int positionIndex)
    {
        CollectibleSet collectibleSet = Instantiate(collectibleSetPrefabs[Random.Range(0, collectibleSetPrefabs.Length)], Vector3.up * 20, Quaternion.identity).GetComponent<CollectibleSet>();
        collectibleSet.SetSplinePercent(positions[positionIndex]);
    }
}
