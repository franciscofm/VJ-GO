using UnityEngine;

public static class Values {
	public static class UI {
		public static class EntityInfo {
			public const float Transition = 0.2f;
			public const float Aplha = 0.4f;
		}

		public static class Chat {
			public const float TimePerCharacter = 0.1f;
			public const float PanelTransition = 0.1f;
			public const float ImageTransition = 0.1f;
			public const float ImageAlpha = 0.6f;
			public const float ImageScale = 0.8f;
		}
	}

	public static class Menu {
		public static class Scene {
			public const float StartWait = 1f;

			public const float LightOpen = 47f;
			public const float LightCloseDuration = 1f;
			public const string LightCubesAnimation = "LightCubes";
			public const float LightCubesDuration = 2f;

			public const float TextDuration = 0.4f;
			public const float TextOffset = 0.2f;
		}

		public static class SelectLevel {
			public const float SelectWait = 3f;
			public const float SelectFade = 1f;
		}

		public static class Tutorial {
			public const float PanelOffset = 0.1f;
			public const float PanelDuration = 0.4f;
		}
	}

	public static class Colors {
		public static Color transparentBlack = new Color(0f,0f,0f,0f);
		public static Color transparentWhite = new Color(1f,1f,1f,1f);
	}
}
