// Shader created with Shader Forge v1.42 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Enhanced by Antoine Guillon / Arkham Development - http://www.arkham-development.com/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.42;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:34315,y:32664,varname:node_3138,prsc:2|emission-5646-OUT,alpha-6481-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:33453,y:32720,ptovrint:False,ptlb:Color_Outline,ptin:_Color_Outline,varname:_Color_Outline,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;n:type:ShaderForge.SFN_Fresnel,id:836,x:32151,y:32920,varname:node_836,prsc:2;n:type:ShaderForge.SFN_RemapRange,id:1196,x:32316,y:32902,varname:node_1196,prsc:2,frmn:0,frmx:1,tomn:6,tomx:-0.3|IN-836-OUT;n:type:ShaderForge.SFN_Clamp01,id:2112,x:32481,y:32888,cmnt:Outline,varname:node_2112,prsc:2|IN-1196-OUT;n:type:ShaderForge.SFN_Multiply,id:6808,x:33726,y:32685,cmnt:Outline Col,varname:node_6808,prsc:2|A-607-OUT,B-4388-OUT,C-7241-RGB;n:type:ShaderForge.SFN_Vector1,id:4388,x:33726,y:32810,varname:node_4388,prsc:2,v1:6;n:type:ShaderForge.SFN_Tex2d,id:6345,x:32511,y:32665,ptovrint:False,ptlb:Texture_Ref,ptin:_Texture_Ref,varname:_Texture_Ref,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-5564-OUT;n:type:ShaderForge.SFN_NormalVector,id:1775,x:31875,y:32730,prsc:2,pt:False;n:type:ShaderForge.SFN_Transform,id:9467,x:32032,y:32741,varname:node_9467,prsc:2,tffrom:0,tfto:3|IN-1775-OUT;n:type:ShaderForge.SFN_RemapRange,id:9485,x:32201,y:32741,varname:node_9485,prsc:2,frmn:-1,frmx:1,tomn:0,tomx:1|IN-9467-XYZ;n:type:ShaderForge.SFN_ComponentMask,id:5564,x:32367,y:32741,varname:node_5564,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-9485-OUT;n:type:ShaderForge.SFN_Subtract,id:4879,x:32671,y:32384,varname:node_4879,prsc:2|A-7076-OUT,B-6345-R;n:type:ShaderForge.SFN_Multiply,id:1757,x:33283,y:32068,varname:node_1757,prsc:2|A-4703-OUT,B-7564-RGB,C-8762-OUT,D-1194-OUT;n:type:ShaderForge.SFN_Color,id:7564,x:32963,y:32086,ptovrint:False,ptlb:Color_Base,ptin:_Color_Base,varname:_Color_Base,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Clamp01,id:4703,x:33014,y:32384,varname:node_4703,prsc:2|IN-5432-OUT;n:type:ShaderForge.SFN_Vector1,id:8762,x:33244,y:32188,varname:node_8762,prsc:2,v1:4;n:type:ShaderForge.SFN_OneMinus,id:2050,x:32699,y:32888,varname:node_2050,prsc:2|IN-2112-OUT;n:type:ShaderForge.SFN_Subtract,id:5432,x:32839,y:32384,varname:node_5432,prsc:2|A-4879-OUT,B-2050-OUT;n:type:ShaderForge.SFN_Multiply,id:6481,x:33669,y:32965,varname:node_6481,prsc:2|A-4703-OUT,B-7501-OUT,C-1194-OUT,D-5813-OUT,E-2538-OUT;n:type:ShaderForge.SFN_Vector1,id:7501,x:33669,y:33084,varname:node_7501,prsc:2,v1:0.7;n:type:ShaderForge.SFN_FragmentPosition,id:9560,x:31430,y:30913,varname:node_9560,prsc:2;n:type:ShaderForge.SFN_RemapRange,id:2485,x:32562,y:31748,varname:node_2485,prsc:2,frmn:-0.02,frmx:0.02,tomn:0,tomx:1|IN-9382-OUT;n:type:ShaderForge.SFN_Clamp01,id:1194,x:32769,y:31748,varname:node_1194,prsc:2|IN-2485-OUT;n:type:ShaderForge.SFN_RemapRange,id:4410,x:32604,y:31572,varname:node_4410,prsc:2,frmn:-0.05,frmx:-0.5,tomn:1,tomx:0|IN-9382-OUT;n:type:ShaderForge.SFN_Clamp01,id:5813,x:32807,y:31572,varname:node_5813,prsc:2|IN-4410-OUT;n:type:ShaderForge.SFN_Multiply,id:8475,x:33847,y:32541,varname:node_8475,prsc:2|A-1757-OUT,B-5813-OUT,C-2538-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2538,x:33835,y:32481,ptovrint:False,ptlb:Value_Alpha,ptin:_Value_Alpha,varname:_Value_Alpha,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:3841,x:30703,y:32783,ptovrint:False,ptlb:Outline_Width,ptin:_Outline_Width,varname:_Outline_Width,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_Code,id:3512,x:31755,y:31087,varname:node_3512,prsc:2,code:cgBlAHQAdQByAG4AIABtAHUAbAAoAHcAbwByAGwAZABQAG8AcwAgAC0AIABhAG4AYwBoAG8AcgBQAG8AcwAsACAAdAApADsA,output:3,fname:Function_node_3512,width:633,height:133,input:2,input:2,input:13,input_1_label:worldPos,input_2_label:anchorPos,input_3_label:t|A-9560-XYZ,B-1451-XYZ,C-5595-OUT;n:type:ShaderForge.SFN_ComponentMask,id:9382,x:32069,y:31388,varname:node_9382,prsc:2,cc1:1,cc2:-1,cc3:-1,cc4:-1|IN-3512-OUT;n:type:ShaderForge.SFN_Vector4Property,id:1451,x:31388,y:31098,ptovrint:False,ptlb:anchorPos,ptin:_anchorPos,varname:_anchorPos,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0,v2:0,v3:0,v4:0;n:type:ShaderForge.SFN_Matrix4x4Property,id:5595,x:31445,y:31296,ptovrint:False,ptlb:transformMatrix,ptin:transformMatrix,varname:node_5595,prsc:2,glob:True,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,m00:1,m01:0,m02:0,m03:0,m10:0,m11:1,m12:0,m13:0,m20:0,m21:0,m22:1,m23:0,m30:0,m31:0,m32:0,m33:1;n:type:ShaderForge.SFN_Vector1,id:1861,x:32479,y:35721,varname:node_1861,prsc:2,v1:0.001;n:type:ShaderForge.SFN_VertexColor,id:4766,x:33356,y:31678,varname:node_4766,prsc:2;n:type:ShaderForge.SFN_Multiply,id:607,x:33975,y:31717,varname:node_607,prsc:2|A-7076-OUT,B-3242-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:7555,x:33728,y:31804,varname:node_7555,prsc:2,min:0,max:10|IN-2529-OUT;n:type:ShaderForge.SFN_Tex2d,id:8760,x:32290,y:32272,varname:node_8760,prsc:2,ntxv:0,isnm:False|UVIN-4735-OUT,TEX-4431-TEX;n:type:ShaderForge.SFN_Tex2dAsset,id:4431,x:32290,y:32101,ptovrint:False,ptlb:Texture_Dot_M,ptin:_Texture_Dot_M,varname:node_4431,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Vector1,id:8872,x:32543,y:35785,varname:node_8872,prsc:2,v1:0.001;n:type:ShaderForge.SFN_Tex2d,id:8121,x:32290,y:32409,varname:node_8121,prsc:2,ntxv:0,isnm:False|UVIN-7769-OUT,TEX-4431-TEX;n:type:ShaderForge.SFN_Add,id:7076,x:32451,y:32337,varname:node_7076,prsc:2|A-8760-R,B-8121-R;n:type:ShaderForge.SFN_TexCoord,id:7960,x:30921,y:32569,varname:node_7960,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:6171,x:31309,y:32639,varname:node_6171,prsc:2|A-7960-UVOUT,B-1153-OUT,C-4997-OUT;n:type:ShaderForge.SFN_Vector1,id:1153,x:31291,y:32761,varname:node_1153,prsc:2,v1:1.37;n:type:ShaderForge.SFN_ComponentMask,id:1948,x:31464,y:32639,varname:node_1948,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-6171-OUT;n:type:ShaderForge.SFN_Time,id:7991,x:31370,y:31804,varname:node_7991,prsc:2;n:type:ShaderForge.SFN_Multiply,id:9391,x:31387,y:31930,varname:node_9391,prsc:2|A-7991-T,B-5103-OUT;n:type:ShaderForge.SFN_Multiply,id:9771,x:31387,y:32059,varname:node_9771,prsc:2|A-7991-T,B-8846-OUT;n:type:ShaderForge.SFN_Multiply,id:534,x:31387,y:32187,varname:node_534,prsc:2|A-7991-T,B-7976-OUT;n:type:ShaderForge.SFN_Multiply,id:3628,x:31387,y:32316,varname:node_3628,prsc:2|A-7991-T,B-3518-OUT;n:type:ShaderForge.SFN_Vector1,id:5103,x:31211,y:31930,varname:node_5103,prsc:2,v1:-1.7;n:type:ShaderForge.SFN_Vector1,id:8846,x:31211,y:32059,varname:node_8846,prsc:2,v1:0.15;n:type:ShaderForge.SFN_Vector1,id:7976,x:31199,y:32187,varname:node_7976,prsc:2,v1:-1.26;n:type:ShaderForge.SFN_Vector1,id:3518,x:31199,y:32316,varname:node_3518,prsc:2,v1:-0.27;n:type:ShaderForge.SFN_Add,id:6044,x:31718,y:32180,varname:node_6044,prsc:2|A-7872-R,B-9391-OUT;n:type:ShaderForge.SFN_Add,id:6990,x:31718,y:32306,varname:node_6990,prsc:2|A-7872-G,B-9771-OUT;n:type:ShaderForge.SFN_Add,id:7143,x:31718,y:32429,varname:node_7143,prsc:2|A-1948-R,B-534-OUT;n:type:ShaderForge.SFN_Add,id:8812,x:31718,y:32555,varname:node_8812,prsc:2|A-1948-G,B-3628-OUT;n:type:ShaderForge.SFN_Append,id:4735,x:31893,y:32235,varname:node_4735,prsc:2|A-6044-OUT,B-6990-OUT;n:type:ShaderForge.SFN_Append,id:7769,x:31895,y:32509,varname:node_7769,prsc:2|A-7143-OUT,B-8812-OUT;n:type:ShaderForge.SFN_RemapRange,id:7973,x:30912,y:32794,varname:node_7973,prsc:2,frmn:0,frmx:1,tomn:0.35,tomx:1|IN-3841-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:4997,x:31092,y:32794,varname:node_4997,prsc:2,min:0.3,max:1.5|IN-7973-OUT;n:type:ShaderForge.SFN_Multiply,id:1807,x:31309,y:32483,varname:node_1807,prsc:2|A-7960-UVOUT,B-4997-OUT;n:type:ShaderForge.SFN_ComponentMask,id:7872,x:31464,y:32483,varname:node_7872,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-1807-OUT;n:type:ShaderForge.SFN_RemapRangeAdvanced,id:2529,x:33552,y:31804,varname:node_2529,prsc:2|IN-4766-R,IMIN-7631-OUT,IMAX-6083-OUT,OMIN-7537-OUT,OMAX-5800-OUT;n:type:ShaderForge.SFN_Vector1,id:6083,x:33552,y:31922,varname:node_6083,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:7537,x:33552,y:31974,varname:node_7537,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:5800,x:33552,y:32027,varname:node_5800,prsc:2,v1:3;n:type:ShaderForge.SFN_RemapRange,id:3224,x:33382,y:32430,varname:node_3224,prsc:2,frmn:0.2,frmx:0.5,tomn:0,tomx:0.3|IN-3841-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:7631,x:33540,y:32430,varname:node_7631,prsc:2,min:0,max:0.4|IN-3224-OUT;n:type:ShaderForge.SFN_Add,id:5646,x:34088,y:32686,varname:node_5646,prsc:2|A-8475-OUT,B-6808-OUT;n:type:ShaderForge.SFN_RemapRange,id:2620,x:32562,y:31922,varname:node_2620,prsc:2,frmn:0.03,frmx:0.04,tomn:1,tomx:0|IN-9382-OUT;n:type:ShaderForge.SFN_Clamp01,id:1489,x:32769,y:31922,varname:node_1489,prsc:2|IN-2620-OUT;n:type:ShaderForge.SFN_Multiply,id:4569,x:33182,y:31534,varname:node_4569,prsc:2|A-1194-OUT,B-1489-OUT,C-7365-OUT;n:type:ShaderForge.SFN_Add,id:3242,x:33651,y:31572,varname:node_3242,prsc:2|A-4569-OUT,B-7555-OUT;n:type:ShaderForge.SFN_Vector1,id:7365,x:33182,y:31656,varname:node_7365,prsc:2,v1:3;proporder:7241-6345-7564-2538-3841-1451-4431;pass:END;sub:END;*/

Shader "Shader Forge/HDN_SH_EnergyBall_ShieldBraker" {
    Properties {
        _Color_Outline ("Color_Outline", Color) = (0.07843138,0.3921569,0.7843137,1)
        _Texture_Ref ("Texture_Ref", 2D) = "white" {}
        _Color_Base ("Color_Base", Color) = (0.5,0.5,0.5,1)
        _Value_Alpha ("Value_Alpha", Float ) = 1
        _Outline_Width ("Outline_Width", Float ) = 0.5
        _anchorPos ("anchorPos", Vector) = (0,0,0,0)
        _Texture_Dot_M ("Texture_Dot_M", 2D) = "white" {}
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
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu switch vulkan 
            #pragma target 3.0
            uniform float4 _Color_Outline;
            uniform sampler2D _Texture_Ref; uniform float4 _Texture_Ref_ST;
            uniform float4 _Color_Base;
            uniform float _Value_Alpha;
            uniform float _Outline_Width;
            float4 Function_node_3512( float3 worldPos , float3 anchorPos , float4x4 t ){
            return mul(worldPos - anchorPos, t);
            }
            
            uniform float4 _anchorPos;
            uniform float4x4 transformMatrix;
            uniform sampler2D _Texture_Dot_M; uniform float4 _Texture_Dot_M_ST;
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
                float node_4997 = clamp((_Outline_Width*0.65+0.35),0.3,1.5);
                float2 node_7872 = (i.uv0*node_4997).rg;
                float4 node_7991 = _Time;
                float2 node_4735 = float2((node_7872.r+(node_7991.g*(-1.7))),(node_7872.g+(node_7991.g*0.15)));
                float4 node_8760 = tex2D(_Texture_Dot_M,TRANSFORM_TEX(node_4735, _Texture_Dot_M));
                float2 node_1948 = (i.uv0*1.37*node_4997).rg;
                float2 node_7769 = float2((node_1948.r+(node_7991.g*(-1.26))),(node_1948.g+(node_7991.g*(-0.27))));
                float4 node_8121 = tex2D(_Texture_Dot_M,TRANSFORM_TEX(node_7769, _Texture_Dot_M));
                float node_7076 = (node_8760.r+node_8121.r);
                float2 node_5564 = (mul( UNITY_MATRIX_V, float4(i.normalDir,0) ).xyz.rgb*0.5+0.5).rg;
                float4 _Texture_Ref_var = tex2D(_Texture_Ref,TRANSFORM_TEX(node_5564, _Texture_Ref));
                float node_836 = (1.0-max(0,dot(normalDirection, viewDirection)));
                float node_2112 = saturate((node_836*-6.3+6.0)); // Outline
                float node_4703 = saturate(((node_7076-_Texture_Ref_var.r)-(1.0 - node_2112)));
                float node_9382 = Function_node_3512( i.posWorld.rgb , _anchorPos.rgb , transformMatrix ).g;
                float node_1194 = saturate((node_9382*25.0+0.5));
                float3 node_1757 = (node_4703*_Color_Base.rgb*4.0*node_1194);
                float node_5813 = saturate((node_9382*2.222222+1.111111));
                float node_7631 = clamp((_Outline_Width*1.0+-0.2),0,0.4);
                float node_7537 = 0.0;
                float node_607 = (node_7076*((node_1194*saturate((node_9382*-100.0+4.0))*3.0)+clamp((node_7537 + ( (i.vertexColor.r - node_7631) * (3.0 - node_7537) ) / (1.0 - node_7631)),0,10)));
                float3 node_6808 = (node_607*6.0*_Color_Outline.rgb); // Outline Col
                float3 emissive = ((node_1757*node_5813*_Value_Alpha)+node_6808);
                float3 finalColor = emissive;
                return fixed4(finalColor,(node_4703*0.7*node_1194*node_5813*_Value_Alpha));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
