using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public static WorldManager instance;
    //Sensor variables from IRL
    public int light_sensor_read_value;
    public int heat_sensor_read_value;
    public bool water_sensor_read_value;
    public bool touch_sensor_read_value;
    public int pressure_sensor_read_value;
    public int sound_sensor_read_value;

    //Sensor calibrated default values
    private int sensor_calibration_cycles = 15;
    private int light_sensor_calibrated_value;
    private int heat_sensor_calibrated_value;
    private int pressure_sensor_calibrated_value;
    private int sound_sensor_calibrated_value;

    //Value thresholds for state changes
    private int threshold_modifier = 25;
    private int[] light_sensor_thresholds = new int[] {-200, 0, 50};
    private int[] heat_sensor_thresholds = new int[] {0, 20, 40};
    //private float water_sensor_cooldown = 0;
    private float touch_sensor_cooldown = 0;
    private int[] pressure_sensor_thresholds = new int[] {0, 150, 300};
    private int[] sound_sensor_thresholds = new int[] {0, 150, 300};

    //States for combining
    public enum sensor_state {
        low    = 0,
        medium = 1,
        high   = 2,
    }
    public sensor_state light_state = sensor_state.medium;
    public sensor_state heat_state = sensor_state.low;
    public sensor_state water_state = sensor_state.low;
    public sensor_state touch_state = sensor_state.low;
    public sensor_state pressure_state = sensor_state.low;
    public sensor_state sound_state = sensor_state.low;

    //VFX for extreme states
    public ParticleSystem[] rainParticleSystems;
    private bool rainOn = false;
    //VFX for persistent effects
    [SerializeField] private GameObject go_cube_world;
    private ResourceManager resource_manager;
    private CivilizationController civ_controller;
    [SerializeField] private Light vfx_lighting;
    [SerializeField] private Material vfx_cold_material;
    [SerializeField] private Material vfx_hot_material;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Calibrate sensor values
        light_sensor_calibrated_value = 500;
        light_sensor_read_value = 500;
        heat_sensor_calibrated_value = 200;
        heat_sensor_read_value = 200;

        //Set references
        resource_manager = go_cube_world.GetComponent<ResourceManager>();
        civ_controller = go_cube_world.GetComponent<CivilizationController>();

        //Calculate HIGH, MED, and LOW values
    }

    // Update is called once per frame
    void Update() {
        if(touch_sensor_cooldown > 0) {
            touch_sensor_cooldown -= Time.deltaTime;
            //Debug.Log("touch sensor jiggle time left: "+touch_sensor_cooldown);

            if(touch_sensor_cooldown < 0) {
                go_cube_world.transform.localPosition = Vector3.zero;
            } else {
                //Jiggle
                go_cube_world.transform.localPosition += 5*Time.deltaTime*Mathf.Sin(8*touch_sensor_cooldown)*Vector3.right +
                        5*Time.deltaTime*Mathf.Cos(20*touch_sensor_cooldown)*Vector3.up;
            }
        }
    }

    void OnMessageArrived(string msg) {
        //Read sensor values from Serial
        //Debug.Log("New message for WorldManager: "+msg);
        UpdateValues(msg);
    }

    void OnConnectionEvent(bool success)
    {
        if (success) {
            print("Connected");
        }
        else
        {
            print("Disconnected");
        }
    }

    void UpdateValues(string msg)
    {
        //Decypher and update tracked values
        string[] sensor_message = msg.Split(',');
        touch_sensor_read_value = sensor_message[0].Contains("TRUE");

        string light_sensor_message = sensor_message[1].Substring(msg.IndexOf(":")+1);
        //Debug.Log("Parsing light from "+light_sensor_message);
        light_sensor_read_value = int.Parse(light_sensor_message);

        water_sensor_read_value = sensor_message[2].Contains("TRUE");

        string heat_sensor_message = sensor_message[3].Substring(msg.IndexOf(":"));
        //Debug.Log("Parsing heat from "+heat_sensor_message);
        heat_sensor_read_value = int.Parse(heat_sensor_message);
            
        if(sensor_calibration_cycles == 0) {
            //State-based actions
            if(light_state == sensor_state.high) {
                //Display appropriate VFX
                //Debug.Log("HIGH light state");

                //Check for state changes
                if(light_sensor_read_value < light_sensor_calibrated_value + light_sensor_thresholds[1]) {
                    light_state = sensor_state.medium;
                    for(int i = 0; i < 3; ++i) {
                        //Adjust thresholds to prevent flickering
                        light_sensor_thresholds[i] += threshold_modifier;
                    }
                }
            } else if(light_state == sensor_state.low) {
                //Display appropriate VFX
                //Debug.Log("LOW light state");

                //Check for state changes
                if(light_sensor_read_value > light_sensor_calibrated_value + light_sensor_thresholds[1]) {
                    light_state = sensor_state.low;
                    for(int i = 0; i < 3; ++i) {
                        //Adjust thresholds to prevent flickering
                        light_sensor_thresholds[i] -= threshold_modifier;
                    }
                }
            } else {
                //Display appropriate VFX
                //Debug.Log("MED light state");

                //Check for state changes
                if(light_sensor_read_value > light_sensor_calibrated_value + light_sensor_thresholds[2]) {
                    light_state = sensor_state.high;
                    for(int i = 0; i < 3; ++i) {
                        //Adjust thresholds to prevent flickering
                        light_sensor_thresholds[i] -= threshold_modifier;
                    }
                } else if(light_sensor_read_value < light_sensor_calibrated_value + light_sensor_thresholds[0]) {
                    light_state = sensor_state.low;
                    for(int i = 0; i < 3; ++i) {
                        //Adjust thresholds to prevent flickering
                        light_sensor_thresholds[i] += threshold_modifier;
                    }
                }
            }

            if(heat_state == sensor_state.high) {
                //Display appropriate VFX
                //Debug.Log("HIGH heat state");

                //Check for state changes
                if(heat_sensor_read_value < heat_sensor_calibrated_value + heat_sensor_thresholds[1]) {
                    heat_state = sensor_state.medium;
                    for(int i = 0; i < 3; ++i) {
                        //Adjust thresholds to prevent flickering
                        heat_sensor_thresholds[i] += threshold_modifier;
                    }
                }
            } else if(heat_state == sensor_state.low) {
                //Display appropriate VFX
                //Debug.Log("LOW heat state");

                //Check for state changes
                if(heat_sensor_read_value > heat_sensor_calibrated_value + heat_sensor_thresholds[1]) {
                    heat_state = sensor_state.low;
                    for(int i = 0; i < 3; ++i) {
                        //Adjust thresholds to prevent flickering
                        heat_sensor_thresholds[i] -= threshold_modifier;
                    }
                }
            } else {
                //Display appropriate VFX
                //Debug.Log("MED heat state");

                //Check for state changes
                if(heat_sensor_read_value > heat_sensor_calibrated_value + heat_sensor_thresholds[2]) {
                    heat_state = sensor_state.high;
                    for(int i = 0; i < 3; ++i) {
                        //Adjust thresholds to prevent flickering
                        heat_sensor_thresholds[i] -= threshold_modifier;
                    }
                } else if(heat_sensor_read_value < heat_sensor_calibrated_value + heat_sensor_thresholds[0]) {
                    heat_state = sensor_state.low;
                    for(int i = 0; i < 3; ++i) {
                        //Adjust thresholds to prevent flickering
                        heat_sensor_thresholds[i] += threshold_modifier;
                    }
                }
            }

            // Test Water Sensor in scene
            //if (Input.GetKeyDown("e"))
            //{
            //    water_sensor_read_value = true;
            //}
            //if (Input.GetKeyDown("q"))
            //{
            //    water_sensor_read_value = false;
            //}

            // Water State
            if (water_sensor_read_value)
            {
                if(!rainOn) {
                    rainOn = true;
                    rainParticleSystems[0].Play();
                }
            }
            else
            {
                rainOn = false;
                rainParticleSystems[0].Pause();
                rainParticleSystems[0].Clear();
            }

            // Replace with new hold effect for touch sensor
            if(touch_sensor_cooldown <= 0 && touch_sensor_read_value) {
                //Debug.Log("TOuch sensor read");
                touch_sensor_cooldown = Mathf.PI/4;
            } 

            //Persistent VFX
            //Debug.Log("Comparing light value of "+light_sensor_read_value+" against calibrated "+light_sensor_calibrated_value);
            float light_ratio = (float)(light_sensor_read_value - light_sensor_calibrated_value) / (light_sensor_thresholds[2] - light_sensor_thresholds[0]);
            light_ratio = Mathf.Max(Mathf.Min(light_ratio, 1), -1);
            vfx_lighting.intensity = light_ratio * 2.5f + 2.5f;

            float heat_ratio = (float)(heat_sensor_read_value - heat_sensor_calibrated_value) / (heat_sensor_thresholds[2] - heat_sensor_thresholds[0]);
            heat_ratio = Mathf.Max(Mathf.Min(heat_ratio, 1), -1);
            Color heat_color = heat_ratio*vfx_hot_material.color + (1-heat_ratio)*vfx_cold_material.color;
            go_cube_world.GetComponent<Renderer>().material.SetColor("_Color", heat_color);

            resource_manager.UpdateValues(this);
        } else {
            light_sensor_calibrated_value += light_sensor_read_value;
            light_sensor_calibrated_value = light_sensor_calibrated_value/2;

            heat_sensor_calibrated_value += heat_sensor_read_value;
            heat_sensor_calibrated_value = heat_sensor_calibrated_value/2;

            --sensor_calibration_cycles;
        }
    }

    public void Recalibrate_Sensors() {
        sensor_calibration_cycles = 15;
    }
}
