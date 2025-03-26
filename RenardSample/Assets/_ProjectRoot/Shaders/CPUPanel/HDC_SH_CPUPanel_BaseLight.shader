// Shader created with Shader Forge v1.42 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Enhanced by Antoine Guillon / Arkham Development - http://www.arkham-development.com/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.42;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33941,y:32651,varname:node_3138,prsc:2|emission-6059-OUT,voffset-3108-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32241,y:32181,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;n:type:ShaderForge.SFN_VertexColor,id:5580,x:32390,y:32820,varname:node_5580,prsc:2;n:type:ShaderForge.SFN_Multiply,id:6059,x:33012,y:32678,varname:node_6059,prsc:2|A-1802-OUT,B-1272-OUT;n:type:ShaderForge.SFN_Fresnel,id:6956,x:32360,y:32952,varname:node_6956,prsc:2;n:type:ShaderForge.SFN_RemapRange,id:2904,x:32528,y:32952,varname:node_2904,prsc:2,frmn:0.8,frmx:1,tomn:1,tomx:0|IN-6956-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:9756,x:32701,y:32952,varname:node_9756,prsc:2,min:0,max:1|IN-2904-OUT;n:type:ShaderForge.SFN_Multiply,id:3485,x:32701,y:32823,varname:node_3485,prsc:2|A-5580-R,B-9756-OUT,C-1424-OUT,D-9092-OUT,E-1895-OUT;n:type:ShaderForge.SFN_RemapRange,id:1835,x:32528,y:33125,varname:node_1835,prsc:2,frmn:0.7,frmx:1,tomn:1,tomx:3|IN-6956-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:1424,x:32701,y:33125,varname:node_1424,prsc:2,min:1,max:10|IN-1835-OUT;n:type:ShaderForge.SFN_RemapRange,id:9535,x:32546,y:33280,varname:node_9535,prsc:2,frmn:0,frmx:0.5,tomn:1.1,tomx:0.5|IN-6956-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:9092,x:32719,y:33280,varname:node_9092,prsc:2,min:0.5,max:2|IN-9535-OUT;n:type:ShaderForge.SFN_Vector3,id:6084,x:32241,y:32336,varname:node_6084,prsc:2,v1:0.6981132,v2:0.5617356,v3:0.06915272;n:type:ShaderForge.SFN_Slider,id:3104,x:32141,y:32436,ptovrint:False,ptlb:Damage,ptin:_Damage,varname:node_3104,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Lerp,id:4672,x:32458,y:32181,varname:node_4672,prsc:2|A-7241-RGB,B-6084-OUT,T-3104-OUT;n:type:ShaderForge.SFN_Lerp,id:1802,x:32930,y:32362,varname:node_1802,prsc:2|A-4672-OUT,B-4503-OUT,T-5270-OUT;n:type:ShaderForge.SFN_Vector3,id:4503,x:32559,y:32415,varname:node_4503,prsc:2,v1:0.7924528,v2:0.0710217,v3:0.1007943;n:type:ShaderForge.SFN_Slider,id:9483,x:32129,y:32533,ptovrint:False,ptlb:Break,ptin:_Break,varname:node_9483,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_RemapRange,id:1399,x:32458,y:32511,varname:node_1399,prsc:2,frmn:0,frmx:0.2,tomn:0,tomx:1|IN-9483-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:5270,x:32627,y:32511,varname:node_5270,prsc:2,min:0,max:1|IN-1399-OUT;n:type:ShaderForge.SFN_RemapRange,id:7385,x:32458,y:32669,varname:node_7385,prsc:2,frmn:0.8,frmx:1,tomn:1,tomx:0|IN-9483-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:1895,x:32642,y:32669,varname:node_1895,prsc:2,min:0,max:1|IN-7385-OUT;n:type:ShaderForge.SFN_Multiply,id:1272,x:32949,y:32823,varname:node_1272,prsc:2|A-3485-OUT,B-8783-OUT;n:type:ShaderForge.SFN_Time,id:2549,x:32949,y:32952,varname:node_2549,prsc:2;n:type:ShaderForge.SFN_Multiply,id:9920,x:33150,y:32952,varname:node_9920,prsc:2|A-2549-T,B-4637-OUT;n:type:ShaderForge.SFN_Vector1,id:4637,x:33150,y:33070,varname:node_4637,prsc:2,v1:43.73;n:type:ShaderForge.SFN_Frac,id:2507,x:33323,y:32952,varname:node_2507,prsc:2|IN-9920-OUT;n:type:ShaderForge.SFN_RemapRange,id:8783,x:33497,y:32952,varname:node_8783,prsc:2,frmn:0,frmx:1,tomn:0.7,tomx:1|IN-2507-OUT;n:type:ShaderForge.SFN_Sin,id:2522,x:33150,y:33145,varname:node_2522,prsc:2|IN-2549-T;n:type:ShaderForge.SFN_Append,id:3108,x:33638,y:33135,varname:node_3108,prsc:2|A-9413-OUT,B-2203-OUT,C-9413-OUT;n:type:ShaderForge.SFN_Vector1,id:9413,x:33639,y:33265,varname:node_9413,prsc:2,v1:0;n:type:ShaderForge.SFN_Multiply,id:2203,x:33340,y:33145,varname:node_2203,prsc:2|A-2522-OUT,B-3931-OUT;n:type:ShaderForge.SFN_Vector1,id:3931,x:33340,y:33267,varname:node_3931,prsc:2,v1:0.05;proporder:7241-3104-9483;pass:END;sub:END;*/

Shader "Shader Forge/CustomHado/HDC_SH_CPUPanel_BaseLight" {
    Properties {
        _Color ("Color", Color) = (0.07843138,0.3921569,0.7843137,1)
        _Damage ("Damage", Range(0, 1)) = 0
        _Break ("Break", Range(0, 1)) = 0
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
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #ifndef UNITY_PASS_FORWARDBASE
            #define UNITY_PASS_FORWARDBASE
            #endif //UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu switch vulkan 
            #pragma target 3.0
            uniform float4 _Color;
            uniform float _Damage;
            uniform float _Break;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float node_9413 = 0.0;
                float4 node_2549 = _Time;
                v.vertex.xyz += float3(node_9413,(sin(node_2549.g)*0.05),node_9413);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float node_6956 = (1.0-max(0,dot(normalDirection, viewDirection)));
                float4 node_2549 = _Time;
                float node_2507 = frac((node_2549.g*43.73));
                float3 emissive = (lerp(lerp(_Color.rgb,float3(0.6981132,0.5617356,0.06915272),_Damage),float3(0.7924528,0.0710217,0.1007943),clamp((_Break*5.0+0.0),0,1))*((i.vertexColor.r*clamp((node_6956*-5.0+5.0),0,1)*clamp((node_6956*6.666667+-3.666667),1,10)*clamp((node_6956*-1.2+1.1),0.5,2)*clamp((_Break*-5.0+5.0),0,1))*(node_2507*0.3+0.7)));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Back
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #ifndef UNITY_PASS_SHADOWCASTER
            #define UNITY_PASS_SHADOWCASTER
            #endif //UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu switch vulkan 
            #pragma target 3.0
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                float node_9413 = 0.0;
                float4 node_2549 = _Time;
                v.vertex.xyz += float3(node_9413,(sin(node_2549.g)*0.05),node_9413);
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
