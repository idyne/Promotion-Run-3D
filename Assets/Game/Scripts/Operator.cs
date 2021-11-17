using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Dreamteck.Splines;
using FateGames;

public class Operator : MonoBehaviour
{
    [Range(-25, 25)]
    [SerializeField] private float promotionPoint = 10;
    [SerializeField] private int money = 10;
    [SerializeField] private string operatorName;
    [SerializeField] private OperatorColor operatorColor;
    public Operator adjacentOperator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image nameTextBackground;
    [SerializeField] private Transform meshTransform;
    [SerializeField] private ParticleSystem glassBreakEffect;
    private SplinePositioner splinePositioner;
    private BoxCollider coll;

    public BoxCollider Coll { get => coll; }

    private void Awake()
    {
        SetColor();
        SetNameText();
        coll = GetComponentInChildren<BoxCollider>();
        splinePositioner = GetComponent<SplinePositioner>();
        if (splinePositioner)
            splinePositioner.spline = MainLevelManager.Instance.Spline;
    }

    private void SetColor()
    {
        switch (operatorColor)
        {
            case OperatorColor.GREEN:
                spriteRenderer.color = MainLevelManager.Instance.GreenOperatorColor;
                nameTextBackground.color = MainLevelManager.Instance.GreenOperatorColor;
                break;
            case OperatorColor.RED:
                spriteRenderer.color = MainLevelManager.Instance.RedOperatorColor;
                nameTextBackground.color = MainLevelManager.Instance.RedOperatorColor;
                break;
            case OperatorColor.YELLOW:
                spriteRenderer.color = MainLevelManager.Instance.YellowOperatorColor;
                nameTextBackground.color = MainLevelManager.Instance.YellowOperatorColor;
                break;
        }
    }

    public void MakeYellow()
    {
        operatorColor = OperatorColor.YELLOW;
        SetColor();
    }
    public float GetUsed()
    {
        coll.enabled = false;
        if (adjacentOperator)
            adjacentOperator.Coll.enabled = false;
        LevitatingText levitatingText = ObjectPooler.Instance.SpawnFromPool("LevitatingText", meshTransform.position + Vector3.up * 2, transform.rotation).GetComponent<LevitatingText>();
        levitatingText.SetText((promotionPoint >= 0 ? "+" : "-") + "$" + Mathf.Abs(money));
        levitatingText.SetColor(promotionPoint >= 0 ? MainLevelManager.Instance.GreenTextColor : MainLevelManager.Instance.RedTextColor);
        glassBreakEffect.Play();
        nameTextBackground.gameObject.SetActive(false);
        meshTransform.gameObject.SetActive(false);
        spriteRenderer.gameObject.SetActive(false);
        return promotionPoint;
    }
    private void SetNameText()
    {
        nameText.text = operatorName;
    }

    public void StartMeshAnimation()
    {
        meshTransform.LeanRotateY(meshTransform.rotation.eulerAngles.y + 90, 1).setEaseInOutSine().setLoopPingPong();
    }

    private enum OperatorColor { GREEN, YELLOW, RED }
}
