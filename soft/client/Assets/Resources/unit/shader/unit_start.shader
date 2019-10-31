// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/unit_start" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_TdTex("通道流光图", 2D) = "black" {}
		_ScrollYSpeed("速度", Range(0.1, 1)) = 0.5
		_ScrollDirection("方向", Range(-1, 1)) = 1
		_FlowColor("流光颜色",Color) = (1,1,1,1)
		_ScrollJG("间隔", Range(0, 3)) = 0
	}
    SubShader {
    
		Tags { "RenderType"="Opaque" }
		LOD 200	
		Blend SrcAlpha OneMinusSrcAlpha

		Cull Back
		Lighting Off

		CGPROGRAM
		#pragma surface surf BlinnPhong 

		sampler2D _MainTex;
		sampler2D _TdTex;
		fixed _ScrollYSpeed;
		fixed _ScrollDirection;
		float4 _FlowColor;
		fixed _ScrollJG;

		struct Input {
			float2 uv_MainTex;
			float2 uv2_TdTex;
		};

		void surf (Input IN, inout SurfaceOutput o) 
		{
			fixed t = 1 / _ScrollYSpeed;
			fixed t1 = t * (1 + _ScrollJG);
			fixed tt = fmod(_Time.y, t1);
			half4 d = tex2D(_MainTex, IN.uv_MainTex);
			if (tt < t)
			{
				fixed2 scrolledUV = IN.uv2_TdTex;
				fixed yScrollValue = tt * _ScrollYSpeed;
				scrolledUV += fixed2(0, yScrollValue * _ScrollDirection);

				//颜色混合  
				half4 c = tex2D(_TdTex, scrolledUV);
				half4 e = tex2D(_TdTex, IN.uv_MainTex);
				d.rgb = c.b * _FlowColor.rgb * e.r + d.rgb;
			}
			   
			o.Albedo = d.rgb;
			o.Alpha = d.a;
			o.Specular = 120;
			o.Gloss = 1;
		}

		ENDCG
	}
	FallBack "Diffuse"
}