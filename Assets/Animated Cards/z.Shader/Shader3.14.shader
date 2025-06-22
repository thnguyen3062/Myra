Shader "ReadyPrefab/Shader3.14"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "black" {}
		_DistortionAndRippleTexture("Distortion & RippleTexture", 2D) = "grey" {}
		_ZoneMask("Effect Mask", 2D) = "black" {}
		_DistortionSpeed("Distortion Spd", Vector) = (0.5 , 0.5 , 0 , 0) //3
		_IsShaking("Is Shaking", int) = 1 //4
		_ShakeThings("Shake Things", Vector) = (0.5 , 0.5 , 0 , 0) //5
		// _RippleDistort("Ripple", float) = 0 //Old6
		// _RippleSize("Ripple Size", float) = 0.05 //Old7
		// _RippleThings("Ripple Things", Vector) = (0.5 , 0.5 , 0 , 0) //Old8
		_TileOff ("Texture Tiling/Offset", Vector) = (1, 1, 0, 0) //9 -> 6
		_Tired("Tired", int) = 0 //10 ->7
		// _CardColorMask("CardColorMask", 2D) = "black" {} //11 -> 8
		// _ColTileOff("Card Color Tiling/Offset", Vector) = (1, 1, 0, 0) //12 -> 9
		// _MainTexBias ("Mip Bias (-1 to 1)", float) = -0.9  //10

		_EF1Tex("1.Effect 1 Tex", 2D) = "black"{}
		_EF1Color("1.Color", Color) = (1,1,1,1)
		_EF1Intensity("1.BlendIntensity", float) = 1
		_EF1Motion("1.Motion", 2D) = "black"{}
		_EF1MotionSpeed("1.MotionSpeed", float) = 0 
		_EF1BackAndForth("1.MotionBackAndForth", int) = 1
		_EF1Rotation("1.RotationSpeed", float) = 0
		_EF1Oscillate("1.Oscillate", int) = 1
		_EF1PivotScale("1.Pivot&Scale", Vector) = (0.5,0.5,1,1)
		_EF1Translation("1.Translation", Vector) = (0,0,0,0)
		_EF1Foreground("1.Foreground", int) = 0
		_EF1IsCutPart("1.IsCutPart", int) = 0
		_EF1IsBlinking("1.IsBlinking", int) = 0
		_EF1BlinkSpdDurOffsetAdd("1.BlinkSpdDurOffsetAdd", Vector) = (0,0,0,0)
		_EF1IsAdditive("1.IsAdditive", int) = 1
		_EF1Distorted("1.Distorted", int) = 1
		_EF1DistortionIntensity("1.DistortionIntensity", float) = 0.025
		_EF1IsBlinkOffset("1.IsBlinkOffset", int) = 0
		_EF1BreatheX("1.BreatheX", int) = 0
		_EF1BreatheY("1.BreatheY", int) = 0
		_EF1BreatheXYFregMag("1.BreatheXYFregMag", Vector) = (1,1,1,1)
		_EF1BackForthOsciCycleMag("1.BackForthOsciCycleMag", Vector) = (1,1,1,1)

		_EF2Tex("2.Effect 2 Tex", 2D) = "black"{}
		_EF2Color("2.Color", Color) = (1,1,1,1)
		_EF2Intensity("2.BlendIntensity", float) = 1
		_EF2Motion("2.Motion", 2D) = "black"{}
		_EF2MotionSpeed("2.MotionSpeed", float) = 0 
		_EF2BackAndForth("2.MotionBackAndForth", int) = 1
		_EF2Rotation("2.RotationSpeed", float) = 0
		_EF2Oscillate("2.Oscillate", int) = 1
		_EF2PivotScale("2.Pivot&Scale", Vector) = (0.5,0.5,1,1)
		_EF2Translation("2.Translation", Vector) = (0,0,0,0)
		_EF2Foreground("2.Foreground", int) = 0
		_EF2IsCutPart("2.IsCutPart", int) = 0
		_EF2IsBlinking("2.IsBlinking", int) = 0
		_EF2BlinkSpdDurOffsetAdd("2.BlinkSpdDurOffsetAdd", Vector) = (0,0,0,0)
		_EF2IsAdditive("2.IsAdditive", int) = 1
		_EF2Distorted("2.Distorted", int) = 1
		_EF2DistortionIntensity("2.DistortionIntensity", float) = 0.025
		_EF2IsBlinkOffset("2.IsBlinkOffset", int) = 0
		_EF2BreatheX("2.BreatheX", int) = 0
		_EF2BreatheY("2.BreatheY", int) = 0
		_EF2BreatheXYFregMag("2.BreatheXYFregMag", Vector) = (1,1,1,1)
		_EF2BackForthOsciCycleMag("1.BackForthOsciCycleMag", Vector) = (1,1,1,1)

		_EF3Tex("3.Effect 3 Tex", 2D) = "black"{}
		_EF3Color("3.Color", Color) = (1,1,1,1)
		_EF3Intensity("3.BlendIntensity", float) = 1
		_EF3Motion("3.Motion", 2D) = "black"{}
		_EF3MotionSpeed("3.MotionSpeed", float) = 0 
		_EF3BackAndForth("3.MotionBackAndForth", int) = 1
		_EF3Rotation("3.RotationSpeed", float) = 0
		_EF3Oscillate("3.Oscillate", int) = 1
		_EF3PivotScale("3.Pivot&Scale", Vector) = (0.5,0.5,1,1)
		_EF3Translation("3.Translation", Vector) = (0,0,0,0)
		_EF3Foreground("3.Foreground", int) = 0
		_EF3IsCutPart("3.IsCutPart", int) = 0
		_EF3IsBlinking("3.IsBlinking", int) = 0
		_EF3BlinkSpdDurOffsetAdd("3.BlinkSpdDurOffsetAdd", Vector) = (0,0,0,0)
		_EF3IsAdditive("3.IsAdditive", int) = 1
		_EF3Distorted("3.Distorted", int) = 1
		_EF3DistortionIntensity("3.DistortionIntensity", float) = 0.025
		_EF3IsBlinkOffset("3.IsBlinkOffset", int) = 0
		_EF3BreatheX("3.BreatheX", int) = 0
		_EF3BreatheY("3.BreatheY", int) = 0
		_EF3BreatheXYFregMag("3.BreatheXYFregMag", Vector) = (1,1,1,1)
		_EF3BackForthOsciCycleMag("1.BackForthOsciCycleMag", Vector) = (1,1,1,1)

		_EF4Tex("4.Effect 4 Tex", 2D) = "black"{}
		_EF4Color("4.Color", Color) = (1,1,1,1)
		_EF4Intensity("4.BlendIntensity", float) = 1
		_EF4Motion("4.Motion", 2D) = "black"{}
		_EF4MotionSpeed("4.MotionSpeed", float) = 0 
		_EF4BackAndForth("4.MotionBackAndForth", int) = 1
		_EF4Rotation("4.RotationSpeed", float) = 0
		_EF4Oscillate("4.Oscillate", int) = 1
		_EF4PivotScale("4.Pivot&Scale", Vector) = (0.5,0.5,1,1)
		_EF4Translation("4.Translation", Vector) = (0,0,0,0)
		_EF4Foreground("4.Foreground", int) = 0
		_EF4IsCutPart("4.IsCutPart", int) = 0
		_EF4IsBlinking("4.IsBlinking", int) = 0
		_EF4BlinkSpdDurOffsetAdd("4.BlinkSpdDurOffsetAdd", Vector) = (0,0,0,0)
		_EF4IsAdditive("4.IsAdditive", int) = 1
		_EF4Distorted("4.Distorted", int) = 1
		_EF4DistortionIntensity("4.DistortionIntensity", float) = 0.025
		_EF4IsBlinkOffset("4.IsBlinkOffset", int) = 0
		_EF4BreatheX("4.BreatheX", int) = 0
		_EF4BreatheY("4.BreatheY", int) = 0
		_EF4BreatheXYFregMag("4.BreatheXYFregMag", Vector) = (1,1,1,1)
		_EF4BackForthOsciCycleMag("1.BackForthOsciCycleMag", Vector) = (1,1,1,1)
		_DistortionScroll ("Distortion Scroll", Vector) = (3 , 3 , 0 , 0)

//Declare to use UI shader.
		_StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15




	}

		SubShader
		{
			Tags {
				"Queue" = "Transparent"
	            // "Queue"="Geometry" Background AlphaTest GeometryLast Transparent Overlay
				"RenderType" = "Transparent" 
				// "RenderType" = "Opaque" Transparent TransparentCutout Background Overlay
				// "PreviewType" = "Plane"
			// "RenderPipeline"="UniversalPipeline"
            // "RenderType"="Opaque"
            "UniversalMaterialType" = "Unlit"
			// "UI" = "True"

			}
			LOD 100
			Cull Back
			// Cull Back | Front | Off
			ZWrite Off
			// ZWrite On | Off
			ZTest [unity_GUIZTestMode]
			// ZTest Less | Greater | LEqual | GEqual | Equal | NotEqual | Always | [unity_GUIZTestMode]
	        Lighting Off
	        ColorMask [_ColorMask] //For UI shader.
			

//For UI Shader.
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
		}

		Pass
		{

			// ZWrite On
            // ZTest Greater
            //SetTexture [_MainTex] {}

			// Blend One One
			// Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			// #pragma glsl
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature EFFECTS_LAYER_1_OFF EFFECTS_LAYER_1_ON
			#pragma shader_feature EFFECTS_LAYER_2_OFF EFFECTS_LAYER_2_ON
			#pragma shader_feature EFFECTS_LAYER_3_OFF EFFECTS_LAYER_3_ON
			#pragma shader_feature EFFECTS_LAYER_4_OFF EFFECTS_LAYER_4_ON
			#pragma multi_compile_instancing


			#include "UnityCG.cginc"
            #include "Assets/Animated Cards/z.Shader/MyCGInclude.cginc"
	

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				// float4 uv1 : TEXCOORD5; 
				float2 uv2 : TEXCOORD6;
				// float2 uv3 : TEXCOORD7;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 blinkVar : TEXCOORD5; //Blink calculation
				float2 uv2 : TEXCOORD6;
				float4 backAndForthOrMove : TEXCOORD7; //Back and forth movement calculations.

#if EFFECTS_LAYER_1_ON	
				float4 effect1uv : TEXCOORD1;
#endif
#if EFFECTS_LAYER_2_ON	
				float4 effect2uv : TEXCOORD2;
#endif
#if EFFECTS_LAYER_3_ON	
				float4 effect3uv : TEXCOORD3;
#endif
#if EFFECTS_LAYER_4_ON	
				float4 effect4uv : TEXCOORD4;
#endif
			};

			sampler2D _MainTex;
			sampler2D _DistortionAndRippleTexture;
			sampler2D _ZoneMask;
			float2 _DistortionSpeed; //3
			bool _IsShaking; //4
			float4 _ShakeThings; //5
			// float _RippleDistort; //Old6
			// float _RippleSize; //Old7
			// float4 _RippleThings; //Old8
			float4 _TileOff; //9
			bool _Tired; //10
			// sampler2D _CardColorMask; //11
			// float4 _ColTileOff; //12
			// float _MainTexBias; //10



			sampler2D _EF1Tex;
			sampler2D _EF1Motion;
			float _EF1MotionSpeed;
			float _EF1Rotation;
			float4 _EF1PivotScale;
			float4 _EF1Color;
			float _EF1Intensity;
			bool _EF1Foreground;
			float2 _EF1Translation;
			bool _EF1Oscillate;
			bool _EF1BackAndForth;
			bool _EF1IsCutPart;
			bool _EF1IsBlinking;
			bool _EF1IsAdditive;
			bool _EF1Distorted;
			float _EF1DistortionIntensity;
			bool _EF1IsBlinkOffset;
			bool _EF1BreatheX;
			bool _EF1BreatheY;
			float4 _EF1BreatheXYFregMag;
			float4 _EF1BlinkSpdDurOffsetAdd;
			float4 _EF1BackForthOsciCycleMag;

			sampler2D _EF2Tex;
			sampler2D _EF2Motion;
			float _EF2MotionSpeed;
			float _EF2Rotation;
			float4 _EF2PivotScale;
			float4 _EF2Color;
			float _EF2Intensity;
			bool _EF2Foreground;
			float2 _EF2Translation;
			bool _EF2Oscillate;
			bool _EF2BackAndForth;
			bool _EF2IsCutPart;
			bool _EF2IsBlinking;
			bool _EF2IsAdditive;
			bool _EF2Distorted;
			float _EF2DistortionIntensity;
			bool _EF2IsBlinkOffset;
			bool _EF2BreatheX;
			bool _EF2BreatheY;
			float4 _EF2BreatheXYFregMag;
			float4 _EF2BlinkSpdDurOffsetAdd;
			float4 _EF2BackForthOsciCycleMag;

			sampler2D _EF3Tex;
			sampler2D _EF3Motion;
			float _EF3MotionSpeed;
			float _EF3Rotation;
			float4 _EF3PivotScale;
			float4 _EF3Color;
			float _EF3Intensity;
			bool _EF3Foreground;
			float2 _EF3Translation;
			bool _EF3Oscillate;
			bool _EF3BackAndForth;
			bool _EF3IsCutPart;
			bool _EF3IsBlinking;
			bool _EF3IsAdditive;
			bool _EF3Distorted;
			float _EF3DistortionIntensity;
			bool _EF3IsBlinkOffset;
			bool _EF3BreatheX;
			bool _EF3BreatheY;
			float4 _EF3BreatheXYFregMag;
			float4 _EF3BlinkSpdDurOffsetAdd;
			float4 _EF3BackForthOsciCycleMag;

			sampler2D _EF4Tex;
			sampler2D _EF4Motion;
			float _EF4MotionSpeed;
			float _EF4Rotation;
			float4 _EF4PivotScale;
			float4 _EF4Color;
			float _EF4Intensity;
			bool _EF4Foreground;
			float2 _EF4Translation;
			bool _EF4Oscillate;
			bool _EF4BackAndForth;
			bool _EF4IsCutPart;
			bool _EF4IsBlinking;
			bool _EF4IsAdditive;
			bool _EF4Distorted;
			float _EF4DistortionIntensity;
			bool _EF4IsBlinkOffset;
			bool _EF4BreatheX;
			bool _EF4BreatheY;
			float4 _EF4BreatheXYFregMag;
			float4 _EF4BlinkSpdDurOffsetAdd;
			float4 _EF4BackForthOsciCycleMag;

			float2 _DistortionScroll;
			float2 newRippleCenter;
			float2 ripples;

//This is the main VERTEX shader part, applies to all effects.
//This is where whole screen shaking happens.
//Back and forth movement calculation.

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				float shake = sin(frac(_Time.x) * 6.28 * _ShakeThings.z /*_ShakeSpeed */ * 100) * _ShakeThings.w /*_ShakeMagnitude */ * 0.001;

				//Shake whole scene or not, plus tiling and offset.
				o.uv = (v.uv + _IsShaking * shake * step(frac(_Time.y / _ShakeThings.x /*_TotalShakeEffectPeriod */) * 6.28, _ShakeThings.y /*_ShakeDuration */)) 
						* _TileOff.xy + _TileOff.zw; //Tiling & offset
				
				//UV of color of the card
				// o.uv1.xy = v.uv1.xy * _ColTileOff.xy + _ColTileOff.zw;
				
                o.uv2 = v.uv + distortionScrollFunc(_DistortionSpeed.x, _DistortionSpeed.y);

				//Calculate move back and forth for each effect in the vertex shader.
				o.backAndForthOrMove = float4 ( frac(backAndForthOrMoveFunc(_EF1BackAndForth , _EF1BackForthOsciCycleMag.x , _EF1BackForthOsciCycleMag.y) * _EF1MotionSpeed),
												frac(backAndForthOrMoveFunc(_EF2BackAndForth , _EF2BackForthOsciCycleMag.x , _EF2BackForthOsciCycleMag.y) * _EF2MotionSpeed),
												frac(backAndForthOrMoveFunc(_EF3BackAndForth , _EF3BackForthOsciCycleMag.x , _EF3BackForthOsciCycleMag.y) * _EF3MotionSpeed),
												frac(backAndForthOrMoveFunc(_EF4BackAndForth , _EF4BackForthOsciCycleMag.x , _EF4BackForthOsciCycleMag.y) * _EF4MotionSpeed)
												 ); //Multiply with motion speed, add frac to avoid texture tiling issues.
				//Calculate blink for each effect.
				o.blinkVar = float4 ( 
									  blinkFunc (_EF1Intensity, _EF1IsBlinking,_EF1BlinkSpdDurOffsetAdd.x, _EF1BlinkSpdDurOffsetAdd.y, _EF1BlinkSpdDurOffsetAdd.z, _EF1IsBlinkOffset, 				_EF1BlinkSpdDurOffsetAdd.w),
									  blinkFunc (_EF2Intensity, _EF2IsBlinking,_EF2BlinkSpdDurOffsetAdd.x, _EF2BlinkSpdDurOffsetAdd.y, _EF2BlinkSpdDurOffsetAdd.z, _EF2IsBlinkOffset, _EF2BlinkSpdDurOffsetAdd.w),
									  blinkFunc (_EF3Intensity, _EF3IsBlinking,_EF3BlinkSpdDurOffsetAdd.x, _EF3BlinkSpdDurOffsetAdd.y, _EF3BlinkSpdDurOffsetAdd.z, _EF3IsBlinkOffset, _EF3BlinkSpdDurOffsetAdd.w),
									  blinkFunc (_EF4Intensity, _EF4IsBlinking,_EF4BlinkSpdDurOffsetAdd.x, _EF4BlinkSpdDurOffsetAdd.y, _EF4BlinkSpdDurOffsetAdd.z, _EF4IsBlinkOffset, _EF4BlinkSpdDurOffsetAdd.w)
									);
				//Declaring the rotation matrix components.
				float2x2 rotationMatrix;
				float oscillate; //Rotate or oscillate based on value of _Oscillate being 0 or 1.

				// For each effect channel, calculate UV rotations and scale about the pivot, and translate the point.
				// Oscillate must be between 0 and 1.

//This is the separate vertex effect for each effect if switched on
//Rotation, oscillation, translation, scaling and changing pivots happen here.
#if EFFECTS_LAYER_1_ON		
				o.effect1uv.xy = o.uv - _EF1PivotScale.xy;  //Minus Pivot before transformation. After shake.
				//Rotation matrix, with oscillating bool, frequency and magnitude, rotation.
				rotationMatrix = rotationMatrixFunc(_EF1Oscillate , _EF1BackForthOsciCycleMag.z , _EF1BackForthOsciCycleMag.w, _EF1Rotation);
				//Add rotation, scale and add back pivot, then add translation.
				o.effect1uv.xy = 	mul(o.effect1uv.xy , rotationMatrix)   //Rotation
								* (1 / _EF1PivotScale.zw) //Scale
								+ _EF1PivotScale.xy 		//Add back Pivot
								- _EF1Translation.xy;	//Translation
				o.effect1uv.zw = breatheFuncShort(_EF1BreatheX,_EF1BreatheXYFregMag.x,_EF1BreatheXYFregMag.y,_EF1BreatheY,_EF1BreatheXYFregMag.z,_EF1BreatheXYFregMag.w);  //Breathe
#endif

#if EFFECTS_LAYER_2_ON		
				o.effect2uv.xy = o.uv - _EF2PivotScale.xy;
				rotationMatrix = rotationMatrixFunc(_EF2Oscillate , _EF2BackForthOsciCycleMag.z , _EF2BackForthOsciCycleMag.w, _EF2Rotation);
				o.effect2uv.xy = mul(o.effect2uv.xy , rotationMatrix)
								* (1 / _EF2PivotScale.zw)
								+ _EF2PivotScale.xy
								- _EF2Translation.xy ;
				o.effect2uv.zw = breatheFuncShort(_EF2BreatheX,_EF2BreatheXYFregMag.x,_EF2BreatheXYFregMag.y,_EF2BreatheY,_EF2BreatheXYFregMag.z,_EF2BreatheXYFregMag.w);  //Breathe
#endif

#if EFFECTS_LAYER_3_ON	

//Pivot and rotation has to be here. I don't really know why. Scale and translation will be at the frag part.
				o.effect3uv.xy = o.uv - _EF3PivotScale.xy;
				rotationMatrix = rotationMatrixFunc(_EF3Oscillate , _EF3BackForthOsciCycleMag.z , _EF3BackForthOsciCycleMag.w, _EF3Rotation);
				o.effect3uv.xy = mul(o.effect3uv.xy ,  rotationMatrix)
									* (1 / _EF3PivotScale.zw) //Scale
									+ _EF3PivotScale.xy 		//Add back Pivot
									- _EF1Translation.xy;	//Translation;
				o.effect3uv.zw = breatheFuncShort(_EF3BreatheX,_EF3BreatheXYFregMag.x,_EF3BreatheXYFregMag.y,_EF3BreatheY,_EF3BreatheXYFregMag.z,_EF3BreatheXYFregMag.w);  //Breathe
#endif

#if EFFECTS_LAYER_4_ON		
				o.effect4uv.xy = o.uv - _EF4PivotScale.xy;
				rotationMatrix = rotationMatrixFunc(_EF4Oscillate , _EF4BackForthOsciCycleMag.z , _EF4BackForthOsciCycleMag.w, _EF4Rotation);
				o.effect4uv.xy = mul(o.effect4uv.xy , rotationMatrix)  * (1 / _EF4PivotScale.zw) + _EF4PivotScale.xy - _EF4Translation.xy ;
				o.effect4uv.zw = breatheFuncShort(_EF4BreatheX,_EF4BreatheXYFregMag.x,_EF4BreatheXYFregMag.y,_EF4BreatheY,_EF4BreatheXYFregMag.z,_EF4BreatheXYFregMag.w);  //Breathe
#endif	
				return o;
			}



//This is the main FRAGMENT SHADER part, applies to all effects.
//What happens here:
	//Distortion scrolling in separate directions.
	//Declaring the masks for distortion.
	//Circular ripple effect (whole image, all effects) ----> not used for now.

			fixed4 frag(v2f i) : SV_Target
			{
				fixed2 distortion = (tex2D(_DistortionAndRippleTexture, i.uv2).rg);

				fixed4 distortionAndRippleMask = tex2D(_DistortionAndRippleTexture, i.uv);
				
				fixed4 zoneMask = tex2D(_ZoneMask, i.uv);

				// newRippleCenter = float2 (_RippleThings.z, _RippleThings.w);

				fixed4 col = tex2D(_MainTex, i.uv + distortion.rg * distortionAndRippleMask.b * 0.025

				 //Ripple effects
				 				// + 
								// _RippleDistort * rippleFunc(_RippleThings.y , _RippleSize , i.uv , newRippleCenter, _RippleThings.x )

								);
				//B is mask 0, for distortion				

				fixed4 bfm = i.backAndForthOrMove; //Back and forth movement from vertex shader to fragment.
				fixed4 blinkZ = i.blinkVar;
				fixed bg = col.a;


//This is the separate FRAGMENT shader for each separate effects.
//What happens here:
	//Moving the effect in different directions, back and forth speed and magnitude.
	//Blinking.	
	//Breathing.
	//Turning distortion for this effect layer.
	//Apply different masking and colors.

#if EFFECTS_LAYER_1_ON		

				fixed4 motion1 = tex2D(_EF1Motion, i.uv); //Sample the color of Motion Texture of Effect 1 and UV of output UV of VERTEX shader.

																			// If _EF1BackAndForth is true, play the sine curve. If not, move it normally. Frequency and magnitude can change the sin curve features.
				if (_EF1MotionSpeed) 										//If speed <> 0, use this.
					motion1.y -= bfm.r; //Get the Green channel of Motion Texture of Effect 1, minus the product of backAndForthOrMove1 and the speed value, which give us 
																					//the move back and forth, or move normally, depend on the _EffectLayer1BackAndForth being 1 or 0.
				else 																//If speed = 0, use below formula.
					motion1 = fixed4(i.effect1uv.rg, motion1.b, motion1.a); 		//Fixed4 motion1 uses float2 effect1uv green and red channel of the out UV (become the input UV) from VERTEX shader,
																					//blue channel =  blue channel of Motion Texture of Effect 1, alpha = alpha of Motion Texture of Effect 1.

				//This scale is for scaling back and forth.
				/* fixed2 _Scale1 =  (1 - _EF1ScaleBF) * _EF1PivotScale.zw  
								+
								 _EF1ScaleBF
								    						* (sin(frac(_Time.x) * 6.283 * round(20 / _EF1ScaleFrequency)) * 0.01  * _EF1PivotScale.zw)  + 1 ;  */
				//This translate is for translating back and forth.
				/* fixed2 _Translate1 =  (1 - _EF1TranslateBF) * _EF1Translation.xy  
								+
								 _EF1TranslateBF
								    						* sin(frac(_Time.x) * 6.28 * round(20 / _EF1TranslateFrequency))   * _EF1Translation.xy;  */

				//Effect1 's UV, movement, pivot, scale and translation without moving back the pivot, multiply by alpha of motion1, multiply by color, intensity, and blink or not.
				fixed2 breathe1 = fixed2(i.effect1uv.z * motion1.r , i.effect1uv.w * motion1.g) * motion1.b;
				//motion1.r: Multiply by Red channel of motion1 - the U (not V)
				//motion1.b = zone: Blue channel of Main Distortion Texture Zone of breathing, white is moving a lot, black is not moving.

				fixed2 addDistort1 = singleDistortFunc(_EF1Distorted,distortionAndRippleMask.b,_EF1DistortionIntensity,distortion.rg);

				fixed4 effect1 = 							   		//Find the color of Effect1 before blending with main tex.
									(  tex2D(_EF1Tex 		//Now sample the color of the Effect Texture 1.
											,
											motion1.rg       		//And use the UV of Motion Texture of Effect 1, only red and green channel to move.
																	+ breathe1 + addDistort1
																										//Scaling back and forth, but not used.
																										//* ( 1 / _Scale1)  
																										//Translating back and forth, but not used.
																										//-  _Translate1
											)
											// * motion1.a			//After getting the texture from Effect Texture of Effect1, and the UV from calculations, 
											)        				//multiply with alpha of Motion Texture of Effect1, which is now nonexistent. Doesn't make sense.			
									*_EF1Color 			//Multiply with color tint.
									* blinkZ.r						//And Blinks if any.
									;
				// zoneMask.Red is mask 1 for effect.

				fixed4 effect1Separate =  separateEffFunc(_EF1IsCutPart, col, effect1, zoneMask.r, _EF1IsAdditive);

				col = _EF1Foreground * lerp(col, effect1Separate, smoothstep(0, 1 , min(bg, _EF1Foreground)))
					+ (1 - _EF1Foreground) * ( effect1Separate);
#endif

#if EFFECTS_LAYER_2_ON		

				fixed4 motion2 = tex2D(_EF2Motion, i.uv);

				if (_EF2MotionSpeed)
					motion2.y -= bfm.g;
				else
					motion2 = fixed4(i.effect2uv.rg, motion2.b, motion2.a);

				fixed2 breathe2 = fixed2(i.effect2uv.z * motion2.r , i.effect2uv.w * motion2.g) * motion2.b;
				
				fixed2 addDistort2 = singleDistortFunc(_EF2Distorted,distortionAndRippleMask.b,_EF2DistortionIntensity,distortion.rg);

				//Effect2 's UV, movement, pivot, scale and translation without moving back the pivot, multiply by alpha of motion1, multiply by color, intensity, and blink or not.
				fixed4 effect2 = 
									(tex2D(_EF2Tex, motion2.rg + breathe2 + addDistort2))
									*_EF2Color 
									* blinkZ.g
									;
				//zoneMask.Green is mask 2 for effect.

				fixed4 effect2Separate =  separateEffFunc(_EF2IsCutPart, col, effect2, zoneMask.g, _EF2IsAdditive);

				col = _EF2Foreground * lerp(col, effect2Separate, smoothstep(0, 1 , min(bg, _EF2Foreground)))
					+ (1 - _EF2Foreground) * (effect2Separate);
#endif

#if EFFECTS_LAYER_3_ON		

				fixed4 motion3 = tex2D(_EF3Motion, i.uv);

				if (_EF3MotionSpeed)
					motion3.y -= bfm.b;
				else
					motion3 = fixed4(i.effect3uv.rg, motion3.b, motion3.a);

				fixed2 breathe3 = fixed2(i.effect3uv.z * motion3.r , i.effect3uv.w * motion3.g) * motion3.b;

				fixed2 addDistort3 = singleDistortFunc(_EF3Distorted,distortionAndRippleMask.b,_EF3DistortionIntensity,distortion.rg);

				//Effect3 's UV, movement, pivot, scale and translation without moving back the pivot, multiply by alpha of motion1, multiply by color, intensity, and blink or not.
				
				fixed4 effect3 = 							   		//Find the color of Effect1 before blending with main tex.
									(  tex2D(_EF3Tex 		//Now sample the color of the Effect Texture 1.
											,
											motion3.rg  + breathe3  + addDistort3   		//And use the UV of Motion Texture of Effect 1, only red and green channel to move.
																))
									*_EF3Color 			//Multiply with color tint.
									* blinkZ.b						//And Blinks if any.
									;

				//zoneMask.Blue is mask 3 for effect.

				fixed4 effect3Separate =  separateEffFunc(_EF3IsCutPart, col, effect3, zoneMask.b, _EF3IsAdditive);

				col = _EF3Foreground * lerp(col, effect3Separate, smoothstep(0, 1 , min(bg, _EF3Foreground)))
					+ (1 - _EF3Foreground) * (effect3Separate);
#endif

#if EFFECTS_LAYER_4_ON		

				fixed4 motion4 = tex2D(_EF4Motion, i.uv);

				if (_EF4MotionSpeed)
					motion4.y -= bfm.a;
				else
					motion4 = fixed4(i.effect4uv.rg, motion4.b, motion4.a);

				fixed2 breathe4 = fixed2(i.effect4uv.z * motion4.r , i.effect4uv.w * motion4.g) * motion4.b;

				fixed2 addDistort4 = singleDistortFunc(_EF4Distorted,distortionAndRippleMask.b,_EF4DistortionIntensity,distortion.rg);

				//Effect4 's UV, movement, pivot, scale and translation without moving back the pivot, multiply by alpha of motion1, multiply by color, intensity, and blink or not.
				fixed4 effect4 = 
									(tex2D(_EF4Tex, (motion4.rg + breathe4 + addDistort4)))
									*_EF4Color 
									* blinkZ.a
									;
				
				fixed4 effect4Separate =  separateEffFunc(_EF4IsCutPart, col, effect4, zoneMask.a, _EF4IsAdditive);
				//zoneMask.Alpha is mask 4 for effect.

				col = _EF4Foreground * lerp(col, effect4Separate, smoothstep(0, 1 , min(bg, _EF4Foreground)))
					+ (1 - _EF4Foreground) * (effect4Separate);
#endif
				//Grayscale when tired.
				col = toGray(col, _Tired);

				//Putting card color on top.
				// fixed4 cardCol = tex2D(_CardColorMask, i.uv1.xy);
				// col = lerp(col, cardCol, cardCol.a);
				//Final color of the whole shader.
				return col;
		}
		ENDCG
	}
	}
    FallBack "UI/Default"
	CustomEditor "CustomMaterialEditorEF"
}