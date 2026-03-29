using System;
using System.Collections;
using System.Collections.Generic;
using Sandbox.Arm;
using ULTRAKILL.Cheats;
using ULTRAKILL.Portal;
using ULTRAKILL.Portal.Geometry;
using UltraPortal.Colorizers;
using UltraPortal.Projectiles;
using UnityEngine;

using static UltraPortal.Constants;
using static UltraPortal.DebugUtils;

namespace UltraPortal {
	[DefaultExecutionOrder(-100000)]
	public class PortalGunManager : MonoSingleton<PortalGunManager> {
		private static float PortalGunResetWait = 0.1f;
		private static int PortalGunSlot = -1;

		public bool IsUsingSpawnerArm { get; private set; } = false;
		
		private bool AnyPortalsInit {
			get {
				bool output = false;

				if (_portalGun) {
					output |= _portalGun.BothPortalsInit;
				}

				if (_mirrorGun) {
					output |= _mirrorGun.PrimaryMirror || _mirrorGun.FlippedMirror;
				}

				if (_twistGun) {
					output |= _twistGun.BothPortalsInit;
				}

				return output;
			}
		}

		public static bool UsedPortalGun = false;
		private int _currentVariationIndex = -1;
		
		private bool _wasEnabledLastFrame = false;
		private bool _showedWarning = false;

		private PortalGun _portalGun;
		private MirrorGun _mirrorGun;
		private TwistGun _twistGun;

		public static void SummonPortalExit(DynamicPortalExit exit, Portal portal, Vector3 position, Vector3 forward,
			Transform parent = null, Collider collider = null) {
			exit.transform.parent = null;
			
			Vector3 appliedScale = Vector3.one * PortalProjectileHelper.PortalScaleSceneStart;
			
			exit.transform.localScale = new Vector3(appliedScale.x, appliedScale.y, 1.0f);
			exit.transform.SetParent(parent, true);
			
			exit.Initialize(portal, exit.side, position, forward, collider);
		}

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

			_twistGun = SpawnPortalGun(typeof(TwistGun), AssetPaths.TwistGun, WeaponVariant.RedVariant, defaultPos,
				middlePos) as TwistGun;

			if (!_portalGun || !_mirrorGun || !_twistGun) {
				LogError($"Portal Gun?: {_portalGun}, Mirror Gun?: {_mirrorGun}, Twist Gun?: {_twistGun}");
			}
			
			if(ModConfig.IsEnabled.GetValue())
				AddToSlots();
		}

		private void AddToSlots() {
			if (!_portalGun || !_mirrorGun || !_twistGun) {
				LogError($"Portal Gun: {_portalGun}; Mirror Gun: {_mirrorGun}; Twist Gun: {_twistGun}");
				return;
			}
			
			GunControl.Instance.slots.Add(new List<GameObject>()
				{ _portalGun.gameObject, _mirrorGun.gameObject, _twistGun.gameObject });
			PortalGunSlot = GunControl.Instance.slots.Count;
			
			GunControl.Instance.UpdateWeaponList();
		}

		private IEnumerator IDestroyPortals(PortalGunBase gun, params DynamicPortalExit[] exits) {
			if (!gun) {
				yield break;
			}
					
			bool gunShouldReset = gun.ShouldBeReset();
			if (!gunShouldReset) {
				foreach (var exit in exits) {
					SpawnPortalExplosion(exit.PortalCenter);
				}

				yield return new WaitForSeconds(PortalGunResetWait);
			}

			if (gun.ShouldPlayReset()) {
				AudioManager.Instance.PlayAudioFromAsset(AssetPaths.Sfx.PortalClose, MainCamera.transform.position,
					spatialBlend: 0.0f);
			}
			
			gun.Invoke("Reset", 0.0f);
		}
		
		private void SpawnPortalExplosion(Vector3 position) {
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

		public void DestroyPortals(WeaponVariant variant, bool useSfx = false) {
			switch (variant) {
				case WeaponVariant.BlueVariant: {
					StartCoroutine(IDestroyPortals(_portalGun, _portalGun.PortalEntry, _portalGun.PortalExit));
					break;
				}
				case WeaponVariant.GreenVariant: {
					StartCoroutine(IDestroyPortals(_mirrorGun, _mirrorGun.PrimaryMirror, _mirrorGun.FlippedMirror));
					break;
				}
				case WeaponVariant.RedVariant: {
					StartCoroutine(IDestroyPortals(_twistGun, _twistGun.TwistExit, _twistGun.TwistExit));
					break;
				}
			}
		}
		
		private void Update() {
			if (GunControl.Instance.currentSlotIndex != PortalGunSlot) {
				_currentVariationIndex = -1;
			}

			IsUsingSpawnerArm = GunControl.Instance.currentSlotIndex == 6; 
			if (IsUsingSpawnerArm) {
				if (AnyPortalsInit) {
					if (!_showedWarning) {
						HudMessageReceiver.Instance.SendHudMessage(
							"Custom Portals may behave <color=red>differently</color> when the spawner arm is equipped.");
						_showedWarning = true;
					}
				}
			}

			if (!ModConfig.IsEnabled.GetValue()) {
				if (_wasEnabledLastFrame) {
					_wasEnabledLastFrame = false;
					
					DestroyPortals(WeaponVariant.BlueVariant);
					DestroyPortals(WeaponVariant.GreenVariant);
					DestroyPortals(WeaponVariant.RedVariant);
					DestroyPortals(WeaponVariant.GoldVariant);
					
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

			if (OptionsManager.Instance.paused || GameStateManager.Instance.PlayerInputLocked || !GunControl.Instance.activated) {
				return;
			}

			if (_portalGun.WantsToReset) {
				DestroyPortals(WeaponVariant.BlueVariant, true);
			}
			
			if (_mirrorGun.WantsToReset) {
				DestroyPortals(WeaponVariant.GreenVariant, true);
			}
			
			if (_twistGun.WantsToReset) {
				DestroyPortals(WeaponVariant.RedVariant, true);
			}

			int slotIndex = PortalGunSlot - 1;
			if (Input.GetKeyDown(ModConfig.PortalGunKeybind.GetValue()) && GunControl.Instance &&
			    GunControl.Instance.slots[slotIndex].Count > 0 && GunControl.Instance.slots[slotIndex][0]) {
				GunControl.Instance.SwitchWeapon(PortalGunSlot, targetVariationIndex: _currentVariationIndex + 1, cycleVariation: true);
				_currentVariationIndex = GunControl.Instance.currentVariationIndex;
			}
		}
	}
}