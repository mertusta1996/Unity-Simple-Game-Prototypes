public abstract class SpeedEffect
{
    public static float CalculateSpeedEffectValue(float velocityMagnitude, float maxSpeed, float minSpeedEffect, float maxSpeedEffect)
    {
        return maxSpeedEffect - velocityMagnitude / maxSpeed * (maxSpeedEffect - minSpeedEffect);
    }
}
