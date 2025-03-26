// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33337,y:32674,varname:node_3138,prsc:2|emission-7042-OUT,alpha-945-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32507,y:32631,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;n:type:ShaderForge.SFN_Fresnel,id:4213,x:32418,y:32816,varname:node_4213,prsc:2;n:type:ShaderForge.SFN_RemapRange,id:7688,x:32581,y:32816,varname:node_7688,prsc:2,frmn:0,frmx:1,tomn:1,tomx:0.2|IN-4213-OUT;n:type:ShaderForge.SFN_RemapRange,id:5656,x:32581,y:32972,varname:node_5656,prsc:2,frmn:0,frmx:1,tomn:3,tomx:-5|IN-4213-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:838,x:32745,y:32972,varname:node_838,prsc:2,min:0,max:3|IN-5656-OUT;n:type:ShaderForge.SFN_Add,id:3458,x:32922,y:32901,varname:node_3458,prsc:2|A-7688-OUT,B-838-OUT;n:type:ShaderForge.SFN_Multiply,id:7042,x:33076,y:32774,varname:node_7042,prsc:2|A-7241-RGB,B-3458-OUT,C-11-A;n:type:ShaderForge.SFN_Multiply,id:945,x:33029,y:33064,varname:node_945,prsc:2|A-7688-OUT,B-7887-OUT,C-11-A;n:type:ShaderForge.SFN_Vector1,id:7887,x:33019,y:33188,varname:node_7887,prsc:2,v1:0.5;n:type:ShaderForge.SFN_VertexColor,id:11,x:32614,y:33183,varname:node_11,prsc:2;proporder:7241;pass:END;sub:END;*/

Shader "Shader Forge/HDN_SH_Effect_3DFlake" {
    Properties {
        _Color ("Color", Color) = (0.07843138,0.3921569,0.7843137,1)
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
            Blend One OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            //#define UNITY_PASS_FORWARDBASE    //TODO: Warning
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 3.0
            uniform float4 _Color;
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
                float node_4213 = (1.0-max(0,dot(normalDirection, viewDirection)));
                float node_7688 = (node_4213*-0.8+1.0);
                float3 emissive = (_Color.rgb*(node_7688+clamp((node_4213*-8.0+3.0),0,3))*i.vertexColor.a);
                float3 finalColor = emissive;
                return fixed4(finalColor,(node_7688*0.5*i.vertexColor.a));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
