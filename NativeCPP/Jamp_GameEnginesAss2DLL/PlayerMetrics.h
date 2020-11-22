#pragma once
#include "PluginSettings.h"

class PLUGIN_API PlayerMetrics
{
public:
	void ResetMetrics();

	int   GetNumWallRuns();
	float GetDamageDealt();
	float GetCompletedTime();
	float GetWeaponAccuracy();

	void IncrementNumWallRuns();
	void AddDamageDealt(float a_damageToAdd);
	void SetCompletedTime(float a_timeCompleted);
	void ProcessWeaponFired(bool a_wasHit);
	
private:
	int   m_numWallRuns;
	float m_damageDealt = 0;
	float m_timeCompleted;
	
	float m_weaponAccuracy;
	int   m_numWeaponHits = 0;
	int   m_numWeaponMisses = 0;
};