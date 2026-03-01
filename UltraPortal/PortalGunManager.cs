using System;
using System.Collections.Generic;
using UnityEngine;

namespace UltraPortal {
	public class PortalGunManager : MonoBehaviour {
		private void Start() {
			AssetBundle weapons = AssetBundleHelpers.LoadAssetBundle("weapons");
                
			GameObject portalGunPrefab = weapons.LoadAsset<GameObject>("Portal Gun");
			GameObject portalGun = Instantiate(portalGunPrefab, Vector3.zero, Quaternion.identity,
				GunControl.Instance.transform);
			portalGun.transform.localPosition = Vector3.zero;
			portalGun.transform.localRotation = Quaternion.identity;
			portalGun.AddComponent<PortalSpawner>();
			WeaponPos pos = portalGun.AddComponent<WeaponPos>();
                
			WeaponIcon icon = portalGun.AddComponent<WeaponIcon>();

			if (!icon.weaponDescriptor) {
				icon.weaponDescriptor = ScriptableObject.CreateInstance<WeaponDescriptor>();
				icon.weaponDescriptor.icon = weapons.LoadAsset<Sprite>("UltraPortalGunIcon");
				icon.weaponDescriptor.glowIcon = weapons.LoadAsset<Sprite>("UltraPortalGunIconGlow");
				icon.weaponDescriptor.variationColor = WeaponVariant.GoldVariant;
			}

			WeaponIdentifier identifier = portalGun.AddComponent<WeaponIdentifier>();
			identifier.speedMultiplier = 1.0f;
                
			portalGun.SetActive(false);
                
			GunControl.Instance.allWeapons.Add(portalGun);
			GunControl.Instance.slots.Add(new List<GameObject>() { portalGun });
		}

		private void Update() {
			if (Input.GetKeyDown(ModConfig.PortalGunKeybind)) {
				GunControl.Instance.SwitchWeapon(7);
			}
		}
	}
}