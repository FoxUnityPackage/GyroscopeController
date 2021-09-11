using System;
using UnityEngine;
public class GyroscopicMovementLinker : MonoBehaviour
{
    [Header("Settings")]
    [Range(0f, 2f), Tooltip("Rotation in degres for 1 degres in gyroscope")]
    public float rotationByGyroRot = 1f;

    [Range(0f, 1f), Tooltip("This value will be used for lerp to prevent shake caused by gyroscope sensibility")]
    public float smoothness = 0f;

    [Header("Constraints")]
    public bool lockX = false;
    public bool lockY = false;
    public bool lockZ = false;

    Quaternion mGyroStart;

    void Start()
    {
        if (!SystemInfo.supportsGyroscope)
        {
            Debug.Log("Gyroscope unsupported");
            enabled = false;
        }
        else
        {
            Input.gyro.enabled = true;
            ResetGyro();
        }
    }

    Quaternion DiffQuat(Quaternion from, Quaternion to)
    {
        return Quaternion.Inverse(from) * to;
    }

    public void ResetGyro()
    {
        mGyroStart = Input.gyro.attitude;
    }

    void Update()
    {
        // Get the difference between start angle and current angle because user want rotate from it's current phone orientation
        Quaternion rotFromStartToCurrent = DiffQuat(mGyroStart, Input.gyro.attitude);

        Vector3 axis;
        float angleDeg;
        rotFromStartToCurrent.ToAngleAxis(out angleDeg, out axis);
        axis = new Vector3(lockX ? 0f : -axis.x, lockY ? 0f : axis.z, lockZ ? 0f : -axis.y);
        angleDeg *= rotationByGyroRot;

        // Limit rotation sensibility
        Quaternion angle = Quaternion.AngleAxis(angleDeg, Vector3.Normalize(axis));

        // Apply the result
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, angle, smoothness);
    }
}
