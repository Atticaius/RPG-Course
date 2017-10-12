using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour
{

    private Paddle paddle;

    private Vector3 paddleToBallVector;
    public static bool hasStarted = false;

    // NEW Variables for storing default ball and paddle positions
    private Vector3 defaultBallPosition;
    private Vector3 defaultPaddlePosition;

    // Use this for initialization
    public void Start ()
    {
        paddle = GameObject.FindObjectOfType<Paddle>();
        paddleToBallVector = this.transform.position - paddle.transform.position;

        // NEW These set the default positions of the ball and paddle
        defaultBallPosition = transform.position;
        defaultPaddlePosition = paddle.transform.position;
    }

    // NEW This resets the default positions
    public void ResetBall()
    {
        hasStarted = false;
        paddle.transform.position = defaultPaddlePosition;
        transform.position = defaultBallPosition;
        
    }

    // Update is called once per frame
    public void Update ()
    {
        if (!hasStarted)
        {
            // Lock the ball relative to the paddle.
            this.transform.position = paddle.transform.position + paddleToBallVector;

            // Wait for a mouse press to launch.
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Pressed left click, launch ball.");

                hasStarted = true;
                this.rigidbody2D.velocity = new Vector2(Random.Range(7.6f, -7.6f), Random.Range(4.9f, 7.6f));
            }
        }

    }

    void OnCollisionEnter2D (Collision2D collision)
    {

        Vector2 tweak = new Vector2(Random.Range(0.21f, 0.26f), Random.Range(0.21f, 0.26f));

        if (hasStarted && collision.gameObject.tag != "Breakable")
        {
            audio.Play();
            rigidbody2D.velocity += tweak;
        }

    }

}
