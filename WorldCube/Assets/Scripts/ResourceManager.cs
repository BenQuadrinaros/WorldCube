using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    //Different resource types
    public enum resource_types {
        water   = 0,
        crops   = 1,
        wood    = 2,
        metal   = 3,
        animals = 4,
    }
    private float lake_fill_amount = 0;
    private int plains_growth_stage = 0;
    private int forest_growth_stage = 0;
    private int mountain_growth_stage = 0;
    private int swamp_growth_stage = 0;

    //Rates and timers for advancements
    [SerializeField] private float lake_fill_rate = 0.15f;
    [SerializeField] private float plains_growth_timer_max = 2.5f;
    private float plains_growth_timer;
    [SerializeField] private float forest_growth_timer_max = 3f;
    private float forest_growth_timer;
    [SerializeField] private float mountain_growth_timer_max = 4.5f;
    private float mountain_growth_timer;
    [SerializeField] private float swamp_growth_timer_max = 5f;
    private float swamp_growth_timer;

    //Track all the sides that can produce resources
    public Transform location_buildings;
    public Transform location_lake;
    public Transform location_plains;
    public Transform location_forest;
    public Transform location_mountain;
    public Transform location_swamp;

    // Start is called before the first frame update
    void Start()
    {
        plains_growth_timer = plains_growth_timer_max;
        forest_growth_timer = forest_growth_timer_max;
        mountain_growth_timer = mountain_growth_timer_max;
        swamp_growth_timer = swamp_growth_timer_max;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateValues(WorldManager world_state) {
        bool lake_updated = false;
        bool plains_updated = false;
        bool forest_updated = false;
        bool mountain_updated = false;
        bool swamp_updated = false;

        if((int)world_state.light_state == 2) {
            if(plains_growth_stage == 1) {
                plains_updated = true;
                plains_growth_timer -= Time.deltaTime;
                if(plains_growth_stage < 0) {
                    ++plains_growth_stage;
                    plains_growth_timer = plains_growth_timer_max;
                }
            }
            if(forest_growth_stage == 1) {
                forest_updated = true;
                forest_growth_timer -= Time.deltaTime;
                if(forest_growth_stage < 0) {
                    ++forest_growth_stage;
                    forest_growth_timer = forest_growth_timer_max;
                }
            }
            lake_updated = true;
            lake_fill_amount = Mathf.Max(0f, lake_fill_amount - lake_fill_rate*Time.deltaTime);
        }
        if((int)world_state.heat_state == 2) {
            if(mountain_growth_stage == 1) {
                mountain_updated = true;
                mountain_growth_timer -= Time.deltaTime;
                if(mountain_growth_timer < 0) {
                    ++mountain_growth_stage;
                    mountain_growth_timer = mountain_growth_timer_max;
                }
            }
        }
        if((int)world_state.water_state == 2) {
            if(plains_growth_stage == 0) {
                plains_updated = true;
                plains_growth_timer -= Time.deltaTime;
                if(plains_growth_stage < 0) {
                    ++plains_growth_stage;
                    plains_growth_timer = plains_growth_timer_max;
                }
            }
            if(forest_growth_stage == 0) {
                forest_updated = true;
                forest_growth_timer -= Time.deltaTime;
                if(forest_growth_stage < 0) {
                    ++forest_growth_stage;
                    forest_growth_timer = forest_growth_timer_max;
                }
            }
            lake_updated = true;
            lake_fill_amount = Mathf.Min(1f, lake_fill_amount + lake_fill_rate*Time.deltaTime);
        }

        //Automatic decay
        if(!lake_updated) { lake_fill_amount = Mathf.Max(0f, lake_fill_amount - 0.5f*lake_fill_rate*Time.deltaTime); }
        if(!plains_updated) { plains_growth_timer = Mathf.Min(plains_growth_timer_max, plains_growth_timer + Time.deltaTime); }
        if(!forest_updated) { forest_growth_timer = Mathf.Min(forest_growth_timer_max, forest_growth_timer + Time.deltaTime); }
        if(!mountain_updated) { mountain_growth_timer = Mathf.Min(mountain_growth_timer_max, mountain_growth_timer + Time.deltaTime); }
        if(!swamp_updated) { swamp_growth_timer = Mathf.Min(swamp_growth_timer_max, swamp_growth_timer + Time.deltaTime); }
    }
}
