Shader "Custom/CrossShader" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Texture", 2D) = "white" {}
	_Glossiness("Smoothness", Range(0,1)) = 0.0
		_Metallic("Metallic", Range(0,1)) = 0.0

		_uvLL("LowerLeftUV", Vector) = (0,0,0,0)
		_uvUL("UpperLeftUV", Vector) = (0,0,0,0)
		_uvLR("LowerRightUV", Vector) = (0,0,0,0)
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

		sampler2D _MainTex;

	struct Input {
		float2 uv_MainTex;
	};

	half _Glossiness;
	half _Metallic;
	fixed4 _Color;
	float4 _uvLL;
	float4 _uvLR;
	float4 _uvUL;

	// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
	// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
	// #pragma instancing_options assumeuniformscaling
	UNITY_INSTANCING_CBUFFER_START(Props)
		// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		void surf(Input IN, inout SurfaceOutputStandard o) {
		// Albedo comes from a texture tinted by color
		float2 true_uv = IN.uv_MainTex;
		float uvx = abs(abs(2 * (.5 - IN.uv_MainTex.x)) - 1) * (_uvLR.x - _uvLL.x) + _uvLL.x;
		float uvy = IN.uv_MainTex.y * (_uvUL.y - _uvLL.y) + _uvLL.y;

		true_uv.x = uvx;
		true_uv.y = uvy;

		fixed4 c = tex2D(_MainTex, true_uv) * _Color;
		//fixed4 c = tex2D(_MainTex, IN.uv_MainTex);// * _Color;
		o.Albedo = c.rgb;
		// Metallic and smoothness come from slider variables
		o.Metallic = _Metallic;
		o.Smoothness = _Glossiness;
		o.Alpha = c.a;
	}
	ENDCG
	}
		FallBack "Diffuse"
}
