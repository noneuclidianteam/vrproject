﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/PortalShader" {
	Properties{
		_LeftEyeTexture("Left Eye Texture", 2D) = "white" {}
		_RightEyeTexture("Left Eye Texture", 2D) = "white" {}

		_VREnabled("VR Enabled", Int) = 0

		_BluredPortal("Portal blur effect", Int) = 0
	}

	SubShader{
		Tags{ "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True" }
		LOD 100

		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#pragma multi_compile __ STEREO_RENDER

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv:TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
			};

			sampler2D _LeftEyeTexture;
			sampler2D _RightEyeTexture;
			int _VREnabled;
			int _BluredPortal;

			float offsetX(float input) {
				return input + cos(input * 200.0f + _Time.w * 2.0f) / 300.0f;
			}

			float offsetY(float input) {
				return input + sin(input * 200.0f + _Time.w * 2.0f) / 300.0f;
			}


			v2f vert(appdata v, out float4 outpos : SV_POSITION)
			{
				v2f o;
				outpos = UnityObjectToClipPos(v.vertex);

				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i, UNITY_VPOS_TYPE screenPos : VPOS) : SV_Target
			{
				float2 sUV = screenPos.xy / _ScreenParams.xy;

				if (unity_CameraProjection[0][2] < 0 || _VREnabled == 0)
				{
					return _BluredPortal ? tex2D(_LeftEyeTexture, float2(offsetX(sUV.x), offsetY(sUV.y))) : tex2D(_LeftEyeTexture, sUV);
				}else {
					return _BluredPortal ? tex2D(_RightEyeTexture, float2(offsetX(sUV.x), offsetY(sUV.y))) : tex2D(_RightEyeTexture, sUV);
				}
			}
			ENDCG
		}
	}

	Fallback "Diffuse"
}