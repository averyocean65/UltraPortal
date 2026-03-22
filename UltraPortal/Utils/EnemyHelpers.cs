using System.Collections.Generic;

namespace UltraPortal.Extensions {
	public static class EnemyHelpers {
		// These get initialized in Start() of HookArm, thankfully.
		
		/// <summary>
		/// Enemies that can be pulled by the whiplash.
		/// </summary>
		public static readonly List<EnemyType> LightEnemies = new List<EnemyType>() {
			EnemyType.Drone,
			EnemyType.Filth,
			EnemyType.Schism,
			EnemyType.Soldier,
			EnemyType.Stray,
			EnemyType.Streetcleaner
		};
		
		/// <summary>
		/// Enemies that spawn corpses that still, for whatever reason it may be, have EnemyIdentifiers.
		/// </summary>
		public static readonly List<EnemyType> CorpseTypes = new List<EnemyType>() {
			EnemyType.Drone,
			EnemyType.MaliciousFace,
			EnemyType.Mindflayer,
			EnemyType.Gutterman,
			EnemyType.Virtue,
			EnemyType.HideousMass
		};
		
		public static bool IsLightEnemy(EnemyType enemy) {
			return LightEnemies.Contains(enemy);
		}
		
		public static bool SpawnsUnhookableCorpse(EnemyType enemy) {
			return LightEnemies.Contains(enemy);
		}
	}
}