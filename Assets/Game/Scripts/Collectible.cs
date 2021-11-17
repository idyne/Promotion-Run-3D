using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;


public class Collectible : MonoBehaviour
{
    [SerializeField] private bool turn = false;
    [SerializeField] private Vector3 meshPosition = Vector3.zero;
    [SerializeField] private ParticleSystem effect;
    [SerializeField] private string collectEffectName = "";
    [Range(-25, 25)]
    [SerializeField] private float promotionPointGain = 5;
    [SerializeField] private int money = 15;
    [SerializeField] private Transform meshParentTransform;
    [SerializeField] private Transform meshTransform;
    [SerializeField] private BoxCollider coll;
    private SplinePositioner splinePositioner;

    public SplinePositioner SplinePositioner { get => splinePositioner; }

    private void Awake()
    {
        splinePositioner = GetComponent<SplinePositioner>();
        if (splinePositioner)
            splinePositioner.spline = MainLevelManager.Instance.Spline;
        if (turn)
            TurnManager.Instance.AddTransformToTurn(meshTransform);
        meshPosition *= 0.75f;
        meshPosition.z *= 2;
        meshParentTransform.localPosition = meshPosition;
    }

    public float GetCollected()
    {
        coll.enabled = false;
        meshTransform.LeanScale(Vector3.zero, 0.5f).setEaseOutBounce();
        if (effect)
            effect.Stop();
        ObjectPooler.Instance.SpawnFromPool(collectEffectName, meshTransform.position, Quaternion.identity);
        LevitatingText levitatingText = ObjectPooler.Instance.SpawnFromPool("LevitatingText", meshTransform.position + Vector3.up * 2, transform.rotation).GetComponent<LevitatingText>();
        levitatingText.SetText((promotionPointGain >= 0 ? "+" : "-") + "$" + Mathf.Abs(money));
        levitatingText.SetColor(promotionPointGain >= 0 ? MainLevelManager.Instance.GreenTextColor : MainLevelManager.Instance.RedTextColor);
        if (promotionPointGain >= 0)
            MainLevelManager.Instance.AddCoin(money);
        else
            MainLevelManager.Instance.AddCoin(-money);
        return promotionPointGain;
    }
}
