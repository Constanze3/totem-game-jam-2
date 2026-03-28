using UnityEngine;

public class Person : MonoBehaviour
{
    public int rage_score = 0;
    public int rage_limit = 100;

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
                rage_score += 1;
            }
            rageIncreaseTimer = 0f;
        }
    }

    public void resetRage()
    {
        rage_score = 0;
    }
}
