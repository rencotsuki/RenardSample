// Shader created with Shader Forge v1.42 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Enhanced by Antoine Guillon / Arkham Development - http://www.arkham-development.com/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.42;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:34145,y:32356,varname:node_3138,prsc:2|emission-4868-OUT,alpha-2911-OUT,voffset-3872-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:6572,x:30530,y:31133,ptovrint:False,ptlb:Texture_Photo,ptin:_Texture_Photo,varname:node_6572,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:1935,x:30758,y:31211,varname:node_1935,prsc:2,ntxv:0,isnm:False|UVIN-7816-OUT,TEX-6572-TEX;n:type:ShaderForge.SFN_Fresnel,id:5803,x:33128,y:31686,varname:node_5803,prsc:2;n:type:ShaderForge.SFN_RemapRange,id:5642,x:33312,y:31686,varname:node_5642,prsc:2,frmn:0.3,frmx:1,tomn:0,tomx:1|IN-5803-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:2783,x:33502,y:31686,varname:node_2783,prsc:2,min:0,max:1|IN-5642-OUT;n:type:ShaderForge.SFN_Multiply,id:5356,x:33865,y:31686,varname:node_5356,prsc:2|A-2783-OUT,B-484-RGB;n:type:ShaderForge.SFN_Color,id:484,x:33886,y:31548,ptovrint:False,ptlb:Color_Fresnel,ptin:_Color_Fresnel,varname:node_484,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Add,id:5109,x:32865,y:32437,varname:node_5109,prsc:2|A-1282-OUT,B-5356-OUT,C-7718-OUT,D-6407-OUT,E-6559-OUT;n:type:ShaderForge.SFN_Slider,id:5470,x:33177,y:32058,ptovrint:False,ptlb:Appear,ptin:_Appear,varname:node_5470,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Multiply,id:2911,x:33716,y:32851,varname:node_2911,prsc:2|A-6289-OUT,B-8308-OUT,C-463-OUT,D-4276-OUT;n:type:ShaderForge.SFN_Slider,id:7611,x:31194,y:34264,ptovrint:False,ptlb:Damage,ptin:_Damage,varname:node_7611,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:7955,x:32720,y:33484,ptovrint:False,ptlb:Break,ptin:_Break,varname:node_7955,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_ConstantClamp,id:6289,x:33490,y:32851,varname:node_6289,prsc:2,min:0,max:1|IN-8790-OUT;n:type:ShaderForge.SFN_Time,id:5395,x:30582,y:33381,varname:node_5395,prsc:2;n:type:ShaderForge.SFN_Append,id:2170,x:31041,y:33391,varname:node_2170,prsc:2|A-1032-OUT,B-4990-OUT;n:type:ShaderForge.SFN_Multiply,id:4990,x:30759,y:33358,varname:node_4990,prsc:2|A-5395-T,B-1167-OUT;n:type:ShaderForge.SFN_Vector1,id:1167,x:30759,y:33477,varname:node_1167,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Vector1,id:1032,x:31233,y:33621,varname:node_1032,prsc:2,v1:0.9;n:type:ShaderForge.SFN_RemapRange,id:7482,x:31461,y:33353,varname:node_7482,prsc:2,frmn:0,frmx:1,tomn:-0.1,tomx:0.1|IN-3309-R;n:type:ShaderForge.SFN_RemapRange,id:9343,x:31461,y:33519,varname:node_9343,prsc:2,frmn:0,frmx:1,tomn:0.1,tomx:-0.1|IN-3309-R;n:type:ShaderForge.SFN_TexCoord,id:3302,x:29920,y:31487,varname:node_3302,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Append,id:7816,x:30323,y:31504,varname:node_7816,prsc:2|A-4960-OUT,B-3302-V;n:type:ShaderForge.SFN_Add,id:4960,x:30152,y:31425,varname:node_4960,prsc:2|A-3302-U,B-1936-OUT;n:type:ShaderForge.SFN_Add,id:3258,x:30151,y:31600,varname:node_3258,prsc:2|A-3302-U,B-3525-OUT;n:type:ShaderForge.SFN_Append,id:9274,x:30323,y:31679,varname:node_9274,prsc:2|A-3258-OUT,B-3302-V;n:type:ShaderForge.SFN_Append,id:1282,x:32431,y:32271,varname:node_1282,prsc:2|A-5522-OUT,B-6729-OUT,C-1370-OUT;n:type:ShaderForge.SFN_Tex2d,id:7256,x:30758,y:31339,varname:node_7256,prsc:2,ntxv:0,isnm:False|TEX-6572-TEX;n:type:ShaderForge.SFN_Tex2d,id:2320,x:30758,y:31475,varname:node_2320,prsc:2,ntxv:0,isnm:False|UVIN-9274-OUT,TEX-6572-TEX;n:type:ShaderForge.SFN_Multiply,id:7551,x:32530,y:32693,varname:node_7551,prsc:2|A-5453-OUT,B-9645-OUT,C-1741-R;n:type:ShaderForge.SFN_Multiply,id:4765,x:32530,y:32821,varname:node_4765,prsc:2|A-555-OUT,B-9645-OUT,C-4383-R;n:type:ShaderForge.SFN_Multiply,id:229,x:32530,y:32949,varname:node_229,prsc:2|A-143-OUT,B-9645-OUT,C-6523-R;n:type:ShaderForge.SFN_Vector1,id:9645,x:32523,y:33073,varname:node_9645,prsc:2,v1:0.35;n:type:ShaderForge.SFN_Add,id:890,x:32808,y:32881,varname:node_890,prsc:2|A-7551-OUT,B-4765-OUT,C-229-OUT;n:type:ShaderForge.SFN_Clamp01,id:9435,x:33112,y:32851,varname:node_9435,prsc:2|IN-890-OUT;n:type:ShaderForge.SFN_Slider,id:8105,x:30810,y:33935,ptovrint:False,ptlb:NoiseOn,ptin:_NoiseOn,varname:node_8105,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:1936,x:31631,y:33353,varname:node_1936,prsc:2|A-7482-OUT,B-4006-OUT;n:type:ShaderForge.SFN_Multiply,id:3525,x:31653,y:33519,varname:node_3525,prsc:2|A-9343-OUT,B-4708-OUT,C-4006-OUT;n:type:ShaderForge.SFN_RemapRange,id:9856,x:32708,y:33310,varname:node_9856,prsc:2,frmn:0,frmx:1,tomn:0.5,tomx:0|IN-4006-OUT;n:type:ShaderForge.SFN_Vector1,id:828,x:33276,y:32973,varname:node_828,prsc:2,v1:1;n:type:ShaderForge.SFN_RemapRangeAdvanced,id:8790,x:33301,y:32851,varname:node_8790,prsc:2|IN-9435-OUT,IMIN-9856-OUT,IMAX-828-OUT,OMIN-821-OUT,OMAX-828-OUT;n:type:ShaderForge.SFN_Vector1,id:821,x:33276,y:33024,varname:node_821,prsc:2,v1:0;n:type:ShaderForge.SFN_Tex2dAsset,id:4546,x:31957,y:33562,ptovrint:False,ptlb:Texture_SideMask,ptin:_Texture_SideMask,varname:node_4546,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:4383,x:32239,y:33595,varname:node_4383,prsc:2,ntxv:0,isnm:False|TEX-4546-TEX;n:type:ShaderForge.SFN_Tex2d,id:6523,x:32239,y:33723,varname:node_6523,prsc:2,ntxv:0,isnm:False|UVIN-9274-OUT,TEX-4546-TEX;n:type:ShaderForge.SFN_Tex2d,id:1741,x:32239,y:33437,varname:node_1741,prsc:2,ntxv:0,isnm:False|UVIN-7816-OUT,TEX-4546-TEX;n:type:ShaderForge.SFN_ValueProperty,id:7705,x:31169,y:34088,ptovrint:False,ptlb:Value_LifeRemain,ptin:_Value_LifeRemain,varname:node_7705,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:4;n:type:ShaderForge.SFN_RemapRange,id:7913,x:31367,y:34054,varname:node_7913,prsc:2,frmn:1.3,frmx:1.4,tomn:0.5,tomx:0|IN-7705-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:8823,x:31540,y:34063,varname:node_8823,prsc:2,min:0,max:0.5|IN-7913-OUT;n:type:ShaderForge.SFN_Tex2d,id:945,x:32561,y:31057,ptovrint:False,ptlb:Texture_Noise,ptin:_Texture_Noise,varname:node_945,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-2626-OUT;n:type:ShaderForge.SFN_TexCoord,id:5111,x:31642,y:30701,varname:node_5111,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Time,id:2551,x:31964,y:31227,varname:node_2551,prsc:2;n:type:ShaderForge.SFN_Multiply,id:8084,x:32139,y:31171,varname:node_8084,prsc:2|A-2551-T,B-3481-OUT;n:type:ShaderForge.SFN_Multiply,id:4518,x:32139,y:31293,varname:node_4518,prsc:2|A-2551-T,B-8126-OUT;n:type:ShaderForge.SFN_Vector1,id:3481,x:32139,y:31120,varname:node_3481,prsc:2,v1:17.39;n:type:ShaderForge.SFN_Vector1,id:8126,x:32139,y:31422,varname:node_8126,prsc:2,v1:25.47;n:type:ShaderForge.SFN_Add,id:4273,x:32127,y:30876,varname:node_4273,prsc:2|A-1891-OUT,B-8084-OUT;n:type:ShaderForge.SFN_Add,id:225,x:32127,y:30999,varname:node_225,prsc:2|A-5634-OUT,B-4518-OUT;n:type:ShaderForge.SFN_Append,id:2626,x:32358,y:31053,varname:node_2626,prsc:2|A-4273-OUT,B-225-OUT;n:type:ShaderForge.SFN_Multiply,id:7956,x:33088,y:32437,varname:node_7956,prsc:2|A-5109-OUT,B-4042-OUT,C-8308-OUT;n:type:ShaderForge.SFN_Lerp,id:4042,x:32903,y:31044,varname:node_4042,prsc:2|A-7097-OUT,B-8355-OUT,T-4006-OUT;n:type:ShaderForge.SFN_Vector1,id:7097,x:32903,y:30985,varname:node_7097,prsc:2,v1:1;n:type:ShaderForge.SFN_RemapRange,id:8355,x:32736,y:31044,varname:node_8355,prsc:2,frmn:0,frmx:1,tomn:0.5,tomx:1.5|IN-945-R;n:type:ShaderForge.SFN_Add,id:5695,x:31653,y:33712,varname:node_5695,prsc:2|A-4708-OUT,B-8823-OUT,C-7355-OUT,D-7611-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:4006,x:31810,y:33696,varname:node_4006,prsc:2,min:0,max:1|IN-5695-OUT;n:type:ShaderForge.SFN_RemapRange,id:158,x:33392,y:33252,varname:node_158,prsc:2,frmn:0,frmx:1,tomn:-20,tomx:0|IN-945-R;n:type:ShaderForge.SFN_Add,id:8942,x:33582,y:33252,varname:node_8942,prsc:2|A-158-OUT,B-2248-OUT;n:type:ShaderForge.SFN_Multiply,id:2248,x:33863,y:33432,varname:node_2248,prsc:2|A-5120-OUT,B-2377-OUT;n:type:ShaderForge.SFN_Vector1,id:2377,x:33744,y:33589,varname:node_2377,prsc:2,v1:21;n:type:ShaderForge.SFN_ConstantClamp,id:8308,x:33758,y:33252,varname:node_8308,prsc:2,min:0,max:1|IN-8942-OUT;n:type:ShaderForge.SFN_RemapRange,id:8420,x:33705,y:33702,varname:node_8420,prsc:2,frmn:0.9,frmx:1,tomn:1,tomx:0|IN-6404-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:7355,x:33886,y:33737,varname:node_7355,prsc:2,min:0,max:1|IN-8420-OUT;n:type:ShaderForge.SFN_Multiply,id:7718,x:34087,y:33904,varname:node_7718,prsc:2|A-4282-OUT,B-8828-OUT;n:type:ShaderForge.SFN_Vector3,id:8828,x:34087,y:34053,varname:node_8828,prsc:2,v1:0.05873977,v2:0.3299046,v3:0.3773585;n:type:ShaderForge.SFN_RemapRange,id:8327,x:33657,y:33928,varname:node_8327,prsc:2,frmn:0.5,frmx:1,tomn:1,tomx:0|IN-6404-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:4282,x:33833,y:33928,varname:node_4282,prsc:2,min:0,max:1|IN-8327-OUT;n:type:ShaderForge.SFN_Multiply,id:6407,x:31748,y:34166,varname:node_6407,prsc:2|A-7611-OUT,B-657-OUT;n:type:ShaderForge.SFN_Vector3,id:657,x:31748,y:34315,varname:node_657,prsc:2,v1:0.2264151,v2:0.1693536,v3:0.02669989;n:type:ShaderForge.SFN_RemapRange,id:4324,x:33073,y:33498,varname:node_4324,prsc:2,frmn:0.5,frmx:1,tomn:0,tomx:1|IN-7955-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:9404,x:33244,y:33498,varname:node_9404,prsc:2,min:0,max:1|IN-4324-OUT;n:type:ShaderForge.SFN_Subtract,id:5120,x:33613,y:33491,varname:node_5120,prsc:2|A-6404-OUT,B-9404-OUT;n:type:ShaderForge.SFN_RemapRange,id:9319,x:32892,y:33662,varname:node_9319,prsc:2,frmn:0,frmx:0.1,tomn:0,tomx:1|IN-7955-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:949,x:33054,y:33662,varname:node_949,prsc:2,min:0,max:1|IN-9319-OUT;n:type:ShaderForge.SFN_Multiply,id:6559,x:33054,y:33849,varname:node_6559,prsc:2|A-949-OUT,B-1860-OUT;n:type:ShaderForge.SFN_Vector3,id:1860,x:33054,y:33998,varname:node_1860,prsc:2,v1:0.227451,v2:0.04392763,v3:0.02745098;n:type:ShaderForge.SFN_Multiply,id:1891,x:31882,y:30902,varname:node_1891,prsc:2|A-5111-U,B-1990-OUT;n:type:ShaderForge.SFN_Multiply,id:5634,x:31846,y:31138,varname:node_5634,prsc:2|A-5111-V,B-1990-OUT;n:type:ShaderForge.SFN_Vector1,id:9096,x:31845,y:30821,varname:node_9096,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Vector1,id:3041,x:31846,y:31081,varname:node_3041,prsc:2,v1:1;n:type:ShaderForge.SFN_Time,id:6495,x:34017,y:32856,varname:node_6495,prsc:2;n:type:ShaderForge.SFN_Sin,id:7057,x:34304,y:32859,varname:node_7057,prsc:2|IN-38-OUT;n:type:ShaderForge.SFN_Multiply,id:4098,x:34476,y:32859,varname:node_4098,prsc:2|A-7057-OUT,B-9960-OUT;n:type:ShaderForge.SFN_Vector1,id:9960,x:34476,y:32975,varname:node_9960,prsc:2,v1:0.05;n:type:ShaderForge.SFN_Append,id:3872,x:34663,y:32859,varname:node_3872,prsc:2|A-9266-OUT,B-4098-OUT,C-9266-OUT;n:type:ShaderForge.SFN_Vector1,id:9266,x:34663,y:32975,varname:node_9266,prsc:2,v1:0;n:type:ShaderForge.SFN_Depth,id:5171,x:33426,y:32573,varname:node_5171,prsc:2;n:type:ShaderForge.SFN_RemapRange,id:7870,x:33598,y:32573,varname:node_7870,prsc:2,frmn:0.5,frmx:1.2,tomn:0.3,tomx:1|IN-5171-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:463,x:33759,y:32573,varname:node_463,prsc:2,min:0.3,max:1|IN-7870-OUT;n:type:ShaderForge.SFN_Lerp,id:350,x:33311,y:32437,varname:node_350,prsc:2|A-7956-OUT,B-387-OUT,T-7858-OUT;n:type:ShaderForge.SFN_Vector3,id:4007,x:33269,y:32334,varname:node_4007,prsc:2,v1:0.9,v2:0.9,v3:0.9;n:type:ShaderForge.SFN_ValueProperty,id:7858,x:33311,y:32283,ptovrint:False,ptlb:Back,ptin:_Back,varname:node_7858,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Tex2d,id:3309,x:31247,y:33471,varname:node_3309,prsc:2,ntxv:0,isnm:False|UVIN-2170-OUT,TEX-7389-TEX;n:type:ShaderForge.SFN_Tex2dAsset,id:7389,x:31234,y:33304,ptovrint:False,ptlb:Texture_Blur,ptin:_Texture_Blur,varname:node_7389,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:3979,x:30737,y:33564,varname:node_3979,prsc:2|A-5395-T,B-8413-OUT;n:type:ShaderForge.SFN_Vector1,id:8413,x:30737,y:33691,varname:node_8413,prsc:2,v1:0.13;n:type:ShaderForge.SFN_Append,id:1456,x:30935,y:33553,varname:node_1456,prsc:2|A-1032-OUT,B-3979-OUT;n:type:ShaderForge.SFN_RemapRange,id:3763,x:31106,y:33728,varname:node_3763,prsc:2,frmn:0.7,frmx:1,tomn:0,tomx:0.7|IN-2681-R;n:type:ShaderForge.SFN_ConstantClamp,id:3153,x:31259,y:33728,varname:node_3153,prsc:2,min:0,max:1|IN-3763-OUT;n:type:ShaderForge.SFN_Add,id:9007,x:31200,y:33915,varname:node_9007,prsc:2|A-8105-OUT,B-3153-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:4708,x:31388,y:33891,varname:node_4708,prsc:2,min:0,max:1|IN-9007-OUT;n:type:ShaderForge.SFN_Tex2d,id:2681,x:30889,y:33768,varname:node_2681,prsc:2,ntxv:0,isnm:False|UVIN-1456-OUT,TEX-7389-TEX;n:type:ShaderForge.SFN_Vector1,id:6404,x:33251,y:33651,varname:node_6404,prsc:2,v1:1;n:type:ShaderForge.SFN_TexCoord,id:9654,x:33139,y:31880,varname:node_9654,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:833,x:33535,y:31902,varname:node_833,prsc:2|A-7392-OUT,B-7613-OUT,C-6068-OUT;n:type:ShaderForge.SFN_Multiply,id:6068,x:33533,y:32040,varname:node_6068,prsc:2|A-5470-OUT,B-4997-OUT;n:type:ShaderForge.SFN_Vector1,id:4997,x:33535,y:32176,varname:node_4997,prsc:2,v1:2.3;n:type:ShaderForge.SFN_RemapRange,id:8827,x:33762,y:31930,varname:node_8827,prsc:2,frmn:1.55,frmx:1.6,tomn:0,tomx:1|IN-833-OUT;n:type:ShaderForge.SFN_OneMinus,id:7392,x:33312,y:31880,varname:node_7392,prsc:2|IN-9654-V;n:type:ShaderForge.SFN_ConstantClamp,id:4276,x:33940,y:31930,varname:node_4276,prsc:2,min:0,max:1|IN-8827-OUT;n:type:ShaderForge.SFN_Multiply,id:7613,x:33056,y:31997,varname:node_7613,prsc:2|A-945-R,B-7672-OUT;n:type:ShaderForge.SFN_Vector1,id:7672,x:33056,y:32118,varname:node_7672,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Lerp,id:4868,x:33613,y:32402,varname:node_4868,prsc:2|A-350-OUT,B-6615-OUT,T-6332-OUT;n:type:ShaderForge.SFN_RemapRange,id:9644,x:33788,y:32186,varname:node_9644,prsc:2,frmn:1.65,frmx:1.8,tomn:1,tomx:0|IN-833-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:6332,x:33966,y:32186,varname:node_6332,prsc:2,min:0,max:1|IN-9644-OUT;n:type:ShaderForge.SFN_Vector3,id:6615,x:33535,y:32267,varname:node_6615,prsc:2,v1:0.2122642,v2:0.9456735,v3:1;n:type:ShaderForge.SFN_RemapRange,id:6954,x:32808,y:32203,varname:node_6954,prsc:2,frmn:0.3,frmx:1,tomn:0.7,tomx:1|IN-5803-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:387,x:32991,y:32203,varname:node_387,prsc:2,min:0,max:1|IN-6954-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:4490,x:30540,y:31774,ptovrint:False,ptlb:Texture_Photo_Fire,ptin:_Texture_Photo_Fire,varname:node_4490,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2dAsset,id:9405,x:30547,y:32111,ptovrint:False,ptlb:Texture_Photo_Hit,ptin:_Texture_Photo_Hit,varname:node_9405,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2dAsset,id:6013,x:30556,y:32503,ptovrint:False,ptlb:Texture_Photo_Die,ptin:_Texture_Photo_Die,varname:node_6013,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:6298,x:30758,y:31622,varname:node_6298,prsc:2,ntxv:0,isnm:False|UVIN-7816-OUT,TEX-4490-TEX;n:type:ShaderForge.SFN_Tex2d,id:4894,x:30758,y:31745,varname:node_4894,prsc:2,ntxv:0,isnm:False|TEX-4490-TEX;n:type:ShaderForge.SFN_Tex2d,id:6171,x:30758,y:31871,varname:node_6171,prsc:2,ntxv:0,isnm:False|UVIN-9274-OUT,TEX-4490-TEX;n:type:ShaderForge.SFN_Tex2d,id:8083,x:30771,y:32034,varname:node_8083,prsc:2,ntxv:0,isnm:False|UVIN-7816-OUT,TEX-9405-TEX;n:type:ShaderForge.SFN_Tex2d,id:7486,x:30771,y:32160,varname:node_7486,prsc:2,ntxv:0,isnm:False|TEX-9405-TEX;n:type:ShaderForge.SFN_Tex2d,id:618,x:30771,y:32281,varname:node_618,prsc:2,ntxv:0,isnm:False|UVIN-9274-OUT,TEX-9405-TEX;n:type:ShaderForge.SFN_Tex2d,id:3445,x:30771,y:32434,varname:node_3445,prsc:2,ntxv:0,isnm:False|UVIN-7816-OUT,TEX-6013-TEX;n:type:ShaderForge.SFN_Tex2d,id:308,x:30771,y:32568,varname:node_308,prsc:2,ntxv:0,isnm:False|TEX-6013-TEX;n:type:ShaderForge.SFN_Tex2d,id:2092,x:30771,y:32693,varname:node_2092,prsc:2,ntxv:0,isnm:False|UVIN-9274-OUT,TEX-6013-TEX;n:type:ShaderForge.SFN_SwitchProperty,id:6053,x:30705,y:30519,ptovrint:False,ptlb:Switch_Fire,ptin:_Switch_Fire,varname:node_6053,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-8322-OUT,B-2033-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:2387,x:30705,y:30664,ptovrint:False,ptlb:Switch_Hit,ptin:_Switch_Hit,varname:node_2387,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-8322-OUT,B-2033-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:1203,x:30705,y:30805,ptovrint:False,ptlb:Switch_Die,ptin:_Switch_Die,varname:node_1203,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-8322-OUT,B-2033-OUT;n:type:ShaderForge.SFN_Vector1,id:8322,x:30457,y:30599,varname:node_8322,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:2033,x:30457,y:30805,varname:node_2033,prsc:2,v1:1;n:type:ShaderForge.SFN_Lerp,id:3881,x:31392,y:31779,varname:node_3881,prsc:2|A-1935-R,B-6298-R,T-6053-OUT;n:type:ShaderForge.SFN_Lerp,id:2799,x:31408,y:31932,varname:node_2799,prsc:2|A-1935-A,B-6298-A,T-6053-OUT;n:type:ShaderForge.SFN_Lerp,id:256,x:31408,y:32103,varname:node_256,prsc:2|A-7256-G,B-4894-G,T-6053-OUT;n:type:ShaderForge.SFN_Lerp,id:4651,x:31432,y:32294,varname:node_4651,prsc:2|A-7256-A,B-4894-A,T-6053-OUT;n:type:ShaderForge.SFN_Lerp,id:5600,x:31432,y:32499,varname:node_5600,prsc:2|A-2320-B,B-6171-B,T-6053-OUT;n:type:ShaderForge.SFN_Lerp,id:3612,x:31444,y:32716,varname:node_3612,prsc:2|A-2320-A,B-6171-A,T-6053-OUT;n:type:ShaderForge.SFN_Lerp,id:7108,x:31588,y:31779,varname:node_7108,prsc:2|A-3881-OUT,B-8083-R,T-2387-OUT;n:type:ShaderForge.SFN_Lerp,id:3399,x:31604,y:31930,varname:node_3399,prsc:2|A-2799-OUT,B-8083-A,T-2387-OUT;n:type:ShaderForge.SFN_Lerp,id:3889,x:31604,y:32103,varname:node_3889,prsc:2|A-256-OUT,B-7486-G,T-2387-OUT;n:type:ShaderForge.SFN_Lerp,id:2695,x:31628,y:32301,varname:node_2695,prsc:2|A-4651-OUT,B-7486-A,T-2387-OUT;n:type:ShaderForge.SFN_Lerp,id:3039,x:31628,y:32508,varname:node_3039,prsc:2|A-5600-OUT,B-618-B,T-2387-OUT;n:type:ShaderForge.SFN_Lerp,id:634,x:31645,y:32716,varname:node_634,prsc:2|A-3612-OUT,B-618-A,T-2387-OUT;n:type:ShaderForge.SFN_Lerp,id:6927,x:31802,y:32103,varname:node_6927,prsc:2|A-3889-OUT,B-308-G,T-1203-OUT;n:type:ShaderForge.SFN_Lerp,id:6435,x:31805,y:32301,varname:node_6435,prsc:2|A-2695-OUT,B-308-A,T-1203-OUT;n:type:ShaderForge.SFN_Lerp,id:8250,x:31805,y:32508,varname:node_8250,prsc:2|A-3039-OUT,B-2092-B,T-1203-OUT;n:type:ShaderForge.SFN_Lerp,id:2166,x:31830,y:32716,varname:node_2166,prsc:2|A-634-OUT,B-2092-A,T-1203-OUT;n:type:ShaderForge.SFN_Lerp,id:7526,x:31791,y:31925,varname:node_7526,prsc:2|A-3399-OUT,B-3445-A,T-1203-OUT;n:type:ShaderForge.SFN_Lerp,id:4184,x:31776,y:31779,varname:node_4184,prsc:2|A-7108-OUT,B-3445-R,T-1203-OUT;n:type:ShaderForge.SFN_Add,id:38,x:34077,y:33092,varname:node_38,prsc:2|A-6495-T,B-4465-OUT;n:type:ShaderForge.SFN_Vector1,id:4465,x:34153,y:33043,varname:node_4465,prsc:2,v1:-0.5;n:type:ShaderForge.SFN_Tex2dAsset,id:887,x:30581,y:32842,ptovrint:False,ptlb:Texture_Photo_Win,ptin:_Texture_Photo_Win,varname:node_887,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:3155,x:30771,y:32880,varname:node_3155,prsc:2,ntxv:0,isnm:False|TEX-887-TEX;n:type:ShaderForge.SFN_SwitchProperty,id:6399,x:30705,y:30954,ptovrint:False,ptlb:Switch_Win,ptin:_Switch_Win,varname:node_6399,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-8322-OUT,B-2033-OUT;n:type:ShaderForge.SFN_Lerp,id:5522,x:32005,y:31779,varname:node_5522,prsc:2|A-4184-OUT,B-3155-R,T-6399-OUT;n:type:ShaderForge.SFN_Lerp,id:5453,x:32005,y:31925,varname:node_5453,prsc:2|A-7526-OUT,B-3155-A,T-6399-OUT;n:type:ShaderForge.SFN_Lerp,id:6729,x:31994,y:32103,varname:node_6729,prsc:2|A-6927-OUT,B-3155-G,T-6399-OUT;n:type:ShaderForge.SFN_Lerp,id:555,x:32004,y:32301,varname:node_555,prsc:2|A-6435-OUT,B-3155-A,T-6399-OUT;n:type:ShaderForge.SFN_Lerp,id:1370,x:32019,y:32508,varname:node_1370,prsc:2|A-8250-OUT,B-3155-B,T-6399-OUT;n:type:ShaderForge.SFN_Lerp,id:143,x:32036,y:32716,varname:node_143,prsc:2|A-2166-OUT,B-3155-A,T-6399-OUT;n:type:ShaderForge.SFN_Vector1,id:1990,x:31514,y:31116,varname:node_1990,prsc:2,v1:2;proporder:6572-484-5470-7611-7955-8105-4546-7705-945-7858-7389-4490-9405-6013-6053-2387-1203-887-6399;pass:END;sub:END;*/

Shader "Shader Forge/CustomHado/HDC_SH_CPUPanel_Panel_Face" {
    Properties {
        _Texture_Photo ("Texture_Photo", 2D) = "white" {}
        _Color_Fresnel ("Color_Fresnel", Color) = (0.5,0.5,0.5,1)
        _Appear ("Appear", Range(0, 1)) = 1
        _Damage ("Damage", Range(0, 1)) = 0
        _Break ("Break", Range(0, 1)) = 0
        _NoiseOn ("NoiseOn", Range(0, 1)) = 0
        _Texture_SideMask ("Texture_SideMask", 2D) = "white" {}
        _Value_LifeRemain ("Value_LifeRemain", Float ) = 4
        _Texture_Noise ("Texture_Noise", 2D) = "white" {}
        _Back ("Back", Float ) = 0
        _Texture_Blur ("Texture_Blur", 2D) = "white" {}
        _Texture_Photo_Fire ("Texture_Photo_Fire", 2D) = "white" {}
        _Texture_Photo_Hit ("Texture_Photo_Hit", 2D) = "white" {}
        _Texture_Photo_Die ("Texture_Photo_Die", 2D) = "white" {}
        [MaterialToggle] _Switch_Fire ("Switch_Fire", Float ) = 0
        [MaterialToggle] _Switch_Hit ("Switch_Hit", Float ) = 0
        [MaterialToggle] _Switch_Die ("Switch_Die", Float ) = 0
        _Texture_Photo_Win ("Texture_Photo_Win", 2D) = "white" {}
        [MaterialToggle] _Switch_Win ("Switch_Win", Float ) = 0
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
            uniform sampler2D _Texture_Photo; uniform float4 _Texture_Photo_ST;
            uniform float4 _Color_Fresnel;
            uniform float _Appear;
            uniform float _Damage;
            uniform float _Break;
            uniform float _NoiseOn;
            uniform sampler2D _Texture_SideMask; uniform float4 _Texture_SideMask_ST;
            uniform float _Value_LifeRemain;
            uniform sampler2D _Texture_Noise; uniform float4 _Texture_Noise_ST;
            uniform float _Back;
            uniform sampler2D _Texture_Blur; uniform float4 _Texture_Blur_ST;
            uniform sampler2D _Texture_Photo_Fire; uniform float4 _Texture_Photo_Fire_ST;
            uniform sampler2D _Texture_Photo_Hit; uniform float4 _Texture_Photo_Hit_ST;
            uniform sampler2D _Texture_Photo_Die; uniform float4 _Texture_Photo_Die_ST;
            uniform fixed _Switch_Fire;
            uniform fixed _Switch_Hit;
            uniform fixed _Switch_Die;
            uniform sampler2D _Texture_Photo_Win; uniform float4 _Texture_Photo_Win_ST;
            uniform fixed _Switch_Win;
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
                float4 projPos : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float node_9266 = 0.0;
                float4 node_6495 = _Time;
                v.vertex.xyz += float3(node_9266,(sin((node_6495.g+(-0.5)))*0.05),node_9266);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
////// Lighting:
////// Emissive:
                float node_1032 = 0.9;
                float4 node_5395 = _Time;
                float2 node_2170 = float2(node_1032,(node_5395.g*0.5));
                float4 node_3309 = tex2D(_Texture_Blur,TRANSFORM_TEX(node_2170, _Texture_Blur));
                float2 node_1456 = float2(node_1032,(node_5395.g*0.13));
                float4 node_2681 = tex2D(_Texture_Blur,TRANSFORM_TEX(node_1456, _Texture_Blur));
                float node_4708 = clamp((_NoiseOn+clamp((node_2681.r*2.333333+-1.633333),0,1)),0,1);
                float node_6404 = 1.0;
                float node_4006 = clamp((node_4708+clamp((_Value_LifeRemain*-4.999999+6.999999),0,0.5)+clamp((node_6404*-9.999998+9.999998),0,1)+_Damage),0,1);
                float2 node_7816 = float2((i.uv0.r+((node_3309.r*0.2+-0.1)*node_4006)),i.uv0.g);
                float4 node_1935 = tex2D(_Texture_Photo,TRANSFORM_TEX(node_7816, _Texture_Photo));
                float4 node_6298 = tex2D(_Texture_Photo_Fire,TRANSFORM_TEX(node_7816, _Texture_Photo_Fire));
                float node_8322 = 0.0;
                float node_2033 = 1.0;
                float _Switch_Fire_var = lerp( node_8322, node_2033, _Switch_Fire );
                float4 node_8083 = tex2D(_Texture_Photo_Hit,TRANSFORM_TEX(node_7816, _Texture_Photo_Hit));
                float _Switch_Hit_var = lerp( node_8322, node_2033, _Switch_Hit );
                float4 node_3445 = tex2D(_Texture_Photo_Die,TRANSFORM_TEX(node_7816, _Texture_Photo_Die));
                float _Switch_Die_var = lerp( node_8322, node_2033, _Switch_Die );
                float4 node_3155 = tex2D(_Texture_Photo_Win,TRANSFORM_TEX(i.uv0, _Texture_Photo_Win));
                float _Switch_Win_var = lerp( node_8322, node_2033, _Switch_Win );
                float4 node_7256 = tex2D(_Texture_Photo,TRANSFORM_TEX(i.uv0, _Texture_Photo));
                float4 node_4894 = tex2D(_Texture_Photo_Fire,TRANSFORM_TEX(i.uv0, _Texture_Photo_Fire));
                float4 node_7486 = tex2D(_Texture_Photo_Hit,TRANSFORM_TEX(i.uv0, _Texture_Photo_Hit));
                float4 node_308 = tex2D(_Texture_Photo_Die,TRANSFORM_TEX(i.uv0, _Texture_Photo_Die));
                float2 node_9274 = float2((i.uv0.r+((node_3309.r*-0.2+0.1)*node_4708*node_4006)),i.uv0.g);
                float4 node_2320 = tex2D(_Texture_Photo,TRANSFORM_TEX(node_9274, _Texture_Photo));
                float4 node_6171 = tex2D(_Texture_Photo_Fire,TRANSFORM_TEX(node_9274, _Texture_Photo_Fire));
                float4 node_618 = tex2D(_Texture_Photo_Hit,TRANSFORM_TEX(node_9274, _Texture_Photo_Hit));
                float4 node_2092 = tex2D(_Texture_Photo_Die,TRANSFORM_TEX(node_9274, _Texture_Photo_Die));
                float node_5803 = (1.0-max(0,dot(normalDirection, viewDirection)));
                float node_1990 = 2.0;
                float4 node_2551 = _Time;
                float2 node_2626 = float2(((i.uv0.r*node_1990)+(node_2551.g*17.39)),((i.uv0.g*node_1990)+(node_2551.g*25.47)));
                float4 _Texture_Noise_var = tex2D(_Texture_Noise,TRANSFORM_TEX(node_2626, _Texture_Noise));
                float node_8308 = clamp(((_Texture_Noise_var.r*20.0+-20.0)+((node_6404-clamp((_Break*2.0+-1.0),0,1))*21.0)),0,1);
                float node_387 = clamp((node_5803*0.4285715+0.5714285),0,1);
                float node_833 = ((1.0 - i.uv0.g)+(_Texture_Noise_var.r*0.5)+(_Appear*2.3));
                float3 emissive = lerp(lerp(((float3(lerp(lerp(lerp(lerp(node_1935.r,node_6298.r,_Switch_Fire_var),node_8083.r,_Switch_Hit_var),node_3445.r,_Switch_Die_var),node_3155.r,_Switch_Win_var),lerp(lerp(lerp(lerp(node_7256.g,node_4894.g,_Switch_Fire_var),node_7486.g,_Switch_Hit_var),node_308.g,_Switch_Die_var),node_3155.g,_Switch_Win_var),lerp(lerp(lerp(lerp(node_2320.b,node_6171.b,_Switch_Fire_var),node_618.b,_Switch_Hit_var),node_2092.b,_Switch_Die_var),node_3155.b,_Switch_Win_var))+(clamp((node_5803*1.428571+-0.4285715),0,1)*_Color_Fresnel.rgb)+(clamp((node_6404*-2.0+2.0),0,1)*float3(0.05873977,0.3299046,0.3773585))+(_Damage*float3(0.2264151,0.1693536,0.02669989))+(clamp((_Break*10.0+0.0),0,1)*float3(0.227451,0.04392763,0.02745098)))*lerp(1.0,(_Texture_Noise_var.r*1.0+0.5),node_4006)*node_8308),float3(node_387,node_387,node_387),_Back),float3(0.2122642,0.9456735,1),clamp((node_833*-6.666668+12.0),0,1));
                float3 finalColor = emissive;
                float node_9645 = 0.35;
                float4 node_1741 = tex2D(_Texture_SideMask,TRANSFORM_TEX(node_7816, _Texture_SideMask));
                float4 node_4383 = tex2D(_Texture_SideMask,TRANSFORM_TEX(i.uv0, _Texture_SideMask));
                float4 node_6523 = tex2D(_Texture_SideMask,TRANSFORM_TEX(node_9274, _Texture_SideMask));
                float node_9856 = (node_4006*-0.5+0.5);
                float node_828 = 1.0;
                float node_821 = 0.0;
                return fixed4(finalColor,(clamp((node_821 + ( (saturate(((lerp(lerp(lerp(lerp(node_1935.a,node_6298.a,_Switch_Fire_var),node_8083.a,_Switch_Hit_var),node_3445.a,_Switch_Die_var),node_3155.a,_Switch_Win_var)*node_9645*node_1741.r)+(lerp(lerp(lerp(lerp(node_7256.a,node_4894.a,_Switch_Fire_var),node_7486.a,_Switch_Hit_var),node_308.a,_Switch_Die_var),node_3155.a,_Switch_Win_var)*node_9645*node_4383.r)+(lerp(lerp(lerp(lerp(node_2320.a,node_6171.a,_Switch_Fire_var),node_618.a,_Switch_Hit_var),node_2092.a,_Switch_Die_var),node_3155.a,_Switch_Win_var)*node_9645*node_6523.r))) - node_9856) * (node_828 - node_821) ) / (node_828 - node_9856)),0,1)*node_8308*clamp((partZ*0.9999999+-0.2),0.3,1)*clamp((node_833*19.99997+-30.99995),0,1)));
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
                float node_9266 = 0.0;
                float4 node_6495 = _Time;
                v.vertex.xyz += float3(node_9266,(sin((node_6495.g+(-0.5)))*0.05),node_9266);
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
