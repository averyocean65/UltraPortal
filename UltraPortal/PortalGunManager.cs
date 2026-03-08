using System;
using System.Collections.Generic;
using System.Linq;
using ULTRAKILL.Portal;
using UltraPortal.Colorizers;
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
			
			if(ModConfig.IsEnabled.GetValue())
				AddToSlots();
		}

		private void AddToSlots() {
			GunControl.Instance.slots.Add(new List<GameObject>() { _portalGun.gameObject, _mirrorGun.gameObject });
			PortalGunSlot = GunControl.Instance.slots.Count;
			
			GunControl.Instance.UpdateWeaponList();
		}

		private void Explode() {
			AssetBundle weapons = AssetBundleHelpers.LoadAssetBundle(AssetPaths.WeaponBundle);
			GameObject explosionPrefab = weapons.LoadAsset<GameObject>(AssetPaths.Explosion);

			EnemyPatches.AlreadyDealtWith.Clear();
				
			Vector3 position = MainCamera.transform.position + MainCamera.transform.forward * 10f;
				
			GameObject explosionObject = Instantiate(explosionPrefab, position, Quaternion.identity);
			float maxSize = 10f;

			ExplosionColorManager colors = explosionObject.AddComponent<ExplosionColorManager>();
			colors.ColorExplosion();
				
			Explosion explosion = explosionObject.AddComponent<Explosion>();
			explosion.sourceWeapon = gameObject;
			explosion.hitterWeapon = PortalExplosionWeapon;
			explosion.damage = 50;
			explosion.speed = 7.5f;
			explosion.maxSize = maxSize;
				
			explosion.harmless = false;
		}

		public void DestroyPortals(bool isMirrorGun) {
			void Explode(Vector3 position) {
				AssetBundle weapons = AssetBundleHelpers.LoadAssetBundle(AssetPaths.WeaponBundle);
				GameObject explosionPrefab = weapons.LoadAsset<GameObject>(AssetPaths.Explosion);
				
				GameObject explosionObject = Instantiate(explosionPrefab, position, Quaternion.identity);
				float maxSize = 15f;

				ExplosionColorManager colors = explosionObject.AddComponent<ExplosionColorManager>();
				colors.ColorExplosion();
				
				Explosion explosion = explosionObject.AddComponent<Explosion>();
				explosion.enemyDamageMultiplier = 1.5f;
				explosion.toIgnore = new List<EnemyType>();
				
				explosion.sourceWeapon = gameObject;
				explosion.hitterWeapon = PortalExplosionWeapon;
				explosion.damage = 25;
				explosion.speed = 15f;
				explosion.maxSize = maxSize;
				
				explosion.harmless = false;
				explosion.ultrabooster = ModConfig.AreExplosionsUltraboosters.GetValue();
			}
			
			bool gunShouldReset = true;

			if (!isMirrorGun) {
				if (_portalGun) {
					gunShouldReset = _portalGun.ShouldBeReset();
					if (!gunShouldReset) {
						Explode(_portalGun.PortalEntry.PortalCenter);
						Explode(_portalGun.PortalExit.PortalCenter);
					}

					_portalGun.Reset();
				}

				return;
			}

			if (_mirrorGun) {
				gunShouldReset = _mirrorGun.ShouldBeReset();
				if (!gunShouldReset) {
					Explode(_mirrorGun.PrimaryMirror.PortalCenter);
				}
				
				_mirrorGun.Reset();
			}
		}
		
		private bool _wasEnabledLastFrame = false;
		private void Update() {
			if (GunControl.Instance.currentSlotIndex != PortalGunSlot) {
				_currentVariationIndex = -1;
			}

			if (!ModConfig.IsEnabled.GetValue()) {
				if (_wasEnabledLastFrame) {
					_wasEnabledLastFrame = false;
					
					DestroyPortals(false);
					DestroyPortals(true);
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

			if (_portalGun.WantsToReset) {
				DestroyPortals(false);
			}
			
			if (_mirrorGun.WantsToReset) {
				DestroyPortals(true);
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