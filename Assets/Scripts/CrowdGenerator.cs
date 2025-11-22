using UnityEngine;

public class CrowdGenerator : MonoBehaviour
{
    public GameObject agentPrefab;
    public int agentCount = 10;
    public float spawnRadius = 8f;

    void Start()
    {
        GenerateCrowd();
    }

    void GenerateCrowd()
    {
        for (int i = 0; i < agentCount; i++)
        {
            Vector3 pos = GetValidRandomPosition();
            Agent agent = Instantiate(agentPrefab, pos, Quaternion.identity).GetComponent<Agent>();
            Simulator.GetInstance().AddAgent(agent);
        }
    }

    Vector3 GetValidRandomPosition()
    {
        for (int attempt = 0; attempt < 50; attempt++)
        {
            Vector3 pos = new Vector3(
                Random.Range(-spawnRadius, spawnRadius),
                0f,
                Random.Range(-spawnRadius, spawnRadius)
            );

            Collider[] hits = Physics.OverlapSphere(pos, 0.6f);

            if (hits.Length == 0)
                return pos;
        }

        return new Vector3(
            Random.Range(-spawnRadius, spawnRadius),
            0f,
            Random.Range(-spawnRadius, spawnRadius)
        );
    }

}