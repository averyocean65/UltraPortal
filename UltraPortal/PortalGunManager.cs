using System;
using System.Collections.Generic;
using System.Linq;
using ULTRAKILL.Portal;
using UnityEngine;

using static UltraPortal.Constants;

namespace UltraPortal {
	[DefaultExecutionOrder(-100000)]
	public class PortalGunManager : MonoBehaviour {
		public static PortalGunManager Instance;
		
		private static int PortalGunSlot = -1;
		
		public static bool EquippedPortalGun = false;
		private int _currentVariationIndex = -1;

		private PortalGun _portalGun;
		private MirrorGun _mirrorGun;

		private GunBase SpawnPortalGun(Type type, string assetPrefabPath, WeaponVariant variant, GunPosition defaultPos, GunPosition middlePos) {
			if (!type.IsSubclassOf(typeof(GunBase))) {
				Plugin.LogSource.LogError("Type must be a subclass of GunBase!");
				return null;
			}
			
			AssetBundle weapons = AssetBundleHelpers.LoadAssetBundle(AssetPaths.WeaponBundle);
			GameObject prefab = weapons.LoadAsset<GameObject>(assetPrefabPath);

			GameObject gun = Instantiate(prefab, Vector3.zero, Quaternion.identity, GunControl.Instance.transform);
			gun.transform.localPosition = defaultPos.Position;
			gun.transform.localEulerAngles = defaultPos.Rotation;
			gun.transform.localScale = defaultPos.Scale;
			
			GunBase gb = gun.AddComponent(type) as GunBase;
			gb.fireCooldown = 0.5f;
			gb.icon = weapons.LoadAsset<Sprite>(AssetPaths.PortalGunIcon);
			gb.glowIcon = weapons.LoadAsset<Sprite>(AssetPaths.PortalGunIconGlow);
			gb.variant = variant;

			gb.WeaponPos = gun.AddComponent<WeaponPos>();
			
			gb.WeaponPos.middlePos = middlePos.Position;
			gb.WeaponPos.middleRot = middlePos.Rotation;
			gb.WeaponPos.middleScale = middlePos.Scale;
			
			gun.SetActive(false);
			return gb;
		}
		
		private void Start() {
			GunPosition middlePos = new GunPosition(new Vector3(0, -0.8478f, 0.8907f), new Vector3(0, 270, 0),
				Vector3.one * 1.2f);

			GunPosition defaultPos = new GunPosition(new Vector3(0.8236f, -0.7478f, 0.8907f),
				new Vector3(0, 263.3673f, 14.1545f),
				Vector3.one * 1.2f);

			_portalGun = SpawnPortalGun(typeof(PortalGun), AssetPaths.PortalGun, WeaponVariant.BlueVariant, defaultPos,
				middlePos) as PortalGun;

			_mirrorGun = SpawnPortalGun(typeof(MirrorGun), AssetPaths.MirrorGun, WeaponVariant.GreenVariant,
				defaultPos, middlePos) as MirrorGun;

			if (!_portalGun || !_mirrorGun) {
				Plugin.LogSource.LogError($"Portal Gun?: {_portalGun}, Mirror Gun?: {_mirrorGun}");
			}
			
			if(ModConfig.IsEnabled)
				AddToSlots();
		}

		private void AddToSlots() {
			GunControl.Instance.slots.Add(new List<GameObject>() { _portalGun.gameObject, _mirrorGun.gameObject });
			PortalGunSlot = GunControl.Instance.slots.Count;
			
			GunControl.Instance.UpdateWeaponList();
		}

		public void DestroyPortals() {
			void Error(string weaponName) {
				HudMessageReceiver.Instance.SendHudMessage($"<color=#FF0000>Cannot disable portals for {weaponName}!</color>");
			}
			
			bool portalGunShouldReset = true;
			bool mirrorGunShouldReset = true;
			
			if (_portalGun) {
				portalGunShouldReset = _portalGun.ShouldBeReset();
				if (portalGunShouldReset) {
					_portalGun.Reset();
				}
				else {
					Error("Portal Gun");
				}
			}

			if (_mirrorGun) {
				mirrorGunShouldReset = _mirrorGun.ShouldBeReset();
				if (mirrorGunShouldReset) {
					_mirrorGun.Reset();
				}
				else {
					Error("Mirror Gun");
				}
			}
		}
		
		private bool _wasEnabledLastFrame = false;
		private void Update() {
			if (GunControl.Instance.currentSlotIndex != PortalGunSlot) {
				_currentVariationIndex = -1;
			}

			if (!ModConfig.IsEnabled) {
				if (_wasEnabledLastFrame) {
					_wasEnabledLastFrame = false;
					
					DestroyPortals();
					int previousSlot = GunControl.Instance.currentSlotIndex;
					
					// take out of the slots
					GunControl.Instance.slots.RemoveAt(PortalGunSlot - 1);
					PortalGunSlot = -1;
					GunControl.Instance.UpdateWeaponList();
					
					// is there an easier way to do this? probably! but i'm not going to look for it, because this works.
					// (even if it is held together by hopes and dreams)
					if (GunControl.Instance.currentSlotIndex == previousSlot) {
						if (GunControl.Instance.noWeapons) {
							GunControl.Instance.currentSlotIndex = 1;
							GunControl.Instance.NoWeapon();
						}
						else {
							for (int i = 0; i < GunControl.Instance.slots.Count; i++)
							{
								if (GunControl.Instance.slots[i].Count > 0) {
									GunControl.Instance.SwitchWeapon(i + 1);
									return;
								}
							}

							GunControl.Instance.stayUnarmed = true;
						}
					}
				}
				return;
			}

			bool prevNoWeapons = GunControl.Instance.noWeapons;
			if (PortalGunSlot == -1) {
				AddToSlots();
			}
			
			if (!_wasEnabledLastFrame) {
				if (prevNoWeapons && PortalGunSlot != -1) {
					GunControl.Instance.SwitchWeapon(PortalGunSlot, 0);
				}
			}

			_wasEnabledLastFrame = true;

			if (OptionsManager.Instance.paused) {
				return;
			}

			if (Input.GetKeyDown(ModConfig.DespawnPortalsKeybind)) {
				DestroyPortals();
			}

			int slotIndex = PortalGunSlot - 1;
			if (Input.GetKeyDown(ModConfig.PortalGunKeybind) && GunControl.Instance &&
			    GunControl.Instance.slots[slotIndex].Count > 0 && GunControl.Instance.slots[slotIndex][0]) {
				EquippedPortalGun = true;
				GunControl.Instance.SwitchWeapon(PortalGunSlot, targetVariationIndex: _currentVariationIndex + 1, cycleVariation: true);
				_currentVariationIndex = GunControl.Instance.currentVariationIndex;
			}
		}
	}
}