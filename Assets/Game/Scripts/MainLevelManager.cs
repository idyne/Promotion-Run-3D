using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FateGames;
using Dreamteck.Splines;

public class MainLevelManager : LevelManager
{
    public static MainLevelManager Instance { get => (MainLevelManager)_Instance; }
    [SerializeField] private GameObject[] splinePrefabs;
    [SerializeField] private Color greenTextColor, redTextColor;
    [SerializeField] private SplineComputer spline;
    [SerializeField] private Color redOperatorColor, greenOperatorColor, yellowOperatorColor;
    [SerializeField] private Text coinText;
    public float Multiplier = 1;
    public Bonus Bonus;
    public SplineComputer BonusSpline;
    private Player player = null;
    private int coin = 0;
    private Camera mainCamera;
    public Text CoinText { get => coinText; }
    public int Coin { get => coin; }
    public Color RedOperatorColor { get => redOperatorColor; }
    public Color GreenOperatorColor { get => greenOperatorColor; }
    public Color YellowOperatorColor { get => yellowOperatorColor; }
    public SplineComputer Spline { get => spline; }
    public Player Player { get => player; }
    public Color GreenTextColor { get => greenTextColor; }
    public Color RedTextColor { get => redTextColor; }

    private new void Awake()
    {
        base.Awake();
        mainCamera = Camera.main;
        player = FindObjectOfType<Player>();
        spline = Instantiate(splinePrefabs[Random.Range(0, splinePrefabs.Length)]).GetComponent<SplineComputer>();
        
    }

    private void Start()
    {
        string text;
        if (PlayerProgression.COIN < 1000)
            text = PlayerProgression.COIN.ToString();
        else if(PlayerProgression.COIN < 1000000)
            text = (PlayerProgression.COIN / 1000).ToString("0.0") + "K";
        else
            text = (PlayerProgression.COIN / 1000000).ToString("0.0") + "M";
        coinText.text = text;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            AddCoin(5);
        if (Input.GetKeyDown(KeyCode.D))
            FinishLevel(true);
        if (Input.GetKeyDown(KeyCode.F))
            FinishLevel(true);
        //mainCamera.transform.localRotation 
    }
    public override void StartLevel()
    {
        player.ChangeState(Player.CharacterState.WALKING);
    }
    public override void FinishLevel(bool success)
    {
        if (GameManager.Instance.State != GameManager.GameState.FINISHED)
        {
            GameManager.Instance.State = GameManager.GameState.FINISHED;
            // CODE HERE ********
            coin = Mathf.RoundToInt(coin * Multiplier);





            // ******************
            LeanTween.delayedCall(1, () => { GameManager.Instance.FinishLevel(success); });
            //GameManager.Instance.FinishLevel(success);
            
        }
    }

    public void AddCoin(int number)
    {
        coin += number;
    }


}
