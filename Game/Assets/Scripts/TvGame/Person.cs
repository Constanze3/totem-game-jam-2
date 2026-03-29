using UnityEngine;

public class TvPerson : MonoBehaviour
{
    public int rageScore = 0;
    public int rageLimit = 100;

    public float rageIncreaseTimer = 1f;

    public bool satisfied = false;

    void Update()
    {
        rageIncreaseTimer += Time.deltaTime;
        // Increase rage score over time if the person is not satisfied
        if (rageIncreaseTimer >= 1f)
        {
            if (!satisfied)
            {
                rageScore += 1;
            }
            rageIncreaseTimer = 0f;
        }
    }

    public void resetRage()
    {
        rageScore = 0;
    }
}
