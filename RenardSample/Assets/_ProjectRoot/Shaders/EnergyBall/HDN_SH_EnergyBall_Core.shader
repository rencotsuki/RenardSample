// Shader created with Shader Forge v1.42 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Enhanced by Antoine Guillon / Arkham Development - http://www.arkham-development.com/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.42;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33067,y:32734,varname:node_3138,prsc:2|emission-8638-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32457,y:32599,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;n:type:ShaderForge.SFN_Fresnel,id:4355,x:31736,y:32921,varname:node_4355,prsc:2;n:type:ShaderForge.SFN_Multiply,id:8638,x:32925,y:32848,varname:node_8638,prsc:2|A-7241-RGB,B-1949-OUT,C-801-OUT;n:type:ShaderForge.SFN_RemapRange,id:562,x:32035,y:32908,varname:node_562,prsc:2,frmn:-0.5,frmx:0.7,tomn:1,tomx:0|IN-4355-OUT;n:type:ShaderForge.SFN_Add,id:1949,x:32628,y:32927,varname:node_1949,prsc:2|A-7238-OUT,B-1489-OUT;n:type:ShaderForge.SFN_RemapRange,id:2659,x:32122,y:33094,varname:node_2659,prsc:2,frmn:0,frmx:1,tomn:0.5,tomx:-2|IN-4355-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:1489,x:32297,y:33094,varname:node_1489,prsc:2,min:0,max:10|IN-2659-OUT;n:type:ShaderForge.SFN_FragmentPosition,id:348,x:31083,y:33216,varname:node_348,prsc:2;n:type:ShaderForge.SFN_RemapRange,id:8133,x:32316,y:33242,varname:node_8133,prsc:2,frmn:-0.01,frmx:0.02,tomn:0,tomx:1|IN-3295-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:6841,x:32476,y:33242,varname:node_6841,prsc:2,min:0.3,max:1|IN-8133-OUT;n:type:ShaderForge.SFN_Multiply,id:925,x:32688,y:33144,varname:node_925,prsc:2|A-1489-OUT,B-6841-OUT;n:type:ShaderForge.SFN_Multiply,id:2544,x:32788,y:33326,varname:node_2544,prsc:2|A-7238-OUT,B-6373-OUT;n:type:ShaderForge.SFN_Clamp01,id:6373,x:32476,y:33389,varname:node_6373,prsc:2|IN-8133-OUT;n:type:ShaderForge.SFN_RemapRange,id:1416,x:32332,y:33499,varname:node_1416,prsc:2,frmn:-0.2,frmx:-1,tomn:1,tomx:0|IN-3295-OUT;n:type:ShaderForge.SFN_Clamp01,id:2576,x:32505,y:33499,varname:node_2576,prsc:2|IN-1416-OUT;n:type:ShaderForge.SFN_ValueProperty,id:801,x:32853,y:33001,ptovrint:False,ptlb:Value_alpha,ptin:_Value_alpha,varname:node_801,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Clamp01,id:7238,x:32202,y:32908,varname:node_7238,prsc:2|IN-562-OUT;n:type:ShaderForge.SFN_Code,id:6084,x:31470,y:33355,varname:node_6084,prsc:2,code:cgBlAHQAdQByAG4AIABtAHUAbAAoAHcAbwByAGwAZABQAG8AcwAgAC0AIABhAG4AYwBoAG8AcgBQAG8AcwAsACAAdAApADsA,output:2,fname:Function_node_6084,width:334,height:112,input:2,input:2,input:13,input_1_label:worldPos,input_2_label:anchorPos,input_3_label:t|A-348-XYZ,B-4151-XYZ,C-3500-OUT;n:type:ShaderForge.SFN_ComponentMask,id:3295,x:31994,y:33355,varname:node_3295,prsc:2,cc1:1,cc2:-1,cc3:-1,cc4:-1|IN-6084-OUT;n:type:ShaderForge.SFN_Vector4Property,id:4151,x:31098,y:33397,ptovrint:False,ptlb:anchorPos,ptin:_anchorPos,varname:node_4151,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0,v2:0,v3:0,v4:0;n:type:ShaderForge.SFN_Matrix4x4Property,id:3500,x:31114,y:33599,ptovrint:False,ptlb:transformMatrix,ptin:transformMatrix,varname:node_3500,prsc:2,glob:True,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,m00:1,m01:0,m02:0,m03:0,m10:0,m11:1,m12:0,m13:0,m20:0,m21:0,m22:1,m23:0,m30:0,m31:0,m32:0,m33:1;proporder:7241-801-4151;pass:END;sub:END;*/

Shader "Shader Forge/HDN_SH_EnergyBall_Core" {
    Properties {
        _Color ("Color", Color) = (0.07843138,0.3921569,0.7843137,1)
        _Value_alpha ("Value_alpha", Float ) = 1
        _anchorPos ("anchorPos", Vector) = (0,0,0,0)
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
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal vulkan 
            #pragma target 3.0
            uniform float4 _Color;
            uniform float _Value_alpha;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
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
                float node_4355 = (1.0-max(0,dot(normalDirection, viewDirection)));
                float node_7238 = saturate((node_4355*-0.8333333+0.5833334));
                float node_1489 = clamp((node_4355*-2.5+0.5),0,10);
                float3 emissive = (_Color.rgb*(node_7238+node_1489)*_Value_alpha);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
