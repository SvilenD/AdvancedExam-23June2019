using UnityEngine;

public class BirdController : MonoBehaviour
{
    private const int flapY = 120;
    private const float fwdSpeed = 60;
    private const int maxFlapSpeed = 40;
    private Animator animator;
    private Rigidbody2D rBody;

    private bool isStarted;
    private bool hasFlapped;
    private bool isDead;

    public GameObject GameOverObject { get; private set; }

    // Start is called before the first frame update
    public void Start()
    {
        //Debug.Log(PlayerPrefs.GetFloat("HighScore"));
        this.animator = this.GetComponent<Animator>();
        this.animator.enabled = false;
        this.rBody = this.GetComponent<Rigidbody2D>();
        this.rBody.gravityScale = 0;
        this.GameOverObject = GameObject.FindGameObjectWithTag("Finish");
        GameOverObject.SetActive(false);
    }

    // Update is called once per frame
    //read input, change graphics
    public void Update()
    {
        if (this.isStarted == false)
        {
            GetStarted();
        }

        if (Input.GetButtonDown("Fire1") && isDead == false)
        {
            this.hasFlapped = true;
        }
    }

    //apply physics
    public void FixedUpdate()
    {
        if (this.hasFlapped == true && this.rBody.velocity.y < maxFlapSpeed)
        {
            this.hasFlapped = false;
            this.rBody.AddForce(new Vector2(0, flapY));
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
            for (float i = position.y; i >= -1; i -= 0.1f)
            {
                Time.timeScale = 0.1f;
                position.y = i;
                this.rBody.position = position;
            }
            this.animator.enabled = false;
            var updatedPsn = this.rBody.position;
            updatedPsn.y++; 
            this.GameOverObject.transform.position = updatedPsn;
            this.GameOverObject.SetActive(true);

            var score = this.rBody.position.x;

            if (PlayerPrefs.GetFloat("HighScore") < score)
            {
                PlayerPrefs.SetFloat("HighScore", score);
            }
        }
    }

    private void GetStarted()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            this.rBody.gravityScale = 1;
            this.rBody.AddForce(new Vector2(fwdSpeed, 0));
            this.isStarted = true;
            this.animator.enabled = true;

            var startObject = GameObject.FindGameObjectWithTag("Start");
            startObject.SetActive(false);
        }
    }
}