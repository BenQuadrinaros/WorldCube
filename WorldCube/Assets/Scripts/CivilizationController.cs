using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivilizationController : MonoBehaviour
{
    //Resources required for advancing through civilization stages
    private int civilization_stage = 0;
    public ResourceManager resource_manager;
    private int[] required_resource_amounts = new int[] {1, 1, 1, 1, 1};
    private Dictionary<ResourceManager.resource_types, int> civilization_resource_stores;

    //Keep track of the peeps and buildings
    private List<GameObject> peeps;
    //[SerializeField] private 
    private List<GameObject> buildings;
    //[SerializeField] private 

    // Start is called before the first frame update
    void Start()
    {
        //resource_manager = gameObject.GetComponent<ResourceManager>();
        //for(int i = 0; i < 5; ++i) {
        //    civilization_resource_stores.Add((ResourceManager.resource_types)i, 0);
        //}
        
        GameObject[] temp = GameObject.FindGameObjectsWithTag("Peep");
        peeps = new List<GameObject>();
        foreach(GameObject peep in temp) {
            peeps.Add(peep);
        }
        temp = GameObject.FindGameObjectsWithTag("Building");
        buildings = new List<GameObject>();
        foreach(GameObject building in temp) {
            buildings.Add(building);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (civilization_resource_stores[(ResourceManager.resource_types)civilization_stage] > required_resource_amounts[civilization_stage])
        //{
        //    //Upgrade buildings
        //    foreach (GameObject building in buildings)
        //    {

        //    }

        //    civilization_resource_stores[(ResourceManager.resource_types)civilization_stage] -= required_resource_amounts[civilization_stage];
        //    ++civilization_stage;
        //}
    }
}
