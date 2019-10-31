// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:0,lgpr:1,limd:0,spmd:1,trmd:1,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:False,nrmq:0,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:7,dpts:2,wrdp:True,dith:0,rfrpo:False,rfrpn:Refraction,coma:15,ufog:True,aust:False,igpj:False,qofs:0,qpre:3,rntp:2,fgom:True,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.2784314,fgcg:0.2784314,fgcb:0.2784314,fgca:1,fgde:0.01,fgrn:30,fgrf:150,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:6749,x:32719,y:32712,varname:node_6749,prsc:2|emission-2893-OUT,custl-37-OUT,alpha-4805-OUT;n:type:ShaderForge.SFN_LightVector,id:8257,x:29883,y:32627,varname:node_8257,prsc:2;n:type:ShaderForge.SFN_NormalVector,id:6650,x:29883,y:32910,prsc:2,pt:False;n:type:ShaderForge.SFN_Dot,id:5474,x:30094,y:32688,varname:node_5474,prsc:2,dt:0|A-8257-OUT,B-6650-OUT;n:type:ShaderForge.SFN_Tex2d,id:2960,x:30714,y:32670,ptovrint:False,ptlb:DiffuseRamp,ptin:_DiffuseRamp,varname:_DiffuseRamp,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:ed0e30f53a1403e4a8586bad92925d9e,ntxv:2,isnm:False|UVIN-6716-OUT;n:type:ShaderForge.SFN_Append,id:4742,x:30342,y:32670,varname:node_4742,prsc:2|A-5474-OUT,B-5474-OUT;n:type:ShaderForge.SFN_LightAttenuation,id:7648,x:30342,y:32793,varname:node_7648,prsc:2;n:type:ShaderForge.SFN_Dot,id:7467,x:30098,y:33071,varname:node_7467,prsc:2,dt:0|A-6650-OUT,B-6544-OUT;n:type:ShaderForge.SFN_Append,id:7523,x:30339,y:33058,varname:node_7523,prsc:2|A-7467-OUT,B-7467-OUT;n:type:ShaderForge.SFN_Add,id:348,x:31396,y:32820,varname:node_348,prsc:2|A-2535-OUT,B-8594-OUT;n:type:ShaderForge.SFN_Tex2d,id:9229,x:30728,y:33061,ptovrint:False,ptlb:SpecularRamp,ptin:_SpecularRamp,varname:_SpecularRamp,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:2f195ccfea4de9d49992515793382b4f,ntxv:0,isnm:False|UVIN-7523-OUT;n:type:ShaderForge.SFN_Color,id:7332,x:30728,y:33233,ptovrint:False,ptlb:SpecularRamp_Color,ptin:_SpecularRamp_Color,varname:_SpecularRamp_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:8594,x:31206,y:33052,varname:node_8594,prsc:2|A-2462-RGB,B-9297-OUT;n:type:ShaderForge.SFN_LightColor,id:2462,x:31048,y:32857,varname:node_2462,prsc:2;n:type:ShaderForge.SFN_Multiply,id:6716,x:30514,y:32670,varname:node_6716,prsc:2|A-4742-OUT,B-7648-OUT;n:type:ShaderForge.SFN_Multiply,id:2535,x:31247,y:32642,varname:node_2535,prsc:2|A-9992-RGB,B-8515-OUT,C-2462-RGB;n:type:ShaderForge.SFN_HalfVector,id:6544,x:29892,y:33212,varname:node_6544,prsc:2;n:type:ShaderForge.SFN_Add,id:2102,x:30892,y:32670,varname:node_2102,prsc:2|A-2960-R,B-4216-OUT;n:type:ShaderForge.SFN_Clamp01,id:8515,x:31073,y:32670,varname:node_8515,prsc:2|IN-2102-OUT;n:type:ShaderForge.SFN_Slider,id:4216,x:30583,y:32868,ptovrint:False,ptlb:shadow_Bright,ptin:_shadow_Bright,varname:_shadow_Bright,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Cubemap,id:3334,x:31396,y:32972,ptovrint:False,ptlb:cubemap,ptin:_cubemap,varname:_cubemap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,pvfc:0;n:type:ShaderForge.SFN_Color,id:9992,x:30879,y:32514,ptovrint:False,ptlb:Diffuse_Color,ptin:_Diffuse_Color,varname:_Diffuse_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:9297,x:30947,y:33061,varname:node_9297,prsc:2|A-9229-R,B-7332-RGB;n:type:ShaderForge.SFN_Lerp,id:37,x:32174,y:32823,varname:node_37,prsc:2|A-348-OUT,B-6897-OUT,T-5290-OUT;n:type:ShaderForge.SFN_Fresnel,id:9235,x:31635,y:33384,varname:node_9235,prsc:2|EXP-45-OUT;n:type:ShaderForge.SFN_RemapRangeAdvanced,id:2342,x:31846,y:33384,varname:node_2342,prsc:2|IN-9235-OUT,IMIN-381-OUT,IMAX-5422-OUT,OMIN-3443-OUT,OMAX-3539-OUT;n:type:ShaderForge.SFN_Vector1,id:381,x:31635,y:33534,varname:node_381,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:5422,x:31635,y:33608,varname:node_5422,prsc:2,v1:1;n:type:ShaderForge.SFN_Slider,id:3443,x:31478,y:33691,ptovrint:False,ptlb:RimPower_min,ptin:_RimPower_min,varname:_RimPower_min,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:-5;n:type:ShaderForge.SFN_Slider,id:3539,x:31478,y:33791,ptovrint:False,ptlb:RimPower_max,ptin:_RimPower_max,varname:_RimPower_max,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:1,cur:1,max:5;n:type:ShaderForge.SFN_Clamp01,id:524,x:32032,y:33384,varname:node_524,prsc:2|IN-2342-OUT;n:type:ShaderForge.SFN_Color,id:6258,x:32102,y:33218,ptovrint:False,ptlb:RimColor,ptin:_RimColor,varname:_RimColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:2893,x:32251,y:33363,varname:node_2893,prsc:2|A-6258-RGB,B-524-OUT;n:type:ShaderForge.SFN_Vector1,id:45,x:31387,y:33407,varname:node_45,prsc:2,v1:1;n:type:ShaderForge.SFN_Slider,id:4805,x:32218,y:33000,ptovrint:False,ptlb:Opacity,ptin:_Opacity,varname:_Opacity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Max,id:6897,x:31895,y:32874,varname:node_6897,prsc:2|A-348-OUT,B-5170-OUT;n:type:ShaderForge.SFN_Slider,id:5290,x:31862,y:33101,ptovrint:False,ptlb:cubemap_brightness,ptin:_cubemap_brightness,varname:_cubemap_brightness,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Multiply,id:5170,x:31732,y:32990,varname:node_5170,prsc:2|A-3334-RGB,B-7897-RGB;n:type:ShaderForge.SFN_Color,id:7897,x:31387,y:33172,ptovrint:False,ptlb:cubemap_Color,ptin:_cubemap_Color,varname:_cubemap_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;proporder:2960-9992-4216-9229-7332-3334-5290-7897-3443-3539-6258-4805;pass:END;sub:END;*/

Shader "yh/Trans/Ice" {
    Properties {
        _DiffuseRamp ("DiffuseRamp", 2D) = "black" {}
        _Diffuse_Color ("Diffuse_Color", Color) = (1,1,1,1)
        _shadow_Bright ("shadow_Bright", Range(0, 1)) = 0
        _SpecularRamp ("SpecularRamp", 2D) = "white" {}
        _SpecularRamp_Color ("SpecularRamp_Color", Color) = (1,1,1,1)
        _cubemap ("cubemap", Cube) = "_Skybox" {}
        _cubemap_brightness ("cubemap_brightness", Range(0, 1)) = 0.5
        _cubemap_Color ("cubemap_Color", Color) = (1,1,1,1)
        _RimPower_min ("RimPower_min", Range(0, -5)) = 0
        _RimPower_max ("RimPower_max", Range(1, 5)) = 1
        _RimColor ("RimColor", Color) = (1,1,1,1)
        _Opacity ("Opacity", Range(0, 1)) = 0.5
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One OneMinusSrcAlpha
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _DiffuseRamp; uniform float4 _DiffuseRamp_ST;
            uniform sampler2D _SpecularRamp; uniform float4 _SpecularRamp_ST;
            uniform float4 _SpecularRamp_Color;
            uniform float _shadow_Bright;
            uniform samplerCUBE _cubemap;
            uniform float4 _Diffuse_Color;
            uniform float _RimPower_min;
            uniform float _RimPower_max;
            uniform float4 _RimColor;
            uniform float _Opacity;
            uniform float _cubemap_brightness;
            uniform float4 _cubemap_Color;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                LIGHTING_COORDS(2,3)
                UNITY_FOG_COORDS(4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
////// Emissive:
                float node_381 = 0.0;
                float3 emissive = (_RimColor.rgb*saturate((_RimPower_min + ( (pow(1.0-max(0,dot(normalDirection, viewDirection)),1.0) - node_381) * (_RimPower_max - _RimPower_min) ) / (1.0 - node_381))));
                float node_5474 = dot(lightDirection,i.normalDir);
                float2 node_6716 = (float2(node_5474,node_5474)*attenuation);
                float4 _DiffuseRamp_var = tex2D(_DiffuseRamp,TRANSFORM_TEX(node_6716, _DiffuseRamp));
                float node_7467 = dot(i.normalDir,halfDirection);
                float2 node_7523 = float2(node_7467,node_7467);
                float4 _SpecularRamp_var = tex2D(_SpecularRamp,TRANSFORM_TEX(node_7523, _SpecularRamp));
                float3 node_348 = ((_Diffuse_Color.rgb*saturate((_DiffuseRamp_var.r+_shadow_Bright))*_LightColor0.rgb)+(_LightColor0.rgb*(_SpecularRamp_var.r*_SpecularRamp_Color.rgb)));
                float3 finalColor = emissive + lerp(node_348,max(node_348,(texCUBE(_cubemap,viewReflectDirection).rgb*_cubemap_Color.rgb)),_cubemap_brightness);
                fixed4 finalRGBA = fixed4(finalColor,_Opacity);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
