using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperatorPair : MonoBehaviour
{
    [SerializeField] private List<GameObject> goodOperators;
    [SerializeField] private List<GameObject> badOperators;
    private SplinePositioner splinePositioner;

    public SplinePositioner SplinePositioner { get => splinePositioner; }

    private void Awake()
    {
        splinePositioner = GetComponent<SplinePositioner>();
        if (splinePositioner)
            splinePositioner.spline = MainLevelManager.Instance.Spline;
    }
    private void Start()
    {
        GameObject firstOperator, secondOperator;
        if (Random.value < 0.5f)
        {
            firstOperator = goodOperators[Random.Range(0, goodOperators.Count)];
            goodOperators.Remove(firstOperator);
            secondOperator = badOperators[Random.Range(0, badOperators.Count)];
            badOperators.Remove(secondOperator);
        }
        else
        {
            secondOperator = goodOperators[Random.Range(0, goodOperators.Count)];
            goodOperators.Remove(secondOperator);
            firstOperator = badOperators[Random.Range(0, badOperators.Count)];
            badOperators.Remove(firstOperator);
        }
        Operator firstObject = Instantiate(firstOperator, transform).GetComponent<Operator>();
        Operator secondObject = Instantiate(secondOperator, transform).GetComponent<Operator>();
        firstObject.transform.localPosition = Vector3.right * -2;
        secondObject.transform.localPosition = Vector3.right * 2;
        firstObject.adjacentOperator = secondObject;
        secondObject.adjacentOperator = firstObject;
        LeanTween.delayedCall(0.2f, () =>
        {
            firstObject.StartMeshAnimation();
            secondObject.StartMeshAnimation();
        });

        if (Random.value < 0.2f)
        {
            firstObject.MakeYellow();
            secondObject.MakeYellow();
        }
    }
}
