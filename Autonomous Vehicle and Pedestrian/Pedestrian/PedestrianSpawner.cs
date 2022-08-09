using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianSpawner : MonoBehaviour
{
    public GameObject pedestrianPrefab;
    public int pedestrianToSpawn;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        int count = 0;
        while( count < pedestrianToSpawn)
        {
            
            Transform child = transform.GetChild(Random.Range(0, transform.childCount -1));
            GameObject obj = Instantiate(pedestrianPrefab,child.position,child.rotation);
            obj.GetComponent<WaypointNavigator>().currentWaypoint = child.GetComponent<Waypoint>();
            obj.GetComponent<UnityEngine.AI.NavMeshAgent>().speed=Random.Range(0.4f,1.5f);
            count++;
        }
        yield return new WaitForEndOfFrame();
        
    }

}
