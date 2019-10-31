// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:False,nrmq:0,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:False,rfrpn:Refraction,coma:15,ufog:True,aust:False,igpj:True,qofs:0,qpre:3,rntp:2,fgom:True,fgoc:False,fgod:False,fgor:True,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:45,fgrf:100,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:1,x:34836,y:32712,varname:node_1,prsc:2|emission-654-OUT,alpha-700-OUT;n:type:ShaderForge.SFN_Tex2d,id:2,x:33820,y:32696,ptovrint:False,ptlb:T01,ptin:_T01,varname:node_6116,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:b66bceaf0cc0ace4e9bdc92f14bba709,ntxv:0,isnm:False|UVIN-554-OUT;n:type:ShaderForge.SFN_Tex2d,id:7,x:33285,y:32654,ptovrint:False,ptlb:niuqu,ptin:_niuqu,varname:node_674,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-15-UVOUT;n:type:ShaderForge.SFN_Panner,id:15,x:33121,y:32615,varname:node_15,prsc:2,spu:0,spv:1|UVIN-743-UVOUT,DIST-631-OUT;n:type:ShaderForge.SFN_Multiply,id:217,x:33493,y:32591,varname:node_217,prsc:2|A-589-OUT,B-7-RGB;n:type:ShaderForge.SFN_TexCoord,id:496,x:33493,y:32716,varname:node_496,prsc:2,uv:0;n:type:ShaderForge.SFN_Add,id:554,x:33655,y:32696,varname:node_554,prsc:2|A-217-OUT,B-496-UVOUT;n:type:ShaderForge.SFN_ValueProperty,id:589,x:33285,y:32569,ptovrint:False,ptlb:niuqudu,ptin:_niuqudu,varname:node_6894,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.2;n:type:ShaderForge.SFN_Time,id:630,x:32803,y:32557,varname:node_630,prsc:2;n:type:ShaderForge.SFN_Multiply,id:631,x:32960,y:32635,varname:node_631,prsc:2|A-630-T,B-633-OUT;n:type:ShaderForge.SFN_ValueProperty,id:633,x:32780,y:32760,ptovrint:False,ptlb:niuqu_sudu,ptin:_niuqu_sudu,varname:node_5502,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:643,x:34203,y:32981,varname:node_643,prsc:2|A-2-R,B-647-R;n:type:ShaderForge.SFN_Tex2d,id:647,x:34021,y:32994,ptovrint:False,ptlb:mask,ptin:_mask,varname:node_6908,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:3a5a96df060a5cf4a9cc0c59e13486b7,ntxv:0,isnm:False|UVIN-740-OUT;n:type:ShaderForge.SFN_VertexColor,id:653,x:34456,y:32673,varname:node_653,prsc:2;n:type:ShaderForge.SFN_Multiply,id:654,x:34660,y:32635,varname:node_654,prsc:2|A-659-OUT,B-653-RGB;n:type:ShaderForge.SFN_Multiply,id:659,x:34144,y:32633,varname:node_659,prsc:2|A-2-RGB,B-664-RGB;n:type:ShaderForge.SFN_Color,id:664,x:33996,y:32765,ptovrint:False,ptlb:Color_T01,ptin:_Color_T01,varname:node_3076,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:669,x:34456,y:32873,varname:node_669,prsc:2|A-675-OUT,B-643-OUT;n:type:ShaderForge.SFN_ValueProperty,id:675,x:34291,y:32863,ptovrint:False,ptlb:Intensity_alpha,ptin:_Intensity_alpha,varname:node_59,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:700,x:34628,y:32842,varname:node_700,prsc:2|A-653-A,B-669-OUT;n:type:ShaderForge.SFN_Add,id:740,x:33841,y:32991,varname:node_740,prsc:2|A-217-OUT,B-743-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:743,x:32960,y:32377,varname:node_743,prsc:2,uv:0;proporder:2-664-7-589-633-647-675;pass:END;sub:END;*/

Shader "Shader Forge/smokeA" {
    Properties {
        _T01 ("T01", 2D) = "white" {}
        _Color_T01 ("Color_T01", Color) = (1,1,1,1)
        _niuqu ("niuqu", 2D) = "white" {}
        _niuqudu ("niuqudu", Float ) = 0.2
        _niuqu_sudu ("niuqu_sudu", Float ) = 1
        _mask ("mask", 2D) = "white" {}
        _Intensity_alpha ("Intensity_alpha", Float ) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _T01; uniform float4 _T01_ST;
            uniform sampler2D _niuqu; uniform float4 _niuqu_ST;
            uniform float _niuqudu;
            uniform float _niuqu_sudu;
            uniform sampler2D _mask; uniform float4 _mask_ST;
            uniform float4 _Color_T01;
            uniform float _Intensity_alpha;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 node_630 = _Time + _TimeEditor;
                float2 node_15 = (i.uv0+(node_630.g*_niuqu_sudu)*float2(0,1));
                float4 _niuqu_var = tex2D(_niuqu,TRANSFORM_TEX(node_15, _niuqu));
                float3 node_217 = (_niuqudu*_niuqu_var.rgb);
                float3 node_554 = (node_217+float3(i.uv0,0.0));
                float4 _T01_var = tex2D(_T01,TRANSFORM_TEX(node_554, _T01));
                float3 emissive = ((_T01_var.rgb*_Color_T01.rgb)*i.vertexColor.rgb);
                float3 finalColor = emissive;
                float3 node_740 = (node_217+float3(i.uv0,0.0));
                float4 _mask_var = tex2D(_mask,TRANSFORM_TEX(node_740, _mask));
                fixed4 finalRGBA = fixed4(finalColor,(i.vertexColor.a*(_Intensity_alpha*(_T01_var.r*_mask_var.r))));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
