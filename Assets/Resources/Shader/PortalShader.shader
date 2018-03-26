Shader "Custom/PortalShader" {
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_uvLL("LowerLeftUV", Vector) = (0,0,0,0)
		_uvUL("UpperLeftUV", Vector) = (0,0,0,0)
		_uvLR("LowerRightUV", Vector) = (0,0,0,0)
	}
	SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		Pass
		{
			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			//#pragma surface surf Standard fullforwardshadows

			// Use shader model 3.0 target, to get nicer looking lighting
			//#pragma target 3.0

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv_MainTex : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float4 t_vertex : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			float4 _uvLL;
			float4 _uvLR;
			float4 _uvUL;

			v2f vert(appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.t_vertex = v.vertex;
				o.uv_MainTex = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o, o.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, i.uv_MainTex);

				return c;
			}
			ENDCG
		}
	}
		FallBack "Diffuse"
}