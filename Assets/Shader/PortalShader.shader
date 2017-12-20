// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/PortalShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Lighting Off
		Cull Back
		ZWrite On
		ZTest Less
		
		Fog{ Mode Off }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			float offsetX(float input) {
				return input + cos(input * 200.0f + _Time.w * 2.0f) / 300.0f;
			}

			float offsetY(float input) {
				return input + sin(input * 200.0f + _Time.w * 2.0f) / 300.0f;
			}

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				//float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 screenPos : TEXCOORD1;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);
				return o;
			}
			
			sampler2D _MainTex;

			fixed4 frag (v2f i) : SV_Target
			{
				i.screenPos /= i.screenPos.w;
				//fixed4 col = tex2D(_MainTex, float2(offsetX(i.screenPos.x), offsetY(i.screenPos.y)));
				//col += reflectX(i.screenPos.x) + reflectY(i.screenPos.y);

				return tex2D(_MainTex, float2(i.screenPos.x, i.screenPos.y));
				
				//return col;
			}
			ENDCG
		}
	}
}
