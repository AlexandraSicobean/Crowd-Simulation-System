using UnityEngine;
using System.Collections;

public class CrowdGenerator : MonoBehaviour
{
    public GameObject agentPrefab;
    public GridManager gridManager;
    public int agentCount = 10;
    public float spawnRadius = 8f;

    void Start()
    {
        StartCoroutine(DelayedGenerate());
    }

    IEnumerator DelayedGenerate()
    {
        yield return null;

        if (gridManager == null)
            gridManager = FindFirstObjectByType<GridManager>();

        GenerateCrowd();
    }

    void GenerateCrowd()
    {
        for (int i = 0; i < agentCount; i++)
        {
            GridCell spawnCell = gridManager.GetRandomFreeCell();
            Vector3 pos = spawnCell.GetWorldPosition(gridManager.gridOrigin);
            Agent agent = Instantiate(agentPrefab, pos, Quaternion.identity).GetComponent<Agent>();
            agent.gridManager = FindFirstObjectByType<GridManager>();
            agent.grid = agent.gridManager.grid;
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