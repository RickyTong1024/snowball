// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:False,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:False,nrmq:0,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:False,rfrpn:Refraction,coma:15,ufog:True,aust:False,igpj:False,qofs:0,qpre:1,rntp:1,fgom:True,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.005,fgrn:60,fgrf:150,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:1,x:33926,y:32549,varname:node_1,prsc:2|diff-130-OUT,spec-8812-OUT,gloss-7315-OUT,emission-99-OUT;n:type:ShaderForge.SFN_Tex2d,id:2,x:31777,y:31926,ptovrint:False,ptlb:D01,ptin:_D01,varname:_D01,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:2,isnm:False|UVIN-3458-UVOUT;n:type:ShaderForge.SFN_Fresnel,id:94,x:33584,y:33084,varname:node_94,prsc:2|EXP-98-OUT;n:type:ShaderForge.SFN_ValueProperty,id:98,x:33418,y:33105,ptovrint:False,ptlb:RimPower,ptin:_RimPower,varname:_RimPower,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2.5;n:type:ShaderForge.SFN_Multiply,id:99,x:33753,y:33064,varname:node_99,prsc:2|A-100-RGB,B-94-OUT;n:type:ShaderForge.SFN_Color,id:100,x:33602,y:32927,ptovrint:False,ptlb:RimColor,ptin:_RimColor,varname:_RimColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Power,id:130,x:33768,y:32242,varname:node_130,prsc:2|VAL-1819-OUT,EXP-132-OUT;n:type:ShaderForge.SFN_ValueProperty,id:132,x:33577,y:32373,ptovrint:False,ptlb:Tex_Power,ptin:_Tex_Power,varname:_Tex_Power,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Code,id:1817,x:31949,y:31675,varname:node_1817,prsc:2,code:ZgBsAG8AYQB0ADQAIABrACAAPQBmAGwAbwBhAHQANAAoADAALgAwACwALQAxAC4AMAAvADMALgAwACwAMgAuADAALwAzAC4AMAAsAC0AMQAuADAAKQA7AAoAZgBsAG8AYQB0ADQAIABwACAAPQBSAEcAQgAuAGcAPABSAEcAQgAuAGIAPwBmAGwAbwBhAHQANAAoAFIARwBCAC4AYgAsAFIARwBCAC4AZwAsAGsALgB3ACwAawAuAHoAKQA6AGYAbABvAGEAdAA0ACgAUgBHAEIALgBnAGIALABrAC4AeAB5ACkAOwAKAGYAbABvAGEAdAA0ACAAcQAgAD0AUgBHAEIALgByADwAcAAuAHgAIAAgAD8AZgBsAG8AYQB0ADQAKABwAC4AeAAsAHAALgB5ACwAcAAuAHcALABSAEcAQgAuAHIAKQA6AGYAbABvAGEAdAA0ACgAUgBHAEIALgByACwAcAAuAHkAegB4ACkAOwAKAGYAbABvAGEAdAAgAGQAIAA9AHEALgB4AC0AbQBpAG4AKABxAC4AdwAsAHEALgB5ACkAOwAKAGYAbABvAGEAdAAgAGUAPQAxAC4AMABlAC0AMQAwADsACgByAGUAdAB1AHIAbgAgAGYAbABvAGEAdAAzACgAYQBiAHMAKABxAC4AegArACgAcQAuAHcALQBxAC4AeQApAC8AKAA2AC4AMAAqAGQAKwBlACkAKQAsAGQALwAoAHEALgB4ACsAZQApACwAcQAuAHgAKQA7AA==,output:2,fname:RGBtoHSV,width:716,height:154,input:2,input_1_label:RGB|A-2-RGB;n:type:ShaderForge.SFN_Code,id:1819,x:33434,y:31545,varname:node_1819,prsc:2,code:ZgBsAG8AYQB0ADQAIABrACAAPQAgAGYAbABvAGEAdAA0ACgAMQAuADAALAAyAC4AMAAvADMALgAwACwAMQAuADAALwAzAC4AMAAsADMALgAwACkAOwAKAGYAbABvAGEAdAAzACAAcAAgAD0AYQBiAHMAKABmAHIAYQBjACgASABTAFYALgB4AHgAeAArAGsALgB4AHkAegApACoANgAuADAALQBrAC4AdwB3AHcAKQA7AAoAcgBlAHQAdQByAG4AIABIAFMAVgAuAHoAKgBsAGUAcgBwACgAawAuAHgAeAB4ACwAYwBsAGEAbQBwACgAcAAtAGsALgB4AHgAeAAsADAALgAwACwAMQAuADAAKQAsAEgAUwBWAC4AeQApADsA,output:2,fname:HSVtoRGB,width:699,height:127,input:2,input_1_label:HSV|A-1960-OUT;n:type:ShaderForge.SFN_Add,id:1939,x:32944,y:31442,varname:node_1939,prsc:2|A-1953-X,B-1969-R;n:type:ShaderForge.SFN_Vector4Property,id:1953,x:32733,y:31524,ptovrint:False,ptlb:Hsv,ptin:_Hsv,varname:_Hsv,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1,v2:1,v3:1,v4:1;n:type:ShaderForge.SFN_Append,id:1960,x:33261,y:31544,varname:node_1960,prsc:2|A-1962-OUT,B-1967-OUT;n:type:ShaderForge.SFN_Append,id:1962,x:33098,y:31544,varname:node_1962,prsc:2|A-1939-OUT,B-1964-OUT;n:type:ShaderForge.SFN_Multiply,id:1964,x:32944,y:31563,varname:node_1964,prsc:2|A-1953-Y,B-1969-G;n:type:ShaderForge.SFN_Multiply,id:1967,x:32944,y:31695,varname:node_1967,prsc:2|A-1953-Z,B-1969-B;n:type:ShaderForge.SFN_ComponentMask,id:1969,x:32733,y:31674,varname:node_1969,prsc:2,cc1:0,cc2:1,cc3:2,cc4:-1|IN-1817-OUT;n:type:ShaderForge.SFN_Slider,id:6120,x:32750,y:33083,ptovrint:False,ptlb:SpecularTex_Max,ptin:_SpecularTex_Max,varname:_SpecularTex_Max,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:1,cur:1,max:5;n:type:ShaderForge.SFN_Slider,id:7315,x:33511,y:32609,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:_Gloss,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Clamp01,id:2026,x:33530,y:32738,varname:node_2026,prsc:2|IN-2509-OUT;n:type:ShaderForge.SFN_Slider,id:4689,x:32750,y:32969,ptovrint:False,ptlb:SpecularTex_Min,ptin:_SpecularTex_Min,varname:_SpecularTex_Min,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:-5;n:type:ShaderForge.SFN_Multiply,id:8812,x:33739,y:32738,varname:node_8812,prsc:2|A-2026-OUT,B-5198-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5198,x:33352,y:32908,ptovrint:False,ptlb:Specular_Intensity,ptin:_Specular_Intensity,varname:_Specular_Intensity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_RemapRangeAdvanced,id:2509,x:33158,y:32640,varname:node_2509,prsc:2|IN-5261-OUT,IMIN-7995-OUT,IMAX-8361-OUT,OMIN-4689-OUT,OMAX-6120-OUT;n:type:ShaderForge.SFN_Vector1,id:8361,x:32921,y:32803,varname:node_8361,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:7995,x:32921,y:32738,varname:node_7995,prsc:2,v1:0;n:type:ShaderForge.SFN_Multiply,id:5261,x:32921,y:32583,varname:node_5261,prsc:2|A-2-R,B-2-R;n:type:ShaderForge.SFN_TexCoord,id:3458,x:31589,y:31926,varname:node_3458,prsc:2,uv:0;proporder:132-2-1953-100-98-7315-5198-4689-6120;pass:END;sub:END;*/

Shader "yh/Diffuse/Diffuse_Rim" {
    Properties {
        _Tex_Power ("Tex_Power", Float ) = 1
        _D01 ("D01", 2D) = "black" {}
        _Hsv ("Hsv", Vector) = (1,1,1,1)
        _RimColor ("RimColor", Color) = (1,1,1,1)
        _RimPower ("RimPower", Float ) = 2.5
        _Gloss ("Gloss", Range(0, 1)) = 0.5
        _Specular_Intensity ("Specular_Intensity", Float ) = 1
        _SpecularTex_Min ("SpecularTex_Min", Range(0, -5)) = 0
        _SpecularTex_Max ("SpecularTex_Max", Range(1, 5)) = 1
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _D01; uniform float4 _D01_ST;
            uniform float _RimPower;
            uniform float4 _RimColor;
            uniform float _Tex_Power;
            float3 RGBtoHSV( float3 RGB ){
            float4 k =float4(0.0,-1.0/3.0,2.0/3.0,-1.0);
            float4 p =RGB.g<RGB.b?float4(RGB.b,RGB.g,k.w,k.z):float4(RGB.gb,k.xy);
            float4 q =RGB.r<p.x  ?float4(p.x,p.y,p.w,RGB.r):float4(RGB.r,p.yzx);
            float d =q.x-min(q.w,q.y);
            float e=1.0e-10;
            return float3(abs(q.z+(q.w-q.y)/(6.0*d+e)),d/(q.x+e),q.x);
            }
            
            float3 HSVtoRGB( float3 HSV ){
            float4 k = float4(1.0,2.0/3.0,1.0/3.0,3.0);
            float3 p =abs(frac(HSV.xxx+k.xyz)*6.0-k.www);
            return HSV.z*lerp(k.xxx,clamp(p-k.xxx,0.0,1.0),HSV.y);
            }
            
            uniform float4 _Hsv;
            uniform float _SpecularTex_Max;
            uniform float _Gloss;
            uniform float _SpecularTex_Min;
            uniform float _Specular_Intensity;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
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
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = _Gloss;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float4 _D01_var = tex2D(_D01,TRANSFORM_TEX(i.uv0, _D01));
                float node_7995 = 0.0;
                float node_8812 = (saturate((_SpecularTex_Min + ( ((_D01_var.r*_D01_var.r) - node_7995) * (_SpecularTex_Max - _SpecularTex_Min) ) / (1.0 - node_7995)))*_Specular_Intensity);
                float3 specularColor = float3(node_8812,node_8812,node_8812);
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float3 node_1969 = RGBtoHSV( _D01_var.rgb ).rgb;
                float3 diffuseColor = pow(HSVtoRGB( float3(float2((_Hsv.r+node_1969.r),(_Hsv.g*node_1969.g)),(_Hsv.b*node_1969.b)) ),_Tex_Power);
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float3 emissive = (_RimColor.rgb*pow(1.0-max(0,dot(normalDirection, viewDirection)),_RimPower));
/// Final Color:
                float3 finalColor = diffuse + specular + emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _D01; uniform float4 _D01_ST;
            uniform float _RimPower;
            uniform float4 _RimColor;
            uniform float _Tex_Power;
            float3 RGBtoHSV( float3 RGB ){
            float4 k =float4(0.0,-1.0/3.0,2.0/3.0,-1.0);
            float4 p =RGB.g<RGB.b?float4(RGB.b,RGB.g,k.w,k.z):float4(RGB.gb,k.xy);
            float4 q =RGB.r<p.x  ?float4(p.x,p.y,p.w,RGB.r):float4(RGB.r,p.yzx);
            float d =q.x-min(q.w,q.y);
            float e=1.0e-10;
            return float3(abs(q.z+(q.w-q.y)/(6.0*d+e)),d/(q.x+e),q.x);
            }
            
            float3 HSVtoRGB( float3 HSV ){
            float4 k = float4(1.0,2.0/3.0,1.0/3.0,3.0);
            float3 p =abs(frac(HSV.xxx+k.xyz)*6.0-k.www);
            return HSV.z*lerp(k.xxx,clamp(p-k.xxx,0.0,1.0),HSV.y);
            }
            
            uniform float4 _Hsv;
            uniform float _SpecularTex_Max;
            uniform float _Gloss;
            uniform float _SpecularTex_Min;
            uniform float _Specular_Intensity;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
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
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = _Gloss;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float4 _D01_var = tex2D(_D01,TRANSFORM_TEX(i.uv0, _D01));
                float node_7995 = 0.0;
                float node_8812 = (saturate((_SpecularTex_Min + ( ((_D01_var.r*_D01_var.r) - node_7995) * (_SpecularTex_Max - _SpecularTex_Min) ) / (1.0 - node_7995)))*_Specular_Intensity);
                float3 specularColor = float3(node_8812,node_8812,node_8812);
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 node_1969 = RGBtoHSV( _D01_var.rgb ).rgb;
                float3 diffuseColor = pow(HSVtoRGB( float3(float2((_Hsv.r+node_1969.r),(_Hsv.g*node_1969.g)),(_Hsv.b*node_1969.b)) ),_Tex_Power);
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
