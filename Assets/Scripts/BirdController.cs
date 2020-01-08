using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.IO;
using System.Collections.Generic;

public class BirdController : MonoBehaviour
{
    private const int FlapY = 120;
    private const float FwdSpeed = 60;
    //private const int MaxFlapSpeed = 40;
    private const float ScoreY = 1.8f;
    private const string DateTimeFormat = "dd/MM/yyyy - hh:mm";
    private const string HighScoresPath = "highscores.txt";

    private Animator animator;
    private Rigidbody2D rBody;
    private bool isStarted;
    private bool hasScore;
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
        this.rBody = this.GetComponent<Rigidbody2D>();
        this.rBody.gravityScale = 0;
        this.ScoreObjects = new List<GameObject>();
        this.StartObject = GameObject.FindGameObjectWithTag("Start");
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
        if (isDead && Input.GetButtonDown("Fire1"))
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
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
            var position = this.rBody.position;
            this.isDead = true;
            Time.timeScale = 0.1f;

            for (float i = position.y; i >= -1; i -= 0.1f)
            {
                position.y = i;
                this.rBody.position = position;
                this.rBody.MoveRotation(-30);
            }

            this.animator.enabled = false;
            if (hasScore == false)
            {
                this.score = (int)position.x;
                GetScoreObjects(position);
                this.hasScore = true;

                Debug.Log(score);
                if (PlayerPrefs.GetFloat("HighScore") < this.score)
                {
                    PlayerPrefs.SetFloat("HighScore", this.score);
                }

                var dateTime = DateTime.Now.ToString(DateTimeFormat);
                using (StreamWriter writer = (File.Exists(HighScoresPath)) ? File.AppendText(HighScoresPath) : File.CreateText(HighScoresPath))
                {
                    writer.WriteLine($"{dateTime} -> Score = {score}");
                }
            }

            position.y += 1.5f;
            this.GameOverObject.transform.position = position;
            this.GameOverObject.SetActive(true);

            Reset();
        }
    }

    private void DeActivateObjects()
    {
        this.GameOverObject = GameObject.FindGameObjectWithTag("Finish");
        GameOverObject.SetActive(false);

        this.StartObject.SetActive(false);
    }

    private void GetStarted()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            this.rBody.gravityScale = 1;
            Time.timeScale = 1;
            this.rBody.AddForce(new Vector2(FwdSpeed, 0));
            this.isStarted = true;
            DeActivateObjects();
        }
    }

    private void GetScoreObjects(Vector2 position)
    {
        var scoreString = this.score.ToString();

        for (int i = 0; i < scoreString.Length; i++)
        {
            var num = scoreString[i];

            var digitObject = Instantiate(GameObject.Find(num.ToString()));

            this.ScoreObjects.Add(digitObject);
        }

        var scoreObjPsn = position;
        scoreObjPsn.y++;

        foreach (var item in this.ScoreObjects)
        {
            scoreObjPsn.x += 0.2f;
            item.transform.position = scoreObjPsn;
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
        this.gamePaused = true;
        var position = this.rBody.transform.position;
        position.y--;
        this.StartObject.transform.position = position;
        this.StartObject.SetActive(true);
    }

    //reset game
    private void Reset()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}