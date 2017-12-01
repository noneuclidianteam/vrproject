Shader "Custom/portal_shader" {
	Properties {
		_LeftTex ("Albedo (RGB)", 2D) = "white" {}
		_RightTex ("Albedo (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _LeftTex;
		sampler2D _RightTex;

		struct Input {
			float2 uv_MainTex;
		};

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Metallic and smoothness come from slider variables
			//if (unity_StereoEyeIndex == 0)
			if (unity_CameraProjection[0][2] < 0) {
				float4 c = tex2D(_LeftTex, IN.uv_MainTex);
				o.Albedo = c.rgb;
				o.Alpha = c.a;
			} else {
				float4 c = tex2D(_RightTex, IN.uv_MainTex);
				o.Albedo = c.rgb;
				o.Alpha = c.a;
			}
		}
		ENDCG
	}
	FallBack "Diffuse"
}
