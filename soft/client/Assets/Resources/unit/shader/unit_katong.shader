// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/unit_katong" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Outline("Outline", Range(0,0.1)) = 0.02
		_alpha("alpha", Range(0,1)) = 1.0
		_white("white", Range(0,1)) = 0
	}
    SubShader {
		Pass {
			Tags{ "Queue" = "AlphaTest+1" }
			//Tags{ "LightMode" = "ForwardBase" }
			Blend SrcAlpha OneMinusSrcAlpha

			Cull Front
			Lighting Off
			ZWrite Off

			CGPROGRAM
			#pragma vertex vert  
			#pragma fragment frag  
			
			#include "UnityCG.cginc"  
			
			fixed _Outline;
			fixed _alpha;
			fixed _white;

			struct a2v
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : POSITION;
			};

			v2f vert(a2v v)
			{
				v2f o;

				float4 pos = mul(UNITY_MATRIX_MV, v.vertex);
				float3 normal = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
				normal.z = -0.5;
				pos = pos + float4(normalize(normal),0) * _Outline;
				o.pos = mul(UNITY_MATRIX_P, pos);

				return o;
			}

			float4 frag(v2f i) : COLOR
			{
				return float4(_white, _white, _white, _alpha);
			}

			ENDCG
		}


		Tags{ "Queue" = "AlphaTest" }
		Blend SrcAlpha OneMinusSrcAlpha

		Cull Back
		Lighting Off

		CGPROGRAM
		#pragma surface surf Lambert alphatest:_Cutoff

		sampler2D _MainTex;
		fixed _alpha;
		fixed _white;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) 
		{
			float4 c = tex2D(_MainTex, IN.uv_MainTex);
			c.rgb = c.rgb + _white;
			c.a *= _alpha;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}

		ENDCG
	}
	FallBack "Diffuse"
}