#pragma once
#include "PlayerMetrics.h"

#ifdef __cplusplus
extern "C"
{
#endif

	PLUGIN_API void ResetMetrics();
	
	PLUGIN_API int   GetNumWallRuns();
	PLUGIN_API float GetDamageDealt();
	PLUGIN_API float GetCompletedTime();
	PLUGIN_API float GetWeaponAccuracy();
	
	PLUGIN_API void IncrementNumWallRuns();
	PLUGIN_API void AddDamageDealt(float a_damageToAdd);
	PLUGIN_API void SetCompletedTime(float a_timeCompleted);
	PLUGIN_API void ProcessWeaponFired(bool a_wasHit);

#ifdef __cplusplus
}
#endif