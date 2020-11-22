#include "Wrapper.h"

PlayerMetrics metrics;

PLUGIN_API void ResetMetrics()
{
	metrics.ResetMetrics();
}

PLUGIN_API int GetNumWallRuns()
{
	return metrics.GetNumWallRuns();
}

PLUGIN_API float GetDamageDealt()
{
	return metrics.GetDamageDealt();
}

PLUGIN_API float GetCompletedTime()
{
	return metrics.GetCompletedTime();
}

PLUGIN_API float GetWeaponAccuracy()
{
	return metrics.GetWeaponAccuracy();
}

PLUGIN_API void IncrementNumWallRuns()
{
	metrics.IncrementNumWallRuns();
}

PLUGIN_API void AddDamageDealt(float a_damageToAdd)
{
	metrics.AddDamageDealt(a_damageToAdd);
}

PLUGIN_API void SetCompletedTime(float a_timeCompleted)
{
	metrics.SetCompletedTime(a_timeCompleted);
}

PLUGIN_API void ProcessWeaponFired(bool a_wasHit)
{
	metrics.ProcessWeaponFired(a_wasHit);
}