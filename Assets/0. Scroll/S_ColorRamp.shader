Shader "Unlit/S_ColorRamp"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		_MainTex("Main Texture", 2D) = "white" {}
		_MainTextureChannel("Main Texture Channel", Vector) = (1,1,1,0)
		_MainAlphaChannel("Main Alpha Channel", Vector) = (0,0,0,1)
		_MainTexturePanning("Main Texture Panning ", Vector) = (0.2522222,0,0,0)
		_Desaturate("Desaturate? ", Range( 0 , 1)) = 0
		[Toggle(_USEALPHAOVERRIDE_ON)] _UseAlphaOverride("Use Alpha Override", Float) = 0
		_AlphaOverride("Alpha Override", 2D) = "white" {}
		_AlphaOverrideChannel("Alpha Override Channel", Vector) = (0,0,0,1)
		_AlphaOverridePanning("Alpha Override Panning", Vector) = (0,0,0,0)
		_DetailAdditiveChannel("Detail Additive Channel", Vector) = (0,0,0,1)
		_DetailDisolveChannel("Detail Disolve Channel", Vector) = (0,0,0,1)
		[Toggle(_USERAMP_ON)] _UseRamp("Use Color Ramping?", Float) = 0
		_MiddlePointPos("Middle Point Position", Range( 0 , 1)) = 0.5
		_WhiteColor("Highs", Color) = (1,0.8950032,0,0)
		_MidColor("Middles", Color) = (1,0.4447915,0,0)
		_LastColor("Lows", Color) = (1,0,0,0)
		_StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255
        _ColorMask("Color Mask", Float) = 15


	}

	SubShader
	{
		LOD 0

		
		Tags { "RenderPipeline"="UniversalPipeline"
				"RenderType"="Transparent"
				"Queue"="Transparent" }

		Stencil
        {
			Ref[_Stencil]
			Comp[_StencilComp]
			Pass[_StencilOp]
			ReadMask[_StencilReadMask]
			WriteMask[_StencilWriteMask]
        }
		ColorMask[_ColorMask]

        Cull Back
		Blend SrcAlpha One
		ZTest[unity_GUIZTestMode]
		ZWrite Off
		
		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForward" }
			
			Blend SrcAlpha One
			ZTest[unity_GUIZTestMode]
			ZWrite Off
			Offset 0 , 0
			ColorMask[_ColorMask]
			

			HLSLPROGRAM
			#pragma multi_compile_instancing
			#define ASE_SRP_VERSION 999999
			#define REQUIRE_DEPTH_TEXTURE 1

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#pragma shader_feature_local _USEALPHAOVERRIDE_ON


			#if ASE_SRP_VERSION <= 70108
			#define REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
			#endif

			#define ASE_NEEDS_FRAG_COLOR
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#define ASE_NEEDS_VERT_NORMAL
			#pragma shader_feature_local _USERAMP_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				#ifdef ASE_FOG
				float fogFactor : TEXCOORD2;
				#endif
				float4 ase_color : COLOR;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord6 : TEXCOORD6;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _LastColor;
			float4 _WhiteColor;
			float4 _AlphaOverrideChannel;
			float4 _MidColor;
			float4 _MainAlphaChannel;
			float4 _DetailAdditiveChannel;
			float4 _DetailMultiplyChannel;
			float4 _DetailDisolveChannel;
			float _Desaturate;
			float4 _MainTex_ST;
			float4 _MainTextureChannel;
			float4 _AlphaOverride_ST;
			float2 _AlphaOverridePanning;

			float2 _MainTexturePanning;
			float _SourceBlendRGB;
			float _MultiplyNoiseDesaturation;
			float _MiddlePointPos;
			CBUFFER_END
			sampler2D _MainTex;
			sampler2D _AlphaOverride;


						
			VertexOutput VertexFunction ( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				
				o.ase_color = v.ase_color;
				o.ase_texcoord3 = v.ase_texcoord;
				o.ase_texcoord4 = v.ase_texcoord1;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				o.clipPos = positionCS;
				return o;
			}

			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}

			half4 frag ( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float2 uv_MainTex = IN.ase_texcoord3.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float2 panner22 = ( 1.0 * _Time.y * _MainTexturePanning + ( uv_MainTex ));
				float4 tex2DNode6 = tex2D( _MainTex, panner22 );
				float4 break376 = tex2DNode6;
				float4 break379 = _MainTextureChannel;
				float4 appendResult375 = (float4(( break376.r * break379.x ) , ( break376.g * break379.y ) , ( break376.b * break379.z ) , ( break376.a * break379.w )));
				float4 MainTexInfo25 = appendResult375;
				float3 desaturateInitialColor166 = MainTexInfo25.xyz;
				float desaturateDot166 = dot( desaturateInitialColor166, float3( 0.299, 0.587, 0.114 ));
				float3 desaturateVar166 = lerp( desaturateInitialColor166, desaturateDot166.xxx, _Desaturate );
				float4 break364 = ( _DetailMultiplyChannel );
				float4 appendResult365 = (float4(break364.x , break364.y , break364.z , break364.w));
				float3 desaturateInitialColor362 = appendResult365.xyz;
				float desaturateDot362 = dot( desaturateInitialColor362, float3( 0.299, 0.587, 0.114 ));
				float3 desaturateVar362 = lerp( desaturateInitialColor362, desaturateDot362.xxx, _MultiplyNoiseDesaturation );
				float3 temp_cast_5 = (1.0).xxx;
				float3 temp_cast_6 = (1.0).xxx;
				float3 ifLocalVar106 = 0;
				float3 MultiplyNoise92 = ifLocalVar106;
				float4 break156 = ( _DetailAdditiveChannel );
				float4 appendResult155 = (float4(break156.x , break156.y , break156.z , break156.w));
				float3 desaturateInitialColor191 = appendResult155.xyz;
				float desaturateDot191 = dot( desaturateInitialColor191, float3( 0.299, 0.587, 0.114 ));
				float3 desaturateVar191 = lerp( desaturateInitialColor191, desaturateDot191.xxx, 1.0 );
				float3 AdditiveNoise91 = desaturateVar191;
				float3 PreRamp210 = desaturateVar166;
				float temp_output_215_0 = ( 1.0 - _MiddlePointPos );
				float3 temp_cast_10 = (temp_output_215_0).xxx;
				float4 lerpResult220 = lerp( _LastColor , _MidColor , float4( (float3( 0,0,0 ) + (( PreRamp210 * temp_output_215_0 ) - float3( 0,0,0 )) * (float3( 1,1,1 ) - float3( 0,0,0 )) / (temp_cast_10 - float3( 0,0,0 ))) , 0.0 ));
				float3 temp_cast_12 = (_MiddlePointPos).xxx;
				float3 clampResult218 = clamp( ( PreRamp210 - temp_cast_12 ) , float3( 0,0,0 ) , float3( 1,1,1 ) );
				float3 temp_cast_13 = (temp_output_215_0).xxx;
				float4 lerpResult225 = lerp( _MidColor , _WhiteColor , float4( (float3( 0,0,0 ) + (clampResult218 - float3( 0,0,0 )) * (float3( 1,1,1 ) - float3( 0,0,0 )) / (temp_cast_13 - float3( 0,0,0 ))) , 0.0 ));
				float4 lerpResult226 = lerp( lerpResult220 , lerpResult225 , float4( PreRamp210 , 0.0 ));
				float4 break230 = lerpResult226;
				float4 appendResult231 = (float4(break230.r , break230.g , break230.b , PreRamp210.x));
				float4 PostRamp232 = appendResult231;
				#ifdef _USERAMP_ON
				float4 staticSwitch236 = PostRamp232;
				#else
				float4 staticSwitch236 = float4( ( ( desaturateVar166 * MultiplyNoise92 ) + AdditiveNoise91 ) , 0.0 );
				#endif
				float4 texCoord71 = IN.ase_texcoord3;
				texCoord71.xy = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float4 temp_output_39_0 = ( IN.ase_color * staticSwitch236 );
				float2 uv_AlphaOverride = IN.ase_texcoord3.xy * _AlphaOverride_ST.xy + _AlphaOverride_ST.zw;
				float2 panner44 = ( 1.0 * _Time.y * _AlphaOverridePanning + uv_AlphaOverride);
				float4 break2_g205 = ( tex2D( _AlphaOverride, panner44 ) * _AlphaOverrideChannel );
				float AlphaOverride49 = saturate( ( break2_g205.x + break2_g205.y + break2_g205.z + break2_g205.w ) );
				#ifdef _USEALPHAOVERRIDE_ON
				float staticSwitch313 = AlphaOverride49;
				#else
				float staticSwitch313 = 1.0;
				#endif
				float2 panner33 = ( 1.0 * _Time.y * _MainTexturePanning + (  uv_MainTex ));
				float4 break2_g206 = ( tex2D( _MainTex, panner33 ) * _MainAlphaChannel );
				float MainAlpha30 = saturate( ( break2_g206.x + break2_g206.y + break2_g206.z + break2_g206.w ) );
				float temp_output_55_0 = ( staticSwitch313 * MainAlpha30 );
				float4 screenPos = IN.ase_texcoord5;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float staticSwitch198 = temp_output_55_0;
				float4 break17_g207 = break2_g206;
				float4 appendResult18_g207 = (float4(break17_g207.x , break17_g207.y , break17_g207.z , break17_g207.w));
				float4 clampResult19_g207 = clamp( ( appendResult18_g207 * _DetailDisolveChannel ) , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
				float4 break2_g207 = clampResult19_g207;
				float clampResult20_g207 = clamp( ( break2_g207.x + break2_g207.y + break2_g207.z + break2_g207.w ) , 0.0 , 1.0 );
				float DisolveNoise275 = clampResult20_g207;
				float4 texCoord258 = IN.ase_texcoord3;
				texCoord258.xy = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_396_0 = ( ( saturate( step( ( 1.0 - ( staticSwitch198 * DisolveNoise275 ) ) , ( texCoord258.w + 1.0 ) ) ) * staticSwitch198 ) * IN.ase_color.a );
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 ase_worldNormal = IN.ase_texcoord6.xyz;

				float4 staticSwitch403 = temp_output_39_0;
				
				float3 Color = staticSwitch403.rgb;
				float Alpha = temp_output_396_0;


				return half4( Color, Alpha );
			}

			ENDHLSL
		}

	
	}
	CustomEditor "UnityEditor.ShaderGraph.PBRMasterGUI"
	Fallback "Hidden/InternalErrorShader"
	
}