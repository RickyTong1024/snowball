// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge Beta 0.36 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.36;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:0,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:5,bsrc:2,bdst:0,culm:2,dpts:2,wrdp:False,ufog:True,aust:False,igpj:False,qofs:1,qpre:3,rntp:2,fgom:True,fgoc:True,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.005,fgrn:60,fgrf:150,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32630,y:32507|emission-29-OUT;n:type:ShaderForge.SFN_Tex2d,id:2,x:34622,y:32685,ptlb:DisT01,ptin:_DisT01,tex:899c5fd66853a0a4a976e047fe41bfd7,ntxv:0,isnm:False|UVIN-8-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:3,x:34103,y:32645,ptlb:Tex01,ptin:_Tex01,tex:98162de2fd8acd845995a2a109fad096,ntxv:0,isnm:False|UVIN-11-OUT;n:type:ShaderForge.SFN_Tex2d,id:4,x:34118,y:33057,ptlb:Tex02,ptin:_Tex02,tex:d1ee51aa13469654eaffaaa70f68ec25,ntxv:2,isnm:False|UVIN-20-UVOUT;n:type:ShaderForge.SFN_Time,id:5,x:35121,y:32684;n:type:ShaderForge.SFN_ValueProperty,id:6,x:35121,y:32815,ptlb:PanY_DisT01,ptin:_PanY_DisT01,glob:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:7,x:34938,y:32705|A-5-T,B-6-OUT;n:type:ShaderForge.SFN_Panner,id:8,x:34782,y:32685,spu:0,spv:1|DIST-7-OUT;n:type:ShaderForge.SFN_Multiply,id:9,x:34436,y:32665|A-10-OUT,B-2-RGB;n:type:ShaderForge.SFN_ValueProperty,id:10,x:34598,y:32600,ptlb:DisT01Power,ptin:_DisT01Power,glob:False,v1:0;n:type:ShaderForge.SFN_Add,id:11,x:34272,y:32645|A-9-OUT,B-12-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:12,x:34436,y:32796,uv:0;n:type:ShaderForge.SFN_Color,id:13,x:33995,y:32497,ptlb:Color_T01,ptin:_Color_T01,glob:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:14,x:33824,y:32614|A-13-RGB,B-3-RGB;n:type:ShaderForge.SFN_Multiply,id:18,x:33836,y:33038|A-109-OUT,B-4-RGB;n:type:ShaderForge.SFN_Panner,id:20,x:34306,y:33057,spu:1,spv:0|DIST-102-OUT;n:type:ShaderForge.SFN_ValueProperty,id:24,x:34660,y:33155,ptlb:PanX_T02,ptin:_PanX_T02,glob:False,v1:1;n:type:ShaderForge.SFN_VertexColor,id:28,x:33057,y:32674;n:type:ShaderForge.SFN_Multiply,id:29,x:32863,y:32607|A-119-OUT,B-28-RGB;n:type:ShaderForge.SFN_Multiply,id:102,x:34495,y:33085|A-28-A,B-24-OUT;n:type:ShaderForge.SFN_ValueProperty,id:109,x:33987,y:32969,ptlb:Intensity_T02,ptin:_Intensity_T02,glob:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:119,x:33291,y:32615|A-156-OUT,B-131-OUT;n:type:ShaderForge.SFN_Tex2d,id:120,x:33403,y:33350,ptlb:Mask,ptin:_Mask,tex:61768f446f8ba9244a57115e4b0712bc,ntxv:2,isnm:False|UVIN-121-UVOUT;n:type:ShaderForge.SFN_Panner,id:121,x:33577,y:33350,spu:0,spv:1|DIST-127-OUT;n:type:ShaderForge.SFN_Time,id:123,x:33934,y:33350;n:type:ShaderForge.SFN_ValueProperty,id:125,x:33934,y:33481,ptlb:PanY_TexAlpha,ptin:_PanY_TexAlpha,glob:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:127,x:33751,y:33371|A-123-T,B-125-OUT;n:type:ShaderForge.SFN_Multiply,id:131,x:33144,y:33073|A-132-OUT,B-120-RGB;n:type:ShaderForge.SFN_ValueProperty,id:132,x:33405,y:33104,ptlb:Intensity_Mask,ptin:_Intensity_Mask,glob:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:156,x:33506,y:32615|A-14-OUT,B-18-OUT;proporder:3-13-2-10-6-4-109-24-120-125-132;pass:END;sub:END;*/

Shader "Shader Forge/Emission_daoguang01" {
    Properties {
        _Tex01 ("Tex01", 2D) = "white" {}
        _Color_T01 ("Color_T01", Color) = (1,1,1,1)
        _DisT01 ("DisT01", 2D) = "white" {}
        _DisT01Power ("DisT01Power", Float ) = 0
        _PanY_DisT01 ("PanY_DisT01", Float ) = 1
        _Tex02 ("Tex02", 2D) = "black" {}
        _Intensity_T02 ("Intensity_T02", Float ) = 1
        _PanX_T02 ("PanX_T02", Float ) = 1
        _Mask ("Mask", 2D) = "black" {}
        _PanY_TexAlpha ("PanY_TexAlpha", Float ) = 0
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
            Fog { Color (0,0,0,1) }
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
            uniform float _PanX_T02;
            uniform float _Intensity_T02;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            uniform float _PanY_TexAlpha;
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
                float4 node_5 = _Time + _TimeEditor;
                float2 node_165 = i.uv0;
                float2 node_8 = (node_165.rg+(node_5.g*_PanY_DisT01)*float2(0,1));
                float3 node_11 = ((_DisT01Power*tex2D(_DisT01,TRANSFORM_TEX(node_8, _DisT01)).rgb)+float3(i.uv0.rg,0.0));
                float4 node_28 = i.vertexColor;
                float2 node_20 = (node_165.rg+(node_28.a*_PanX_T02)*float2(1,0));
                float4 node_123 = _Time + _TimeEditor;
                float2 node_121 = (node_165.rg+(node_123.g*_PanY_TexAlpha)*float2(0,1));
                float3 emissive = ((((_Color_T01.rgb*tex2D(_Tex01,TRANSFORM_TEX(node_11, _Tex01)).rgb)*(_Intensity_T02*tex2D(_Tex02,TRANSFORM_TEX(node_20, _Tex02)).rgb))*(_Intensity_Mask*tex2D(_Mask,TRANSFORM_TEX(node_121, _Mask)).rgb))*node_28.rgb);
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
