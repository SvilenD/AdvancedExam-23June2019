using UnityEngine;
using System.Collections.Generic;

public class BirdController : MonoBehaviour
{
    private const int FlapY = 120;
    private const float FwdSpeed = 60;
    private const int MaxFlapSpeed = 40;
    private const float ScoreX = 0.2f;
    private const float ScoreY = 1.8f;

    private Animator animator;
    private Rigidbody2D rBody;
    private bool isStarted;
    private bool hasFlapped;
    private bool isDead;
    private bool gamePaused;
    private int score;

    public GameObject StartObject { get; private set; }
    public GameObject GameOverObject { get; private set; }
    public GameObject[] Digits { get; private set; }
    public List<GameObject> ScoreObjects { get; private set; }

    // Start is called before the first frame update
    public void Start()
    {
        this.animator = this.GetComponent<Animator>();
        this.animator.enabled = false;
        this.rBody = this.GetComponent<Rigidbody2D>();
        this.rBody.gravityScale = 0;
        this.ScoreObjects = new List<GameObject>();
        this.StartObject = GameObject.FindGameObjectWithTag("Start");
        DeActivateObjects();
    }

    // Update is called once per frame
    //read input, change graphics
    public void Update()
    {
        if (this.isStarted == false)
        {
            GetStarted();
        }
        if (this.gamePaused && Input.GetButtonUp("Fire1"))
        {
            this.StartObject.SetActive(false);
            Time.timeScale = 1;
        }

        if (Input.GetButtonDown("Fire1") && isDead == false)
        {
            this.hasFlapped = true;
        }

        if (Input.GetButtonDown("Cancel"))
        {
            PauseGame();
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
        this.gamePaused = true;
        var position = this.rBody.transform.position;
        position.y = 0;
        position.x++;
        this.StartObject.transform.position = position;
        this.StartObject.SetActive(true);
    }

    //apply physics
    public void FixedUpdate()
    {
        if (this.hasFlapped == true && this.rBody.velocity.y < ScoreY)
        {
            this.hasFlapped = false;
            this.rBody.AddForce(new Vector2(0, FlapY));
        }

        if (this.rBody.velocity.y > 0.2)
        {
            this.rBody.MoveRotation(30);
        }
        else if (this.rBody.velocity.y <= 0.2 && this.rBody.velocity.y > -0.1)
        {
            this.rBody.MoveRotation(0);
        }
        else
        {
            this.rBody.MoveRotation(-30);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("Pipe"))
        {
            this.isDead = true;

            var position = this.rBody.position;
            Time.timeScale = 0.1f;

            for (float i = position.y; i >= -1; i -= 0.1f)
            {
                position.y = i;
                this.rBody.position = position;
            }

            this.GameOverObject.transform.position = position;
            this.GameOverObject.SetActive(true);
            this.animator.enabled = false;
            this.score = (int)position.x;
            Time.timeScale = 1;

            //Debug.Log(score);
            if (PlayerPrefs.GetFloat("HighScore") < this.score)
            {
                PlayerPrefs.SetFloat("HighScore", this.score);
            }

            position.y += ScoreY;
            GetScoreObjects();
            
            for (int i = 0; i < this.ScoreObjects.Count; i++)
            {
                position.x += ScoreX;
                this.ScoreObjects[i].transform.position = position;
            }
        }
    }

    private void DeActivateObjects()
    {
        this.GameOverObject = GameObject.FindGameObjectWithTag("Finish");
        GameOverObject.SetActive(false);
        this.Digits = GameObject.FindGameObjectsWithTag("Score");
        var newPsn = this.Digits[0].transform.position;
        newPsn.x -= 10;

        foreach (var item in this.Digits)
        {
            item.transform.position = newPsn;
            item.SetActive(false);
        }
    }

    private void GetStarted()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            this.rBody.gravityScale = 1;
            this.rBody.AddForce(new Vector2(FwdSpeed, 0));
            this.isStarted = true;
            this.animator.enabled = true;

            this.StartObject.SetActive(false);
        }
    }

    private void GetScoreObjects()
    {
        var scoreString = this.score.ToString();

        foreach (var item in this.Digits)
        {
            item.SetActive(true);
        }

        for (int i = 0; i < scoreString.Length; i++)
        {
            var num = scoreString[i];

            var digitObject = GameObject.Find(num.ToString());
            this.ScoreObjects.Add(digitObject);
        }
    }
}