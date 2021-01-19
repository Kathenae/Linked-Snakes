using System;
using UnityEngine;

public class GameEvents
{
    public static Action<Snake,GameObject> collectibleEaten;
    public static Action<Snake,SnakeNode> nodeStolen;
    public static Action<Snake> snakeDeath;

    public static Action gameStart;
    public static Action<bool> gamePaused;
    public static Action gameLost;

    public static void TriggerNodeStolen(Snake stealer, SnakeNode node)
    {
        if (nodeStolen != null)
            nodeStolen(stealer, node);
    }

    public static void TriggerCollectibleEaten(Snake eater, GameObject collectible)
    {
        if (collectibleEaten != null)
            collectibleEaten(eater, collectible);
    }

    public static void TriggerSnakeDeath(Snake deceased)
    {
        if (snakeDeath != null)
            snakeDeath(deceased);
    }

    public static void TriggerGameStart()
    {
        if (gameStart != null)
            gameStart();
    } 

    public static void TriggerGamePause(bool state)
    {
        if (gamePaused != null)
            gamePaused(state);
    }

}
