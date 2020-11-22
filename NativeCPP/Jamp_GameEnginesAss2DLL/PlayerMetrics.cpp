#include "PlayerMetrics.h"

void PlayerMetrics::ResetMetrics()
{
	m_numWallRuns   = 0;
	m_damageDealt   = 0.0f;
	m_timeCompleted = 0.0f;
	
	m_weaponAccuracy  = 0.0f;
	m_numWeaponHits   = 0;
	m_numWeaponMisses = 0;
}

float PlayerMetrics::GetDamageDealt()
{
	return m_damageDealt;
}

float PlayerMetrics::GetWeaponAccuracy()
{
	return m_weaponAccuracy;
}

int PlayerMetrics::GetNumWallRuns()
{
	return m_numWallRuns;
}

float PlayerMetrics::GetCompletedTime()
{
	return m_timeCompleted;
}

void PlayerMetrics::IncrementNumWallRuns()
{
	m_numWallRuns++;
}

void PlayerMetrics::AddDamageDealt(float a_damageToAdd)
{
	m_damageDealt += a_damageToAdd;
}

void PlayerMetrics::SetCompletedTime(float a_timeCompleted)
{
	m_timeCompleted = a_timeCompleted;
}

void PlayerMetrics::ProcessWeaponFired(bool a_wasHit)
{
	if (a_wasHit)
	{
		m_numWeaponHits++;
	}
	else
	{
		m_numWeaponMisses++;
	}

	m_weaponAccuracy = static_cast<float>(m_numWeaponHits) / static_cast<float>(m_numWeaponHits + m_numWeaponMisses);
}