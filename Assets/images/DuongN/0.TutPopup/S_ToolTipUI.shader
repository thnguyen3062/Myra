Shader "S_ToolTipUI"
{
    Properties
    {
        [NoScaleOffset]_MainTex("MainTex", 2D) = "white" {}
        [NoScaleOffset]_MaskTex("MaskTex", 2D) = "white" {}
        [NoScaleOffset]_PatternTex("PatternTex", 2D) = "white" {}
        _Speed("Speed", Vector) = (0.1, 0.1, 0, 0)
        _Color("Color", Color) = (1, 0, 0, 0)
        _Intensity("Intensity", Range(0, 3)) = 1
        _Opacity("Opacity", Range(0, 1)) = 1
        [HideInInspector]_QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector]_QueueControl("_QueueControl", Float) = -1
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255
        _ColorMask("Color Mask", Float) = 15
    }
        SubShader
    {
        Tags
        {
            //"RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            //"UniversalMaterialType" = "Unlit"
            "Queue" = "Transparent"
        }

        Stencil
        {
			Ref[_Stencil]
			Comp[_StencilComp]
			Pass[_StencilOp]
			ReadMask[_StencilReadMask]
			WriteMask[_StencilWriteMask]
        }
		
        ColorMask[_ColorMask]
        Pass
        {
            Name "Pass"
            Tags
            {
            // LightMode: <None>
        }

        // Render State
        Cull Back
		Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
		ZTest[unity_GUIZTestMode]
		ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 3.0
        //#pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma instancing_options renderinglayer
        #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma vertex vert
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma shader_feature _ _SAMPLE_GI
        #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_UNLIT
        #define _FOG_FRAGMENT 1
        #define _SURFACE_TYPE_TRANSPARENT 1
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 texCoord0;
             float3 viewDirectionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float3 interp1 : INTERP1;
             float4 interp2 : INTERP2;
             float3 interp3 : INTERP3;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyz =  input.normalWS;
            output.interp2.xyzw =  input.texCoord0;
            output.interp3.xyz =  input.viewDirectionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.normalWS = input.interp1.xyz;
            output.texCoord0 = input.interp2.xyzw;
            output.viewDirectionWS = input.interp3.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float4 _MaskTex_TexelSize;
        float4 _PatternTex_TexelSize;
        float2 _Speed;
        float4 _Color;
        float _Intensity;
        float _Opacity;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_MaskTex);
        SAMPLER(sampler_MaskTex);
        TEXTURE2D(_PatternTex);
        SAMPLER(sampler_PatternTex);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_b69b92fe925b459f89b90a5365f839d6_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_e5dcbf9031a347ffa1c1580757cd66a5_RGBA_0 = SAMPLE_TEXTURE2D(_Property_b69b92fe925b459f89b90a5365f839d6_Out_0.tex, _Property_b69b92fe925b459f89b90a5365f839d6_Out_0.samplerstate, _Property_b69b92fe925b459f89b90a5365f839d6_Out_0.GetTransformedUV(IN.uv0.xy));
            float _SampleTexture2D_e5dcbf9031a347ffa1c1580757cd66a5_R_4 = _SampleTexture2D_e5dcbf9031a347ffa1c1580757cd66a5_RGBA_0.r;
            float _SampleTexture2D_e5dcbf9031a347ffa1c1580757cd66a5_G_5 = _SampleTexture2D_e5dcbf9031a347ffa1c1580757cd66a5_RGBA_0.g;
            float _SampleTexture2D_e5dcbf9031a347ffa1c1580757cd66a5_B_6 = _SampleTexture2D_e5dcbf9031a347ffa1c1580757cd66a5_RGBA_0.b;
            float _SampleTexture2D_e5dcbf9031a347ffa1c1580757cd66a5_A_7 = _SampleTexture2D_e5dcbf9031a347ffa1c1580757cd66a5_RGBA_0.a;
            UnityTexture2D _Property_1659b9e1271147d5bad27147b802d317_Out_0 = UnityBuildTexture2DStructNoScale(_MaskTex);
            float4 _SampleTexture2D_876ecf2d33074e2b8751907a9e48add7_RGBA_0 = SAMPLE_TEXTURE2D(_Property_1659b9e1271147d5bad27147b802d317_Out_0.tex, _Property_1659b9e1271147d5bad27147b802d317_Out_0.samplerstate, _Property_1659b9e1271147d5bad27147b802d317_Out_0.GetTransformedUV(IN.uv0.xy));
            float _SampleTexture2D_876ecf2d33074e2b8751907a9e48add7_R_4 = _SampleTexture2D_876ecf2d33074e2b8751907a9e48add7_RGBA_0.r;
            float _SampleTexture2D_876ecf2d33074e2b8751907a9e48add7_G_5 = _SampleTexture2D_876ecf2d33074e2b8751907a9e48add7_RGBA_0.g;
            float _SampleTexture2D_876ecf2d33074e2b8751907a9e48add7_B_6 = _SampleTexture2D_876ecf2d33074e2b8751907a9e48add7_RGBA_0.b;
            float _SampleTexture2D_876ecf2d33074e2b8751907a9e48add7_A_7 = _SampleTexture2D_876ecf2d33074e2b8751907a9e48add7_RGBA_0.a;
            float _Saturate_bf14bf185a934b9ab0c2e13aa5e00b8c_Out_1;
            Unity_Saturate_float(_SampleTexture2D_876ecf2d33074e2b8751907a9e48add7_A_7, _Saturate_bf14bf185a934b9ab0c2e13aa5e00b8c_Out_1);
            float _Property_3639fc1bb1f4451c9b3859f02c29dfcd_Out_0 = _Opacity;
            float _Multiply_2cd6b4285ba243128ed740c6e563b134_Out_2;
            Unity_Multiply_float_float(_Saturate_bf14bf185a934b9ab0c2e13aa5e00b8c_Out_1, _Property_3639fc1bb1f4451c9b3859f02c29dfcd_Out_0, _Multiply_2cd6b4285ba243128ed740c6e563b134_Out_2);
            float4 _Multiply_7760fbcb741c41fe889c297b7ef8eeaa_Out_2;
            Unity_Multiply_float4_float4(_SampleTexture2D_e5dcbf9031a347ffa1c1580757cd66a5_RGBA_0, (_Multiply_2cd6b4285ba243128ed740c6e563b134_Out_2.xxxx), _Multiply_7760fbcb741c41fe889c297b7ef8eeaa_Out_2);
            float4 _Property_c58a526edbab48f29e2cb094d5838868_Out_0 = _Color;
            float4 _Multiply_9ede746f64864df798076fedc35bed70_Out_2;
            Unity_Multiply_float4_float4(_SampleTexture2D_876ecf2d33074e2b8751907a9e48add7_RGBA_0, (_SampleTexture2D_876ecf2d33074e2b8751907a9e48add7_A_7.xxxx), _Multiply_9ede746f64864df798076fedc35bed70_Out_2);
            UnityTexture2D _Property_0ba228d48a8c452f9e49bef25154cad4_Out_0 = UnityBuildTexture2DStructNoScale(_PatternTex);
            float2 _Property_152aaeb23a5840e0bbbee5a43d2466e7_Out_0 = _Speed;
            float2 _Multiply_33450094b23044fda72184f942cb1c36_Out_2;
            Unity_Multiply_float2_float2(_Property_152aaeb23a5840e0bbbee5a43d2466e7_Out_0, (IN.TimeParameters.x.xx), _Multiply_33450094b23044fda72184f942cb1c36_Out_2);
            float2 _TilingAndOffset_eec8c65b3f574535a1aef84f96e95e4d_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Multiply_33450094b23044fda72184f942cb1c36_Out_2, _TilingAndOffset_eec8c65b3f574535a1aef84f96e95e4d_Out_3);
            float4 _SampleTexture2D_186838e7e7234278882e89832ae68a90_RGBA_0 = SAMPLE_TEXTURE2D(_Property_0ba228d48a8c452f9e49bef25154cad4_Out_0.tex, _Property_0ba228d48a8c452f9e49bef25154cad4_Out_0.samplerstate, _Property_0ba228d48a8c452f9e49bef25154cad4_Out_0.GetTransformedUV(_TilingAndOffset_eec8c65b3f574535a1aef84f96e95e4d_Out_3));
            float _SampleTexture2D_186838e7e7234278882e89832ae68a90_R_4 = _SampleTexture2D_186838e7e7234278882e89832ae68a90_RGBA_0.r;
            float _SampleTexture2D_186838e7e7234278882e89832ae68a90_G_5 = _SampleTexture2D_186838e7e7234278882e89832ae68a90_RGBA_0.g;
            float _SampleTexture2D_186838e7e7234278882e89832ae68a90_B_6 = _SampleTexture2D_186838e7e7234278882e89832ae68a90_RGBA_0.b;
            float _SampleTexture2D_186838e7e7234278882e89832ae68a90_A_7 = _SampleTexture2D_186838e7e7234278882e89832ae68a90_RGBA_0.a;
            float4 _Multiply_8c0de77f1550420ea575c7db6b25f835_Out_2;
            Unity_Multiply_float4_float4(_Multiply_9ede746f64864df798076fedc35bed70_Out_2, (_SampleTexture2D_186838e7e7234278882e89832ae68a90_R_4.xxxx), _Multiply_8c0de77f1550420ea575c7db6b25f835_Out_2);
            float4 _Multiply_a37a6e762c8c47f4aab24b722b0dd918_Out_2;
            Unity_Multiply_float4_float4(_Property_c58a526edbab48f29e2cb094d5838868_Out_0, _Multiply_8c0de77f1550420ea575c7db6b25f835_Out_2, _Multiply_a37a6e762c8c47f4aab24b722b0dd918_Out_2);
            float _Property_d1dcb7dee39648e59a56a69dc9726797_Out_0 = _Intensity;
            float4 _Multiply_41e19a819a06469eba00556993aa3383_Out_2;
            Unity_Multiply_float4_float4(_Multiply_a37a6e762c8c47f4aab24b722b0dd918_Out_2, (_Property_d1dcb7dee39648e59a56a69dc9726797_Out_0.xxxx), _Multiply_41e19a819a06469eba00556993aa3383_Out_2);
            float4 _Add_4c5a6abee59d47d194c07e4919f28a93_Out_2;
            Unity_Add_float4(_Multiply_7760fbcb741c41fe889c297b7ef8eeaa_Out_2, _Multiply_41e19a819a06469eba00556993aa3383_Out_2, _Add_4c5a6abee59d47d194c07e4919f28a93_Out_2);
            surface.BaseColor = (_Add_4c5a6abee59d47d194c07e4919f28a93_Out_2.xyz);
            surface.Alpha = _Multiply_2cd6b4285ba243128ed740c6e563b134_Out_2;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
            output.uv0 = input.texCoord0;
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Transparent"
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalUnlitSubTarget"
        }
        Pass
        {
            Name "Universal Forward"
            Tags
            {
                // LightMode: <None>
            }
        
        // Render State
        Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma instancing_options renderinglayer
        #pragma vertex vert
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma shader_feature _ _SAMPLE_GI
        #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_UNLIT
        #define _FOG_FRAGMENT 1
        #define _SURFACE_TYPE_TRANSPARENT 1
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 texCoord0;
             float3 viewDirectionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float3 interp1 : INTERP1;
             float4 interp2 : INTERP2;
             float3 interp3 : INTERP3;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyz =  input.normalWS;
            output.interp2.xyzw =  input.texCoord0;
            output.interp3.xyz =  input.viewDirectionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.normalWS = input.interp1.xyz;
            output.texCoord0 = input.interp2.xyzw;
            output.viewDirectionWS = input.interp3.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float4 _MaskTex_TexelSize;
        float4 _PatternTex_TexelSize;
        float2 _Speed;
        float4 _Color;
        float _Intensity;
        float _Opacity;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_MaskTex);
        SAMPLER(sampler_MaskTex);
        TEXTURE2D(_PatternTex);
        SAMPLER(sampler_PatternTex);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_b69b92fe925b459f89b90a5365f839d6_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_e5dcbf9031a347ffa1c1580757cd66a5_RGBA_0 = SAMPLE_TEXTURE2D(_Property_b69b92fe925b459f89b90a5365f839d6_Out_0.tex, _Property_b69b92fe925b459f89b90a5365f839d6_Out_0.samplerstate, _Property_b69b92fe925b459f89b90a5365f839d6_Out_0.GetTransformedUV(IN.uv0.xy));
            float _SampleTexture2D_e5dcbf9031a347ffa1c1580757cd66a5_R_4 = _SampleTexture2D_e5dcbf9031a347ffa1c1580757cd66a5_RGBA_0.r;
            float _SampleTexture2D_e5dcbf9031a347ffa1c1580757cd66a5_G_5 = _SampleTexture2D_e5dcbf9031a347ffa1c1580757cd66a5_RGBA_0.g;
            float _SampleTexture2D_e5dcbf9031a347ffa1c1580757cd66a5_B_6 = _SampleTexture2D_e5dcbf9031a347ffa1c1580757cd66a5_RGBA_0.b;
            float _SampleTexture2D_e5dcbf9031a347ffa1c1580757cd66a5_A_7 = _SampleTexture2D_e5dcbf9031a347ffa1c1580757cd66a5_RGBA_0.a;
            UnityTexture2D _Property_1659b9e1271147d5bad27147b802d317_Out_0 = UnityBuildTexture2DStructNoScale(_MaskTex);
            float4 _SampleTexture2D_876ecf2d33074e2b8751907a9e48add7_RGBA_0 = SAMPLE_TEXTURE2D(_Property_1659b9e1271147d5bad27147b802d317_Out_0.tex, _Property_1659b9e1271147d5bad27147b802d317_Out_0.samplerstate, _Property_1659b9e1271147d5bad27147b802d317_Out_0.GetTransformedUV(IN.uv0.xy));
            float _SampleTexture2D_876ecf2d33074e2b8751907a9e48add7_R_4 = _SampleTexture2D_876ecf2d33074e2b8751907a9e48add7_RGBA_0.r;
            float _SampleTexture2D_876ecf2d33074e2b8751907a9e48add7_G_5 = _SampleTexture2D_876ecf2d33074e2b8751907a9e48add7_RGBA_0.g;
            float _SampleTexture2D_876ecf2d33074e2b8751907a9e48add7_B_6 = _SampleTexture2D_876ecf2d33074e2b8751907a9e48add7_RGBA_0.b;
            float _SampleTexture2D_876ecf2d33074e2b8751907a9e48add7_A_7 = _SampleTexture2D_876ecf2d33074e2b8751907a9e48add7_RGBA_0.a;
            float _Saturate_bf14bf185a934b9ab0c2e13aa5e00b8c_Out_1;
            Unity_Saturate_float(_SampleTexture2D_876ecf2d33074e2b8751907a9e48add7_A_7, _Saturate_bf14bf185a934b9ab0c2e13aa5e00b8c_Out_1);
            float _Property_3639fc1bb1f4451c9b3859f02c29dfcd_Out_0 = _Opacity;
            float _Multiply_2cd6b4285ba243128ed740c6e563b134_Out_2;
            Unity_Multiply_float_float(_Saturate_bf14bf185a934b9ab0c2e13aa5e00b8c_Out_1, _Property_3639fc1bb1f4451c9b3859f02c29dfcd_Out_0, _Multiply_2cd6b4285ba243128ed740c6e563b134_Out_2);
            float4 _Multiply_7760fbcb741c41fe889c297b7ef8eeaa_Out_2;
            Unity_Multiply_float4_float4(_SampleTexture2D_e5dcbf9031a347ffa1c1580757cd66a5_RGBA_0, (_Multiply_2cd6b4285ba243128ed740c6e563b134_Out_2.xxxx), _Multiply_7760fbcb741c41fe889c297b7ef8eeaa_Out_2);
            float4 _Property_c58a526edbab48f29e2cb094d5838868_Out_0 = _Color;
            float4 _Multiply_9ede746f64864df798076fedc35bed70_Out_2;
            Unity_Multiply_float4_float4(_SampleTexture2D_876ecf2d33074e2b8751907a9e48add7_RGBA_0, (_SampleTexture2D_876ecf2d33074e2b8751907a9e48add7_A_7.xxxx), _Multiply_9ede746f64864df798076fedc35bed70_Out_2);
            UnityTexture2D _Property_0ba228d48a8c452f9e49bef25154cad4_Out_0 = UnityBuildTexture2DStructNoScale(_PatternTex);
            float2 _Property_152aaeb23a5840e0bbbee5a43d2466e7_Out_0 = _Speed;
            float2 _Multiply_33450094b23044fda72184f942cb1c36_Out_2;
            Unity_Multiply_float2_float2(_Property_152aaeb23a5840e0bbbee5a43d2466e7_Out_0, (IN.TimeParameters.x.xx), _Multiply_33450094b23044fda72184f942cb1c36_Out_2);
            float2 _TilingAndOffset_eec8c65b3f574535a1aef84f96e95e4d_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Multiply_33450094b23044fda72184f942cb1c36_Out_2, _TilingAndOffset_eec8c65b3f574535a1aef84f96e95e4d_Out_3);
            float4 _SampleTexture2D_186838e7e7234278882e89832ae68a90_RGBA_0 = SAMPLE_TEXTURE2D(_Property_0ba228d48a8c452f9e49bef25154cad4_Out_0.tex, _Property_0ba228d48a8c452f9e49bef25154cad4_Out_0.samplerstate, _Property_0ba228d48a8c452f9e49bef25154cad4_Out_0.GetTransformedUV(_TilingAndOffset_eec8c65b3f574535a1aef84f96e95e4d_Out_3));
            float _SampleTexture2D_186838e7e7234278882e89832ae68a90_R_4 = _SampleTexture2D_186838e7e7234278882e89832ae68a90_RGBA_0.r;
            float _SampleTexture2D_186838e7e7234278882e89832ae68a90_G_5 = _SampleTexture2D_186838e7e7234278882e89832ae68a90_RGBA_0.g;
            float _SampleTexture2D_186838e7e7234278882e89832ae68a90_B_6 = _SampleTexture2D_186838e7e7234278882e89832ae68a90_RGBA_0.b;
            float _SampleTexture2D_186838e7e7234278882e89832ae68a90_A_7 = _SampleTexture2D_186838e7e7234278882e89832ae68a90_RGBA_0.a;
            float4 _Multiply_8c0de77f1550420ea575c7db6b25f835_Out_2;
            Unity_Multiply_float4_float4(_Multiply_9ede746f64864df798076fedc35bed70_Out_2, (_SampleTexture2D_186838e7e7234278882e89832ae68a90_R_4.xxxx), _Multiply_8c0de77f1550420ea575c7db6b25f835_Out_2);
            float4 _Multiply_a37a6e762c8c47f4aab24b722b0dd918_Out_2;
            Unity_Multiply_float4_float4(_Property_c58a526edbab48f29e2cb094d5838868_Out_0, _Multiply_8c0de77f1550420ea575c7db6b25f835_Out_2, _Multiply_a37a6e762c8c47f4aab24b722b0dd918_Out_2);
            float _Property_d1dcb7dee39648e59a56a69dc9726797_Out_0 = _Intensity;
            float4 _Multiply_41e19a819a06469eba00556993aa3383_Out_2;
            Unity_Multiply_float4_float4(_Multiply_a37a6e762c8c47f4aab24b722b0dd918_Out_2, (_Property_d1dcb7dee39648e59a56a69dc9726797_Out_0.xxxx), _Multiply_41e19a819a06469eba00556993aa3383_Out_2);
            float4 _Add_4c5a6abee59d47d194c07e4919f28a93_Out_2;
            Unity_Add_float4(_Multiply_7760fbcb741c41fe889c297b7ef8eeaa_Out_2, _Multiply_41e19a819a06469eba00556993aa3383_Out_2, _Add_4c5a6abee59d47d194c07e4919f28a93_Out_2);
            surface.BaseColor = (_Add_4c5a6abee59d47d194c07e4919f28a93_Out_2.xyz);
            surface.Alpha = _Multiply_2cd6b4285ba243128ed740c6e563b134_Out_2;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
            output.uv0 = input.texCoord0;
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
    }
    CustomEditorForRenderPipeline "UnityEditor.ShaderGraphUnlitGUI" "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset"
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    FallBack "Hidden/Shader Graph/FallbackError"
}