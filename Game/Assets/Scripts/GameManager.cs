using Game;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Player player;

    private static GameManager instance = null;

    public static GameManager Instance => instance;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
}
