using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroscopeCommunication : MonoBehaviour
{
    /*
     *  1) Serial control through Unity: https://ardity.dwilches.com/
     * 
     *  2) Processing based code translated to Unity C#:
     *  
     *  Processing Accelerometer and Gyroscope:
     *  
     *  A processing sketch that reads the rotation values sent over serial via the MPU 6050.
     *  Then rotates a 3D box based on those values.
     *  Assumes that the values sent are ASCII numbers using .println() on the Arduino side.
     *  If .write() is used by the Arduino then different logic would be necessary to read the binary values.
     *
     *  Created by Eddie Melcer for the Foundations of Alternative Controller Games course.
     *
     *  This example code is available under Creative Commons Attribution 4.0 International Public License.
     */

    // The number of sensor values that arduino will send via serial
    const int NUMBER_VALUES = 3;

    // The delimiter string between sensor values in a single message
    string delimiter = ",";

    // Stores the values of the arduino readings via serial once we parse them out of the string message
    float[] values = new float[NUMBER_VALUES];

    // A simple threshold to reduce jitter/bounce from the MPU 6050's rotation values
    float rotationSensitivityThreshold = 0.01f;

    // The x, y, and z rotation of the box
    float[] boxRotation = new float[3];

    // Setting anything with the sketch or baudrate is handled in Unity or Ardity

    // Each reading value is delimited by a ','
    // Order of sensor values is: MPU_6050_Orientation_X, MPU_6050_Orientation_Y, MPU_6050_Orientation_Z

    // IAN NOTE: Serial Controller handles the function being called here
    // Invoked when a line of data is recieved from the serial device.
    void OnMessageArrived(string msg)
    {
        //print("Recieved " + msg);
        GetValuesFromString(msg);
    }

    // Invoked when a connect/disconnect event occurs. The parameter 'success'
    // will be 'true' upon connection, and 'false' upon disconnection or
    // failure to connect.
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

    // Eddie's string parsing functionality from previous referenced code
    void GetValuesFromString(string message)
    {
        // Now that we have values represented as a string, we need to convert them to floats
        // Sometimes the string doesn't come through right via serial or there's a timeout on the read so make sure it's not null or empty before trying to process
        if (message != null && message.Trim() != "")
        {
            // If we have what seems to be a valid mesage, use split to separate the readings on the delimiter
            string[] readings = message.Split(delimiter);

            // Make sure we have the right number of read values from our string (i.e., that the string wasn't partially corrupted)
            if (readings.Length == NUMBER_VALUES)
            {
                // Then go through each reading and convert it to a float, storing it in the values array and updating the corresponding box rotation
                for (int i = 0; i < NUMBER_VALUES; i++)
                {
                    // Cast the string's value to a float to get our reading value from Arduino
                    // Don't forget to trim the string first or extra whitespace will cause an error
                    try
                    {
                        // Use parseFloat to case a string to a float
                        //print(readings[i].Trim());
                        values[i] = float.Parse(readings[i].Trim());
                        //print("READING (" + i + "): " + values[i]);

                        // Once we have the new rotation value, do a check to prevent jittering
                        // I.e., make sure the difference between the current box rotation and the new rotation we just received is greater than the rotationSensitivityThreshold
                        if (Mathf.Abs(values[i] - boxRotation[i]) > rotationSensitivityThreshold)
                        {
                            // Store the new rotation value in the boxRotation array
                            boxRotation[i] = values[i];
                        }
                        // If the conversion failed, print out what the reading was that cause the error for debugging purposes
                        // Then carry on as normal
                    }
                    catch (System.Exception e)
                    {
                        print("Error converting reading (" + i + ") -> " + readings[i] + " to float");
                    }
                }
            }
        }

        // Send values to the in-game cube
        CubeBehavior.Instance.setCubePosition(values);
        CubeBehavior.Instance.setCubeRotation(boxRotation);
    }
}
