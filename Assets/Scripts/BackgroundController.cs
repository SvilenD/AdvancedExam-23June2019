using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    private const float GroundSize = 3.33f;
    private const float BackgroundSize = 2.8f;
    private const float MinPipeAdvance = 25;
    private const float RandomConst = 0.3f;
    private float pipeAdvance = 27;
    private int backgroundsCount;
    private float backgroundAdvance;
    private int groundsCount;
    private float groundAdvance;

    public void Start()
    {
        this.backgroundsCount = GameObject.FindGameObjectsWithTag("Background").Length;
        this.backgroundAdvance = (this.backgroundsCount - 1) * BackgroundSize;
        this.groundsCount = GameObject.FindGameObjectsWithTag("Ground").Length;
        this.groundAdvance = (this.groundsCount - 1) * GroundSize;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Background"))
        {
            var background = collision.gameObject;
            var backGrPosition = background.transform.position;
            backGrPosition.x += backgroundAdvance;
            background.transform.position = backGrPosition;
        }

        if (collision.CompareTag("Ground"))
        {
            var ground = collision.gameObject;
            var groundPosition = ground.transform.position;
            groundPosition.x += groundAdvance;
            ground.transform.position = groundPosition;
        }

        if (collision.CompareTag("Pipe"))
        {
            var pipe = collision.gameObject;
            var pipePosition = pipe.transform.position;
            pipePosition.x += pipeAdvance;

            var pipeRandomChange = Random.Range(-RandomConst, RandomConst);
            pipePosition.x += pipeRandomChange;
            pipePosition.y += pipeRandomChange / 3;

            pipe.transform.position = pipePosition;

            if (this.pipeAdvance > MinPipeAdvance)
            {
                this.pipeAdvance -= Random.Range(0, RandomConst);
            }
            else
            {
                this.pipeAdvance += 1f;
            }
        }
    }
}