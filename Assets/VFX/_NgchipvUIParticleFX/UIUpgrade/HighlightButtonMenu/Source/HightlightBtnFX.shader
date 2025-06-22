Shader "HighlightBtnFX"
{
    Properties
    {
        [NoScaleOffset] Texture2D_a54af73971754a788508672b9d7b8cc3("MainTex", 2D) = "white" {}
        [NoScaleOffset]Texture2D_d59f074fadd14270974ce7a66ab4f51c("Mask", 2D) = "white" {}
        Vector1_1dbe385c06844617a395f8d425b37c2e("Speed", Float) = 0.05
        [HDR]Color_022597fd83634c529dcce3fbfd9ed6e6("Color", Color) = (1, 0.9191281, 0.4481132, 0)
        Vector1_e3eb3cafdee54e098df5ab84fc0392eb("Opacity", Float) = 0.15
        Vector1_0a04d443530040849477bafffc25fcb2("ScaleX", Float) = 0
        Vector1_019cb3139e9c46d6b9592ac14eda99e9("ScaleY", Float) = 0
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
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "UniversalMaterialType" = "Unlit"
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
        #pragma target 4.5
    #pragma exclude_renderers gles gles3 glcore
    #pragma multi_compile_instancing
    #pragma multi_compile_fog
    #pragma multi_compile _ DOTS_INSTANCING_ON
    #pragma vertex vert
    #pragma fragment frag

        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>

        // Keywords
        #pragma multi_compile _ LIGHTMAP_ON
    #pragma multi_compile _ DIRLIGHTMAP_COMBINED
    #pragma shader_feature _ _SAMPLE_GI
        // GraphKeywords: <None>

        // Defines
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_UNLIT
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

        // --------------------------------------------------
        // Structs and Packing

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
        float4 texCoord0;
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
        float4 interp0 : TEXCOORD0;
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

        PackedVaryings PackVaryings(Varyings input)
    {
        PackedVaryings output;
        output.positionCS = input.positionCS;
        output.interp0.xyzw = input.texCoord0;
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
    Varyings UnpackVaryings(PackedVaryings input)
    {
        Varyings output;
        output.positionCS = input.positionCS;
        output.texCoord0 = input.interp0.xyzw;
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
float4 Texture2D_a54af73971754a788508672b9d7b8cc3_TexelSize;
float4 Texture2D_d59f074fadd14270974ce7a66ab4f51c_TexelSize;
float Vector1_1dbe385c06844617a395f8d425b37c2e;
float4 Color_022597fd83634c529dcce3fbfd9ed6e6;
float Vector1_e3eb3cafdee54e098df5ab84fc0392eb;
float Vector1_0a04d443530040849477bafffc25fcb2;
float Vector1_019cb3139e9c46d6b9592ac14eda99e9;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(Texture2D_a54af73971754a788508672b9d7b8cc3);
SAMPLER(samplerTexture2D_a54af73971754a788508672b9d7b8cc3);
TEXTURE2D(Texture2D_d59f074fadd14270974ce7a66ab4f51c);
SAMPLER(samplerTexture2D_d59f074fadd14270974ce7a66ab4f51c);

// Graph Functions

void Unity_Multiply_float(float A, float B, out float Out)
{
    Out = A * B;
}

void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
{
    Out = UV * Tiling + Offset;
}

void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
{
    Out = A * B;
}

void Unity_Add_float4(float4 A, float4 B, out float4 Out)
{
    Out = A + B;
}

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

// Graph Pixel
struct SurfaceDescription
{
    float3 BaseColor;
    float Alpha;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
    SurfaceDescription surface = (SurfaceDescription)0;
    UnityTexture2D _Property_3fffd5b434a0469d9b04635fcb63c6c3_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_a54af73971754a788508672b9d7b8cc3);
    float4 _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_RGBA_0 = SAMPLE_TEXTURE2D(_Property_3fffd5b434a0469d9b04635fcb63c6c3_Out_0.tex, _Property_3fffd5b434a0469d9b04635fcb63c6c3_Out_0.samplerstate, IN.uv0.xy);
    float _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_R_4 = _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_RGBA_0.r;
    float _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_G_5 = _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_RGBA_0.g;
    float _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_B_6 = _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_RGBA_0.b;
    float _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_A_7 = _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_RGBA_0.a;
    float4 _Property_6e2d60fe964f48d0918c8c18804f6477_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_022597fd83634c529dcce3fbfd9ed6e6) : Color_022597fd83634c529dcce3fbfd9ed6e6;
    UnityTexture2D _Property_b76c3d6b6dc046ce8403260f8921d15f_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_d59f074fadd14270974ce7a66ab4f51c);
    float _Property_6ef0b561508844acab00f3c2cbb24745_Out_0 = Vector1_0a04d443530040849477bafffc25fcb2;
    float _Property_58430ee132824964abcd98b69db3c5c8_Out_0 = Vector1_019cb3139e9c46d6b9592ac14eda99e9;
    float2 _Vector2_d7069df31bca44fa9f61582a6b7e92d3_Out_0 = float2(_Property_6ef0b561508844acab00f3c2cbb24745_Out_0, _Property_58430ee132824964abcd98b69db3c5c8_Out_0);
    float _Property_ccc69f470f044b5abd484d9618fc12fa_Out_0 = Vector1_1dbe385c06844617a395f8d425b37c2e;
    float _Multiply_6cd763b91e2142ffa812d9f105375846_Out_2;
    Unity_Multiply_float(IN.TimeParameters.x, _Property_ccc69f470f044b5abd484d9618fc12fa_Out_0, _Multiply_6cd763b91e2142ffa812d9f105375846_Out_2);
    float2 _Vector2_9f63ec4f1e35408584fd3ec08f6fa99d_Out_0 = float2(_Multiply_6cd763b91e2142ffa812d9f105375846_Out_2, _Multiply_6cd763b91e2142ffa812d9f105375846_Out_2);
    float2 _TilingAndOffset_44448e476b684fb694eca03e1641ee38_Out_3;
    Unity_TilingAndOffset_float(IN.uv0.xy, _Vector2_d7069df31bca44fa9f61582a6b7e92d3_Out_0, _Vector2_9f63ec4f1e35408584fd3ec08f6fa99d_Out_0, _TilingAndOffset_44448e476b684fb694eca03e1641ee38_Out_3);
    float4 _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_RGBA_0 = SAMPLE_TEXTURE2D(_Property_b76c3d6b6dc046ce8403260f8921d15f_Out_0.tex, _Property_b76c3d6b6dc046ce8403260f8921d15f_Out_0.samplerstate, _TilingAndOffset_44448e476b684fb694eca03e1641ee38_Out_3);
    float _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_R_4 = _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_RGBA_0.r;
    float _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_G_5 = _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_RGBA_0.g;
    float _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_B_6 = _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_RGBA_0.b;
    float _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_A_7 = _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_RGBA_0.a;
    float4 _Multiply_44cfb8519225432aa464b4b9077afcc3_Out_2;
    Unity_Multiply_float(_Property_6e2d60fe964f48d0918c8c18804f6477_Out_0, _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_RGBA_0, _Multiply_44cfb8519225432aa464b4b9077afcc3_Out_2);
    float4 _Add_0666426ce8274b45ae1f71663f7d5607_Out_2;
    Unity_Add_float4(_SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_RGBA_0, _Multiply_44cfb8519225432aa464b4b9077afcc3_Out_2, _Add_0666426ce8274b45ae1f71663f7d5607_Out_2);
    float _Property_0afad8be45e4452dbbce1b4758ba67a1_Out_0 = Vector1_e3eb3cafdee54e098df5ab84fc0392eb;
    float _Multiply_ddeb41d8f8b24e09b102459c31ef3147_Out_2;
    Unity_Multiply_float(_SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_A_7, _Property_0afad8be45e4452dbbce1b4758ba67a1_Out_0, _Multiply_ddeb41d8f8b24e09b102459c31ef3147_Out_2);
    float4 _Multiply_9e78d15eab2b46419f35da011a140a78_Out_2;
    Unity_Multiply_float((_SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_A_7.xxxx), _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_RGBA_0, _Multiply_9e78d15eab2b46419f35da011a140a78_Out_2);
    float4 _Add_70dd5dd1ffc144189e6b1b1d465e8302_Out_2;
    Unity_Add_float4((_Multiply_ddeb41d8f8b24e09b102459c31ef3147_Out_2.xxxx), _Multiply_9e78d15eab2b46419f35da011a140a78_Out_2, _Add_70dd5dd1ffc144189e6b1b1d465e8302_Out_2);
    float4 _Multiply_01f80994b30a4f47b069be7f1ad2f032_Out_2;
    Unity_Multiply_float((_SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_A_7.xxxx), _Add_70dd5dd1ffc144189e6b1b1d465e8302_Out_2, _Multiply_01f80994b30a4f47b069be7f1ad2f032_Out_2);
    surface.BaseColor = (_Add_0666426ce8274b45ae1f71663f7d5607_Out_2.xyz);
    surface.Alpha = (_Multiply_01f80994b30a4f47b069be7f1ad2f032_Out_2).x;
    return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
    VertexDescriptionInputs output;
    ZERO_INITIALIZE(VertexDescriptionInputs, output);

    output.ObjectSpaceNormal = input.normalOS;
    output.ObjectSpaceTangent = input.tangentOS.xyz;
    output.ObjectSpacePosition = input.positionOS;

    return output;
}
    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
    SurfaceDescriptionInputs output;
    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





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

    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"

    ENDHLSL
}
Pass
{
    Name "ShadowCaster"
    Tags
    {
        "LightMode" = "ShadowCaster"
    }

        // Render State
        Cull Back
    Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
    ZTest LEqual
    ZWrite On
    ColorMask 0

        // Debug
        // <None>

        // --------------------------------------------------
        // Pass

        HLSLPROGRAM

        // Pragmas
        #pragma target 4.5
    #pragma exclude_renderers gles gles3 glcore
    #pragma multi_compile_instancing
    #pragma multi_compile _ DOTS_INSTANCING_ON
    #pragma vertex vert
    #pragma fragment frag

        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>

        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>

        // Defines
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SHADOWCASTER
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

        // --------------------------------------------------
        // Structs and Packing

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
        float4 texCoord0;
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
        float4 interp0 : TEXCOORD0;
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

        PackedVaryings PackVaryings(Varyings input)
    {
        PackedVaryings output;
        output.positionCS = input.positionCS;
        output.interp0.xyzw = input.texCoord0;
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
    Varyings UnpackVaryings(PackedVaryings input)
    {
        Varyings output;
        output.positionCS = input.positionCS;
        output.texCoord0 = input.interp0.xyzw;
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
float4 Texture2D_a54af73971754a788508672b9d7b8cc3_TexelSize;
float4 Texture2D_d59f074fadd14270974ce7a66ab4f51c_TexelSize;
float Vector1_1dbe385c06844617a395f8d425b37c2e;
float4 Color_022597fd83634c529dcce3fbfd9ed6e6;
float Vector1_e3eb3cafdee54e098df5ab84fc0392eb;
float Vector1_0a04d443530040849477bafffc25fcb2;
float Vector1_019cb3139e9c46d6b9592ac14eda99e9;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(Texture2D_a54af73971754a788508672b9d7b8cc3);
SAMPLER(samplerTexture2D_a54af73971754a788508672b9d7b8cc3);
TEXTURE2D(Texture2D_d59f074fadd14270974ce7a66ab4f51c);
SAMPLER(samplerTexture2D_d59f074fadd14270974ce7a66ab4f51c);

// Graph Functions

void Unity_Multiply_float(float A, float B, out float Out)
{
    Out = A * B;
}

void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
{
    Out = UV * Tiling + Offset;
}

void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
{
    Out = A * B;
}

void Unity_Add_float4(float4 A, float4 B, out float4 Out)
{
    Out = A + B;
}

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

// Graph Pixel
struct SurfaceDescription
{
    float Alpha;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
    SurfaceDescription surface = (SurfaceDescription)0;
    UnityTexture2D _Property_3fffd5b434a0469d9b04635fcb63c6c3_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_a54af73971754a788508672b9d7b8cc3);
    float4 _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_RGBA_0 = SAMPLE_TEXTURE2D(_Property_3fffd5b434a0469d9b04635fcb63c6c3_Out_0.tex, _Property_3fffd5b434a0469d9b04635fcb63c6c3_Out_0.samplerstate, IN.uv0.xy);
    float _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_R_4 = _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_RGBA_0.r;
    float _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_G_5 = _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_RGBA_0.g;
    float _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_B_6 = _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_RGBA_0.b;
    float _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_A_7 = _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_RGBA_0.a;
    float _Property_0afad8be45e4452dbbce1b4758ba67a1_Out_0 = Vector1_e3eb3cafdee54e098df5ab84fc0392eb;
    float _Multiply_ddeb41d8f8b24e09b102459c31ef3147_Out_2;
    Unity_Multiply_float(_SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_A_7, _Property_0afad8be45e4452dbbce1b4758ba67a1_Out_0, _Multiply_ddeb41d8f8b24e09b102459c31ef3147_Out_2);
    UnityTexture2D _Property_b76c3d6b6dc046ce8403260f8921d15f_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_d59f074fadd14270974ce7a66ab4f51c);
    float _Property_6ef0b561508844acab00f3c2cbb24745_Out_0 = Vector1_0a04d443530040849477bafffc25fcb2;
    float _Property_58430ee132824964abcd98b69db3c5c8_Out_0 = Vector1_019cb3139e9c46d6b9592ac14eda99e9;
    float2 _Vector2_d7069df31bca44fa9f61582a6b7e92d3_Out_0 = float2(_Property_6ef0b561508844acab00f3c2cbb24745_Out_0, _Property_58430ee132824964abcd98b69db3c5c8_Out_0);
    float _Property_ccc69f470f044b5abd484d9618fc12fa_Out_0 = Vector1_1dbe385c06844617a395f8d425b37c2e;
    float _Multiply_6cd763b91e2142ffa812d9f105375846_Out_2;
    Unity_Multiply_float(IN.TimeParameters.x, _Property_ccc69f470f044b5abd484d9618fc12fa_Out_0, _Multiply_6cd763b91e2142ffa812d9f105375846_Out_2);
    float2 _Vector2_9f63ec4f1e35408584fd3ec08f6fa99d_Out_0 = float2(_Multiply_6cd763b91e2142ffa812d9f105375846_Out_2, _Multiply_6cd763b91e2142ffa812d9f105375846_Out_2);
    float2 _TilingAndOffset_44448e476b684fb694eca03e1641ee38_Out_3;
    Unity_TilingAndOffset_float(IN.uv0.xy, _Vector2_d7069df31bca44fa9f61582a6b7e92d3_Out_0, _Vector2_9f63ec4f1e35408584fd3ec08f6fa99d_Out_0, _TilingAndOffset_44448e476b684fb694eca03e1641ee38_Out_3);
    float4 _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_RGBA_0 = SAMPLE_TEXTURE2D(_Property_b76c3d6b6dc046ce8403260f8921d15f_Out_0.tex, _Property_b76c3d6b6dc046ce8403260f8921d15f_Out_0.samplerstate, _TilingAndOffset_44448e476b684fb694eca03e1641ee38_Out_3);
    float _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_R_4 = _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_RGBA_0.r;
    float _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_G_5 = _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_RGBA_0.g;
    float _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_B_6 = _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_RGBA_0.b;
    float _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_A_7 = _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_RGBA_0.a;
    float4 _Multiply_9e78d15eab2b46419f35da011a140a78_Out_2;
    Unity_Multiply_float((_SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_A_7.xxxx), _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_RGBA_0, _Multiply_9e78d15eab2b46419f35da011a140a78_Out_2);
    float4 _Add_70dd5dd1ffc144189e6b1b1d465e8302_Out_2;
    Unity_Add_float4((_Multiply_ddeb41d8f8b24e09b102459c31ef3147_Out_2.xxxx), _Multiply_9e78d15eab2b46419f35da011a140a78_Out_2, _Add_70dd5dd1ffc144189e6b1b1d465e8302_Out_2);
    float4 _Multiply_01f80994b30a4f47b069be7f1ad2f032_Out_2;
    Unity_Multiply_float((_SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_A_7.xxxx), _Add_70dd5dd1ffc144189e6b1b1d465e8302_Out_2, _Multiply_01f80994b30a4f47b069be7f1ad2f032_Out_2);
    surface.Alpha = (_Multiply_01f80994b30a4f47b069be7f1ad2f032_Out_2).x;
    return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
    VertexDescriptionInputs output;
    ZERO_INITIALIZE(VertexDescriptionInputs, output);

    output.ObjectSpaceNormal = input.normalOS;
    output.ObjectSpaceTangent = input.tangentOS.xyz;
    output.ObjectSpacePosition = input.positionOS;

    return output;
}
    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
    SurfaceDescriptionInputs output;
    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





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

    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"

    ENDHLSL
}
Pass
{
    Name "DepthOnly"
    Tags
    {
        "LightMode" = "DepthOnly"
    }

        // Render State
        Cull Back
    Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
    ZTest LEqual
    ZWrite On
    ColorMask 0

        // Debug
        // <None>

        // --------------------------------------------------
        // Pass

        HLSLPROGRAM

        // Pragmas
        #pragma target 4.5
    #pragma exclude_renderers gles gles3 glcore
    #pragma multi_compile_instancing
    #pragma multi_compile _ DOTS_INSTANCING_ON
    #pragma vertex vert
    #pragma fragment frag

        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>

        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>

        // Defines
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

        // --------------------------------------------------
        // Structs and Packing

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
        float4 texCoord0;
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
        float4 interp0 : TEXCOORD0;
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

        PackedVaryings PackVaryings(Varyings input)
    {
        PackedVaryings output;
        output.positionCS = input.positionCS;
        output.interp0.xyzw = input.texCoord0;
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
    Varyings UnpackVaryings(PackedVaryings input)
    {
        Varyings output;
        output.positionCS = input.positionCS;
        output.texCoord0 = input.interp0.xyzw;
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
float4 Texture2D_a54af73971754a788508672b9d7b8cc3_TexelSize;
float4 Texture2D_d59f074fadd14270974ce7a66ab4f51c_TexelSize;
float Vector1_1dbe385c06844617a395f8d425b37c2e;
float4 Color_022597fd83634c529dcce3fbfd9ed6e6;
float Vector1_e3eb3cafdee54e098df5ab84fc0392eb;
float Vector1_0a04d443530040849477bafffc25fcb2;
float Vector1_019cb3139e9c46d6b9592ac14eda99e9;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(Texture2D_a54af73971754a788508672b9d7b8cc3);
SAMPLER(samplerTexture2D_a54af73971754a788508672b9d7b8cc3);
TEXTURE2D(Texture2D_d59f074fadd14270974ce7a66ab4f51c);
SAMPLER(samplerTexture2D_d59f074fadd14270974ce7a66ab4f51c);

// Graph Functions

void Unity_Multiply_float(float A, float B, out float Out)
{
    Out = A * B;
}

void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
{
    Out = UV * Tiling + Offset;
}

void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
{
    Out = A * B;
}

void Unity_Add_float4(float4 A, float4 B, out float4 Out)
{
    Out = A + B;
}

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

// Graph Pixel
struct SurfaceDescription
{
    float Alpha;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
    SurfaceDescription surface = (SurfaceDescription)0;
    UnityTexture2D _Property_3fffd5b434a0469d9b04635fcb63c6c3_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_a54af73971754a788508672b9d7b8cc3);
    float4 _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_RGBA_0 = SAMPLE_TEXTURE2D(_Property_3fffd5b434a0469d9b04635fcb63c6c3_Out_0.tex, _Property_3fffd5b434a0469d9b04635fcb63c6c3_Out_0.samplerstate, IN.uv0.xy);
    float _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_R_4 = _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_RGBA_0.r;
    float _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_G_5 = _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_RGBA_0.g;
    float _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_B_6 = _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_RGBA_0.b;
    float _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_A_7 = _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_RGBA_0.a;
    float _Property_0afad8be45e4452dbbce1b4758ba67a1_Out_0 = Vector1_e3eb3cafdee54e098df5ab84fc0392eb;
    float _Multiply_ddeb41d8f8b24e09b102459c31ef3147_Out_2;
    Unity_Multiply_float(_SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_A_7, _Property_0afad8be45e4452dbbce1b4758ba67a1_Out_0, _Multiply_ddeb41d8f8b24e09b102459c31ef3147_Out_2);
    UnityTexture2D _Property_b76c3d6b6dc046ce8403260f8921d15f_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_d59f074fadd14270974ce7a66ab4f51c);
    float _Property_6ef0b561508844acab00f3c2cbb24745_Out_0 = Vector1_0a04d443530040849477bafffc25fcb2;
    float _Property_58430ee132824964abcd98b69db3c5c8_Out_0 = Vector1_019cb3139e9c46d6b9592ac14eda99e9;
    float2 _Vector2_d7069df31bca44fa9f61582a6b7e92d3_Out_0 = float2(_Property_6ef0b561508844acab00f3c2cbb24745_Out_0, _Property_58430ee132824964abcd98b69db3c5c8_Out_0);
    float _Property_ccc69f470f044b5abd484d9618fc12fa_Out_0 = Vector1_1dbe385c06844617a395f8d425b37c2e;
    float _Multiply_6cd763b91e2142ffa812d9f105375846_Out_2;
    Unity_Multiply_float(IN.TimeParameters.x, _Property_ccc69f470f044b5abd484d9618fc12fa_Out_0, _Multiply_6cd763b91e2142ffa812d9f105375846_Out_2);
    float2 _Vector2_9f63ec4f1e35408584fd3ec08f6fa99d_Out_0 = float2(_Multiply_6cd763b91e2142ffa812d9f105375846_Out_2, _Multiply_6cd763b91e2142ffa812d9f105375846_Out_2);
    float2 _TilingAndOffset_44448e476b684fb694eca03e1641ee38_Out_3;
    Unity_TilingAndOffset_float(IN.uv0.xy, _Vector2_d7069df31bca44fa9f61582a6b7e92d3_Out_0, _Vector2_9f63ec4f1e35408584fd3ec08f6fa99d_Out_0, _TilingAndOffset_44448e476b684fb694eca03e1641ee38_Out_3);
    float4 _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_RGBA_0 = SAMPLE_TEXTURE2D(_Property_b76c3d6b6dc046ce8403260f8921d15f_Out_0.tex, _Property_b76c3d6b6dc046ce8403260f8921d15f_Out_0.samplerstate, _TilingAndOffset_44448e476b684fb694eca03e1641ee38_Out_3);
    float _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_R_4 = _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_RGBA_0.r;
    float _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_G_5 = _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_RGBA_0.g;
    float _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_B_6 = _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_RGBA_0.b;
    float _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_A_7 = _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_RGBA_0.a;
    float4 _Multiply_9e78d15eab2b46419f35da011a140a78_Out_2;
    Unity_Multiply_float((_SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_A_7.xxxx), _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_RGBA_0, _Multiply_9e78d15eab2b46419f35da011a140a78_Out_2);
    float4 _Add_70dd5dd1ffc144189e6b1b1d465e8302_Out_2;
    Unity_Add_float4((_Multiply_ddeb41d8f8b24e09b102459c31ef3147_Out_2.xxxx), _Multiply_9e78d15eab2b46419f35da011a140a78_Out_2, _Add_70dd5dd1ffc144189e6b1b1d465e8302_Out_2);
    float4 _Multiply_01f80994b30a4f47b069be7f1ad2f032_Out_2;
    Unity_Multiply_float((_SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_A_7.xxxx), _Add_70dd5dd1ffc144189e6b1b1d465e8302_Out_2, _Multiply_01f80994b30a4f47b069be7f1ad2f032_Out_2);
    surface.Alpha = (_Multiply_01f80994b30a4f47b069be7f1ad2f032_Out_2).x;
    return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
    VertexDescriptionInputs output;
    ZERO_INITIALIZE(VertexDescriptionInputs, output);

    output.ObjectSpaceNormal = input.normalOS;
    output.ObjectSpaceTangent = input.tangentOS.xyz;
    output.ObjectSpacePosition = input.positionOS;

    return output;
}
    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
    SurfaceDescriptionInputs output;
    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





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

    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"

    ENDHLSL
}
    }
        SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue" = "Transparent"
        }
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
    #pragma vertex vert
    #pragma fragment frag

        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>

        // Keywords
        #pragma multi_compile _ LIGHTMAP_ON
    #pragma multi_compile _ DIRLIGHTMAP_COMBINED
    #pragma shader_feature _ _SAMPLE_GI
        // GraphKeywords: <None>

        // Defines
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_UNLIT
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

        // --------------------------------------------------
        // Structs and Packing

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
        float4 texCoord0;
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
        float4 interp0 : TEXCOORD0;
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

        PackedVaryings PackVaryings(Varyings input)
    {
        PackedVaryings output;
        output.positionCS = input.positionCS;
        output.interp0.xyzw = input.texCoord0;
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
    Varyings UnpackVaryings(PackedVaryings input)
    {
        Varyings output;
        output.positionCS = input.positionCS;
        output.texCoord0 = input.interp0.xyzw;
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
float4 Texture2D_a54af73971754a788508672b9d7b8cc3_TexelSize;
float4 Texture2D_d59f074fadd14270974ce7a66ab4f51c_TexelSize;
float Vector1_1dbe385c06844617a395f8d425b37c2e;
float4 Color_022597fd83634c529dcce3fbfd9ed6e6;
float Vector1_e3eb3cafdee54e098df5ab84fc0392eb;
float Vector1_0a04d443530040849477bafffc25fcb2;
float Vector1_019cb3139e9c46d6b9592ac14eda99e9;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(Texture2D_a54af73971754a788508672b9d7b8cc3);
SAMPLER(samplerTexture2D_a54af73971754a788508672b9d7b8cc3);
TEXTURE2D(Texture2D_d59f074fadd14270974ce7a66ab4f51c);
SAMPLER(samplerTexture2D_d59f074fadd14270974ce7a66ab4f51c);

// Graph Functions

void Unity_Multiply_float(float A, float B, out float Out)
{
    Out = A * B;
}

void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
{
    Out = UV * Tiling + Offset;
}

void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
{
    Out = A * B;
}

void Unity_Add_float4(float4 A, float4 B, out float4 Out)
{
    Out = A + B;
}

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

// Graph Pixel
struct SurfaceDescription
{
    float3 BaseColor;
    float Alpha;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
    SurfaceDescription surface = (SurfaceDescription)0;
    UnityTexture2D _Property_3fffd5b434a0469d9b04635fcb63c6c3_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_a54af73971754a788508672b9d7b8cc3);
    float4 _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_RGBA_0 = SAMPLE_TEXTURE2D(_Property_3fffd5b434a0469d9b04635fcb63c6c3_Out_0.tex, _Property_3fffd5b434a0469d9b04635fcb63c6c3_Out_0.samplerstate, IN.uv0.xy);
    float _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_R_4 = _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_RGBA_0.r;
    float _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_G_5 = _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_RGBA_0.g;
    float _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_B_6 = _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_RGBA_0.b;
    float _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_A_7 = _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_RGBA_0.a;
    float4 _Property_6e2d60fe964f48d0918c8c18804f6477_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_022597fd83634c529dcce3fbfd9ed6e6) : Color_022597fd83634c529dcce3fbfd9ed6e6;
    UnityTexture2D _Property_b76c3d6b6dc046ce8403260f8921d15f_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_d59f074fadd14270974ce7a66ab4f51c);
    float _Property_6ef0b561508844acab00f3c2cbb24745_Out_0 = Vector1_0a04d443530040849477bafffc25fcb2;
    float _Property_58430ee132824964abcd98b69db3c5c8_Out_0 = Vector1_019cb3139e9c46d6b9592ac14eda99e9;
    float2 _Vector2_d7069df31bca44fa9f61582a6b7e92d3_Out_0 = float2(_Property_6ef0b561508844acab00f3c2cbb24745_Out_0, _Property_58430ee132824964abcd98b69db3c5c8_Out_0);
    float _Property_ccc69f470f044b5abd484d9618fc12fa_Out_0 = Vector1_1dbe385c06844617a395f8d425b37c2e;
    float _Multiply_6cd763b91e2142ffa812d9f105375846_Out_2;
    Unity_Multiply_float(IN.TimeParameters.x, _Property_ccc69f470f044b5abd484d9618fc12fa_Out_0, _Multiply_6cd763b91e2142ffa812d9f105375846_Out_2);
    float2 _Vector2_9f63ec4f1e35408584fd3ec08f6fa99d_Out_0 = float2(_Multiply_6cd763b91e2142ffa812d9f105375846_Out_2, _Multiply_6cd763b91e2142ffa812d9f105375846_Out_2);
    float2 _TilingAndOffset_44448e476b684fb694eca03e1641ee38_Out_3;
    Unity_TilingAndOffset_float(IN.uv0.xy, _Vector2_d7069df31bca44fa9f61582a6b7e92d3_Out_0, _Vector2_9f63ec4f1e35408584fd3ec08f6fa99d_Out_0, _TilingAndOffset_44448e476b684fb694eca03e1641ee38_Out_3);
    float4 _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_RGBA_0 = SAMPLE_TEXTURE2D(_Property_b76c3d6b6dc046ce8403260f8921d15f_Out_0.tex, _Property_b76c3d6b6dc046ce8403260f8921d15f_Out_0.samplerstate, _TilingAndOffset_44448e476b684fb694eca03e1641ee38_Out_3);
    float _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_R_4 = _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_RGBA_0.r;
    float _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_G_5 = _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_RGBA_0.g;
    float _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_B_6 = _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_RGBA_0.b;
    float _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_A_7 = _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_RGBA_0.a;
    float4 _Multiply_44cfb8519225432aa464b4b9077afcc3_Out_2;
    Unity_Multiply_float(_Property_6e2d60fe964f48d0918c8c18804f6477_Out_0, _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_RGBA_0, _Multiply_44cfb8519225432aa464b4b9077afcc3_Out_2);
    float4 _Add_0666426ce8274b45ae1f71663f7d5607_Out_2;
    Unity_Add_float4(_SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_RGBA_0, _Multiply_44cfb8519225432aa464b4b9077afcc3_Out_2, _Add_0666426ce8274b45ae1f71663f7d5607_Out_2);
    float _Property_0afad8be45e4452dbbce1b4758ba67a1_Out_0 = Vector1_e3eb3cafdee54e098df5ab84fc0392eb;
    float _Multiply_ddeb41d8f8b24e09b102459c31ef3147_Out_2;
    Unity_Multiply_float(_SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_A_7, _Property_0afad8be45e4452dbbce1b4758ba67a1_Out_0, _Multiply_ddeb41d8f8b24e09b102459c31ef3147_Out_2);
    float4 _Multiply_9e78d15eab2b46419f35da011a140a78_Out_2;
    Unity_Multiply_float((_SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_A_7.xxxx), _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_RGBA_0, _Multiply_9e78d15eab2b46419f35da011a140a78_Out_2);
    float4 _Add_70dd5dd1ffc144189e6b1b1d465e8302_Out_2;
    Unity_Add_float4((_Multiply_ddeb41d8f8b24e09b102459c31ef3147_Out_2.xxxx), _Multiply_9e78d15eab2b46419f35da011a140a78_Out_2, _Add_70dd5dd1ffc144189e6b1b1d465e8302_Out_2);
    float4 _Multiply_01f80994b30a4f47b069be7f1ad2f032_Out_2;
    Unity_Multiply_float((_SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_A_7.xxxx), _Add_70dd5dd1ffc144189e6b1b1d465e8302_Out_2, _Multiply_01f80994b30a4f47b069be7f1ad2f032_Out_2);
    surface.BaseColor = (_Add_0666426ce8274b45ae1f71663f7d5607_Out_2.xyz);
    surface.Alpha = (_Multiply_01f80994b30a4f47b069be7f1ad2f032_Out_2).x;
    return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
    VertexDescriptionInputs output;
    ZERO_INITIALIZE(VertexDescriptionInputs, output);

    output.ObjectSpaceNormal = input.normalOS;
    output.ObjectSpaceTangent = input.tangentOS.xyz;
    output.ObjectSpacePosition = input.positionOS;

    return output;
}
    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
    SurfaceDescriptionInputs output;
    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





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

    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"

    ENDHLSL
}
Pass
{
    Name "ShadowCaster"
    Tags
    {
        "LightMode" = "ShadowCaster"
    }

        // Render State
        Cull Back
    Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
    ZTest LEqual
    ZWrite On
    ColorMask 0

        // Debug
        // <None>

        // --------------------------------------------------
        // Pass

        HLSLPROGRAM

        // Pragmas
        #pragma target 2.0
    #pragma only_renderers gles gles3 glcore d3d11
    #pragma multi_compile_instancing
    #pragma vertex vert
    #pragma fragment frag

        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>

        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>

        // Defines
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SHADOWCASTER
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

        // --------------------------------------------------
        // Structs and Packing

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
        float4 texCoord0;
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
        float4 interp0 : TEXCOORD0;
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

        PackedVaryings PackVaryings(Varyings input)
    {
        PackedVaryings output;
        output.positionCS = input.positionCS;
        output.interp0.xyzw = input.texCoord0;
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
    Varyings UnpackVaryings(PackedVaryings input)
    {
        Varyings output;
        output.positionCS = input.positionCS;
        output.texCoord0 = input.interp0.xyzw;
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
float4 Texture2D_a54af73971754a788508672b9d7b8cc3_TexelSize;
float4 Texture2D_d59f074fadd14270974ce7a66ab4f51c_TexelSize;
float Vector1_1dbe385c06844617a395f8d425b37c2e;
float4 Color_022597fd83634c529dcce3fbfd9ed6e6;
float Vector1_e3eb3cafdee54e098df5ab84fc0392eb;
float Vector1_0a04d443530040849477bafffc25fcb2;
float Vector1_019cb3139e9c46d6b9592ac14eda99e9;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(Texture2D_a54af73971754a788508672b9d7b8cc3);
SAMPLER(samplerTexture2D_a54af73971754a788508672b9d7b8cc3);
TEXTURE2D(Texture2D_d59f074fadd14270974ce7a66ab4f51c);
SAMPLER(samplerTexture2D_d59f074fadd14270974ce7a66ab4f51c);

// Graph Functions

void Unity_Multiply_float(float A, float B, out float Out)
{
    Out = A * B;
}

void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
{
    Out = UV * Tiling + Offset;
}

void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
{
    Out = A * B;
}

void Unity_Add_float4(float4 A, float4 B, out float4 Out)
{
    Out = A + B;
}

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

// Graph Pixel
struct SurfaceDescription
{
    float Alpha;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
    SurfaceDescription surface = (SurfaceDescription)0;
    UnityTexture2D _Property_3fffd5b434a0469d9b04635fcb63c6c3_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_a54af73971754a788508672b9d7b8cc3);
    float4 _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_RGBA_0 = SAMPLE_TEXTURE2D(_Property_3fffd5b434a0469d9b04635fcb63c6c3_Out_0.tex, _Property_3fffd5b434a0469d9b04635fcb63c6c3_Out_0.samplerstate, IN.uv0.xy);
    float _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_R_4 = _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_RGBA_0.r;
    float _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_G_5 = _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_RGBA_0.g;
    float _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_B_6 = _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_RGBA_0.b;
    float _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_A_7 = _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_RGBA_0.a;
    float _Property_0afad8be45e4452dbbce1b4758ba67a1_Out_0 = Vector1_e3eb3cafdee54e098df5ab84fc0392eb;
    float _Multiply_ddeb41d8f8b24e09b102459c31ef3147_Out_2;
    Unity_Multiply_float(_SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_A_7, _Property_0afad8be45e4452dbbce1b4758ba67a1_Out_0, _Multiply_ddeb41d8f8b24e09b102459c31ef3147_Out_2);
    UnityTexture2D _Property_b76c3d6b6dc046ce8403260f8921d15f_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_d59f074fadd14270974ce7a66ab4f51c);
    float _Property_6ef0b561508844acab00f3c2cbb24745_Out_0 = Vector1_0a04d443530040849477bafffc25fcb2;
    float _Property_58430ee132824964abcd98b69db3c5c8_Out_0 = Vector1_019cb3139e9c46d6b9592ac14eda99e9;
    float2 _Vector2_d7069df31bca44fa9f61582a6b7e92d3_Out_0 = float2(_Property_6ef0b561508844acab00f3c2cbb24745_Out_0, _Property_58430ee132824964abcd98b69db3c5c8_Out_0);
    float _Property_ccc69f470f044b5abd484d9618fc12fa_Out_0 = Vector1_1dbe385c06844617a395f8d425b37c2e;
    float _Multiply_6cd763b91e2142ffa812d9f105375846_Out_2;
    Unity_Multiply_float(IN.TimeParameters.x, _Property_ccc69f470f044b5abd484d9618fc12fa_Out_0, _Multiply_6cd763b91e2142ffa812d9f105375846_Out_2);
    float2 _Vector2_9f63ec4f1e35408584fd3ec08f6fa99d_Out_0 = float2(_Multiply_6cd763b91e2142ffa812d9f105375846_Out_2, _Multiply_6cd763b91e2142ffa812d9f105375846_Out_2);
    float2 _TilingAndOffset_44448e476b684fb694eca03e1641ee38_Out_3;
    Unity_TilingAndOffset_float(IN.uv0.xy, _Vector2_d7069df31bca44fa9f61582a6b7e92d3_Out_0, _Vector2_9f63ec4f1e35408584fd3ec08f6fa99d_Out_0, _TilingAndOffset_44448e476b684fb694eca03e1641ee38_Out_3);
    float4 _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_RGBA_0 = SAMPLE_TEXTURE2D(_Property_b76c3d6b6dc046ce8403260f8921d15f_Out_0.tex, _Property_b76c3d6b6dc046ce8403260f8921d15f_Out_0.samplerstate, _TilingAndOffset_44448e476b684fb694eca03e1641ee38_Out_3);
    float _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_R_4 = _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_RGBA_0.r;
    float _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_G_5 = _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_RGBA_0.g;
    float _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_B_6 = _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_RGBA_0.b;
    float _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_A_7 = _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_RGBA_0.a;
    float4 _Multiply_9e78d15eab2b46419f35da011a140a78_Out_2;
    Unity_Multiply_float((_SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_A_7.xxxx), _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_RGBA_0, _Multiply_9e78d15eab2b46419f35da011a140a78_Out_2);
    float4 _Add_70dd5dd1ffc144189e6b1b1d465e8302_Out_2;
    Unity_Add_float4((_Multiply_ddeb41d8f8b24e09b102459c31ef3147_Out_2.xxxx), _Multiply_9e78d15eab2b46419f35da011a140a78_Out_2, _Add_70dd5dd1ffc144189e6b1b1d465e8302_Out_2);
    float4 _Multiply_01f80994b30a4f47b069be7f1ad2f032_Out_2;
    Unity_Multiply_float((_SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_A_7.xxxx), _Add_70dd5dd1ffc144189e6b1b1d465e8302_Out_2, _Multiply_01f80994b30a4f47b069be7f1ad2f032_Out_2);
    surface.Alpha = (_Multiply_01f80994b30a4f47b069be7f1ad2f032_Out_2).x;
    return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
    VertexDescriptionInputs output;
    ZERO_INITIALIZE(VertexDescriptionInputs, output);

    output.ObjectSpaceNormal = input.normalOS;
    output.ObjectSpaceTangent = input.tangentOS.xyz;
    output.ObjectSpacePosition = input.positionOS;

    return output;
}
    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
    SurfaceDescriptionInputs output;
    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





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

    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"

    ENDHLSL
}
Pass
{
    Name "DepthOnly"
    Tags
    {
        "LightMode" = "DepthOnly"
    }

        // Render State
        Cull Back
    Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
    ZTest LEqual
    ZWrite On
    ColorMask 0

        // Debug
        // <None>

        // --------------------------------------------------
        // Pass

        HLSLPROGRAM

        // Pragmas
        #pragma target 2.0
    #pragma only_renderers gles gles3 glcore d3d11
    #pragma multi_compile_instancing
    #pragma vertex vert
    #pragma fragment frag

        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>

        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>

        // Defines
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

        // --------------------------------------------------
        // Structs and Packing

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
        float4 texCoord0;
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
        float4 interp0 : TEXCOORD0;
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

        PackedVaryings PackVaryings(Varyings input)
    {
        PackedVaryings output;
        output.positionCS = input.positionCS;
        output.interp0.xyzw = input.texCoord0;
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
    Varyings UnpackVaryings(PackedVaryings input)
    {
        Varyings output;
        output.positionCS = input.positionCS;
        output.texCoord0 = input.interp0.xyzw;
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
float4 Texture2D_a54af73971754a788508672b9d7b8cc3_TexelSize;
float4 Texture2D_d59f074fadd14270974ce7a66ab4f51c_TexelSize;
float Vector1_1dbe385c06844617a395f8d425b37c2e;
float4 Color_022597fd83634c529dcce3fbfd9ed6e6;
float Vector1_e3eb3cafdee54e098df5ab84fc0392eb;
float Vector1_0a04d443530040849477bafffc25fcb2;
float Vector1_019cb3139e9c46d6b9592ac14eda99e9;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(Texture2D_a54af73971754a788508672b9d7b8cc3);
SAMPLER(samplerTexture2D_a54af73971754a788508672b9d7b8cc3);
TEXTURE2D(Texture2D_d59f074fadd14270974ce7a66ab4f51c);
SAMPLER(samplerTexture2D_d59f074fadd14270974ce7a66ab4f51c);

// Graph Functions

void Unity_Multiply_float(float A, float B, out float Out)
{
    Out = A * B;
}

void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
{
    Out = UV * Tiling + Offset;
}

void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
{
    Out = A * B;
}

void Unity_Add_float4(float4 A, float4 B, out float4 Out)
{
    Out = A + B;
}

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

// Graph Pixel
struct SurfaceDescription
{
    float Alpha;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
    SurfaceDescription surface = (SurfaceDescription)0;
    UnityTexture2D _Property_3fffd5b434a0469d9b04635fcb63c6c3_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_a54af73971754a788508672b9d7b8cc3);
    float4 _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_RGBA_0 = SAMPLE_TEXTURE2D(_Property_3fffd5b434a0469d9b04635fcb63c6c3_Out_0.tex, _Property_3fffd5b434a0469d9b04635fcb63c6c3_Out_0.samplerstate, IN.uv0.xy);
    float _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_R_4 = _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_RGBA_0.r;
    float _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_G_5 = _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_RGBA_0.g;
    float _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_B_6 = _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_RGBA_0.b;
    float _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_A_7 = _SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_RGBA_0.a;
    float _Property_0afad8be45e4452dbbce1b4758ba67a1_Out_0 = Vector1_e3eb3cafdee54e098df5ab84fc0392eb;
    float _Multiply_ddeb41d8f8b24e09b102459c31ef3147_Out_2;
    Unity_Multiply_float(_SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_A_7, _Property_0afad8be45e4452dbbce1b4758ba67a1_Out_0, _Multiply_ddeb41d8f8b24e09b102459c31ef3147_Out_2);
    UnityTexture2D _Property_b76c3d6b6dc046ce8403260f8921d15f_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_d59f074fadd14270974ce7a66ab4f51c);
    float _Property_6ef0b561508844acab00f3c2cbb24745_Out_0 = Vector1_0a04d443530040849477bafffc25fcb2;
    float _Property_58430ee132824964abcd98b69db3c5c8_Out_0 = Vector1_019cb3139e9c46d6b9592ac14eda99e9;
    float2 _Vector2_d7069df31bca44fa9f61582a6b7e92d3_Out_0 = float2(_Property_6ef0b561508844acab00f3c2cbb24745_Out_0, _Property_58430ee132824964abcd98b69db3c5c8_Out_0);
    float _Property_ccc69f470f044b5abd484d9618fc12fa_Out_0 = Vector1_1dbe385c06844617a395f8d425b37c2e;
    float _Multiply_6cd763b91e2142ffa812d9f105375846_Out_2;
    Unity_Multiply_float(IN.TimeParameters.x, _Property_ccc69f470f044b5abd484d9618fc12fa_Out_0, _Multiply_6cd763b91e2142ffa812d9f105375846_Out_2);
    float2 _Vector2_9f63ec4f1e35408584fd3ec08f6fa99d_Out_0 = float2(_Multiply_6cd763b91e2142ffa812d9f105375846_Out_2, _Multiply_6cd763b91e2142ffa812d9f105375846_Out_2);
    float2 _TilingAndOffset_44448e476b684fb694eca03e1641ee38_Out_3;
    Unity_TilingAndOffset_float(IN.uv0.xy, _Vector2_d7069df31bca44fa9f61582a6b7e92d3_Out_0, _Vector2_9f63ec4f1e35408584fd3ec08f6fa99d_Out_0, _TilingAndOffset_44448e476b684fb694eca03e1641ee38_Out_3);
    float4 _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_RGBA_0 = SAMPLE_TEXTURE2D(_Property_b76c3d6b6dc046ce8403260f8921d15f_Out_0.tex, _Property_b76c3d6b6dc046ce8403260f8921d15f_Out_0.samplerstate, _TilingAndOffset_44448e476b684fb694eca03e1641ee38_Out_3);
    float _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_R_4 = _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_RGBA_0.r;
    float _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_G_5 = _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_RGBA_0.g;
    float _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_B_6 = _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_RGBA_0.b;
    float _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_A_7 = _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_RGBA_0.a;
    float4 _Multiply_9e78d15eab2b46419f35da011a140a78_Out_2;
    Unity_Multiply_float((_SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_A_7.xxxx), _SampleTexture2D_9afe0c583bbd438a8d5e706974f1a35e_RGBA_0, _Multiply_9e78d15eab2b46419f35da011a140a78_Out_2);
    float4 _Add_70dd5dd1ffc144189e6b1b1d465e8302_Out_2;
    Unity_Add_float4((_Multiply_ddeb41d8f8b24e09b102459c31ef3147_Out_2.xxxx), _Multiply_9e78d15eab2b46419f35da011a140a78_Out_2, _Add_70dd5dd1ffc144189e6b1b1d465e8302_Out_2);
    float4 _Multiply_01f80994b30a4f47b069be7f1ad2f032_Out_2;
    Unity_Multiply_float((_SampleTexture2D_21b25c6e25ad4de0bb3b7396048345b8_A_7.xxxx), _Add_70dd5dd1ffc144189e6b1b1d465e8302_Out_2, _Multiply_01f80994b30a4f47b069be7f1ad2f032_Out_2);
    surface.Alpha = (_Multiply_01f80994b30a4f47b069be7f1ad2f032_Out_2).x;
    return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
    VertexDescriptionInputs output;
    ZERO_INITIALIZE(VertexDescriptionInputs, output);

    output.ObjectSpaceNormal = input.normalOS;
    output.ObjectSpaceTangent = input.tangentOS.xyz;
    output.ObjectSpacePosition = input.positionOS;

    return output;
}
    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
    SurfaceDescriptionInputs output;
    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





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

    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"

    ENDHLSL
}
    }
        FallBack "Hidden/Shader Graph/FallbackError"
}