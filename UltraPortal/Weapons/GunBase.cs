using System;
using System.Collections;
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

		protected WeaponPos WeaponPos;
		protected WeaponIcon WeaponIcon;
		protected WeaponIdentifier WeaponIdentifier;

		protected virtual void Start() {
			WeaponPos = gameObject.AddComponent<WeaponPos>();
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

		protected virtual void Update() {
			if (OptionsMenuToManager.Instance.pauseMenu.activeSelf) {
				return;
			}
			
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

		protected virtual void OnDisable() {
			StopAllCoroutines();
			CanPrimaryFire = true;
			CanSecondaryFire = true;
		}
	}
}