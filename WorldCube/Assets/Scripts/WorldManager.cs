using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    //Sensor variables from IRL
    private int light_sensor_read_value;
    private int heat_sensor_read_value;
    private int water_sensor_read_value;
    private bool touch_sensor_read_value;
    private int pressure_sensor_read_value;
    private int sound_sensor_read_value;

    //Sensor calibrated default values
    private int light_sensor_calibrated_value;
    private int heat_sensor_calibrated_value;
    private int water_sensor_calibrated_value;
    private bool touch_sensor_calibrated_value;
    private int pressure_sensor_calibrated_value;
    private int sound_sensor_calibrated_value;

    //Value thresholds for state changes
    private int threshold_modifier = 25;
    private int[] light_sensor_thresholds = new int[] {-200, 0, 200};
    private int[] heat_sensor_thresholds = new int[] {0, 100, 200};
    private int[] water_sensor_thresholds = new int[] {0, 5, 10};
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
    

    // Start is called before the first frame update
    void Start()
    {
        //Calibrate sensor values

        //Calculate HIGH, MED, and LOW values
    }

    // Update is called once per frame
    void Update()
    {
        if(light_state != sensor_state.high && light_sensor_read_value > light_sensor_calibrated_value + light_sensor_thresholds[2]) {
            Debug.Log("HIGH light state");
            light_state = sensor_state.high;
            for(int i = 0; i < 3; ++i) {
                light_sensor_thresholds[i] += -1*threshold_modifier;
            }
        }
    }
}
