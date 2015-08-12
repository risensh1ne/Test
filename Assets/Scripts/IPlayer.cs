using UnityEngine;
using System.Collections;

public interface IPlayer {

	bool isMoving { get; set; }
	bool isAttacking {get; set; }
	bool isDead { get; set; }

	GameManager.team checkTeam();
	void damage(float d);
	void SetAttacker(GameObject attacker);
}
