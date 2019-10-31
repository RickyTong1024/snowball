// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:False,nrmq:0,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,rfrpo:False,rfrpn:Refraction,coma:15,ufog:True,aust:False,igpj:True,qofs:0,qpre:3,rntp:2,fgom:True,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:45,fgrf:100,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:1,x:34900,y:32712,varname:node_1,prsc:2|emission-669-OUT;n:type:ShaderForge.SFN_Tex2d,id:2,x:33793,y:32694,ptovrint:False,ptlb:E01,ptin:_E01,varname:_E01,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:b66bceaf0cc0ace4e9bdc92f14bba709,ntxv:0,isnm:False|UVIN-2473-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:7,x:33090,y:32652,ptovrint:False,ptlb:DisE01,ptin:_DisE01,varname:_DisE01,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-7443-OUT;n:type:ShaderForge.SFN_Panner,id:15,x:32776,y:32564,varname:node_15,prsc:2,spu:1,spv:0|UVIN-6331-UVOUT,DIST-631-OUT;n:type:ShaderForge.SFN_Multiply,id:217,x:33298,y:32589,varname:node_217,prsc:2|A-589-OUT,B-7-R;n:type:ShaderForge.SFN_Add,id:554,x:33460,y:32694,varname:node_554,prsc:2|A-217-OUT,B-6331-UVOUT;n:type:ShaderForge.SFN_ValueProperty,id:589,x:33090,y:32567,ptovrint:False,ptlb:DisE01_value,ptin:_DisE01_value,varname:_DisE01_value,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Time,id:630,x:32440,y:32567,varname:node_630,prsc:2;n:type:ShaderForge.SFN_Multiply,id:631,x:32615,y:32635,varname:node_631,prsc:2|A-630-T,B-633-OUT;n:type:ShaderForge.SFN_ValueProperty,id:633,x:32440,y:32758,ptovrint:False,ptlb:DisE01_UVpan_Speed,ptin:_DisE01_UVpan_Speed,varname:_DisE01_UVpan_Speed,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:643,x:34487,y:32695,varname:node_643,prsc:2|A-8862-OUT,B-653-RGB,C-647-R;n:type:ShaderForge.SFN_Tex2d,id:647,x:34176,y:32958,ptovrint:False,ptlb:MaskE01,ptin:_MaskE01,varname:_MaskE01,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:3a5a96df060a5cf4a9cc0c59e13486b7,ntxv:0,isnm:False|UVIN-8800-UVOUT;n:type:ShaderForge.SFN_VertexColor,id:653,x:33770,y:32942,varname:node_653,prsc:2;n:type:ShaderForge.SFN_Multiply,id:654,x:33990,y:32694,cmnt:在UI里和背景做透明,varname:node_654,prsc:2|A-2-RGB,B-653-A;n:type:ShaderForge.SFN_Color,id:664,x:34385,y:33299,ptovrint:False,ptlb:E01_Color,ptin:_E01_Color,varname:_E01_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:669,x:34683,y:32775,varname:node_669,prsc:2|A-643-OUT,B-4042-OUT,C-664-RGB;n:type:ShaderForge.SFN_Panner,id:4833,x:32776,y:32785,varname:node_4833,prsc:2,spu:0,spv:1|UVIN-6331-UVOUT,DIST-631-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:7443,x:32932,y:32701,ptovrint:False,ptlb:DisE01_U/Vpan,ptin:_DisE01_UVpan,varname:_DisE01_UVpan,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-15-UVOUT,B-4833-UVOUT;n:type:ShaderForge.SFN_Rotator,id:2473,x:33627,y:32694,varname:node_2473,prsc:2|UVIN-554-OUT,ANG-1784-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5289,x:33298,y:32946,ptovrint:False,ptlb:E01_UVangle,ptin:_E01_UVangle,varname:_E01_UVangle,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_TexCoord,id:6331,x:32589,y:32812,varname:node_6331,prsc:2,uv:0;n:type:ShaderForge.SFN_Slider,id:4042,x:34120,y:33167,ptovrint:False,ptlb:E01_Bright,ptin:_E01_Bright,varname:node_4042,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:2;n:type:ShaderForge.SFN_If,id:2584,x:34151,y:32694,varname:node_2584,prsc:2|A-654-OUT,B-457-OUT,GT-1995-OUT,EQ-1995-OUT,LT-5957-OUT;n:type:ShaderForge.SFN_Vector1,id:1995,x:33761,y:33258,varname:node_1995,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:5957,x:33758,y:33341,varname:node_5957,prsc:2,v1:0;n:type:ShaderForge.SFN_Multiply,id:1784,x:33458,y:32946,varname:node_1784,prsc:2|A-5289-OUT,B-3242-OUT;n:type:ShaderForge.SFN_Pi,id:3242,x:33314,y:33043,varname:node_3242,prsc:2;n:type:ShaderForge.SFN_Rotator,id:8800,x:34075,y:33516,varname:node_8800,prsc:2|UVIN-6331-UVOUT,ANG-3377-OUT;n:type:ShaderForge.SFN_Multiply,id:3377,x:33936,y:33686,varname:node_3377,prsc:2|A-8336-OUT,B-9456-OUT;n:type:ShaderForge.SFN_Pi,id:8336,x:33749,y:33695,varname:node_8336,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:9456,x:33749,y:33821,ptovrint:False,ptlb:MaskE01_UVangle,ptin:_MaskE01_UVangle,varname:node_1273,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:5070,x:34196,y:32399,varname:node_5070,prsc:2|A-654-OUT,B-2458-OUT;n:type:ShaderForge.SFN_Add,id:8862,x:34319,y:32673,varname:node_8862,prsc:2|A-5070-OUT,B-2584-OUT;n:type:ShaderForge.SFN_ValueProperty,id:457,x:33758,y:33171,ptovrint:False,ptlb:AlphaClip,ptin:_AlphaClip,varname:node_457,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.05;n:type:ShaderForge.SFN_ValueProperty,id:2458,x:33852,y:32536,ptovrint:False,ptlb:Glow_Bright,ptin:_Glow_Bright,varname:node_2458,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;proporder:4042-2-664-5289-7-589-633-7443-647-9456-457-2458;pass:END;sub:END;*/

Shader "yh/Particle/Particle_Add_Toon" {
    Properties {
        _E01_Bright ("E01_Bright", Range(0, 2)) = 1
        _E01 ("E01", 2D) = "white" {}
        _E01_Color ("E01_Color", Color) = (1,1,1,1)
        _E01_UVangle ("E01_UVangle", Float ) = 0
        _DisE01 ("DisE01", 2D) = "white" {}
        _DisE01_value ("DisE01_value", Float ) = 0
        _DisE01_UVpan_Speed ("DisE01_UVpan_Speed", Float ) = 0
        [MaterialToggle] _DisE01_UVpan ("DisE01_U/Vpan", Float ) = 0
        _MaskE01 ("MaskE01", 2D) = "white" {}
        _MaskE01_UVangle ("MaskE01_UVangle", Float ) = 0
        _AlphaClip ("AlphaClip", Float ) = 0.05
        _Glow_Bright ("Glow_Bright", Float ) = 1
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
            Blend One One
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 ps3 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _E01; uniform float4 _E01_ST;
            uniform sampler2D _DisE01; uniform float4 _DisE01_ST;
            uniform float _DisE01_value;
            uniform float _DisE01_UVpan_Speed;
            uniform sampler2D _MaskE01; uniform float4 _MaskE01_ST;
            uniform float4 _E01_Color;
            uniform fixed _DisE01_UVpan;
            uniform float _E01_UVangle;
            uniform float _E01_Bright;
            uniform float _MaskE01_UVangle;
            uniform float _AlphaClip;
            uniform float _Glow_Bright;
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
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float node_2473_ang = (_E01_UVangle*3.141592654);
                float node_2473_spd = 1.0;
                float node_2473_cos = cos(node_2473_spd*node_2473_ang);
                float node_2473_sin = sin(node_2473_spd*node_2473_ang);
                float2 node_2473_piv = float2(0.5,0.5);
                float4 node_630 = _Time + _TimeEditor;
                float node_631 = (node_630.g*_DisE01_UVpan_Speed);
                float2 _DisE01_UVpan_var = lerp( (i.uv0+node_631*float2(1,0)), (i.uv0+node_631*float2(0,1)), _DisE01_UVpan );
                float4 _DisE01_var = tex2D(_DisE01,TRANSFORM_TEX(_DisE01_UVpan_var, _DisE01));
                float2 node_2473 = (mul(((_DisE01_value*_DisE01_var.r)+i.uv0)-node_2473_piv,float2x2( node_2473_cos, -node_2473_sin, node_2473_sin, node_2473_cos))+node_2473_piv);
                float4 _E01_var = tex2D(_E01,TRANSFORM_TEX(node_2473, _E01));
                float3 node_654 = (_E01_var.rgb*i.vertexColor.a); // 在UI里和背景做透明
                float node_2584_if_leA = step(node_654,_AlphaClip);
                float node_2584_if_leB = step(_AlphaClip,node_654);
                float node_1995 = 1.0;
                float node_8800_ang = (3.141592654*_MaskE01_UVangle);
                float node_8800_spd = 1.0;
                float node_8800_cos = cos(node_8800_spd*node_8800_ang);
                float node_8800_sin = sin(node_8800_spd*node_8800_ang);
                float2 node_8800_piv = float2(0.5,0.5);
                float2 node_8800 = (mul(i.uv0-node_8800_piv,float2x2( node_8800_cos, -node_8800_sin, node_8800_sin, node_8800_cos))+node_8800_piv);
                float4 _MaskE01_var = tex2D(_MaskE01,TRANSFORM_TEX(node_8800, _MaskE01));
                float3 emissive = ((((node_654*_Glow_Bright)+lerp((node_2584_if_leA*0.0)+(node_2584_if_leB*node_1995),node_1995,node_2584_if_leA*node_2584_if_leB))*i.vertexColor.rgb*_MaskE01_var.r)*_E01_Bright*_E01_Color.rgb);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
