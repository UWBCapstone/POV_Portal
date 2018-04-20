Shader "Custom/MyVideoOverlayShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_WorldTex("Texture", 2D) = "white" {}
	}
	SubShader
	{

		// No culling or depth
		//Cull Off ZWrite Off ZTest Always

		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _WorldTex;
			float4 _WorldTex_ST;
			sampler2D _CameraDepthTexture;
			float4 _CameraDepthTexture_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 videoCol = tex2D(_MainTex, i.uv);
				fixed4 depth = tex2D(_CameraDepthTexture, i.uv);
				fixed4 worldCol = tex2D(_WorldTex, i.uv);

				//fixed4 col = step(0, depth) * videoCol + (1 - step(0, depth)) * worldCol;
				//fixed4 col = step(0, depth) * videoCol;
				//fixed4 col = videoCol;
				float isWorld = step(0.001, depth.x) + step(0.001, depth.y) + step(0.001, depth.z);
				fixed4 col = step(0.001, isWorld) * worldCol + (1-(step(0.001, isWorld))) * videoCol;

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}
