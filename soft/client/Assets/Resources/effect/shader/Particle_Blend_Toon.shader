// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:False,nrmq:0,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:False,rfrpn:Refraction,coma:15,ufog:True,aust:False,igpj:True,qofs:0,qpre:3,rntp:2,fgom:True,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:45,fgrf:100,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:1,x:35165,y:32614,varname:node_1,prsc:2|emission-654-OUT,alpha-6745-OUT;n:type:ShaderForge.SFN_Tex2d,id:2,x:33848,y:32694,ptovrint:False,ptlb:E01,ptin:_E01,varname:_E01,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:b66bceaf0cc0ace4e9bdc92f14bba709,ntxv:0,isnm:False|UVIN-2473-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:7,x:33090,y:32652,ptovrint:False,ptlb:DisE01,ptin:_DisE01,varname:_DisE01,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-7443-OUT;n:type:ShaderForge.SFN_Panner,id:15,x:32776,y:32564,varname:node_15,prsc:2,spu:1,spv:0|UVIN-6331-UVOUT,DIST-631-OUT;n:type:ShaderForge.SFN_Multiply,id:217,x:33298,y:32589,varname:node_217,prsc:2|A-589-OUT,B-7-R;n:type:ShaderForge.SFN_Add,id:554,x:33460,y:32694,varname:node_554,prsc:2|A-217-OUT,B-6331-UVOUT;n:type:ShaderForge.SFN_ValueProperty,id:589,x:33090,y:32567,ptovrint:False,ptlb:DisE01_value,ptin:_DisE01_value,varname:_DisE01_value,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Time,id:630,x:32440,y:32567,varname:node_630,prsc:2;n:type:ShaderForge.SFN_Multiply,id:631,x:32615,y:32635,varname:node_631,prsc:2|A-630-T,B-633-OUT;n:type:ShaderForge.SFN_ValueProperty,id:633,x:32440,y:32758,ptovrint:False,ptlb:DisE01_UVpan_Speed,ptin:_DisE01_UVpan_Speed,varname:_DisE01_UVpan_Speed,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Tex2d,id:647,x:33859,y:32956,ptovrint:False,ptlb:Alpha01,ptin:_Alpha01,varname:_MaskE01,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:3a5a96df060a5cf4a9cc0c59e13486b7,ntxv:0,isnm:False|UVIN-6865-UVOUT;n:type:ShaderForge.SFN_VertexColor,id:653,x:34540,y:32673,varname:node_653,prsc:2;n:type:ShaderForge.SFN_Multiply,id:654,x:34724,y:32635,cmnt:在UI里和背景做透明,varname:node_654,prsc:2|A-636-OUT,B-653-RGB;n:type:ShaderForge.SFN_Multiply,id:659,x:34158,y:32633,varname:node_659,prsc:2|A-664-RGB,B-2-RGB;n:type:ShaderForge.SFN_Color,id:664,x:34020,y:32479,ptovrint:False,ptlb:E01_Color,ptin:_E01_Color,varname:_E01_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:675,x:34267,y:32449,ptovrint:False,ptlb:E01_Bright,ptin:_E01_Bright,varname:_E01_Bright,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Panner,id:4833,x:32776,y:32785,varname:node_4833,prsc:2,spu:0,spv:1|UVIN-6331-UVOUT,DIST-631-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:7443,x:32932,y:32701,ptovrint:False,ptlb:DisE01_U/Vpan,ptin:_DisE01_UVpan,varname:_DisE01_UVpan,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-15-UVOUT,B-4833-UVOUT;n:type:ShaderForge.SFN_Rotator,id:2473,x:33688,y:32694,varname:node_2473,prsc:2|UVIN-554-OUT,ANG-9784-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5289,x:33437,y:32963,ptovrint:False,ptlb:E01_UVangle,ptin:_E01_UVangle,varname:_E01_UVangle,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_TexCoord,id:6331,x:32589,y:32812,varname:node_6331,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:3184,x:34740,y:32887,varname:node_3184,prsc:2|A-9128-OUT,B-7035-OUT;n:type:ShaderForge.SFN_Multiply,id:6968,x:34148,y:32924,varname:node_6968,prsc:2|A-2-R,B-647-R;n:type:ShaderForge.SFN_ValueProperty,id:7035,x:34546,y:33242,ptovrint:False,ptlb:Alpha01_Bright,ptin:_Alpha01_Bright,varname:node_7035,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:9784,x:33621,y:32963,varname:node_9784,prsc:2|A-5289-OUT,B-3295-OUT;n:type:ShaderForge.SFN_Pi,id:3295,x:33431,y:33086,varname:node_3295,prsc:2;n:type:ShaderForge.SFN_If,id:9128,x:34557,y:32887,varname:node_9128,prsc:2|A-3505-OUT,B-9735-OUT,GT-1232-OUT,EQ-1232-OUT,LT-6369-OUT;n:type:ShaderForge.SFN_Multiply,id:3505,x:34381,y:32887,varname:node_3505,prsc:2|A-653-A,B-6968-OUT;n:type:ShaderForge.SFN_Vector1,id:1232,x:34375,y:33253,varname:node_1232,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:6369,x:34375,y:33329,varname:node_6369,prsc:2,v1:0;n:type:ShaderForge.SFN_Rotator,id:6865,x:33724,y:33172,varname:node_6865,prsc:2|UVIN-6331-UVOUT,ANG-8568-OUT;n:type:ShaderForge.SFN_Multiply,id:8568,x:33446,y:33422,varname:node_8568,prsc:2|A-58-OUT,B-2314-OUT;n:type:ShaderForge.SFN_Pi,id:58,x:33153,y:33421,varname:node_58,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:2314,x:33153,y:33603,ptovrint:False,ptlb:Alpha01_UVangle,ptin:_Alpha01_UVangle,varname:node_1273,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Add,id:636,x:34426,y:32584,varname:node_636,prsc:2|A-675-OUT,B-659-OUT;n:type:ShaderForge.SFN_Add,id:6745,x:34930,y:32887,varname:node_6745,prsc:2|A-3184-OUT,B-9597-OUT;n:type:ShaderForge.SFN_Multiply,id:9597,x:34796,y:33193,varname:node_9597,prsc:2|A-3505-OUT,B-5687-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5687,x:34556,y:33353,ptovrint:False,ptlb:Glow_Brght,ptin:_Glow_Brght,varname:_Alpha01_Bright_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:9735,x:34188,y:33253,ptovrint:False,ptlb:AlphaClip,ptin:_AlphaClip,varname:_Glow_Brght_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.01;proporder:675-2-664-5289-7-589-633-7443-647-2314-7035-9735-5687;pass:END;sub:END;*/

Shader "yh/Particle/Particle_Blend_Toon" {
    Properties {
        _E01_Bright ("E01_Bright", Float ) = 1
        _E01 ("E01", 2D) = "white" {}
        _E01_Color ("E01_Color", Color) = (1,1,1,1)
        _E01_UVangle ("E01_UVangle", Float ) = 0
        _DisE01 ("DisE01", 2D) = "white" {}
        _DisE01_value ("DisE01_value", Float ) = 0
        _DisE01_UVpan_Speed ("DisE01_UVpan_Speed", Float ) = 0
        [MaterialToggle] _DisE01_UVpan ("DisE01_U/Vpan", Float ) = 0
        _Alpha01 ("Alpha01", 2D) = "white" {}
        _Alpha01_UVangle ("Alpha01_UVangle", Float ) = 0
        _Alpha01_Bright ("Alpha01_Bright", Float ) = 1
        _AlphaClip ("AlphaClip", Float ) = 0.01
        _Glow_Brght ("Glow_Brght", Float ) = 0
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
            uniform sampler2D _Alpha01; uniform float4 _Alpha01_ST;
            uniform float4 _E01_Color;
            uniform float _E01_Bright;
            uniform fixed _DisE01_UVpan;
            uniform float _E01_UVangle;
            uniform float _Alpha01_Bright;
            uniform float _Alpha01_UVangle;
            uniform float _Glow_Brght;
            uniform float _AlphaClip;
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
                float3 emissive = ((_E01_Bright+(_E01_Color.rgb*_E01_var.rgb))*i.vertexColor.rgb);
                float3 finalColor = emissive;
                float node_6865_ang = (3.141592654*_Alpha01_UVangle);
                float node_6865_spd = 1.0;
                float node_6865_cos = cos(node_6865_spd*node_6865_ang);
                float node_6865_sin = sin(node_6865_spd*node_6865_ang);
                float2 node_6865_piv = float2(0.5,0.5);
                float2 node_6865 = (mul(i.uv0-node_6865_piv,float2x2( node_6865_cos, -node_6865_sin, node_6865_sin, node_6865_cos))+node_6865_piv);
                float4 _Alpha01_var = tex2D(_Alpha01,TRANSFORM_TEX(node_6865, _Alpha01));
                float node_3505 = (i.vertexColor.a*(_E01_var.r*_Alpha01_var.r));
                float node_9128_if_leA = step(node_3505,_AlphaClip);
                float node_9128_if_leB = step(_AlphaClip,node_3505);
                float node_1232 = 1.0;
                fixed4 finalRGBA = fixed4(finalColor,((lerp((node_9128_if_leA*0.0)+(node_9128_if_leB*node_1232),node_1232,node_9128_if_leA*node_9128_if_leB)*_Alpha01_Bright)+(node_3505*_Glow_Brght)));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
