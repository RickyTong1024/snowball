// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.06 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.06;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:False,lmpd:False,lprd:False,rprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:False,hqlp:False,tesm:0,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:True,dith:0,ufog:True,aust:False,igpj:False,qofs:1,qpre:3,rntp:2,fgom:True,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.005,fgrn:20,fgrf:80,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:34630,y:32549,varname:node_1,prsc:2|emission-225-OUT,alpha-6663-OUT;n:type:ShaderForge.SFN_Tex2d,id:2,x:32965,y:31850,ptovrint:False,ptlb:E01,ptin:_E01,varname:_Tex01,prsc:2,ntxv:2,isnm:False|UVIN-164-OUT;n:type:ShaderForge.SFN_Color,id:3,x:33145,y:31735,ptovrint:False,ptlb:E01_Color,ptin:_E01_Color,varname:_Color_Tex01,prsc:2,glob:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:4,x:33325,y:31830,varname:node_4,prsc:2|A-3-RGB,B-2-RGB;n:type:ShaderForge.SFN_Fresnel,id:94,x:32831,y:33147,varname:node_94,prsc:2|EXP-4973-OUT;n:type:ShaderForge.SFN_Multiply,id:99,x:33399,y:33124,varname:node_99,prsc:2|A-100-RGB,B-4686-OUT;n:type:ShaderForge.SFN_Color,id:100,x:33230,y:32997,ptovrint:False,ptlb:RimColor,ptin:_RimColor,varname:_RimColor,prsc:2,glob:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Tex2d,id:109,x:33025,y:32630,ptovrint:False,ptlb:E02,ptin:_E02,varname:_Tex02,prsc:2,ntxv:2,isnm:False|UVIN-2291-OUT;n:type:ShaderForge.SFN_Color,id:111,x:33202,y:32475,ptovrint:False,ptlb:E02_Color,ptin:_E02_Color,varname:_Color_Tex02,prsc:2,glob:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:113,x:33383,y:32610,varname:node_113,prsc:2|A-111-RGB,B-109-RGB;n:type:ShaderForge.SFN_Tex2d,id:114,x:33569,y:32953,ptovrint:False,ptlb:AlphaTex,ptin:_AlphaTex,varname:_MaskAlpha,prsc:2,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:116,x:33743,y:32970,varname:node_116,prsc:2|A-114-R,B-117-OUT;n:type:ShaderForge.SFN_ValueProperty,id:117,x:33588,y:33153,ptovrint:False,ptlb:Alpha_Bright,ptin:_Alpha_Bright,varname:_IntensityA,prsc:2,glob:False,v1:1;n:type:ShaderForge.SFN_Power,id:140,x:33840,y:32337,varname:node_140,prsc:2|VAL-218-OUT,EXP-142-OUT;n:type:ShaderForge.SFN_ValueProperty,id:142,x:33679,y:32418,ptovrint:False,ptlb:Tex_power,ptin:_Tex_power,varname:_Tex_power,prsc:2,glob:False,v1:1;n:type:ShaderForge.SFN_Add,id:164,x:32788,y:31849,varname:node_164,prsc:2|A-617-OUT,B-5436-OUT;n:type:ShaderForge.SFN_Time,id:187,x:31630,y:31971,varname:node_187,prsc:2;n:type:ShaderForge.SFN_Multiply,id:188,x:31806,y:32000,varname:node_188,prsc:2|A-187-T,B-189-OUT;n:type:ShaderForge.SFN_ValueProperty,id:189,x:31630,y:32114,ptovrint:False,ptlb:E01_UVpan_Speed,ptin:_E01_UVpan_Speed,varname:_PanV_DisTex01,prsc:2,glob:False,v1:1;n:type:ShaderForge.SFN_Panner,id:202,x:32037,y:31719,varname:node_202,prsc:2,spu:1,spv:0|UVIN-9626-UVOUT,DIST-188-OUT;n:type:ShaderForge.SFN_Add,id:218,x:33560,y:32339,varname:node_218,prsc:2|A-4-OUT,B-113-OUT,C-99-OUT;n:type:ShaderForge.SFN_VertexColor,id:224,x:33885,y:32656,varname:node_224,prsc:2;n:type:ShaderForge.SFN_Multiply,id:225,x:34037,y:32337,varname:node_225,prsc:2|A-140-OUT,B-224-RGB;n:type:ShaderForge.SFN_Multiply,id:6663,x:34349,y:32813,varname:node_6663,prsc:2|A-224-A,B-116-OUT;n:type:ShaderForge.SFN_TexCoord,id:1088,x:31627,y:31511,varname:node_1088,prsc:2,uv:0;n:type:ShaderForge.SFN_RemapRangeAdvanced,id:4281,x:33035,y:33184,varname:node_4281,prsc:2|IN-94-OUT,IMIN-8081-OUT,IMAX-8470-OUT,OMIN-5219-OUT,OMAX-9472-OUT;n:type:ShaderForge.SFN_Clamp01,id:4686,x:33214,y:33184,varname:node_4686,prsc:2|IN-4281-OUT;n:type:ShaderForge.SFN_Vector1,id:8081,x:32831,y:33266,varname:node_8081,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:8470,x:32831,y:33322,varname:node_8470,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:4973,x:32652,y:33167,varname:node_4973,prsc:2,v1:1;n:type:ShaderForge.SFN_Slider,id:5219,x:32766,y:33444,ptovrint:False,ptlb:RimPower_min,ptin:_RimPower_min,varname:node_5219,prsc:2,min:0,cur:0,max:-5;n:type:ShaderForge.SFN_Slider,id:9472,x:32766,y:33548,ptovrint:False,ptlb:RimPower_max,ptin:_RimPower_max,varname:node_9472,prsc:2,min:1,cur:1,max:5;n:type:ShaderForge.SFN_Add,id:2291,x:32791,y:32618,varname:node_2291,prsc:2|A-4142-OUT,B-9416-OUT;n:type:ShaderForge.SFN_Tex2d,id:6900,x:32394,y:32250,ptovrint:False,ptlb:DisEmiss,ptin:_DisEmiss,varname:node_6900,prsc:2,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:5436,x:32603,y:32165,varname:node_5436,prsc:2|A-3387-OUT,B-6900-R;n:type:ShaderForge.SFN_Multiply,id:4142,x:32603,y:32355,varname:node_4142,prsc:2|A-6900-R,B-2926-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3387,x:32394,y:32165,ptovrint:False,ptlb:DisE01_Value,ptin:_DisE01_Value,varname:node_3387,prsc:2,glob:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:2926,x:32394,y:32473,ptovrint:False,ptlb:DisE02_Value,ptin:_DisE02_Value,varname:_node_3387_copy,prsc:2,glob:False,v1:0;n:type:ShaderForge.SFN_SwitchProperty,id:617,x:32500,y:31852,ptovrint:False,ptlb:E01_U/Vpan,ptin:_E01_UVpan,varname:node_617,prsc:2,on:False|A-202-UVOUT,B-3772-UVOUT;n:type:ShaderForge.SFN_Rotator,id:9626,x:31837,y:31511,varname:node_9626,prsc:2|UVIN-1088-UVOUT,ANG-2229-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7736,x:31584,y:31695,ptovrint:False,ptlb:E01_UVangle,ptin:_E01_UVangle,varname:node_7736,prsc:2,glob:False,v1:0;n:type:ShaderForge.SFN_Pi,id:8132,x:31600,y:31775,varname:node_8132,prsc:2;n:type:ShaderForge.SFN_Multiply,id:2229,x:31746,y:31718,varname:node_2229,prsc:2|A-7736-OUT,B-8132-OUT;n:type:ShaderForge.SFN_Panner,id:3772,x:32037,y:31908,varname:node_3772,prsc:2,spu:0,spv:1|UVIN-9626-UVOUT,DIST-188-OUT;n:type:ShaderForge.SFN_Time,id:482,x:31598,y:32919,varname:node_482,prsc:2;n:type:ShaderForge.SFN_Multiply,id:6538,x:31774,y:32948,varname:node_6538,prsc:2|A-482-T,B-7561-OUT;n:type:ShaderForge.SFN_Panner,id:6791,x:32005,y:32667,varname:node_6791,prsc:2,spu:1,spv:0|UVIN-4979-UVOUT,DIST-6538-OUT;n:type:ShaderForge.SFN_TexCoord,id:9127,x:31595,y:32459,varname:node_9127,prsc:2,uv:0;n:type:ShaderForge.SFN_SwitchProperty,id:9416,x:32468,y:32800,ptovrint:False,ptlb:E02_U/Vpan,ptin:_E02_UVpan,varname:_E01_UVpan_copy,prsc:2,on:False|A-6791-UVOUT,B-7821-UVOUT;n:type:ShaderForge.SFN_Rotator,id:4979,x:31805,y:32459,varname:node_4979,prsc:2|UVIN-9127-UVOUT,ANG-8899-OUT;n:type:ShaderForge.SFN_ValueProperty,id:9673,x:31552,y:32643,ptovrint:False,ptlb:E02_UVangle,ptin:_E02_UVangle,varname:_E01_UVangle_copy,prsc:2,glob:False,v1:0;n:type:ShaderForge.SFN_Pi,id:6676,x:31568,y:32723,varname:node_6676,prsc:2;n:type:ShaderForge.SFN_Multiply,id:8899,x:31714,y:32666,varname:node_8899,prsc:2|A-9673-OUT,B-6676-OUT;n:type:ShaderForge.SFN_Panner,id:7821,x:32005,y:32856,varname:node_7821,prsc:2,spu:0,spv:1|UVIN-4979-UVOUT,DIST-6538-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7561,x:31598,y:33062,ptovrint:False,ptlb:E02_UVpan_Speed,ptin:_E02_UVpan_Speed,varname:_E01_UVpan_Speed_copy,prsc:2,glob:False,v1:1;proporder:142-2-3-617-7736-189-109-111-9416-9673-7561-114-117-6900-3387-2926-100-5219-9472;pass:END;sub:END;*/

Shader "yh/Trans/2TransUVpan_Rim" {
    Properties {
        _Tex_power ("Tex_power", Float ) = 1
        _E01 ("E01", 2D) = "black" {}
        _E01_Color ("E01_Color", Color) = (1,1,1,1)
        [MaterialToggle] _E01_UVpan ("E01_U/Vpan", Float ) = 0
        _E01_UVangle ("E01_UVangle", Float ) = 0
        _E01_UVpan_Speed ("E01_UVpan_Speed", Float ) = 1
        _E02 ("E02", 2D) = "black" {}
        _E02_Color ("E02_Color", Color) = (1,1,1,1)
        [MaterialToggle] _E02_UVpan ("E02_U/Vpan", Float ) = 0
        _E02_UVangle ("E02_UVangle", Float ) = 0
        _E02_UVpan_Speed ("E02_UVpan_Speed", Float ) = 1
        _AlphaTex ("AlphaTex", 2D) = "white" {}
        _Alpha_Bright ("Alpha_Bright", Float ) = 1
        _DisEmiss ("DisEmiss", 2D) = "white" {}
        _DisE01_Value ("DisE01_Value", Float ) = 0
        _DisE02_Value ("DisE02_Value", Float ) = 0
        _RimColor ("RimColor", Color) = (1,1,1,1)
        _RimPower_min ("RimPower_min", Range(0, -5)) = 0
        _RimPower_max ("RimPower_max", Range(1, 5)) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
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
            Blend SrcAlpha OneMinusSrcAlpha
            
            
            Fog {Mode Global}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers xbox360 ps3 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _E01; uniform float4 _E01_ST;
            uniform float4 _E01_Color;
            uniform float4 _RimColor;
            uniform sampler2D _E02; uniform float4 _E02_ST;
            uniform float4 _E02_Color;
            uniform sampler2D _AlphaTex; uniform float4 _AlphaTex_ST;
            uniform float _Alpha_Bright;
            uniform float _Tex_power;
            uniform float _E01_UVpan_Speed;
            uniform float _RimPower_min;
            uniform float _RimPower_max;
            uniform sampler2D _DisEmiss; uniform float4 _DisEmiss_ST;
            uniform float _DisE01_Value;
            uniform float _DisE02_Value;
            uniform fixed _E01_UVpan;
            uniform float _E01_UVangle;
            uniform fixed _E02_UVpan;
            uniform float _E02_UVangle;
            uniform float _E02_UVpan_Speed;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = mul(unity_ObjectToWorld, float4(v.normal,0)).xyz;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float4 node_187 = _Time + _TimeEditor;
                float node_188 = (node_187.g*_E01_UVpan_Speed);
                float node_9626_ang = (_E01_UVangle*3.141592654);
                float node_9626_spd = 1.0;
                float node_9626_cos = cos(node_9626_spd*node_9626_ang);
                float node_9626_sin = sin(node_9626_spd*node_9626_ang);
                float2 node_9626_piv = float2(0.5,0.5);
                float2 node_9626 = (mul(i.uv0-node_9626_piv,float2x2( node_9626_cos, -node_9626_sin, node_9626_sin, node_9626_cos))+node_9626_piv);
                float4 _DisEmiss_var = tex2D(_DisEmiss,TRANSFORM_TEX(i.uv0, _DisEmiss));
                float2 node_164 = (lerp( (node_9626+node_188*float2(1,0)), (node_9626+node_188*float2(0,1)), _E01_UVpan )+(_DisE01_Value*_DisEmiss_var.r));
                float4 _E01_var = tex2D(_E01,TRANSFORM_TEX(node_164, _E01));
                float4 node_482 = _Time + _TimeEditor;
                float node_6538 = (node_482.g*_E02_UVpan_Speed);
                float node_4979_ang = (_E02_UVangle*3.141592654);
                float node_4979_spd = 1.0;
                float node_4979_cos = cos(node_4979_spd*node_4979_ang);
                float node_4979_sin = sin(node_4979_spd*node_4979_ang);
                float2 node_4979_piv = float2(0.5,0.5);
                float2 node_4979 = (mul(i.uv0-node_4979_piv,float2x2( node_4979_cos, -node_4979_sin, node_4979_sin, node_4979_cos))+node_4979_piv);
                float2 node_2291 = ((_DisEmiss_var.r*_DisE02_Value)+lerp( (node_4979+node_6538*float2(1,0)), (node_4979+node_6538*float2(0,1)), _E02_UVpan ));
                float4 _E02_var = tex2D(_E02,TRANSFORM_TEX(node_2291, _E02));
                float node_8081 = 0.0;
                float3 emissive = (pow(((_E01_Color.rgb*_E01_var.rgb)+(_E02_Color.rgb*_E02_var.rgb)+(_RimColor.rgb*saturate((_RimPower_min + ( (pow(1.0-max(0,dot(normalDirection, viewDirection)),1.0) - node_8081) * (_RimPower_max - _RimPower_min) ) / (1.0 - node_8081))))),_Tex_power)*i.vertexColor.rgb);
                float3 finalColor = emissive;
                float4 _AlphaTex_var = tex2D(_AlphaTex,TRANSFORM_TEX(i.uv0, _AlphaTex));
                return fixed4(finalColor,(i.vertexColor.a*(_AlphaTex_var.r*_Alpha_Bright)));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
