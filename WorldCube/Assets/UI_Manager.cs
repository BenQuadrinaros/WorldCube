using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Manager : MonoBehaviour
{
    //UI element references
    public GameObject go_sensor_readout_panel;
    private TextMeshProUGUI te_sensor_values;

    //Scene element references
    public WorldManager world_manager;
    public GameObject go_cube_world;

    // Start is called before the first frame update
    void Start()
    {
        te_sensor_values = go_sensor_readout_panel.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        te_sensor_values.text = "Light Level: "+world_manager.light_sensor_read_value+"\nHeat Level: "+world_manager.heat_sensor_read_value;
    }
}
