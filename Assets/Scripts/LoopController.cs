using UnityEngine;

public class LoopController : MonoBehaviour
{
    public GameObject bird;
    private float offsetX;

    // Start is called before the first frame update
    public void Start()
    {
        this.bird = GameObject.FindGameObjectWithTag("Player");
        this.offsetX = this.transform.position.x - this.bird.transform.position.x;
    }

    // Update is called once per frame
    public void Update()
    {
        var position = this.transform.position;
        position.x = this.bird.transform.position.x + this.offsetX;
        this.transform.position = position;
    }
}