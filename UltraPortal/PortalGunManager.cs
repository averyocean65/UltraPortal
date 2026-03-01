using System.Collections.Generic;
using UnityEngine;

namespace UltraPortal {
	[DefaultExecutionOrder(-100000)]
	public class PortalGunManager : MonoBehaviour {
		private void Start() {
			AssetBundle weapons = AssetBundleHelpers.LoadAssetBundle("weapons");
                
			GameObject portalGunPrefab = weapons.LoadAsset<GameObject>("Portal Gun");
			
			GameObject portalGun = Instantiate(portalGunPrefab, Vector3.zero, Quaternion.identity,
				GunControl.Instance.transform);
			
			portalGun.AddComponent<PortalSpawner>();
			
			portalGun.transform.localPosition = new Vector3(0.8358f, -0.6898f, 1.1815f);
			portalGun.transform.localEulerAngles = new Vector3(0, 263.3673f, 14.1545f);
			portalGun.transform.localScale = Vector3.one * 1.5f;
			
			WeaponPos pos = portalGun.AddComponent<WeaponPos>();
			WeaponIdentifier identifier = portalGun.AddComponent<WeaponIdentifier>();
			WeaponIcon icon = portalGun.AddComponent<WeaponIcon>();
			
			icon.weaponDescriptor = ScriptableObject.CreateInstance<WeaponDescriptor>();
			icon.weaponDescriptor.icon = weapons.LoadAsset<Sprite>("UltraPortalGunIcon");
			icon.weaponDescriptor.glowIcon = weapons.LoadAsset<Sprite>("UltraPortalGunIconGlow");
			icon.weaponDescriptor.variationColor = WeaponVariant.GoldVariant;
			
			identifier.speedMultiplier = 1.0f;
                
			portalGun.SetActive(false);
			GunControl.Instance.slots.Add(new List<GameObject>() { portalGun });
		}

		private void Update() {
			if (Input.GetKeyDown(ModConfig.PortalGunKeybind)) {
				GunControl.Instance.SwitchWeapon(7);
			}
		}
	}
}