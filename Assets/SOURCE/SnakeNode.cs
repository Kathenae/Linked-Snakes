using System;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class SnakeNode : MonoBehaviour
{
    public float Speed
    {
        get
        {
            return snake.speed;
        }
    }

    public Snake snake { get; set; }
    public SnakeNode previous { get; set; }
    public SnakeNode next { get; set; }

    public Rigidbody body { get; protected set; }

    public GameObject target { get; set; }

    protected virtual void Start()
    {
        body = GetComponent<Rigidbody>();

        if (previous != null)
            target = previous.gameObject;
    }


    void FixedUpdate()
    {
        if (previous != null)
        {
            FollowPrevious();
        }
    }

    void FollowPrevious()
    {
        if (target == null)
            target = previous.gameObject;

        Vector3 direction = target.transform.position - transform.position;
        float distance = direction.magnitude;

        if (distance > 0.35f)
        {
            Move(direction);
        }
        else
        {
            Move(Vector3.zero);
        }

    }

    void Move(Vector3 direction)
    {
        body.velocity = direction.normalized * Speed;
    }

    public void SwapPosition(Vector3 position)
    {
        if (next != null)
        {
            Vector3 nextPosition = next.transform.position;
            next.transform.position = position;
            next.SwapPosition(nextPosition);
        }
    }

    public void Unlink(Action OnSnakeDeath)
    {
        if (snake == null)
            return;

        snake.TakeDamage(OnSnakeDeath);
        snake.UnlinkNode(this);

    }
}