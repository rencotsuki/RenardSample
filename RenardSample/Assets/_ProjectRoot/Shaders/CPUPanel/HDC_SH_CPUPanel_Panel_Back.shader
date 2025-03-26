// Shader created with Shader Forge v1.42 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Enhanced by Antoine Guillon / Arkham Development - http://www.arkham-development.com/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.42;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:34583,y:32681,varname:node_3138,prsc:2|emission-8892-OUT,alpha-7048-OUT,voffset-7656-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:6572,x:31472,y:32816,ptovrint:False,ptlb:Texture_Photo,ptin:_Texture_Photo,varname:node_6572,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:1935,x:31656,y:32816,varname:node_1935,prsc:2,ntxv:0,isnm:False|TEX-6572-TEX;n:type:ShaderForge.SFN_ValueProperty,id:1401,x:33546,y:33010,ptovrint:False,ptlb:Offset,ptin:_Offset,varname:node_1401,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Append,id:7656,x:33665,y:33112,varname:node_7656,prsc:2|A-9124-OUT,B-4584-OUT,C-1401-OUT;n:type:ShaderForge.SFN_Vector1,id:9124,x:33448,y:33112,varname:node_9124,prsc:2,v1:0;n:type:ShaderForge.SFN_TexCoord,id:681,x:29504,y:33119,varname:node_681,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Vector1,id:5895,x:30005,y:33128,varname:node_5895,prsc:2,v1:0.002;n:type:ShaderForge.SFN_Tex2d,id:3548,x:31674,y:32941,varname:node_3548,prsc:2,ntxv:0,isnm:False|UVIN-7185-OUT,TEX-6572-TEX;n:type:ShaderForge.SFN_Add,id:9368,x:30021,y:33187,varname:node_9368,prsc:2|A-5895-OUT,B-681-V;n:type:ShaderForge.SFN_Append,id:7185,x:30192,y:33215,varname:node_7185,prsc:2|A-2448-OUT,B-9368-OUT;n:type:ShaderForge.SFN_OneMinus,id:235,x:33025,y:33658,varname:node_235,prsc:2|IN-494-OUT;n:type:ShaderForge.SFN_Add,id:4038,x:33186,y:33362,varname:node_4038,prsc:2|A-8634-OUT,B-235-OUT;n:type:ShaderForge.SFN_RemapRange,id:7613,x:32876,y:33524,varname:node_7613,prsc:2,frmn:0,frmx:0.2,tomn:3,tomx:0.5|IN-494-OUT;n:type:ShaderForge.SFN_Clamp,id:8634,x:32979,y:33286,varname:node_8634,prsc:2|IN-7613-OUT,MIN-7951-OUT,MAX-4485-OUT;n:type:ShaderForge.SFN_Vector1,id:7951,x:32979,y:33402,varname:node_7951,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Vector1,id:4485,x:32979,y:33455,varname:node_4485,prsc:2,v1:3;n:type:ShaderForge.SFN_Fresnel,id:5803,x:32957,y:32387,varname:node_5803,prsc:2;n:type:ShaderForge.SFN_SwitchProperty,id:2448,x:29723,y:33007,ptovrint:False,ptlb:BackFace,ptin:_BackFace,varname:node_2448,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-681-U,B-5292-OUT;n:type:ShaderForge.SFN_OneMinus,id:5292,x:29491,y:32925,varname:node_5292,prsc:2|IN-681-U;n:type:ShaderForge.SFN_Color,id:2951,x:33476,y:32548,ptovrint:False,ptlb:Color_Side,ptin:_Color_Side,varname:node_2951,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_RemapRange,id:9092,x:33141,y:32370,varname:node_9092,prsc:2,frmn:0,frmx:1,tomn:0.2,tomx:1|IN-5803-OUT;n:type:ShaderForge.SFN_Clamp,id:1103,x:33325,y:32370,varname:node_1103,prsc:2|IN-9092-OUT,MIN-7698-OUT,MAX-1756-OUT;n:type:ShaderForge.SFN_Vector1,id:7698,x:33306,y:32494,varname:node_7698,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:1756,x:33293,y:32548,varname:node_1756,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:1610,x:33998,y:32519,varname:node_1610,prsc:2|A-1257-OUT,B-3859-OUT;n:type:ShaderForge.SFN_Multiply,id:3756,x:33315,y:33362,varname:node_3756,prsc:2|A-4038-OUT,B-1461-OUT;n:type:ShaderForge.SFN_Vector1,id:1461,x:33315,y:33488,varname:node_1461,prsc:2,v1:2;n:type:ShaderForge.SFN_Multiply,id:1257,x:33580,y:32408,varname:node_1257,prsc:2|A-1103-OUT,B-3756-OUT;n:type:ShaderForge.SFN_RemapRange,id:3289,x:32758,y:32350,varname:node_3289,prsc:2,frmn:0,frmx:1,tomn:0.2,tomx:1|IN-5803-OUT;n:type:ShaderForge.SFN_Clamp,id:6268,x:33306,y:32608,varname:node_6268,prsc:2|IN-1570-OUT,MIN-3920-OUT,MAX-6091-OUT;n:type:ShaderForge.SFN_Vector1,id:6091,x:33274,y:32786,varname:node_6091,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:7048,x:33973,y:32697,varname:node_7048,prsc:2|A-6858-OUT,B-9723-OUT,C-5187-OUT,D-8532-OUT,E-1163-OUT;n:type:ShaderForge.SFN_RemapRangeAdvanced,id:1570,x:33023,y:32569,varname:node_1570,prsc:2|IN-5803-OUT,IMIN-4335-OUT,IMAX-1644-OUT,OMIN-3920-OUT,OMAX-1644-OUT;n:type:ShaderForge.SFN_Vector1,id:4335,x:32769,y:32616,varname:node_4335,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:1644,x:32769,y:32666,varname:node_1644,prsc:2,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:3920,x:32759,y:32730,ptovrint:False,ptlb:TransparentValue,ptin:_TransparentValue,varname:node_3920,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Add,id:5352,x:33557,y:32681,varname:node_5352,prsc:2|A-6268-OUT,B-1040-OUT;n:type:ShaderForge.SFN_Clamp01,id:6858,x:33714,y:32740,varname:node_6858,prsc:2|IN-5352-OUT;n:type:ShaderForge.SFN_RemapRange,id:8882,x:32821,y:32820,varname:node_8882,prsc:2,frmn:0,frmx:0.8,tomn:3,tomx:0|IN-494-OUT;n:type:ShaderForge.SFN_Clamp,id:1040,x:33054,y:32786,varname:node_1040,prsc:2|IN-8882-OUT,MIN-7785-OUT,MAX-9298-OUT;n:type:ShaderForge.SFN_Vector1,id:7785,x:33039,y:32902,varname:node_7785,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:9298,x:33039,y:32955,varname:node_9298,prsc:2,v1:1;n:type:ShaderForge.SFN_Tex2d,id:8062,x:31674,y:33057,varname:node_8062,prsc:2,ntxv:0,isnm:False|UVIN-5220-OUT,TEX-6572-TEX;n:type:ShaderForge.SFN_Append,id:5220,x:30218,y:33382,varname:node_5220,prsc:2|A-3833-OUT,B-681-V;n:type:ShaderForge.SFN_Add,id:3833,x:30047,y:33382,varname:node_3833,prsc:2|A-5943-OUT,B-681-U;n:type:ShaderForge.SFN_Vector1,id:5943,x:30033,y:33336,varname:node_5943,prsc:2,v1:0.01;n:type:ShaderForge.SFN_Add,id:8998,x:30047,y:33576,varname:node_8998,prsc:2|A-6924-OUT,B-681-U;n:type:ShaderForge.SFN_Append,id:4037,x:30218,y:33576,varname:node_4037,prsc:2|A-8998-OUT,B-681-V;n:type:ShaderForge.SFN_Vector1,id:6924,x:30033,y:33530,varname:node_6924,prsc:2,v1:-0.01;n:type:ShaderForge.SFN_Tex2d,id:1290,x:31674,y:33180,varname:node_1290,prsc:2,ntxv:0,isnm:False|UVIN-4037-OUT,TEX-6572-TEX;n:type:ShaderForge.SFN_Multiply,id:8417,x:33008,y:33910,varname:node_8417,prsc:2|A-1476-OUT,B-499-OUT;n:type:ShaderForge.SFN_OneMinus,id:797,x:32584,y:33104,varname:node_797,prsc:2|IN-8417-OUT;n:type:ShaderForge.SFN_Color,id:8025,x:32584,y:32961,ptovrint:False,ptlb:Color_Edge,ptin:_Color_Edge,varname:node_8025,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Multiply,id:5259,x:32805,y:33039,varname:node_5259,prsc:2|A-8025-RGB,B-797-OUT;n:type:ShaderForge.SFN_Add,id:8892,x:34215,y:32519,varname:node_8892,prsc:2|A-1610-OUT,B-5259-OUT;n:type:ShaderForge.SFN_Slider,id:7769,x:34050,y:33145,ptovrint:False,ptlb:Appear,ptin:_Appear,varname:node_7769,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Slider,id:454,x:31907,y:32208,ptovrint:False,ptlb:Damage,ptin:_Damage,varname:node_454,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:7730,x:31837,y:31802,ptovrint:False,ptlb:Break,ptin:_Break,varname:node_7730,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Tex2d,id:5672,x:32101,y:31417,ptovrint:False,ptlb:Texture_Noise,ptin:_Texture_Noise,varname:node_945,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-5600-OUT;n:type:ShaderForge.SFN_TexCoord,id:7646,x:31172,y:31418,varname:node_7646,prsc:2,uv:1,uaff:False;n:type:ShaderForge.SFN_Time,id:3046,x:31429,y:31696,varname:node_3046,prsc:2;n:type:ShaderForge.SFN_Multiply,id:9141,x:31604,y:31640,varname:node_9141,prsc:2|A-3046-T,B-845-OUT;n:type:ShaderForge.SFN_Multiply,id:7686,x:31604,y:31762,varname:node_7686,prsc:2|A-3046-T,B-9450-OUT;n:type:ShaderForge.SFN_Vector1,id:845,x:31604,y:31589,varname:node_845,prsc:2,v1:17.39;n:type:ShaderForge.SFN_Vector1,id:9450,x:31604,y:31891,varname:node_9450,prsc:2,v1:25.47;n:type:ShaderForge.SFN_Add,id:9693,x:31592,y:31345,varname:node_9693,prsc:2|A-6187-OUT,B-9141-OUT;n:type:ShaderForge.SFN_Add,id:9414,x:31604,y:31464,varname:node_9414,prsc:2|A-2225-OUT,B-7686-OUT;n:type:ShaderForge.SFN_Append,id:5600,x:31908,y:31434,varname:node_5600,prsc:2|A-9693-OUT,B-9414-OUT;n:type:ShaderForge.SFN_Lerp,id:2250,x:33041,y:31579,varname:node_2250,prsc:2|A-8983-OUT,B-8277-OUT,T-558-OUT;n:type:ShaderForge.SFN_Vector1,id:8983,x:33041,y:31520,varname:node_8983,prsc:2,v1:1;n:type:ShaderForge.SFN_RemapRange,id:8277,x:32868,y:31520,varname:node_8277,prsc:2,frmn:0,frmx:1,tomn:0.2,tomx:0.7|IN-5672-R;n:type:ShaderForge.SFN_RemapRange,id:8412,x:32807,y:31973,varname:node_8412,prsc:2,frmn:0,frmx:1,tomn:-20,tomx:0|IN-5672-R;n:type:ShaderForge.SFN_Multiply,id:1793,x:32807,y:32132,varname:node_1793,prsc:2|A-1834-OUT,B-2879-OUT;n:type:ShaderForge.SFN_Vector1,id:2879,x:32807,y:32252,varname:node_2879,prsc:2,v1:21;n:type:ShaderForge.SFN_ConstantClamp,id:4791,x:33180,y:31968,varname:node_4791,prsc:2,min:0,max:1|IN-7753-OUT;n:type:ShaderForge.SFN_Add,id:7753,x:32998,y:31968,varname:node_7753,prsc:2|A-8412-OUT,B-1793-OUT;n:type:ShaderForge.SFN_RemapRange,id:1569,x:32868,y:31707,varname:node_1569,prsc:2,frmn:0.7,frmx:1,tomn:0.5,tomx:1|IN-1834-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:558,x:33041,y:31707,varname:node_558,prsc:2,min:0.5,max:1|IN-1569-OUT;n:type:ShaderForge.SFN_Multiply,id:5187,x:33410,y:31978,varname:node_5187,prsc:2|A-4791-OUT,B-558-OUT,C-7683-OUT;n:type:ShaderForge.SFN_RemapRange,id:12,x:32234,y:32208,varname:node_12,prsc:2,frmn:0,frmx:1,tomn:0,tomx:0.5|IN-454-OUT;n:type:ShaderForge.SFN_Subtract,id:5626,x:31834,y:32058,varname:node_5626,prsc:2|A-7875-OUT,B-12-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:1834,x:32599,y:32064,varname:node_1834,prsc:2,min:0,max:1|IN-969-OUT;n:type:ShaderForge.SFN_RemapRange,id:6045,x:32245,y:32381,varname:node_6045,prsc:2,frmn:0,frmx:0.2,tomn:0,tomx:1|IN-454-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:8011,x:32439,y:32381,varname:node_8011,prsc:2,min:0,max:1|IN-6045-OUT;n:type:ShaderForge.SFN_Multiply,id:477,x:32551,y:32278,varname:node_477,prsc:2|A-1413-OUT,B-8011-OUT;n:type:ShaderForge.SFN_Vector3,id:1413,x:32551,y:32206,varname:node_1413,prsc:2,v1:0.3396226,v2:0.2280335,v3:0.09131362;n:type:ShaderForge.SFN_Add,id:3859,x:33669,y:32548,varname:node_3859,prsc:2|A-2951-RGB,B-477-OUT,C-1808-OUT;n:type:ShaderForge.SFN_RemapRange,id:3695,x:32191,y:31793,varname:node_3695,prsc:2,frmn:0,frmx:0.1,tomn:0,tomx:1|IN-7730-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:8585,x:32367,y:31793,varname:node_8585,prsc:2,min:0,max:1|IN-3695-OUT;n:type:ShaderForge.SFN_Subtract,id:969,x:32262,y:32032,varname:node_969,prsc:2|A-5626-OUT,B-7730-OUT;n:type:ShaderForge.SFN_Vector3,id:100,x:32555,y:31731,varname:node_100,prsc:2,v1:0.3411765,v2:0.0953213,v3:0.09019608;n:type:ShaderForge.SFN_Multiply,id:1808,x:32555,y:31803,varname:node_1808,prsc:2|A-100-OUT,B-8585-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1414,x:32777,y:31165,ptovrint:False,ptlb:Value_LifeRemain,ptin:_Value_LifeRemain,varname:node_1414,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:4;n:type:ShaderForge.SFN_Tex2d,id:5738,x:32644,y:31986,ptovrint:False,ptlb:Texture_Noise_copy,ptin:_Texture_Noise_copy,varname:_Texture_Noise_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_TexCoord,id:5670,x:32023,y:31914,varname:node_5670,prsc:2,uv:1,uaff:False;n:type:ShaderForge.SFN_Time,id:4695,x:32047,y:32156,varname:node_4695,prsc:2;n:type:ShaderForge.SFN_Multiply,id:5620,x:32222,y:32100,varname:node_5620,prsc:2|A-4695-T,B-7764-OUT;n:type:ShaderForge.SFN_Multiply,id:978,x:32222,y:32222,varname:node_978,prsc:2|A-4695-T,B-3558-OUT;n:type:ShaderForge.SFN_Vector1,id:7764,x:32047,y:32100,varname:node_7764,prsc:2,v1:17.39;n:type:ShaderForge.SFN_Vector1,id:3558,x:32222,y:32351,varname:node_3558,prsc:2,v1:25.47;n:type:ShaderForge.SFN_Add,id:741,x:32210,y:31805,varname:node_741,prsc:2|A-5670-U,B-5620-OUT;n:type:ShaderForge.SFN_Add,id:5164,x:32222,y:31924,varname:node_5164,prsc:2|A-5670-V,B-978-OUT;n:type:ShaderForge.SFN_Append,id:6685,x:32441,y:31982,varname:node_6685,prsc:2|A-741-OUT,B-5164-OUT;n:type:ShaderForge.SFN_RemapRange,id:8476,x:32955,y:31154,varname:node_8476,prsc:2,frmn:1.4,frmx:1.5,tomn:0,tomx:1|IN-1414-OUT;n:type:ShaderForge.SFN_RemapRange,id:6400,x:32632,y:31333,varname:node_6400,prsc:2,frmn:0,frmx:1,tomn:0,tomx:2|IN-5672-R;n:type:ShaderForge.SFN_ConstantClamp,id:5346,x:33132,y:31154,varname:node_5346,prsc:2,min:0,max:1|IN-8476-OUT;n:type:ShaderForge.SFN_Add,id:8099,x:33063,y:31354,varname:node_8099,prsc:2|A-5672-R,B-5346-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:7683,x:33243,y:31354,varname:node_7683,prsc:2,min:0,max:1|IN-8099-OUT;n:type:ShaderForge.SFN_Multiply,id:6187,x:31369,y:31345,varname:node_6187,prsc:2|A-7646-U,B-5572-OUT;n:type:ShaderForge.SFN_Multiply,id:2225,x:31369,y:31544,varname:node_2225,prsc:2|A-7646-V,B-5854-OUT;n:type:ShaderForge.SFN_Vector1,id:5572,x:31369,y:31293,varname:node_5572,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Vector1,id:5854,x:31369,y:31482,varname:node_5854,prsc:2,v1:1;n:type:ShaderForge.SFN_Time,id:5149,x:33483,y:33258,varname:node_5149,prsc:2;n:type:ShaderForge.SFN_Sin,id:1020,x:33840,y:33259,varname:node_1020,prsc:2|IN-9551-OUT;n:type:ShaderForge.SFN_Vector1,id:2950,x:34001,y:33378,varname:node_2950,prsc:2,v1:0.05;n:type:ShaderForge.SFN_Multiply,id:4584,x:34001,y:33259,varname:node_4584,prsc:2|A-1020-OUT,B-2950-OUT;n:type:ShaderForge.SFN_Depth,id:1012,x:33747,y:32867,varname:node_1012,prsc:2;n:type:ShaderForge.SFN_RemapRange,id:3807,x:33917,y:32867,varname:node_3807,prsc:2,frmn:0.5,frmx:1.2,tomn:0.5,tomx:1|IN-1012-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:8532,x:34090,y:32867,varname:node_8532,prsc:2,min:0.5,max:1|IN-3807-OUT;n:type:ShaderForge.SFN_Vector1,id:7875,x:31562,y:32143,varname:node_7875,prsc:2,v1:1;n:type:ShaderForge.SFN_TexCoord,id:3144,x:34195,y:33283,varname:node_3144,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:5399,x:34590,y:33231,varname:node_5399,prsc:2|A-8454-OUT,B-6480-OUT;n:type:ShaderForge.SFN_RemapRange,id:2301,x:34772,y:33231,varname:node_2301,prsc:2,frmn:1.05,frmx:1.1,tomn:0,tomx:1|IN-5399-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:1163,x:34944,y:33231,varname:node_1163,prsc:2,min:0,max:1|IN-2301-OUT;n:type:ShaderForge.SFN_OneMinus,id:6480,x:34371,y:33291,varname:node_6480,prsc:2|IN-3144-V;n:type:ShaderForge.SFN_Multiply,id:8454,x:34391,y:33129,varname:node_8454,prsc:2|A-7769-OUT,B-2925-OUT;n:type:ShaderForge.SFN_Vector1,id:2925,x:34391,y:33084,varname:node_2925,prsc:2,v1:1.8;n:type:ShaderForge.SFN_Tex2dAsset,id:487,x:31472,y:33385,ptovrint:False,ptlb:Texture_Photo_Fire,ptin:_Texture_Photo_Fire,varname:node_487,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2dAsset,id:382,x:31447,y:33941,ptovrint:False,ptlb:Texture_Photo_Hit,ptin:_Texture_Photo_Hit,varname:node_382,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:783,x:31674,y:33402,varname:node_783,prsc:2,ntxv:0,isnm:False|TEX-487-TEX;n:type:ShaderForge.SFN_Tex2d,id:4601,x:31674,y:33533,varname:node_4601,prsc:2,ntxv:0,isnm:False|UVIN-7185-OUT,TEX-487-TEX;n:type:ShaderForge.SFN_Tex2d,id:7546,x:31674,y:33659,varname:node_7546,prsc:2,ntxv:0,isnm:False|UVIN-5220-OUT,TEX-487-TEX;n:type:ShaderForge.SFN_Tex2d,id:4763,x:31674,y:33789,varname:node_4763,prsc:2,ntxv:0,isnm:False|UVIN-4037-OUT,TEX-487-TEX;n:type:ShaderForge.SFN_Tex2d,id:5307,x:31674,y:33941,varname:node_5307,prsc:2,ntxv:0,isnm:False|TEX-382-TEX;n:type:ShaderForge.SFN_Tex2d,id:7005,x:31674,y:34067,varname:node_7005,prsc:2,ntxv:0,isnm:False|UVIN-7185-OUT,TEX-382-TEX;n:type:ShaderForge.SFN_Tex2d,id:8960,x:31674,y:34195,varname:node_8960,prsc:2,ntxv:0,isnm:False|UVIN-5220-OUT,TEX-382-TEX;n:type:ShaderForge.SFN_Tex2d,id:8224,x:31674,y:34318,varname:node_8224,prsc:2,ntxv:0,isnm:False|UVIN-4037-OUT,TEX-382-TEX;n:type:ShaderForge.SFN_Tex2d,id:517,x:31674,y:34471,varname:node_517,prsc:2,ntxv:0,isnm:False|TEX-1471-TEX;n:type:ShaderForge.SFN_Tex2d,id:1406,x:31674,y:34604,varname:node_1406,prsc:2,ntxv:0,isnm:False|UVIN-7185-OUT,TEX-1471-TEX;n:type:ShaderForge.SFN_Tex2d,id:4067,x:31698,y:34715,varname:node_4067,prsc:2,ntxv:0,isnm:False|UVIN-5220-OUT,TEX-1471-TEX;n:type:ShaderForge.SFN_Tex2d,id:4073,x:31698,y:34846,varname:node_4073,prsc:2,ntxv:0,isnm:False|UVIN-4037-OUT,TEX-1471-TEX;n:type:ShaderForge.SFN_Tex2dAsset,id:1471,x:31434,y:34698,ptovrint:False,ptlb:Texture_Photo_Die,ptin:_Texture_Photo_Die,varname:node_1471,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_SwitchProperty,id:3049,x:31582,y:32267,ptovrint:False,ptlb:Switch_FIre,ptin:_Switch_FIre,varname:node_3049,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-1222-OUT,B-9386-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:3068,x:31582,y:32405,ptovrint:False,ptlb:Switch_HIt,ptin:_Switch_HIt,varname:node_3068,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-1222-OUT,B-9386-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:3287,x:31582,y:32549,ptovrint:False,ptlb:Switch_Die,ptin:_Switch_Die,varname:node_3287,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-1222-OUT,B-9386-OUT;n:type:ShaderForge.SFN_Vector1,id:1222,x:31312,y:32287,varname:node_1222,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:9386,x:31312,y:32474,varname:node_9386,prsc:2,v1:1;n:type:ShaderForge.SFN_Lerp,id:1278,x:31982,y:33320,varname:node_1278,prsc:2|A-1935-A,B-783-A,T-3049-OUT;n:type:ShaderForge.SFN_Lerp,id:7037,x:31982,y:33439,varname:node_7037,prsc:2|A-3548-A,B-4601-A,T-3049-OUT;n:type:ShaderForge.SFN_Lerp,id:5089,x:31982,y:33589,varname:node_5089,prsc:2|A-8062-A,B-7546-A,T-3049-OUT;n:type:ShaderForge.SFN_Lerp,id:8248,x:31982,y:33720,varname:node_8248,prsc:2|A-1290-A,B-4763-A,T-3049-OUT;n:type:ShaderForge.SFN_Lerp,id:65,x:32163,y:33350,varname:node_65,prsc:2|A-1278-OUT,B-5307-A,T-3068-OUT;n:type:ShaderForge.SFN_Lerp,id:5635,x:32163,y:33474,varname:node_5635,prsc:2|A-7037-OUT,B-7005-A,T-3068-OUT;n:type:ShaderForge.SFN_Lerp,id:3661,x:32163,y:33593,varname:node_3661,prsc:2|A-5089-OUT,B-8960-A,T-3068-OUT;n:type:ShaderForge.SFN_Lerp,id:5193,x:32163,y:33709,varname:node_5193,prsc:2|A-8248-OUT,B-8224-A,T-3068-OUT;n:type:ShaderForge.SFN_Lerp,id:6319,x:32362,y:33372,varname:node_6319,prsc:2|A-65-OUT,B-517-A,T-3287-OUT;n:type:ShaderForge.SFN_Lerp,id:1589,x:32362,y:33522,varname:node_1589,prsc:2|A-5635-OUT,B-1406-A,T-3287-OUT;n:type:ShaderForge.SFN_Lerp,id:6277,x:32374,y:33633,varname:node_6277,prsc:2|A-3661-OUT,B-4067-A,T-3287-OUT;n:type:ShaderForge.SFN_Lerp,id:3915,x:32374,y:33777,varname:node_3915,prsc:2|A-5193-OUT,B-4073-A,T-3287-OUT;n:type:ShaderForge.SFN_Add,id:9551,x:33693,y:33293,varname:node_9551,prsc:2|A-5149-T,B-6294-OUT;n:type:ShaderForge.SFN_Vector1,id:6294,x:33693,y:33412,varname:node_6294,prsc:2,v1:-0.5;n:type:ShaderForge.SFN_Tex2dAsset,id:6049,x:31449,y:34975,ptovrint:False,ptlb:Texture_Photo_Win,ptin:_Texture_Photo_Win,varname:node_6049,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:9637,x:31674,y:34975,varname:node_9637,prsc:2,ntxv:0,isnm:False|TEX-6049-TEX;n:type:ShaderForge.SFN_SwitchProperty,id:6576,x:31582,y:32691,ptovrint:False,ptlb:Switch_Win,ptin:_Switch_Win,varname:node_6576,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-1222-OUT,B-9386-OUT;n:type:ShaderForge.SFN_Lerp,id:9723,x:32608,y:33372,varname:node_9723,prsc:2|A-6319-OUT,B-9637-A,T-6576-OUT;n:type:ShaderForge.SFN_Lerp,id:494,x:32558,y:34428,varname:node_494,prsc:2|A-1589-OUT,B-9637-A,T-6576-OUT;n:type:ShaderForge.SFN_Lerp,id:1476,x:32652,y:33701,varname:node_1476,prsc:2|A-6277-OUT,B-9637-A,T-6576-OUT;n:type:ShaderForge.SFN_Lerp,id:499,x:32652,y:33869,varname:node_499,prsc:2|A-3915-OUT,B-9637-A,T-6576-OUT;proporder:6572-1401-2448-2951-3920-8025-7769-454-7730-5672-1414-487-382-1471-3049-3068-3287-6049-6576;pass:END;sub:END;*/

Shader "Shader Forge/CustomHado/HDC_SH_CPUPanel_Panel_Back" {
    Properties {
        _Texture_Photo ("Texture_Photo", 2D) = "white" {}
        _Offset ("Offset", Float ) = 0
        [MaterialToggle] _BackFace ("BackFace", Float ) = 0
        _Color_Side ("Color_Side", Color) = (0,0,0,1)
        _TransparentValue ("TransparentValue", Float ) = 1
        _Color_Edge ("Color_Edge", Color) = (0,0,0,1)
        _Appear ("Appear", Range(0, 1)) = 1
        _Damage ("Damage", Range(0, 1)) = 0
        _Break ("Break", Range(0, 1)) = 0
        _Texture_Noise ("Texture_Noise", 2D) = "white" {}
        _Value_LifeRemain ("Value_LifeRemain", Float ) = 4
        _Texture_Photo_Fire ("Texture_Photo_Fire", 2D) = "white" {}
        _Texture_Photo_Hit ("Texture_Photo_Hit", 2D) = "white" {}
        _Texture_Photo_Die ("Texture_Photo_Die", 2D) = "white" {}
        [MaterialToggle] _Switch_FIre ("Switch_FIre", Float ) = 0
        [MaterialToggle] _Switch_HIt ("Switch_HIt", Float ) = 0
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
            uniform float _Offset;
            uniform fixed _BackFace;
            uniform float4 _Color_Side;
            uniform float _TransparentValue;
            uniform float4 _Color_Edge;
            uniform float _Appear;
            uniform float _Damage;
            uniform float _Break;
            uniform sampler2D _Texture_Noise; uniform float4 _Texture_Noise_ST;
            uniform float _Value_LifeRemain;
            uniform sampler2D _Texture_Photo_Fire; uniform float4 _Texture_Photo_Fire_ST;
            uniform sampler2D _Texture_Photo_Hit; uniform float4 _Texture_Photo_Hit_ST;
            uniform sampler2D _Texture_Photo_Die; uniform float4 _Texture_Photo_Die_ST;
            uniform fixed _Switch_FIre;
            uniform fixed _Switch_HIt;
            uniform fixed _Switch_Die;
            uniform sampler2D _Texture_Photo_Win; uniform float4 _Texture_Photo_Win_ST;
            uniform fixed _Switch_Win;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                float3 normalDir : TEXCOORD3;
                float4 projPos : TEXCOORD4;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 node_5149 = _Time;
                v.vertex.xyz += float3(0.0,(sin((node_5149.g+(-0.5)))*0.05),_Offset);
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
                float node_5803 = (1.0-max(0,dot(normalDirection, viewDirection)));
                float2 node_7185 = float2(lerp( i.uv0.r, (1.0 - i.uv0.r), _BackFace ),(0.002+i.uv0.g));
                float4 node_3548 = tex2D(_Texture_Photo,TRANSFORM_TEX(node_7185, _Texture_Photo));
                float4 node_4601 = tex2D(_Texture_Photo_Fire,TRANSFORM_TEX(node_7185, _Texture_Photo_Fire));
                float node_1222 = 0.0;
                float node_9386 = 1.0;
                float _Switch_FIre_var = lerp( node_1222, node_9386, _Switch_FIre );
                float4 node_7005 = tex2D(_Texture_Photo_Hit,TRANSFORM_TEX(node_7185, _Texture_Photo_Hit));
                float _Switch_HIt_var = lerp( node_1222, node_9386, _Switch_HIt );
                float4 node_1406 = tex2D(_Texture_Photo_Die,TRANSFORM_TEX(node_7185, _Texture_Photo_Die));
                float _Switch_Die_var = lerp( node_1222, node_9386, _Switch_Die );
                float4 node_9637 = tex2D(_Texture_Photo_Win,TRANSFORM_TEX(i.uv0, _Texture_Photo_Win));
                float _Switch_Win_var = lerp( node_1222, node_9386, _Switch_Win );
                float node_494 = lerp(lerp(lerp(lerp(node_3548.a,node_4601.a,_Switch_FIre_var),node_7005.a,_Switch_HIt_var),node_1406.a,_Switch_Die_var),node_9637.a,_Switch_Win_var);
                float2 node_5220 = float2((0.01+i.uv0.r),i.uv0.g);
                float4 node_8062 = tex2D(_Texture_Photo,TRANSFORM_TEX(node_5220, _Texture_Photo));
                float4 node_7546 = tex2D(_Texture_Photo_Fire,TRANSFORM_TEX(node_5220, _Texture_Photo_Fire));
                float4 node_8960 = tex2D(_Texture_Photo_Hit,TRANSFORM_TEX(node_5220, _Texture_Photo_Hit));
                float4 node_4067 = tex2D(_Texture_Photo_Die,TRANSFORM_TEX(node_5220, _Texture_Photo_Die));
                float2 node_4037 = float2(((-0.01)+i.uv0.r),i.uv0.g);
                float4 node_1290 = tex2D(_Texture_Photo,TRANSFORM_TEX(node_4037, _Texture_Photo));
                float4 node_4763 = tex2D(_Texture_Photo_Fire,TRANSFORM_TEX(node_4037, _Texture_Photo_Fire));
                float4 node_8224 = tex2D(_Texture_Photo_Hit,TRANSFORM_TEX(node_4037, _Texture_Photo_Hit));
                float4 node_4073 = tex2D(_Texture_Photo_Die,TRANSFORM_TEX(node_4037, _Texture_Photo_Die));
                float3 emissive = (((clamp((node_5803*0.8+0.2),0.0,1.0)*((clamp((node_494*-12.5+3.0),0.5,3.0)+(1.0 - node_494))*2.0))*(_Color_Side.rgb+(float3(0.3396226,0.2280335,0.09131362)*clamp((_Damage*5.0+0.0),0,1))+(float3(0.3411765,0.0953213,0.09019608)*clamp((_Break*10.0+0.0),0,1))))+(_Color_Edge.rgb*(1.0 - (lerp(lerp(lerp(lerp(node_8062.a,node_7546.a,_Switch_FIre_var),node_8960.a,_Switch_HIt_var),node_4067.a,_Switch_Die_var),node_9637.a,_Switch_Win_var)*lerp(lerp(lerp(lerp(node_1290.a,node_4763.a,_Switch_FIre_var),node_8224.a,_Switch_HIt_var),node_4073.a,_Switch_Die_var),node_9637.a,_Switch_Win_var)))));
                float3 finalColor = emissive;
                float node_4335 = 0.0;
                float node_1644 = 1.0;
                float4 node_1935 = tex2D(_Texture_Photo,TRANSFORM_TEX(i.uv0, _Texture_Photo));
                float4 node_783 = tex2D(_Texture_Photo_Fire,TRANSFORM_TEX(i.uv0, _Texture_Photo_Fire));
                float4 node_5307 = tex2D(_Texture_Photo_Hit,TRANSFORM_TEX(i.uv0, _Texture_Photo_Hit));
                float4 node_517 = tex2D(_Texture_Photo_Die,TRANSFORM_TEX(i.uv0, _Texture_Photo_Die));
                float4 node_3046 = _Time;
                float2 node_5600 = float2(((i.uv1.r*0.5)+(node_3046.g*17.39)),((i.uv1.g*1.0)+(node_3046.g*25.47)));
                float4 _Texture_Noise_var = tex2D(_Texture_Noise,TRANSFORM_TEX(node_5600, _Texture_Noise));
                float node_1834 = clamp(((1.0-(_Damage*0.5+0.0))-_Break),0,1);
                float node_558 = clamp((node_1834*1.666667+-0.6666666),0.5,1);
                return fixed4(finalColor,(saturate((clamp((_TransparentValue + ( (node_5803 - node_4335) * (node_1644 - _TransparentValue) ) / (node_1644 - node_4335)),_TransparentValue,1.0)+clamp((node_494*-3.75+3.0),0.0,1.0)))*lerp(lerp(lerp(lerp(node_1935.a,node_783.a,_Switch_FIre_var),node_5307.a,_Switch_HIt_var),node_517.a,_Switch_Die_var),node_9637.a,_Switch_Win_var)*(clamp(((_Texture_Noise_var.r*20.0+-20.0)+(node_1834*21.0)),0,1)*node_558*clamp((_Texture_Noise_var.r+clamp((_Value_LifeRemain*9.999998+-14.0),0,1)),0,1))*clamp((partZ*0.7142857+0.1428572),0.5,1)*clamp((((_Appear*1.8)+(1.0 - i.uv0.g))*19.99997+-20.99997),0,1)));
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
            uniform float _Offset;
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                float4 node_5149 = _Time;
                v.vertex.xyz += float3(0.0,(sin((node_5149.g+(-0.5)))*0.05),_Offset);
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
