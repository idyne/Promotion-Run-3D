using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 3;
    [SerializeField] private Slider promotionBarSlider;
    [SerializeField] private TextMeshProUGUI promotionBarText;
    [SerializeField] private GameObject[] characters;
    [SerializeField] private Transform meshTransform;
    [SerializeField] private ParticleSystem moneyCollectTrail = null;
    private GameObject currentCharacter;
    private Animator currentAnimator;
    [SerializeField] private Animator runningManAnimator1, runningManAnimator2;
    private Rank currentRank = Rank.CLEANING_GIRL;
    private Swerve1D swerve;
    private SplinePositioner splinePositioner;
    private float currentDistance = 5;
    private Vector3 anchor = Vector3.zero;
    private float promotionPoint = 15;
    private Vector3 lastPosition = Vector3.zero;
    private Vector3 meshVelocity = Vector3.zero;
    private CharacterState state = CharacterState.IDLE;
    private ICharacterRotation characterRotation = null;
    private float moneyCollectTrailTime = 0;
    private bool inBonus = false;
    private bool trail = false;
    private int confettiCount = 0;
    #region Unity Callbacks
    private void Awake()
    {
        splinePositioner = GetComponent<SplinePositioner>();
        splinePositioner.spline = MainLevelManager.Instance.Spline;
        swerve = InputManager.CreateSwerve1D(Vector2.right, Screen.width / 1.3f);
        ChangeRank(Rank.CLEANING_GIRL);
        SetPromotionBarSliderValue();
        swerve.OnStart = () => { anchor = meshTransform.transform.localPosition; };
    }

    private void Start()
    {
        splinePositioner.SetDistance(currentDistance);
    }
    private void Update()
    {
        if (GameManager.Instance.State == GameManager.GameState.IN_GAME)
        {
            MoveForward();
            CheckInput();
            FollowSpline();
            SetVelocity();
            RotateCharacter();
            moneyCollectTrailTime -= Time.deltaTime;
            if (moneyCollectTrailTime <= 0 && !moneyCollectTrail.isStopped)
                moneyCollectTrail.Stop();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance.State == GameManager.GameState.IN_GAME)
        {
            if (other.CompareTag("Operator"))
            {
                Operator op = other.GetComponentInParent<Operator>();
                float gain = op.GetUsed();
                AddPromotionPoint(gain);
            }
            else if (other.CompareTag("Collectible"))
            {
                Collectible collectible = other.GetComponentInParent<Collectible>();
                float gain = collectible.GetCollected();
                AddPromotionPoint(gain);
                if (other.GetComponentInParent<Money>())
                {
                    moneyCollectTrailTime = 0.8f;
                    moneyCollectTrail.Play();
                }
            }
        }
    }
    #endregion
    private void CheckInput()
    {
        if (swerve.Active)
            MoveHorizontally(swerve.Rate);
        if (Input.GetKeyDown(KeyCode.G))
            AddPromotionPoint(25);
        if (Input.GetKeyDown(KeyCode.H))
            AddPromotionPoint(-25);
        if (Input.GetKeyDown(KeyCode.J))
            AddPromotionPoint(5);
        if (Input.GetKeyDown(KeyCode.K))
            AddPromotionPoint(-5);
    }
    private void Die()
    {
        ChangeState(CharacterState.DEAD);
        MainLevelManager.Instance.FinishLevel(false);
    }

    public void ChangeState(CharacterState newState)
    {
        state = newState;
        ChangeAnimation();
    }

    private void ChangeAnimation()
    {
        switch (state)
        {
            case CharacterState.IDLE:
                currentAnimator.SetTrigger("IDLE");
                break;
            case CharacterState.WALKING:
                currentAnimator.SetTrigger("WALK");
                break;
            case CharacterState.DEAD:
                currentAnimator.SetTrigger("DIE");
                break;
        }
    }


    private void SetVelocity()
    {
        meshVelocity = (meshTransform.localPosition - lastPosition) / Time.deltaTime;
        lastPosition = meshTransform.localPosition;
    }
    #region Promotion

    private void Promote()
    {
        Rank newRank;
        switch (currentRank)
        {
            case Rank.CLEANING_GIRL:
                newRank = Rank.WAITRESS;
                break;
            case Rank.WAITRESS:
                newRank = Rank.DELIVERY_GIRL;
                break;
            case Rank.DELIVERY_GIRL:
                newRank = Rank.SECRETARY;
                break;
            default:
                newRank = Rank.LADY_BOSS;
                break;
        }
        ObjectPooler.Instance.SpawnFromPool("PromotedText", meshTransform.position + Vector3.up * 2, transform.rotation).transform.parent = meshTransform;
        ChangeRank(newRank);
    }
    private void Demote()
    {
        Rank newRank;
        switch (currentRank)
        {
            case Rank.LADY_BOSS:
                newRank = Rank.SECRETARY;
                break;
            case Rank.SECRETARY:
                newRank = Rank.DELIVERY_GIRL;
                break;
            case Rank.DELIVERY_GIRL:
                newRank = Rank.WAITRESS;
                break;
            default:
                newRank = Rank.CLEANING_GIRL;
                break;
        }
        ObjectPooler.Instance.SpawnFromPool("Demoted" +
            "Text", meshTransform.position + Vector3.up * 2, transform.rotation).transform.parent = meshTransform;
        ChangeRank(newRank);
    }
    private void ChangeRank(Rank newRank)
    {
        currentCharacter?.SetActive(false);
        switch (newRank)
        {
            case Rank.CLEANING_GIRL:
                currentCharacter = characters[0];
                break;
            case Rank.WAITRESS:
                currentCharacter = characters[1];
                break;
            case Rank.DELIVERY_GIRL:
                currentCharacter = characters[2];
                break;
            case Rank.SECRETARY:
                currentCharacter = characters[3];
                break;
            case Rank.LADY_BOSS:
                currentCharacter = characters[4];
                break;
        }
        ObjectPooler.Instance.SpawnFromPool("ChangeRankEffect", meshTransform.position, Quaternion.identity);
        currentRank = newRank;
        currentCharacter.SetActive(true);
        currentAnimator = currentCharacter.GetComponentInChildren<Animator>();
        ChangeAnimation();
        currentCharacter.transform.localScale = Vector3.one * 0.5f;
        currentCharacter.LeanScale(Vector3.one, 1.5f).setEaseOutElastic();
        characterRotation = currentCharacter.GetComponent<ICharacterRotation>();
        SetPromotionBarText();
    }
    private void AddPromotionPoint(float gain)
    {
        if (gain > 0)
            IncreasePromotionPoint(gain);
        else
            ReducePromotionPoint(-gain);
    }
    private void IncreasePromotionPoint(float promotionPointGain)
    {
        float oldPromotionPoint = promotionPoint;
        promotionPoint = Mathf.Clamp(promotionPoint + promotionPointGain, 0, 105);
        SetPromotionBarSliderValue();
        if ((oldPromotionPoint % 25) + promotionPointGain >= 25)
            Promote();
    }
    private void ReducePromotionPoint(float promotionPointLose)
    {
        float oldPromotionPoint = promotionPoint;
        promotionPoint -= promotionPointLose;
        SetPromotionBarSliderValue();
        if (promotionPoint <= 0)
            Die();
        else if ((oldPromotionPoint % 25) - promotionPointLose < 0)
            Demote();
    }
    private void SetPromotionBarText()
    {
        string text = "";
        switch (currentRank)
        {
            case Rank.CLEANING_GIRL:
                text = "Cleaning Girl";
                break;
            case Rank.WAITRESS:
                text = "Waitress";
                break;
            case Rank.DELIVERY_GIRL:
                text = "Delivery Girl";
                break;
            case Rank.SECRETARY:
                text = "Secretary";
                break;
            case Rank.LADY_BOSS:
                text = "Lady Boss";
                break;
        }
        promotionBarText.text = text;
    }
    private void SetPromotionBarSliderValue()
    {
        LeanTween.cancel(promotionBarSlider.gameObject);
        LeanTween.value(promotionBarSlider.gameObject, promotionBarSlider.value, promotionPoint, 0.5f)
            .setOnUpdate((float value) =>
        {
            promotionBarSlider.value = value;
        })
            .setEaseOutElastic();
    }
    #endregion

    #region Movement
    private void FollowSpline()
    {
        if (inBonus)
        {
            if (trail)
            {
                TrailRenderer[] trails = GetComponentsInChildren<TrailRenderer>();
                for (int i = 0; i < trails.Length; i++)
                {
                    trails[i].emitting = true;
                }
                trail = false;
            }

            if (splinePositioner.position > 0.4f)
                MainLevelManager.Instance.Bonus.OpenDoors();

            if (splinePositioner.position > 0.0f && confettiCount == 0 ||
                splinePositioner.position > 0.08f && confettiCount == 1 ||
                splinePositioner.position > 0.16f && confettiCount == 2 ||
                splinePositioner.position > 0.24f && confettiCount == 3 ||
                splinePositioner.position > 0.32f && confettiCount == 4)
            {
                confettiCount++;
                for (int i = 0; i < 2; i++)
                {
                    Transform confetti = MainLevelManager.Instance.Bonus.ConfettiQueue.Dequeue();
                    ConfettiManager.Instance.CreateConfettiDirectional(confetti.position, confetti.rotation.eulerAngles, Vector3.one * 2.5f);
                }
            }
        }
        if (!inBonus && Vector3.Distance(splinePositioner.EvaluatePosition(1.0), transform.position) <= 0.01)
        {
            trail = true;
            TrailRenderer[] trails = GetComponentsInChildren<TrailRenderer>();
            for (int i = 0; i < trails.Length; i++)
            {
                trails[i].emitting = false;
            }
            splinePositioner.spline = MainLevelManager.Instance.BonusSpline;
            currentDistance = 0;
            splinePositioner.RebuildImmediate();
            inBonus = true;
            float dist;
            if (promotionPoint >= 100)
                dist = 97;
            else
                dist = promotionPoint * 0.4f;
            MainLevelManager.Instance.Multiplier = dist > 40 ? 5 : (dist * 3.5f / 40 + 1.5f);
            LeanTween.value(0, dist, 2 * dist / (speed * 1.5f)).setOnUpdate((float val) =>
              {
                  currentDistance = val;
              }).setEaseOutSine().setOnComplete(() =>
              {
                  ChangeState(CharacterState.IDLE);
                  runningManAnimator1.SetTrigger("CLAP");
                  runningManAnimator2.SetTrigger("CLAP");
                  MainLevelManager.Instance.FinishLevel(true);
              });
        }
        splinePositioner.SetDistance(currentDistance);
    }
    private void MoveForward()
    {
        if (!inBonus)
            currentDistance += Time.deltaTime * speed;
    }
    private void MoveHorizontally(float rate)
    {
        if (Mathf.Abs(rate) == 0) return;
        Vector3 desiredPos = meshTransform.transform.localPosition;
        float clampMin = -3.5f;
        float clampMax = 3.5f;
        desiredPos.x = (anchor + Vector3.right * rate * (7f)).x;
        desiredPos.x = Mathf.Clamp(desiredPos.x, clampMin, clampMax);
        if (meshTransform.transform.localPosition.x == clampMin || meshTransform.transform.localPosition.x == clampMax)
        {
            swerve.Reset();
            swerve.OnStart();
        }
        meshTransform.transform.localPosition = desiredPos;
    }

    private void RotateCharacter()
    {
        characterRotation.RotateCharacter(meshVelocity);
    }
    #endregion
    private enum Rank { CLEANING_GIRL, WAITRESS, DELIVERY_GIRL, SECRETARY, LADY_BOSS }
    public enum CharacterState { IDLE, WALKING, DEAD }
}
