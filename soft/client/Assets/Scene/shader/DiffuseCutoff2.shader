Shader "Custom/DiffuseCutoff2" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Cutoff("Alpha cutoff", Range(0.000000, 1.000000)) = 0.5
	}
	SubShader {
		Tags{ "QUEUE" = "AlphaTest-1"}
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Lambert alphatest:_Cutoff

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
