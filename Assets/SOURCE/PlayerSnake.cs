using UnityEngine;

[RequireComponent(typeof(SnakeController))]
public class PlayerSnake : Snake
{

    SnakeController controller;

    public MovementControls moveInput;

    bool canPlay;

    protected override void Awake()
    {
        base.Awake();
        controller = GetComponent<SnakeController>();
        controller.enabled = false;
        GameEvents.gameStart += StartPlay;
        GameEvents.gamePaused += PausePlay;
    }

    void OnDestroy()
    {
        GameEvents.gameStart -= StartPlay;
        GameEvents.gamePaused -= PausePlay;
    }

    void StartPlay()
    {
        canPlay = true;
        controller.enabled = true;
    }

    void PausePlay(bool pauseState)
    {
        canPlay = pauseState == false;
        controller.enabled = pauseState == false;
    }

    void Update()
    {
        if (canPlay == false)
            return;

        //Movement
        controller.Move(new Vector3(moveInput.Horizontal, 0, moveInput.Vertical));

        //Eating
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Pull();
        }

    }

    [System.Serializable]
    public struct MovementControls
    {
        public KeyCode Up;
        public KeyCode Down;
        public KeyCode Left;
        public KeyCode Right;

        public float Horizontal
        {
            get
            {
                return Input.GetKey(Left) ? -1 : Input.GetKey(Right) ? 1 : 0;
            }
        }

        public float Vertical
        {
            get
            {
                return Input.GetKey(Down) ? -1 : Input.GetKey(Up) ? 1 : 0;
            }
        }

    }

}