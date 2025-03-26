// Shader created with Shader Forge v1.42 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Enhanced by Antoine Guillon / Arkham Development - http://www.arkham-development.com/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.42;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33380,y:32739,varname:node_3138,prsc:2|emission-8237-OUT,alpha-3284-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32476,y:32627,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;n:type:ShaderForge.SFN_Tex2d,id:7059,x:32476,y:32796,ptovrint:False,ptlb:Texture_Color,ptin:_Texture_Color,varname:node_7059,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_VertexColor,id:722,x:32476,y:32965,varname:node_722,prsc:2;n:type:ShaderForge.SFN_Multiply,id:8237,x:32753,y:32701,varname:node_8237,prsc:2|A-7241-RGB,B-7059-R,C-722-A,D-1739-OUT,E-4244-OUT;n:type:ShaderForge.SFN_Multiply,id:3284,x:32731,y:32951,varname:node_3284,prsc:2|A-7059-R,B-722-A,C-2669-OUT,D-4244-OUT;n:type:ShaderForge.SFN_Vector1,id:2669,x:32731,y:33068,varname:node_2669,prsc:2,v1:0.3;n:type:ShaderForge.SFN_Vector1,id:1739,x:32753,y:32819,varname:node_1739,prsc:2,v1:2;n:type:ShaderForge.SFN_FragmentPosition,id:3495,x:31985,y:32282,varname:node_3495,prsc:2;n:type:ShaderForge.SFN_RemapRange,id:6661,x:32873,y:32485,varname:node_6661,prsc:2,frmn:-0.1,frmx:0.1,tomn:0,tomx:1|IN-711-OUT;n:type:ShaderForge.SFN_Clamp01,id:4244,x:33045,y:32485,varname:node_4244,prsc:2|IN-6661-OUT;n:type:ShaderForge.SFN_Code,id:7673,x:32233,y:32405,varname:node_7673,prsc:2,code:cgBlAHQAdQByAG4AIABtAHUAbAAoAHcAbwByAGwAZABQAG8AcwAgAC0AIABhAG4AYwBoAG8AcgBQAG8AcwAsACAAdAApADsA,output:2,fname:Function_node_7673,width:369,height:132,input:2,input:2,input:13,input_1_label:worldPos,input_2_label:anchorPos,input_3_label:t|A-3495-XYZ,B-4681-XYZ,C-3609-OUT;n:type:ShaderForge.SFN_ComponentMask,id:711,x:32702,y:32485,varname:node_711,prsc:2,cc1:1,cc2:-1,cc3:-1,cc4:-1|IN-7673-OUT;n:type:ShaderForge.SFN_Vector4Property,id:4681,x:31985,y:32435,ptovrint:False,ptlb:anchorPos,ptin:_anchorPos,varname:node_4681,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0,v2:0,v3:0,v4:0;n:type:ShaderForge.SFN_Matrix4x4Property,id:3609,x:31985,y:32634,ptovrint:False,ptlb:transformMatrix,ptin:transformMatrix,varname:node_3609,prsc:2,glob:True,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,m00:1,m01:0,m02:0,m03:0,m10:0,m11:1,m12:0,m13:0,m20:0,m21:0,m22:1,m23:0,m30:0,m31:0,m32:0,m33:1;proporder:7241-7059-4681;pass:END;sub:END;*/

Shader "Shader Forge/HDN_SH_Effect_FlatTex" {
    Properties {
        _Color ("Color", Color) = (0.07843138,0.3921569,0.7843137,1)
        _Texture_Color ("Texture_Color", 2D) = "white" {}
        _anchorPos ("anchorPos", Vector) = (0,0,0,0)
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
            #ifndef UNITY_PASS_FORWARDBASE
            #define UNITY_PASS_FORWARDBASE
            #endif //UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal vulkan 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _Texture_Color; uniform float4 _Texture_Color_ST;
            float3 Function_node_7673( float3 worldPos , float3 anchorPos , float4x4 t ){
            return mul(worldPos - anchorPos, t);
            }
            
            uniform float4 _anchorPos;
            uniform float4x4 transformMatrix;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 _Texture_Color_var = tex2D(_Texture_Color,TRANSFORM_TEX(i.uv0, _Texture_Color));
                float node_4244 = saturate((Function_node_7673( i.posWorld.rgb , _anchorPos.rgb , transformMatrix ).g*5.0+0.5));
                float3 emissive = (_Color.rgb*_Texture_Color_var.r*i.vertexColor.a*2.0*node_4244);
                float3 finalColor = emissive;
                return fixed4(finalColor,(_Texture_Color_var.r*i.vertexColor.a*0.3*node_4244));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
