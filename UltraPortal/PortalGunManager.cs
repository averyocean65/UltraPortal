using System;
using System.Collections.Generic;
using UnityEngine;

using static UltraPortal.Constants;

namespace UltraPortal {
	[DefaultExecutionOrder(-100000)]
	public class PortalGunManager : MonoBehaviour {
		private static int PortalGunSlot = -1;
		
		public static bool EquippedPortalGun = false;
		private int _currentVariationIndex = -1;

		private PortalGun _portalGun;
		private MirrorGun _mirrorGun;
		

		private GunBase SpawnPortalGun(Type type, string assetPrefabPath, WeaponVariant variant, Vector3 position, Vector3 rotation, Vector3 scale) {
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
			return gb;
		}
		
		private void Start() {
			_portalGun = SpawnPortalGun(typeof(PortalGun), AssetPaths.PortalGun, WeaponVariant.BlueVariant,
				new Vector3(0.8236f, -0.7478f, 0.8907f), new Vector3(0, 263.3673f, 14.1545f),
				Vector3.one * 1.2f) as PortalGun;

			_mirrorGun = SpawnPortalGun(typeof(MirrorGun), AssetPaths.MirrorGun, WeaponVariant.GreenVariant,
				new Vector3(0.8236f, -0.7478f, 0.8907f), new Vector3(0, 263.3673f, 14.1545f),
				Vector3.one * 1.2f) as MirrorGun;

			if (!_portalGun || !_mirrorGun) {
				Plugin.LogSource.LogError($"Portal Gun?: {_portalGun}, Mirror Gun?: {_mirrorGun}");
			}
			
			if(ModConfig.IsEnabled)
				AddToSlots();
		}

		private void AddToSlots() {
			GunControl.Instance.slots.Add(new List<GameObject>() { _portalGun.gameObject, _mirrorGun.gameObject });
			PortalGunSlot = GunControl.Instance.slots.Count;
		}

		private bool _wasEnabledLastFrame = false;
		private void Update() {
			if (GunControl.Instance.currentSlotIndex != PortalGunSlot) {
				_currentVariationIndex = -1;
			}

			if (!ModConfig.IsEnabled) {
				if (_wasEnabledLastFrame) {
					_wasEnabledLastFrame = false;
					
					if(_portalGun)
						_portalGun.Reset();
					
					if(_mirrorGun)
						_mirrorGun.Reset();

					if (GunControl.Instance.currentSlotIndex == PortalGunSlot) {
						GunControl.Instance.SwitchWeapon(1);
					}
					
					// take out of the slots
					GunControl.Instance.slots.RemoveAt(PortalGunSlot - 1);
					PortalGunSlot = -1;
				}
				return;
			}

			if (PortalGunSlot == -1) {
				AddToSlots();
			}

			if (OptionsManager.Instance.paused) {
				return;
			}

			_wasEnabledLastFrame = true;
			
			if (Input.GetKeyDown(ModConfig.PortalGunKeybind) && GunControl.Instance) {
				EquippedPortalGun = true;
				GunControl.Instance.SwitchWeapon(PortalGunSlot, targetVariationIndex: _currentVariationIndex + 1, cycleVariation: true);
				_currentVariationIndex = GunControl.Instance.currentVariationIndex;
			}
		}
	}
}