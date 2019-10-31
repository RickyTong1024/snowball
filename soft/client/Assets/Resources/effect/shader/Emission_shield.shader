// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge Beta 0.36 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.36;sub:START;pass:START;ps:flbk:,lico:0,lgpr:1,nrmq:1,limd:0,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:5,bsrc:2,bdst:0,culm:2,dpts:2,wrdp:False,ufog:True,aust:False,igpj:False,qofs:1,qpre:3,rntp:2,fgom:True,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.005,fgrn:60,fgrf:150,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32630,y:32507|emission-29-OUT;n:type:ShaderForge.SFN_Tex2d,id:2,x:34383,y:32574,ptlb:DisT01,ptin:_DisT01,tex:899c5fd66853a0a4a976e047fe41bfd7,ntxv:0,isnm:False|UVIN-8-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:3,x:33828,y:32554,ptlb:Tex01,ptin:_Tex01,tex:e5c2d246f2b01444391ec767bb59399c,ntxv:0,isnm:False|UVIN-11-OUT;n:type:ShaderForge.SFN_Tex2d,id:4,x:33843,y:32966,ptlb:Tex02,ptin:_Tex02,tex:a9b9ff1d9e59c7c4f9a4c5ca16654838,ntxv:2,isnm:False|UVIN-20-UVOUT;n:type:ShaderForge.SFN_Time,id:5,x:34882,y:32573;n:type:ShaderForge.SFN_ValueProperty,id:6,x:34882,y:32704,ptlb:PanY_DisT01,ptin:_PanY_DisT01,glob:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:7,x:34699,y:32594|A-5-T,B-6-OUT;n:type:ShaderForge.SFN_Panner,id:8,x:34543,y:32574,spu:0,spv:1|DIST-7-OUT;n:type:ShaderForge.SFN_Multiply,id:9,x:34197,y:32554|A-10-OUT,B-2-RGB;n:type:ShaderForge.SFN_ValueProperty,id:10,x:34359,y:32489,ptlb:DisT01Power,ptin:_DisT01Power,glob:False,v1:0;n:type:ShaderForge.SFN_Add,id:11,x:33997,y:32554|A-9-OUT,B-12-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:12,x:33997,y:32697,uv:0;n:type:ShaderForge.SFN_Color,id:13,x:33720,y:32425,ptlb:Color_T01,ptin:_Color_T01,glob:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:14,x:33559,y:32535|A-13-RGB,B-3-RGB;n:type:ShaderForge.SFN_Color,id:16,x:33722,y:32837,ptlb:Color_T02,ptin:_Color_T02,glob:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:18,x:33561,y:32947|A-16-RGB,B-4-RGB;n:type:ShaderForge.SFN_Panner,id:20,x:34018,y:32966,spu:0,spv:1|DIST-102-OUT;n:type:ShaderForge.SFN_ValueProperty,id:24,x:34369,y:33027,ptlb:PanY_T02,ptin:_PanY_T02,glob:False,v1:1;n:type:ShaderForge.SFN_Add,id:27,x:33390,y:32672|A-14-OUT,B-18-OUT;n:type:ShaderForge.SFN_VertexColor,id:28,x:33026,y:32675;n:type:ShaderForge.SFN_Multiply,id:29,x:32863,y:32608|A-90-OUT,B-28-RGB,C-28-A;n:type:ShaderForge.SFN_Multiply,id:41,x:33195,y:32606|A-44-R,B-27-OUT;n:type:ShaderForge.SFN_Tex2d,id:44,x:33390,y:32489,ptlb:Mask,ptin:_Mask,tex:a3de474356606594493c3c344cbf0f8f,ntxv:0,isnm:False;n:type:ShaderForge.SFN_ValueProperty,id:50,x:33214,y:32413,ptlb:Intensity_Mask,ptin:_Intensity_Mask,glob:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:90,x:33042,y:32499|A-50-OUT,B-41-OUT;n:type:ShaderForge.SFN_Multiply,id:102,x:34192,y:33010|A-28-A,B-24-OUT;proporder:3-13-2-10-6-4-16-24-44-50;pass:END;sub:END;*/

Shader "Shader Forge/Emission_shield" {
    Properties {
        _Tex01 ("Tex01", 2D) = "white" {}
        _Color_T01 ("Color_T01", Color) = (0.5,0.5,0.5,1)
        _DisT01 ("DisT01", 2D) = "white" {}
        _DisT01Power ("DisT01Power", Float ) = 0
        _PanY_DisT01 ("PanY_DisT01", Float ) = 1
        _Tex02 ("Tex02", 2D) = "black" {}
        _Color_T02 ("Color_T02", Color) = (0.5,0.5,0.5,1)
        _PanY_T02 ("PanY_T02", Float ) = 1
        _Mask ("Mask", 2D) = "white" {}
        _Intensity_Mask ("Intensity_Mask", Float ) = 1
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
            
            Fog {Mode Global}
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
            uniform float _Intensity_Mask;
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
                float2 node_108 = i.uv0;
                float4 node_5 = _Time + _TimeEditor;
                float2 node_8 = (node_108.rg+(node_5.g*_PanY_DisT01)*float2(0,1));
                float3 node_11 = ((_DisT01Power*tex2D(_DisT01,TRANSFORM_TEX(node_8, _DisT01)).rgb)+float3(i.uv0.rg,0.0));
                float4 node_28 = i.vertexColor;
                float2 node_20 = (node_108.rg+(node_28.a*_PanY_T02)*float2(0,1));
                float3 emissive = ((_Intensity_Mask*(tex2D(_Mask,TRANSFORM_TEX(node_108.rg, _Mask)).r*((_Color_T01.rgb*tex2D(_Tex01,TRANSFORM_TEX(node_11, _Tex01)).rgb)+(_Color_T02.rgb*tex2D(_Tex02,TRANSFORM_TEX(node_20, _Tex02)).rgb))))*node_28.rgb*node_28.a);
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
