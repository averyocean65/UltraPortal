using System;
using Sandbox;

namespace UltraPortal {
	public class PortalSandboxObject : SandboxProp, IAlter, IAlterOptions<bool> {
		public bool remain { get; private set; } = false;
		public string alterKey => "ultraportal_portals";
		public string alterCategoryName => nameof(DynamicPortalExit);

		public AlterOption<bool>[] options {
			get {
				return new AlterOption<bool>[1] {
					new AlterOption<bool>() {
						key = "remain",
						name = "Remain In Scene?",
						value = remain,
						callback = value => remain = value
					}
				};
			}
		}
	}
}