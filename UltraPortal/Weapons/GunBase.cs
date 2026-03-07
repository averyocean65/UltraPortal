using System;
using System.Collections;
using UltraPortal.Projectiles;
using UnityEngine;

using static UltraPortal.Constants;

namespace UltraPortal {
	public class GunBase : MonoBehaviour {
		public float fireCooldown = 1f;

		protected Action OnPrimaryFire;
		protected Action OnSecondaryFire;
		
		protected bool CanPrimaryFire = true;
		protected bool CanSecondaryFire = true;

		public WeaponVariant variant;
		public Sprite icon;
		public Sprite glowIcon;

		public WeaponPos WeaponPos;
		public WeaponIcon WeaponIcon;
		public WeaponIdentifier WeaponIdentifier;

		protected virtual void Start() {
			if (!WeaponPos) {
				WeaponPos = gameObject.AddComponent<WeaponPos>();
				WeaponPos.middlePos = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
				WeaponPos.middleRot = Vector3.zero;
				WeaponPos.middleScale = transform.localScale;
			}
			
			WeaponIdentifier = gameObject.AddComponent<WeaponIdentifier>();
			WeaponIdentifier.speedMultiplier = 1.0f;
			
			WeaponIcon = gameObject.AddComponent<WeaponIcon>();
			WeaponIcon.weaponDescriptor = ScriptableObject.CreateInstance<WeaponDescriptor>();
			WeaponIcon.weaponDescriptor.icon = icon;
			WeaponIcon.weaponDescriptor.glowIcon = glowIcon;
			WeaponIcon.weaponDescriptor.variationColor = variant;
		}

		private void SetCanFire(bool isPrimary, bool value) {
			if (isPrimary) {
				CanPrimaryFire = value;
			}
			else {
				CanSecondaryFire = false;
			}
		}
		
		protected virtual IEnumerator IFireCooldown(bool isPrimary) {
			SetCanFire(isPrimary, false);
			yield return new WaitForSeconds(fireCooldown);
			SetCanFire(isPrimary, true);
		}

		protected virtual void HandleFiring() {
			if (MonoSingleton<InputManager>.Instance.InputSource.Fire1.WasPerformedThisFrame && CanPrimaryFire) {
				if(OnPrimaryFire != null)
					OnPrimaryFire.Invoke();
				
				StartCoroutine(IFireCooldown(true));
			}
			
			if (MonoSingleton<InputManager>.Instance.InputSource.Fire2.WasPerformedThisFrame && CanPrimaryFire) {
				if(OnSecondaryFire != null)
					OnSecondaryFire.Invoke();
				
				StartCoroutine(IFireCooldown(true));
			}
		}

		protected void PlayerHeadRaycast(out bool success, out RaycastHit hit) {
			success = Physics.Raycast(MainCamera.transform.position,
				MainCamera.transform.forward,
				out hit,
				float.PositiveInfinity,
				EnvironmentLayer,
				QueryTriggerInteraction.Ignore);
		}

		protected Projectile SpawnProjectileFromAsset(string assetName, float speed) {
			AssetBundle weapons = AssetBundleHelpers.LoadAssetBundle(AssetPaths.WeaponBundle);
			return SpawnProjectileFromPrefab(weapons.LoadAsset<GameObject>(assetName), speed);
		}
		
		protected Projectile SpawnProjectileFromPrefab(GameObject prefab, float speed) {
			GameObject spawned = Instantiate(prefab, MainCamera.transform.position + MainCamera.transform.forward,
				Quaternion.identity);
			Projectile projectile = spawned.AddComponent<Projectile>();
			projectile.damage = 0f;
			projectile.sourceWeapon = gameObject;
			projectile.friendly = true;
			projectile.speed = speed;
			projectile.bulletType = "";
			projectile.ignoreEnvironment = false;
			projectile.ignoreExplosions = false;
			projectile.unparryable = true;
			projectile.transform.forward = MainCamera.transform.forward;

			return projectile;
		}

		protected virtual void OnDisable() {
			StopAllCoroutines();
			CanPrimaryFire = true;
			CanSecondaryFire = true;
		}
	}
}