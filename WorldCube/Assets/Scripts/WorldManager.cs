using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
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
    private int[] light_sensor_thresholds = new int[] {-200, 0, 200};
    private int[] heat_sensor_thresholds = new int[] {0, 100, 200};
    //private float water_sensor_cooldown = 0;
    //private float touch_sensor_cooldown = 0;
    private int[] pressure_sensor_thresholds = new int[] {0, 150, 300};
    private int[] sound_sensor_thresholds = new int[] {0, 150, 300};

    //States for combining
    enum sensor_state {
        low    = 0,
        medium = 1,
        high   = 2,
    }
    private sensor_state light_state = sensor_state.medium;
    private sensor_state heat_state = sensor_state.low;
    private sensor_state water_state = sensor_state.low;
    private sensor_state touch_state = sensor_state.low;
    private sensor_state pressure_state = sensor_state.low;
    private sensor_state sound_state = sensor_state.low;

    //VFX for extreme states
    public ParticleSystem[] rainParticleSystems;
    private bool rainOn = false;
    //VFX for persistent effects
    public GameObject go_cube_world;
    public Light vfx_lighting;
    public Material vfx_cold_material;
    public Material vfx_hot_material;

    // Start is called before the first frame update
    void Start()
    {
        //Calibrate sensor values
        light_sensor_calibrated_value = 500;
        light_sensor_read_value = 500;
        heat_sensor_calibrated_value = 200;
        heat_sensor_read_value = 200;

        //Calculate HIGH, MED, and LOW values
    }

    void OnMessageArrived(string msg) {
        //Read sensor values from Serial
        Debug.Log("New message for WorldManager: "+msg);
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

    // Update is called once per frame
    void UpdateValues(string msg)
    {
        //Decypher and update tracked values
        string[] sensor_message = msg.Split(',');
        touch_sensor_read_value = sensor_message[0].Contains("TRUE");
        msg = msg.Remove(0, msg.IndexOf(",")+1);

        string light_sensor_message = sensor_message[1].Substring(msg.IndexOf(":")+1);
        Debug.Log("Parsing from "+light_sensor_message);
        light_sensor_read_value = int.Parse(light_sensor_message);
        msg = msg.Remove(0, msg.IndexOf(",")+1);

        water_sensor_read_value = sensor_message[2].Contains("TRUE");
            
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
            if (water_sensor_read_value && !rainOn)
            {
                rainOn = true;
                rainParticleSystems[0].Play();
            }
            else
            {
                rainOn = false;
                rainParticleSystems[0].Pause();
            }

            /* Replace with new hold effect for touch sensor
            if(touch_sensor_cooldown > 0) {
                touch_sensor_cooldown -= Time.deltaTime;
                //Debug.Log("touch sensor jiggle time left: "+touch_sensor_cooldown);

                //Jiggle
                go_cube_world.transform.localPosition += Time.deltaTime*Mathf.Sin(8*touch_sensor_cooldown)*Vector3.right +
                        Time.deltaTime*Mathf.Cos(18*touch_sensor_cooldown)*Vector3.up;
            } else if(touch_sensor_read_value) {
                touch_sensor_cooldown = Mathf.PI/2;
            } */

            //Persistent VFX
            Debug.Log("Comparing light value of "+light_sensor_read_value+" against calibrated "+light_sensor_calibrated_value);
            float light_ratio = (float)(light_sensor_read_value - light_sensor_calibrated_value) / (light_sensor_thresholds[2] - light_sensor_thresholds[0]);
            light_ratio = Mathf.Max(Mathf.Min(light_ratio, 1), -1);
            vfx_lighting.intensity = light_ratio * 2.5f + 2.5f;

            float heat_ratio = (float)(heat_sensor_read_value - heat_sensor_calibrated_value) / (heat_sensor_thresholds[2] - heat_sensor_thresholds[0]);
            heat_ratio = Mathf.Max(Mathf.Min(heat_ratio, 1), -1);
            Color heat_color = heat_ratio*vfx_hot_material.color + (1-heat_ratio)*vfx_cold_material.color;
            go_cube_world.GetComponent<Renderer>().material.SetColor("_Color", heat_color);
        } else {
            light_sensor_calibrated_value += light_sensor_read_value;
            light_sensor_calibrated_value = light_sensor_calibrated_value/2;

            --sensor_calibration_cycles;
        }
    }
}
