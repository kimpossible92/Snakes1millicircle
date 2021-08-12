using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
	public interface IDamageObject
	{
		bool isAlive { get; }

		Vector3 GetPosition();

		void Damage(float damage);

		void SetDamagedBy(int playerNumber, string explosionId);
	}
}