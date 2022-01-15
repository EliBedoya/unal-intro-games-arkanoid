using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerType
{
    MultiBall,
    Long,
    Short,
    Slow,
    Fast
}

public class PowerUp : MonoBehaviour
{
    private const string POWERUP_PATH = "Sprites/PowerUps/PowerUp_{0}";

    [SerializeField] private PowerType _type = PowerType.MultiBall;

    private SpriteRenderer _renderer;
    private Collider2D _collider;
    private Rigidbody2D _rb;
    private ArkanoidController _akc;

    private int _id;

    public void Init(ArkanoidController controller)
    {
        _akc = controller;

        _type = (PowerType) Random.Range(0,4);

        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();

        _collider.enabled = true;

        _renderer = GetComponentInChildren<SpriteRenderer>();
        _renderer.sprite = GetPowerSprite(_type);
    }

    static Sprite GetPowerSprite(PowerType type)
    {
        string path = string.Empty;
        path = string.Format(POWERUP_PATH, type);

        Debug.Log("Se genera: " + path);

        if (string.IsNullOrEmpty(path))
        {
            return null;
        }

        return Resources.Load<Sprite>(path);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
       
        if (other.gameObject.tag == "Paddle")
        {
            ApplyPowerEffect();
            Destroy(this.gameObject);
        }

        if (other.gameObject.tag == "BottomWall")
        {
            Destroy(this.gameObject);
        }

    }

    private void ApplyPowerEffect()
    {
        switch (_type)
        {
            case PowerType.MultiBall:
                MultiBallEffect();
                break;
            //case PowerType.Long:
            //    LongEffect();
            //    break;
            //case PowerType.Short:
                //ShortEffect();
                //break;
            case PowerType.Slow:
                SlowEffect();
                break;
            case PowerType.Fast:
                FastEffect();
                break;
            default:
                break;
        }
    }

    private void MultiBallEffect() {
        for (int i = 0; i < 2; i++)
        {
           _akc.SetNewBall(); 
        }
        Debug.Log("Efecto multibolas");
    }

    private void SlowEffect() {
        _akc.SetBallSpeed(0.5f);
    }

    private void FastEffect() {
        _akc.SetBallSpeed(2.0f);
    } 
}
