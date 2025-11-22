using UnityEngine;
using System.Collections.Generic;

public class Simulator : MonoBehaviour
{
    public static Simulator _instance = null;

    public static Simulator GetInstance()
    {
        if (_instance == null)
        {
            GameObject simObj = new GameObject("Simulator");
            _instance = simObj.AddComponent<Simulator>();
        }
        return _instance;
    }

    private List<Agent> agents = new List<Agent>();

    public void AddAgent(Agent a)
    {
        agents.Add(a);
    }

    public void RemoveAgent(Agent a)
    {
        agents.Remove(a);
    }

    void Update()
    {
        float dt = Time.deltaTime;

        foreach (Agent a in agents)
        {
            if (a == null) continue;

            Rigidbody rb = a.GetComponent<Rigidbody>();
            PathManager pm = a.GetComponent<PathManager>();

            Vector3 dir = (pm.goal - a.transform.position);
            dir.y = 0f;
            dir = dir.normalized;

            foreach (Agent other in agents)
            {
                if (other == a) continue;

                Vector3 diff = a.transform.position - other.transform.position;
                float dist = diff.magnitude;

                if (dist < 1.0f)     
                {
                    dir += diff.normalized * (1.0f - dist);
                }
            }

            dir = dir.normalized;

            rb.MovePosition(rb.position + dir * a.speed * dt);

            if (pm.GoalReached())
            {
                pm.AssignNewGoal();
            }
        }
    }
}
