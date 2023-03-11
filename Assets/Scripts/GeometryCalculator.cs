using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeometryCalculator
{
    public static float ClosestDifferenceBetweenAnglesOfCircle(float currentAngle, float targetAngle, float minAngleLimit, float maxAngleLimit)
    {
        float distanceToTargetAngleUp, distanceToTargetAngleDown;

        if (targetAngle > currentAngle)
        {
            distanceToTargetAngleUp = targetAngle - currentAngle;
            distanceToTargetAngleDown = (maxAngleLimit - targetAngle) + (currentAngle - minAngleLimit);
        }
        else if (targetAngle < currentAngle)
        {
            distanceToTargetAngleUp = (targetAngle - minAngleLimit) + (maxAngleLimit - currentAngle);
            distanceToTargetAngleDown = currentAngle - targetAngle;
        }
        else
        {
            distanceToTargetAngleUp = 0;
            distanceToTargetAngleDown = 0;
        }

        if (distanceToTargetAngleUp < distanceToTargetAngleDown)
            return distanceToTargetAngleUp;
        else
            return -distanceToTargetAngleDown;
    }
}
