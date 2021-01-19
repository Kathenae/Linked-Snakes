using System.Collections;
using UnityEngine.AI;
using UnityEngine;

public class Level : MonoBehaviour
{

    [SerializeField]
    private Vector2 levelSize;

    private static Level instance;
    public bool showArea;

    void Awake()
    {
        SetInstance(this);
    }

    public static void SetInstance(Level level)
    {
        if (instance != null && instance != level)
        {
            Destroy(level.gameObject);
        }

        instance = level;
    }

    public static Vector3 GetLevelSize()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<Level>();
        }

        return instance.levelSize;
    }

    public static Vector3 GetRandomPosition()
    {
        float x = Random.Range(-instance.levelSize.x * 0.5f, instance.levelSize.x * 0.5f);
        float z = Random.Range(-instance.levelSize.y * 0.5f, instance.levelSize.y * 0.5f);

        Vector3 position = new Vector3(
          instance.transform.position.x + x,
          instance.transform.position.y + 1.05f,
          instance.transform.position.z + z
        );

        instance.RandomPoint(position, 4, out position);

        // Make sure the position is not in a wall
        Ray ray = new Ray(new Vector3(position.x, 10000, position.z), Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitGo = hit.collider.gameObject;

            if (hitGo.tag == "Wall")
            {
                position = GetRandomPosition();
            }

            position.y = hit.point.y;
        }


        // Making sure the position is not in a portal
        SnakePortal closestPortal = FindClosest(position, FindObjectsOfType<SnakePortal>());
        
        if(closestPortal != null)
        {
            float distance = Vector3.Distance(position, closestPortal.transform.position);
            if(distance <= 2)
            {
                position = GetRandomPosition();
            }
        }

        return position;
    }

    public static T FindClosest<T>(Vector3 toPosition, T[] fromList) where T : MonoBehaviour
    {
        float closestDistance = float.MaxValue;
        T closestGo = null;

        foreach (T go in fromList)
        {
            float distance = Vector3.Distance(go.transform.position, toPosition);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestGo = go;
            }
        }

        return closestGo;
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        result = Vector3.zero;
        return false;
    }

    void Start()
    {
        float boundWidth = 1;
        float boundHeight = 4;
        float horizontalBorder = levelSize.x / 2 + boundWidth;
        float verticalBorder = levelSize.y / 2 + boundWidth;

        BoxCollider rightBorder = gameObject.AddComponent<BoxCollider>();
        BoxCollider leftBorder = gameObject.AddComponent<BoxCollider>();
        BoxCollider frontBorder = gameObject.AddComponent<BoxCollider>();
        BoxCollider backBorder = gameObject.AddComponent<BoxCollider>();

        rightBorder.center = Vector3.right * horizontalBorder + Vector3.up * boundHeight * 0.5f;
        rightBorder.size = new Vector3(boundWidth, boundHeight, verticalBorder * 2);

        leftBorder.center = Vector3.left * horizontalBorder + Vector3.up * boundHeight * 0.5f;
        leftBorder.size = new Vector3(boundWidth, boundHeight, verticalBorder * 2);

        frontBorder.center = Vector3.forward * verticalBorder + Vector3.up * boundHeight * 0.5f;
        frontBorder.size = new Vector3(horizontalBorder * 2, boundHeight, boundWidth);

        backBorder.center = Vector3.back * verticalBorder + Vector3.up * boundHeight * 0.5f;
        backBorder.size = new Vector3(horizontalBorder * 2, boundHeight, boundWidth);

    }

    void OnDrawGizmos()
    {
        if (showArea == false)
            return;

        float boundWidth = 1;
        float boundHeight = 4;
        float horizontalBorder = levelSize.x / 2 + boundWidth;
        float verticalBorder = levelSize.y / 2 + boundWidth;

        Gizmos.color = Color.green;
        Gizmos.DrawCube(transform.position + Vector3.right * horizontalBorder + Vector3.up * boundHeight * 0.5f, new Vector3(boundWidth, boundHeight, verticalBorder * 2));
        Gizmos.DrawCube(transform.position + Vector3.right * -horizontalBorder + Vector3.up * boundHeight * 0.5f, new Vector3(boundWidth, boundHeight, verticalBorder * 2));
        Gizmos.DrawCube(transform.position + Vector3.forward * verticalBorder + Vector3.up * boundHeight * 0.5f, new Vector3(horizontalBorder * 2, boundHeight, boundWidth));
        Gizmos.DrawCube(transform.position + Vector3.forward * -verticalBorder + Vector3.up * boundHeight * 0.5f, new Vector3(horizontalBorder * 2, boundHeight, boundWidth));

    }

}
