using System;
using System.Collections.Generic;
using UnityEngine;

using static UltraPortal.Constants;

namespace UltraPortal {
	[DefaultExecutionOrder(-100000)]
	public class PortalGunManager : MonoBehaviour {
		private const int PortalGunSlot = 7;
		
		public static bool EquippedPortalGun = false;
		private int _currentVariationIndex = -1;

		private GameObject SpawnPortalGun(Type type, string assetPrefabPath, WeaponVariant variant, Vector3 position, Vector3 rotation, Vector3 scale) {
			if (!type.IsSubclassOf(typeof(GunBase))) {
				Plugin.LogSource.LogError("Type must be a subclass of GunBase!");
				return null;
			}
			
			AssetBundle weapons = AssetBundleHelpers.LoadAssetBundle(AssetPaths.WeaponBundle);
			GameObject prefab = weapons.LoadAsset<GameObject>(assetPrefabPath);

			GameObject gun = Instantiate(prefab, Vector3.zero, Quaternion.identity, GunControl.Instance.transform);
			gun.transform.localPosition = position;
			gun.transform.localEulerAngles = rotation;
			gun.transform.localScale = scale;

			GunBase gb = gun.AddComponent(type) as GunBase;
			gb.fireCooldown = 0.5f;
			gb.icon = weapons.LoadAsset<Sprite>(AssetPaths.PortalGunIcon);
			gb.glowIcon = weapons.LoadAsset<Sprite>(AssetPaths.PortalGunIconGlow);
			gb.variant = variant;
			
			gun.SetActive(false);
			return gun;
		}
		
		private void Start() {
			GameObject portalGun = SpawnPortalGun(typeof(PortalGun), AssetPaths.PortalGun, WeaponVariant.BlueVariant,
				new Vector3(0.8236f, -0.7478f, 0.8907f), new Vector3(0, 263.3673f, 14.1545f), Vector3.one * 1.2f);
			
			GameObject mirrorGun = SpawnPortalGun(typeof(MirrorGun), AssetPaths.MirrorGun, WeaponVariant.GreenVariant,
				new Vector3(0.8236f, -0.7478f, 0.8907f), new Vector3(0, 263.3673f, 14.1545f), Vector3.one * 1.2f);

			if (!portalGun || !mirrorGun) {
				Plugin.LogSource.LogError($"Portal Gun?: {portalGun}, Mirror Gun?: {mirrorGun}");
			}
			
			GunControl.Instance.slots.Add(new List<GameObject>() { portalGun, mirrorGun });
		}

		private void Update() {
			if (GunControl.Instance.currentSlotIndex != PortalGunSlot) {
				_currentVariationIndex = -1;
			}

			if (!ModConfig.IsGunEnabled) {
				return;
			}
			
			if (Input.GetKeyDown(ModConfig.PortalGunKeybind) && GunControl.Instance) {
				EquippedPortalGun = true;
				GunControl.Instance.SwitchWeapon(PortalGunSlot, targetVariationIndex: _currentVariationIndex + 1, cycleVariation: true);
				_currentVariationIndex = GunControl.Instance.currentVariationIndex;
			}
		}
	}
}