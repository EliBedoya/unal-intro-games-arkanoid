using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArkanoidController : MonoBehaviour
{
    private const string BALL_PREFAB_PATH = "Prefabs/Ball";
    private const string POWERUP_PREFAB_PATH = "Prefabs/PowerUp";
    private readonly Vector2 BALL_INIT_POSITION = new Vector2(0, -0.86f);

    [SerializeField]
    private GridController _gridController;

    [Space(20)]
    [SerializeField]
    private List<LevelData> _levels = new List<LevelData>();

    private int _currentLevel = 0;

    private Ball _ballPrefab = null;
    private List<Ball> _balls = new List<Ball>();

    private PowerUp _powerPrefab = null;

    private int _totalScore = 0;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            InitGame();
        }
    }

    private void InitGame()
    {
        _currentLevel = 0;
        _totalScore = 0;
        _gridController.BuildGrid(_levels[0]);
        SetInitialBall();
    }

    private void SetInitialBall()
    {
        ClearBalls();

        Ball ball = CreateBallAt(BALL_INIT_POSITION);
        ball.Init();
        _balls.Add(ball);
    }

    public void SetNewBall()
    {
        if (_balls.Count >= 3)
            return;

        Ball ball = CreateBallAt(BALL_INIT_POSITION);
        ball.Init();
        _balls.Add(ball);
    }

    private Ball CreateBallAt(Vector2 position)
    {
        if (_ballPrefab == null)
        {
            _ballPrefab = Resources.Load<Ball>(BALL_PREFAB_PATH);
        }

        return Instantiate(_ballPrefab, position, Quaternion.identity);
    }

    private void ClearBalls()
    {
        for (int i = _balls.Count - 1; i >= 0; i--)
        {
            _balls[i].gameObject.SetActive(false);
            Destroy(_balls[i]);
        }

        _balls.Clear();
    }

    private void Start()
    {
        ArkanoidEvent.OnBallReachDeadZoneEvent += OnBallReachDeadZone;
        ArkanoidEvent.OnBlockDestroyedEvent += OnBlockDestroyed;
    }

    private void OnDestroy()
    {
        ArkanoidEvent.OnBallReachDeadZoneEvent -= OnBallReachDeadZone;
        ArkanoidEvent.OnBlockDestroyedEvent -= OnBlockDestroyed;
    }

    private void OnBallReachDeadZone(Ball ball)
    {
        ball.Hide();
        _balls.Remove(ball);
        Destroy(ball.gameObject);

        CheckGameOver();
    }

    private void CheckGameOver()
    {
        //Game over
        if (_balls.Count == 0)
        {
            ClearBalls();

            Debug.Log("Game Over: LOSE!!!");
        }
    }

    private void OnBlockDestroyed(int blockId)
    {
        BlockTile blockDestroyed = _gridController.GetBlockBy(blockId);
        if (blockDestroyed != null)
        {
            _totalScore += blockDestroyed.Score;

            // Spawn PowerUp
            if (Random.value < 0.5f)
            {
                Vector2 blockLocation = blockDestroyed.transform.position;
                SetNewPower(blockLocation);
            }

        }

        if (_gridController.GetBlocksActive() == 0)
        {
            _currentLevel++;
            if (_currentLevel >= _levels.Count)
            {
                ClearBalls();
                Debug.LogError("Game Over: WIN!!!!");
            }
            else
            {
                SetInitialBall();
                _gridController.BuildGrid(_levels[_currentLevel]);
            }

        }
    }

    public void SetNewPower(Vector2 powerPosition)
    {
        PowerUp powerup = CreatePowerAt(powerPosition);
        powerup.Init(this);
    }

    private PowerUp CreatePowerAt(Vector2 position)
    {
        if (_powerPrefab == null)
        {
            _powerPrefab = Resources.Load<PowerUp>(POWERUP_PREFAB_PATH);
        }
        if (_powerPrefab == null)
        {
            Debug.Log("Null charge");
        }

        return Instantiate(_powerPrefab, position, Quaternion.identity);
    }

     public void SetBallSpeed(float multiplier)
    {
        for (int i = _balls.Count - 1; i >= 0; i--)
        {
            _balls[i].gameObject.GetComponent<Rigidbody2D>().velocity *= multiplier;
        }

        _balls.Clear();
    }
}