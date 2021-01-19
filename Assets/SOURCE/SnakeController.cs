using UnityEngine;

[RequireComponent(typeof(Snake))]
public class SnakeController : MonoBehaviour
{
    Vector3 direction; //The direction this snake is traveling in
    public float speedMultiplier { get; set; }
    Snake snake;

    void Start()
    {
        speedMultiplier = 1;
        snake = GetComponent<Snake>();
    }

    public void Move(Vector3 direction)
    {
        if (direction.x != 0 || direction.z != 0)
            this.direction = direction;
    }

    void FixedUpdate()
    {
        UpdateMovement();
    }

    public void UpdateMovement()
    {
        
        float lookAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        //Turn
        Quaternion newRot = Quaternion.Euler(transform.eulerAngles.x, lookAngle, transform.eulerAngles.z);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRot, snake.speed * Time.deltaTime);

        //Move
        Vector3 velocity;
        velocity = transform.forward * speedMultiplier * snake.speed;
        velocity.y = snake.body.velocity.y;
        snake.body.velocity = velocity;
    }


}