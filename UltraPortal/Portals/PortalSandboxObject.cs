using System;
using Sandbox;
using UnityEngine;

namespace UltraPortal {
	public class PortalSandboxObject : SandboxProp, IAlter, IAlterOptions<Vector3> {
		public string alterKey => "ultraportal_portals";
		public string alterCategoryName => "ULTRAPORTALS";

		AlterOption<Vector3>[] IAlterOptions<Vector3>.options {
			get {
				return new[] {
					new AlterOption<Vector3>() {
						key = "rotation",
						name = "Local Rotation",
						value = transform.localEulerAngles,
						callback = value => transform.localEulerAngles = value
					},
					
				};
			}
		}

		public override void SetSize(Vector3 size) { }
	}
}