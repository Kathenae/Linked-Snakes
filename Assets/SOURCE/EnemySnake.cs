using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(SnakeController))]
public class EnemySnake : Snake
{
    public float shootDelay = 0.5f;
    float nextTongueShootTime;

    NavMeshPath path;
    Vector3 currentDestination;
    int currentWaypontIndex = 1;

    SnakeController controller;

    protected override void Start()
    {
        base.Start();

        controller = GetComponent<SnakeController>();

        path = new NavMeshPath();
        currentDestination = Level.GetRandomPosition();
        Recalculatepath();

    }

    protected override void Awake()
    {
        base.Awake();
    }


    void Update()
    {
        //Process Moving
        Vector3 myPosition = new Vector3(
          transform.position.x,
          path.corners[currentWaypontIndex].y,
          transform.position.z
        );

        Vector3 movementDirection = (path.corners[currentWaypontIndex] - myPosition);
        controller.Move(movementDirection.normalized);

        if (movementDirection.sqrMagnitude <= 0.5f)
        {
            currentWaypontIndex += 1;

            //If we've reached our current Destination
            if (currentWaypontIndex >= path.corners.Length)
            {
                ChooseDestination();
                Recalculatepath();
            }

        }

        //Process Shooting
        if (Time.time >= nextTongueShootTime)
        {
            nextTongueShootTime = Time.time + shootDelay;
            Pull();
        }

    }

    float collisionDuration;
    void OnCollisionStay(Collision c)
    {

        if (c.collider.gameObject.tag != "Wall")
            return;

        // TODO: Check if we're actually stuck
        // Right now we're just assuming we're stuck if we collide for more than some seconds
        // We could check if we're actually stuck by checking the distance from the position at 
        // collision enter to the position some seconds after collision stay
        // But this might also not always tell us since we might be sliding in a wall or something similar

        collisionDuration += Time.deltaTime;
        // Debug.Log("Stuck for " + collisionDuration);

        if (collisionDuration >= 1)
        {
            // Debug.Log("Path deviation detected, recalculating path");
            Recalculatepath();
            collisionDuration = 0;
        }

    }

    public void Recalculatepath()
    {
        NavMesh.CalculatePath(transform.position, currentDestination, 1, path);
        currentWaypontIndex = 1;

        // Check if the path is invalid or unusable
        // NOTE: Since the level is currenlty always returning a position inside the navmesh this might never run
        // But i am keeping it anyway in case the'res something i did not account for (There's always something)
        int tryCount = 30;
        while (path.status == NavMeshPathStatus.PathPartial || path.status == NavMeshPathStatus.PathInvalid && tryCount > 0)
        {
            Debug.LogError("Path not usable, recalculating");
            ChooseDestination();
            NavMesh.CalculatePath(transform.position, currentDestination, 1, path);
            tryCount--;
        }

    }

    // Chooses Between Going to a random point in the world or 
    // Going to a random Collectible in the world
    void ChooseDestination()
    {
        float r = Random.Range(0f, 2f);

        if (r <= 0.5f)
        {
            currentDestination = Level.GetRandomPosition();
            return;
        }

        if (r <= 2f)
        {
            GameObject obj = Spawner.instance.GetRandomSpawnedObject();

            if (obj == null)
            {
                // Notthing spawned yet
                currentDestination = Level.GetRandomPosition();
                return;
            }

            currentDestination = obj.transform.position;
            return;
        }

    }

    void OnDrawGizmos()
    {
        if (path == null)
            return;

        Gizmos.color = Color.green;

        Gizmos.DrawLine(transform.position, path.corners[currentWaypontIndex]);

        for (int i = currentWaypontIndex; i < path.corners.Length - 1; i++)
        {
            Gizmos.DrawWireCube(path.corners[i], Vector3.one * 0.5f);
            Gizmos.DrawLine(path.corners[i], path.corners[i + 1]);
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(currentDestination, Vector3.one * 0.5f);

    }

    public void OnRenderObject()
    {
        //if (path == null)
        //    return;

        //GLUtilities.DrawLine(transform.position, path.corners[currentWaypontIndex],Color.green);

        //for (int i = currentWaypontIndex; i < path.corners.Length - 1; i++)
        //{
        //    GLUtilities.DrawLine(path.corners[i], path.corners[i + 1], Color.green);
        //}
    }

}