using UnityEngine;
using System.Collections;

public class CrowdGenerator : MonoBehaviour
{
    public GameObject agentPrefab;
    public int agentCount = 10;
    public float spawnRadius = 8f;

    public GridManager gridManager;
    public bool useGrid = false;

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
        // Different Simulators depending on the scene
        if (useGrid)
            Simulator.GetInstance().mode = Simulator.SimulationMode.Steering;

        for (int i = 0; i < agentCount; i++)
        {
            Vector3 pos;
            if (useGrid)
            {
                GridCell spawnCell = gridManager.GetRandomFreeCell();
                pos = spawnCell.GetWorldPosition(gridManager.gridOrigin);
            }
            else
                pos = GetValidRandomPosition();

            Agent agent = Instantiate(agentPrefab, pos, Quaternion.identity).GetComponent<Agent>();

            if (useGrid)
            {
                agent.gridManager = FindFirstObjectByType<GridManager>();
                agent.grid = agent.gridManager.grid;
            }
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