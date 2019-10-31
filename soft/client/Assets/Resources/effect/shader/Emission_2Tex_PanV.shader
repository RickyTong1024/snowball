// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge Beta 0.36 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.36;sub:START;pass:START;ps:flbk:,lico:0,lgpr:1,nrmq:1,limd:0,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:5,bsrc:2,bdst:0,culm:2,dpts:2,wrdp:False,ufog:True,aust:False,igpj:False,qofs:1,qpre:3,rntp:2,fgom:True,fgoc:True,fgod:False,fgor:True,fgmd:1,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.005,fgrn:60,fgrf:150,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32630,y:32507|emission-29-OUT;n:type:ShaderForge.SFN_Tex2d,id:2,x:34409,y:32892,ptlb:DisT01,ptin:_DisT01,tex:899c5fd66853a0a4a976e047fe41bfd7,ntxv:0,isnm:False|UVIN-8-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:3,x:33854,y:32872,ptlb:Tex01,ptin:_Tex01,tex:e5c2d246f2b01444391ec767bb59399c,ntxv:0,isnm:False|UVIN-11-OUT;n:type:ShaderForge.SFN_Tex2d,id:4,x:33871,y:32544,ptlb:Tex02,ptin:_Tex02,tex:a9b9ff1d9e59c7c4f9a4c5ca16654838,ntxv:2,isnm:False|UVIN-20-UVOUT;n:type:ShaderForge.SFN_Time,id:5,x:34908,y:32891;n:type:ShaderForge.SFN_ValueProperty,id:6,x:34908,y:33022,ptlb:PanY_DisT01,ptin:_PanY_DisT01,glob:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:7,x:34725,y:32912|A-5-T,B-6-OUT;n:type:ShaderForge.SFN_Panner,id:8,x:34569,y:32892,spu:0,spv:1|DIST-7-OUT;n:type:ShaderForge.SFN_Multiply,id:9,x:34223,y:32872|A-10-OUT,B-2-RGB;n:type:ShaderForge.SFN_ValueProperty,id:10,x:34385,y:32807,ptlb:DisT01Power,ptin:_DisT01Power,glob:False,v1:0;n:type:ShaderForge.SFN_Add,id:11,x:34023,y:32872|A-9-OUT,B-12-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:12,x:34023,y:33015,uv:0;n:type:ShaderForge.SFN_Color,id:13,x:33746,y:32743,ptlb:Color_T01,ptin:_Color_T01,glob:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:14,x:33585,y:32853|A-13-RGB,B-3-RGB;n:type:ShaderForge.SFN_Color,id:16,x:33750,y:32415,ptlb:Color_T02,ptin:_Color_T02,glob:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:18,x:33589,y:32525|A-16-RGB,B-4-RGB;n:type:ShaderForge.SFN_Panner,id:20,x:34046,y:32544,spu:0,spv:1|DIST-22-OUT;n:type:ShaderForge.SFN_Multiply,id:22,x:34215,y:32564|A-26-T,B-24-OUT;n:type:ShaderForge.SFN_ValueProperty,id:24,x:34386,y:32674,ptlb:PanY_T02,ptin:_PanY_T02,glob:False,v1:1;n:type:ShaderForge.SFN_Time,id:26,x:34386,y:32543;n:type:ShaderForge.SFN_Add,id:27,x:33390,y:32672|A-18-OUT,B-14-OUT;n:type:ShaderForge.SFN_VertexColor,id:28,x:33026,y:32675;n:type:ShaderForge.SFN_Multiply,id:29,x:32863,y:32608|A-90-OUT,B-28-RGB,C-28-A;n:type:ShaderForge.SFN_Multiply,id:41,x:33195,y:32606|A-44-R,B-27-OUT;n:type:ShaderForge.SFN_Tex2d,id:44,x:33390,y:32489,ptlb:Mask,ptin:_Mask,tex:a3de474356606594493c3c344cbf0f8f,ntxv:0,isnm:False;n:type:ShaderForge.SFN_ValueProperty,id:50,x:33214,y:32413,ptlb:Intensity,ptin:_Intensity,glob:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:90,x:33042,y:32499|A-50-OUT,B-41-OUT;proporder:3-2-10-6-4-24-44-50-13-16;pass:END;sub:END;*/

Shader "Shader Forge/Emission_2Tex_PanV" {
    Properties {
        _Tex01 ("Tex01", 2D) = "white" {}
        _DisT01 ("DisT01", 2D) = "white" {}
        _DisT01Power ("DisT01Power", Float ) = 0
        _PanY_DisT01 ("PanY_DisT01", Float ) = 1
        _Tex02 ("Tex02", 2D) = "black" {}
        _PanY_T02 ("PanY_T02", Float ) = 1
        _Mask ("Mask", 2D) = "white" {}
        _Intensity ("Intensity", Float ) = 1
        _Color_T01 ("Color_T01", Color) = (0.5,0.5,0.5,1)
        _Color_T02 ("Color_T02", Color) = (0.5,0.5,0.5,1)
    }
    SubShader {
        Tags {
            "Queue"="Transparent+1"
            "RenderType"="Transparent"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcColor One
            Cull Off
            ZWrite Off
            
            Fog {Mode Linear}
            Fog { Color (0,0,0,1) }
            Fog {Range 60,150}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _DisT01; uniform float4 _DisT01_ST;
            uniform sampler2D _Tex01; uniform float4 _Tex01_ST;
            uniform sampler2D _Tex02; uniform float4 _Tex02_ST;
            uniform float _PanY_DisT01;
            uniform float _DisT01Power;
            uniform float4 _Color_T01;
            uniform float4 _Color_T02;
            uniform float _PanY_T02;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            uniform float _Intensity;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float2 node_96 = i.uv0;
                float4 node_26 = _Time + _TimeEditor;
                float2 node_20 = (node_96.rg+(node_26.g*_PanY_T02)*float2(0,1));
                float4 node_5 = _Time + _TimeEditor;
                float2 node_8 = (node_96.rg+(node_5.g*_PanY_DisT01)*float2(0,1));
                float3 node_11 = ((_DisT01Power*tex2D(_DisT01,TRANSFORM_TEX(node_8, _DisT01)).rgb)+float3(i.uv0.rg,0.0));
                float4 node_28 = i.vertexColor;
                float3 emissive = ((_Intensity*(tex2D(_Mask,TRANSFORM_TEX(node_96.rg, _Mask)).r*((_Color_T02.rgb*tex2D(_Tex02,TRANSFORM_TEX(node_20, _Tex02)).rgb)+(_Color_T01.rgb*tex2D(_Tex01,TRANSFORM_TEX(node_11, _Tex01)).rgb))))*node_28.rgb*node_28.a);
                float3 finalColor = emissive;
/// Final Color:
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
