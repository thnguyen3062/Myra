Shader "DiamondFlareShader-Speed"
{
    Properties
    {
        [HDR] Color_8b692a2d7297452f92de4622ed9fea60("Color", Color) = (1, 0.5269342, 0.2688679, 1)
        [NoScaleOffset]Texture2D_08500469d2944895a9ad392692f50c3c("Maintextures", 2D) = "white" {}
        [NoScaleOffset]Texture2D_17ff7d92927445598a03023feb129259("FrameMask", 2D) = "white" {}
        [NoScaleOffset]Texture2D_479d5e414ef0402b8a4a2d69cd8a2a56("FlareMask", 2D) = "white" {}
        Vector1_339d0dcbffa04ecd8653a283be69f22e("TotalDuration", Float) = 1
        Vector1_e98f7c484ee14bdcb59d8501640fde5f("Scale", Float) = 0.18
        Vector1_cee448f6c04d417abbe0f33409cee904("Rotation", Float) = -0.8
        Vector1_cd3dc268c340407ab74aaa2f027e5458("Speed", Float) = 0.3
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
            Name "Sprite Unlit"
            Tags
            {
                "LightMode" = "Universal2D"
            }

        // Render State
        Cull Off
    Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
     ZTest[unity_GUIZTestMode]
    ZWrite Off

        // Debug
        // <None>

        // --------------------------------------------------
        // Pass

        HLSLPROGRAM

        // Pragmas
        #pragma target 2.0
    #pragma exclude_renderers d3d11_9x
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
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SPRITEUNLIT
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
        float4 color : COLOR;
        #if UNITY_ANY_INSTANCING_ENABLED
        uint instanceID : INSTANCEID_SEMANTIC;
        #endif
    };
    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float4 texCoord0;
        float4 color;
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
        float4 interp1 : TEXCOORD1;
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
        output.interp1.xyzw = input.color;
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
        output.color = input.interp1.xyzw;
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
float4 Color_8b692a2d7297452f92de4622ed9fea60;
float4 Texture2D_08500469d2944895a9ad392692f50c3c_TexelSize;
float4 Texture2D_17ff7d92927445598a03023feb129259_TexelSize;
float4 Texture2D_479d5e414ef0402b8a4a2d69cd8a2a56_TexelSize;
float Vector1_339d0dcbffa04ecd8653a283be69f22e;
float Vector1_e98f7c484ee14bdcb59d8501640fde5f;
float Vector1_cee448f6c04d417abbe0f33409cee904;
float Vector1_cd3dc268c340407ab74aaa2f027e5458;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(Texture2D_08500469d2944895a9ad392692f50c3c);
SAMPLER(samplerTexture2D_08500469d2944895a9ad392692f50c3c);
TEXTURE2D(Texture2D_17ff7d92927445598a03023feb129259);
SAMPLER(samplerTexture2D_17ff7d92927445598a03023feb129259);
TEXTURE2D(Texture2D_479d5e414ef0402b8a4a2d69cd8a2a56);
SAMPLER(samplerTexture2D_479d5e414ef0402b8a4a2d69cd8a2a56);

// Graph Functions

void Unity_Rotate_Radians_float(float2 UV, float2 Center, float Rotation, out float2 Out)
{
    //rotation matrix
    UV -= Center;
    float s = sin(Rotation);
    float c = cos(Rotation);

    //center rotation matrix
    float2x2 rMatrix = float2x2(c, -s, s, c);
    rMatrix *= 0.5;
    rMatrix += 0.5;
    rMatrix = rMatrix * 2 - 1;

    //multiply the UVs by the rotation matrix
    UV.xy = mul(UV.xy, rMatrix);
    UV += Center;

    Out = UV;
}

void Unity_Multiply_float(float A, float B, out float Out)
{
    Out = A * B;
}

void Unity_Modulo_float(float A, float B, out float Out)
{
    Out = fmod(A, B);
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
    float4 _Property_57d4cd620c464caf993376abfe22463d_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_8b692a2d7297452f92de4622ed9fea60) : Color_8b692a2d7297452f92de4622ed9fea60;
    UnityTexture2D _Property_035e313c8eeb49f5a3f93a86f40b58a1_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_479d5e414ef0402b8a4a2d69cd8a2a56);
    float _Property_e06d4a068750422e95329b2115b4e487_Out_0 = Vector1_cee448f6c04d417abbe0f33409cee904;
    float2 _Rotate_d22b178f14eb4439a613eed789a2ea8f_Out_3;
    Unity_Rotate_Radians_float(IN.uv0.xy, float2 (0.5, 0.5), _Property_e06d4a068750422e95329b2115b4e487_Out_0, _Rotate_d22b178f14eb4439a613eed789a2ea8f_Out_3);
    float _Property_5f4fca2fa3594b39a69d8e017b32ea9d_Out_0 = Vector1_e98f7c484ee14bdcb59d8501640fde5f;
    float _Property_ad018d4a3ff44d9db5cb762325c2f92f_Out_0 = Vector1_cd3dc268c340407ab74aaa2f027e5458;
    float _Multiply_209d76dc11ed469fb68c0469e06a4238_Out_2;
    Unity_Multiply_float(IN.TimeParameters.x, _Property_ad018d4a3ff44d9db5cb762325c2f92f_Out_0, _Multiply_209d76dc11ed469fb68c0469e06a4238_Out_2);
    float _Property_fe8d40899cfb4ef6bca5c32400810134_Out_0 = Vector1_339d0dcbffa04ecd8653a283be69f22e;
    float _Modulo_cb8f697eaa8a4d949f4a25c1aa384cd4_Out_2;
    Unity_Modulo_float(_Multiply_209d76dc11ed469fb68c0469e06a4238_Out_2, _Property_fe8d40899cfb4ef6bca5c32400810134_Out_0, _Modulo_cb8f697eaa8a4d949f4a25c1aa384cd4_Out_2);
    float2 _Vector2_9084c2f109e44b8e8f1772d9744fa3dd_Out_0 = float2(0, _Modulo_cb8f697eaa8a4d949f4a25c1aa384cd4_Out_2);
    float2 _TilingAndOffset_7cb6f25ee828412cb1d0917256e7d8ac_Out_3;
    Unity_TilingAndOffset_float(_Rotate_d22b178f14eb4439a613eed789a2ea8f_Out_3, (_Property_5f4fca2fa3594b39a69d8e017b32ea9d_Out_0.xx), _Vector2_9084c2f109e44b8e8f1772d9744fa3dd_Out_0, _TilingAndOffset_7cb6f25ee828412cb1d0917256e7d8ac_Out_3);
    float4 _SampleTexture2D_c3b4382175ac490392c97697f29b2627_RGBA_0 = SAMPLE_TEXTURE2D(_Property_035e313c8eeb49f5a3f93a86f40b58a1_Out_0.tex, _Property_035e313c8eeb49f5a3f93a86f40b58a1_Out_0.samplerstate, _TilingAndOffset_7cb6f25ee828412cb1d0917256e7d8ac_Out_3);
    float _SampleTexture2D_c3b4382175ac490392c97697f29b2627_R_4 = _SampleTexture2D_c3b4382175ac490392c97697f29b2627_RGBA_0.r;
    float _SampleTexture2D_c3b4382175ac490392c97697f29b2627_G_5 = _SampleTexture2D_c3b4382175ac490392c97697f29b2627_RGBA_0.g;
    float _SampleTexture2D_c3b4382175ac490392c97697f29b2627_B_6 = _SampleTexture2D_c3b4382175ac490392c97697f29b2627_RGBA_0.b;
    float _SampleTexture2D_c3b4382175ac490392c97697f29b2627_A_7 = _SampleTexture2D_c3b4382175ac490392c97697f29b2627_RGBA_0.a;
    float4 _Multiply_8c6db7d42d7b43a493803e230569274a_Out_2;
    Unity_Multiply_float(_SampleTexture2D_c3b4382175ac490392c97697f29b2627_RGBA_0, (_SampleTexture2D_c3b4382175ac490392c97697f29b2627_A_7.xxxx), _Multiply_8c6db7d42d7b43a493803e230569274a_Out_2);
    float4 _Multiply_bf3d22acdd304a0fa3dd305ba02bc4a4_Out_2;
    Unity_Multiply_float(_Property_57d4cd620c464caf993376abfe22463d_Out_0, _Multiply_8c6db7d42d7b43a493803e230569274a_Out_2, _Multiply_bf3d22acdd304a0fa3dd305ba02bc4a4_Out_2);
    UnityTexture2D _Property_a39c54d7155b47369bfeabe86c554565_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_17ff7d92927445598a03023feb129259);
    float4 _SampleTexture2D_4f39e30c146a41c6b34594e6d8188856_RGBA_0 = SAMPLE_TEXTURE2D(_Property_a39c54d7155b47369bfeabe86c554565_Out_0.tex, _Property_a39c54d7155b47369bfeabe86c554565_Out_0.samplerstate, IN.uv0.xy);
    float _SampleTexture2D_4f39e30c146a41c6b34594e6d8188856_R_4 = _SampleTexture2D_4f39e30c146a41c6b34594e6d8188856_RGBA_0.r;
    float _SampleTexture2D_4f39e30c146a41c6b34594e6d8188856_G_5 = _SampleTexture2D_4f39e30c146a41c6b34594e6d8188856_RGBA_0.g;
    float _SampleTexture2D_4f39e30c146a41c6b34594e6d8188856_B_6 = _SampleTexture2D_4f39e30c146a41c6b34594e6d8188856_RGBA_0.b;
    float _SampleTexture2D_4f39e30c146a41c6b34594e6d8188856_A_7 = _SampleTexture2D_4f39e30c146a41c6b34594e6d8188856_RGBA_0.a;
    float4 _Multiply_08bec010d46e401da1694722db7aea32_Out_2;
    Unity_Multiply_float(_Multiply_bf3d22acdd304a0fa3dd305ba02bc4a4_Out_2, _SampleTexture2D_4f39e30c146a41c6b34594e6d8188856_RGBA_0, _Multiply_08bec010d46e401da1694722db7aea32_Out_2);
    UnityTexture2D _Property_dc87be653d124345b18016b175cf92cc_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_08500469d2944895a9ad392692f50c3c);
    float4 _SampleTexture2D_788f1328664a4466b2c14f1239388def_RGBA_0 = SAMPLE_TEXTURE2D(_Property_dc87be653d124345b18016b175cf92cc_Out_0.tex, _Property_dc87be653d124345b18016b175cf92cc_Out_0.samplerstate, IN.uv0.xy);
    float _SampleTexture2D_788f1328664a4466b2c14f1239388def_R_4 = _SampleTexture2D_788f1328664a4466b2c14f1239388def_RGBA_0.r;
    float _SampleTexture2D_788f1328664a4466b2c14f1239388def_G_5 = _SampleTexture2D_788f1328664a4466b2c14f1239388def_RGBA_0.g;
    float _SampleTexture2D_788f1328664a4466b2c14f1239388def_B_6 = _SampleTexture2D_788f1328664a4466b2c14f1239388def_RGBA_0.b;
    float _SampleTexture2D_788f1328664a4466b2c14f1239388def_A_7 = _SampleTexture2D_788f1328664a4466b2c14f1239388def_RGBA_0.a;
    float4 _Multiply_f65bbf6422c944ffb65b5a454044ff97_Out_2;
    Unity_Multiply_float(_SampleTexture2D_788f1328664a4466b2c14f1239388def_RGBA_0, (_SampleTexture2D_788f1328664a4466b2c14f1239388def_A_7.xxxx), _Multiply_f65bbf6422c944ffb65b5a454044ff97_Out_2);
    float4 _Add_a53bbeea75be437ab4a0c20a880c049b_Out_2;
    Unity_Add_float4(_Multiply_08bec010d46e401da1694722db7aea32_Out_2, _Multiply_f65bbf6422c944ffb65b5a454044ff97_Out_2, _Add_a53bbeea75be437ab4a0c20a880c049b_Out_2);
    float4 _Multiply_10e85da68f1d474dbbdfee599635d649_Out_2;
    Unity_Multiply_float(_Multiply_8c6db7d42d7b43a493803e230569274a_Out_2, (_SampleTexture2D_4f39e30c146a41c6b34594e6d8188856_A_7.xxxx), _Multiply_10e85da68f1d474dbbdfee599635d649_Out_2);
    float4 _Add_f235db20fc6d4193802744971aee1743_Out_2;
    Unity_Add_float4(_Multiply_10e85da68f1d474dbbdfee599635d649_Out_2, (_SampleTexture2D_788f1328664a4466b2c14f1239388def_A_7.xxxx), _Add_f235db20fc6d4193802744971aee1743_Out_2);
    surface.BaseColor = (_Add_a53bbeea75be437ab4a0c20a880c049b_Out_2.xyz);
    surface.Alpha = (_Add_f235db20fc6d4193802744971aee1743_Out_2).x;
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

    #include "Library/PackageCache/com.unity.shadergraph@12.1.10/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Library/PackageCache/com.unity.shadergraph@12.1.10/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Library/PackageCache/com.unity.shadergraph@12.1.10/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/SpriteUnlitPass.hlsl"

    ENDHLSL
}
Pass
{
    Name "Sprite Unlit"
    Tags
    {
        "LightMode" = "UniversalForward"
    }

        // Render State
        Cull Off
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
    #pragma exclude_renderers d3d11_9x
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
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SPRITEFORWARD
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
        float4 color : COLOR;
        #if UNITY_ANY_INSTANCING_ENABLED
        uint instanceID : INSTANCEID_SEMANTIC;
        #endif
    };
    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float4 texCoord0;
        float4 color;
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
        float4 interp1 : TEXCOORD1;
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
        output.interp1.xyzw = input.color;
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
        output.color = input.interp1.xyzw;
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
float4 Color_8b692a2d7297452f92de4622ed9fea60;
float4 Texture2D_08500469d2944895a9ad392692f50c3c_TexelSize;
float4 Texture2D_17ff7d92927445598a03023feb129259_TexelSize;
float4 Texture2D_479d5e414ef0402b8a4a2d69cd8a2a56_TexelSize;
float Vector1_339d0dcbffa04ecd8653a283be69f22e;
float Vector1_e98f7c484ee14bdcb59d8501640fde5f;
float Vector1_cee448f6c04d417abbe0f33409cee904;
float Vector1_cd3dc268c340407ab74aaa2f027e5458;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(Texture2D_08500469d2944895a9ad392692f50c3c);
SAMPLER(samplerTexture2D_08500469d2944895a9ad392692f50c3c);
TEXTURE2D(Texture2D_17ff7d92927445598a03023feb129259);
SAMPLER(samplerTexture2D_17ff7d92927445598a03023feb129259);
TEXTURE2D(Texture2D_479d5e414ef0402b8a4a2d69cd8a2a56);
SAMPLER(samplerTexture2D_479d5e414ef0402b8a4a2d69cd8a2a56);

// Graph Functions

void Unity_Rotate_Radians_float(float2 UV, float2 Center, float Rotation, out float2 Out)
{
    //rotation matrix
    UV -= Center;
    float s = sin(Rotation);
    float c = cos(Rotation);

    //center rotation matrix
    float2x2 rMatrix = float2x2(c, -s, s, c);
    rMatrix *= 0.5;
    rMatrix += 0.5;
    rMatrix = rMatrix * 2 - 1;

    //multiply the UVs by the rotation matrix
    UV.xy = mul(UV.xy, rMatrix);
    UV += Center;

    Out = UV;
}

void Unity_Multiply_float(float A, float B, out float Out)
{
    Out = A * B;
}

void Unity_Modulo_float(float A, float B, out float Out)
{
    Out = fmod(A, B);
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
    float4 _Property_57d4cd620c464caf993376abfe22463d_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_8b692a2d7297452f92de4622ed9fea60) : Color_8b692a2d7297452f92de4622ed9fea60;
    UnityTexture2D _Property_035e313c8eeb49f5a3f93a86f40b58a1_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_479d5e414ef0402b8a4a2d69cd8a2a56);
    float _Property_e06d4a068750422e95329b2115b4e487_Out_0 = Vector1_cee448f6c04d417abbe0f33409cee904;
    float2 _Rotate_d22b178f14eb4439a613eed789a2ea8f_Out_3;
    Unity_Rotate_Radians_float(IN.uv0.xy, float2 (0.5, 0.5), _Property_e06d4a068750422e95329b2115b4e487_Out_0, _Rotate_d22b178f14eb4439a613eed789a2ea8f_Out_3);
    float _Property_5f4fca2fa3594b39a69d8e017b32ea9d_Out_0 = Vector1_e98f7c484ee14bdcb59d8501640fde5f;
    float _Property_ad018d4a3ff44d9db5cb762325c2f92f_Out_0 = Vector1_cd3dc268c340407ab74aaa2f027e5458;
    float _Multiply_209d76dc11ed469fb68c0469e06a4238_Out_2;
    Unity_Multiply_float(IN.TimeParameters.x, _Property_ad018d4a3ff44d9db5cb762325c2f92f_Out_0, _Multiply_209d76dc11ed469fb68c0469e06a4238_Out_2);
    float _Property_fe8d40899cfb4ef6bca5c32400810134_Out_0 = Vector1_339d0dcbffa04ecd8653a283be69f22e;
    float _Modulo_cb8f697eaa8a4d949f4a25c1aa384cd4_Out_2;
    Unity_Modulo_float(_Multiply_209d76dc11ed469fb68c0469e06a4238_Out_2, _Property_fe8d40899cfb4ef6bca5c32400810134_Out_0, _Modulo_cb8f697eaa8a4d949f4a25c1aa384cd4_Out_2);
    float2 _Vector2_9084c2f109e44b8e8f1772d9744fa3dd_Out_0 = float2(0, _Modulo_cb8f697eaa8a4d949f4a25c1aa384cd4_Out_2);
    float2 _TilingAndOffset_7cb6f25ee828412cb1d0917256e7d8ac_Out_3;
    Unity_TilingAndOffset_float(_Rotate_d22b178f14eb4439a613eed789a2ea8f_Out_3, (_Property_5f4fca2fa3594b39a69d8e017b32ea9d_Out_0.xx), _Vector2_9084c2f109e44b8e8f1772d9744fa3dd_Out_0, _TilingAndOffset_7cb6f25ee828412cb1d0917256e7d8ac_Out_3);
    float4 _SampleTexture2D_c3b4382175ac490392c97697f29b2627_RGBA_0 = SAMPLE_TEXTURE2D(_Property_035e313c8eeb49f5a3f93a86f40b58a1_Out_0.tex, _Property_035e313c8eeb49f5a3f93a86f40b58a1_Out_0.samplerstate, _TilingAndOffset_7cb6f25ee828412cb1d0917256e7d8ac_Out_3);
    float _SampleTexture2D_c3b4382175ac490392c97697f29b2627_R_4 = _SampleTexture2D_c3b4382175ac490392c97697f29b2627_RGBA_0.r;
    float _SampleTexture2D_c3b4382175ac490392c97697f29b2627_G_5 = _SampleTexture2D_c3b4382175ac490392c97697f29b2627_RGBA_0.g;
    float _SampleTexture2D_c3b4382175ac490392c97697f29b2627_B_6 = _SampleTexture2D_c3b4382175ac490392c97697f29b2627_RGBA_0.b;
    float _SampleTexture2D_c3b4382175ac490392c97697f29b2627_A_7 = _SampleTexture2D_c3b4382175ac490392c97697f29b2627_RGBA_0.a;
    float4 _Multiply_8c6db7d42d7b43a493803e230569274a_Out_2;
    Unity_Multiply_float(_SampleTexture2D_c3b4382175ac490392c97697f29b2627_RGBA_0, (_SampleTexture2D_c3b4382175ac490392c97697f29b2627_A_7.xxxx), _Multiply_8c6db7d42d7b43a493803e230569274a_Out_2);
    float4 _Multiply_bf3d22acdd304a0fa3dd305ba02bc4a4_Out_2;
    Unity_Multiply_float(_Property_57d4cd620c464caf993376abfe22463d_Out_0, _Multiply_8c6db7d42d7b43a493803e230569274a_Out_2, _Multiply_bf3d22acdd304a0fa3dd305ba02bc4a4_Out_2);
    UnityTexture2D _Property_a39c54d7155b47369bfeabe86c554565_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_17ff7d92927445598a03023feb129259);
    float4 _SampleTexture2D_4f39e30c146a41c6b34594e6d8188856_RGBA_0 = SAMPLE_TEXTURE2D(_Property_a39c54d7155b47369bfeabe86c554565_Out_0.tex, _Property_a39c54d7155b47369bfeabe86c554565_Out_0.samplerstate, IN.uv0.xy);
    float _SampleTexture2D_4f39e30c146a41c6b34594e6d8188856_R_4 = _SampleTexture2D_4f39e30c146a41c6b34594e6d8188856_RGBA_0.r;
    float _SampleTexture2D_4f39e30c146a41c6b34594e6d8188856_G_5 = _SampleTexture2D_4f39e30c146a41c6b34594e6d8188856_RGBA_0.g;
    float _SampleTexture2D_4f39e30c146a41c6b34594e6d8188856_B_6 = _SampleTexture2D_4f39e30c146a41c6b34594e6d8188856_RGBA_0.b;
    float _SampleTexture2D_4f39e30c146a41c6b34594e6d8188856_A_7 = _SampleTexture2D_4f39e30c146a41c6b34594e6d8188856_RGBA_0.a;
    float4 _Multiply_08bec010d46e401da1694722db7aea32_Out_2;
    Unity_Multiply_float(_Multiply_bf3d22acdd304a0fa3dd305ba02bc4a4_Out_2, _SampleTexture2D_4f39e30c146a41c6b34594e6d8188856_RGBA_0, _Multiply_08bec010d46e401da1694722db7aea32_Out_2);
    UnityTexture2D _Property_dc87be653d124345b18016b175cf92cc_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_08500469d2944895a9ad392692f50c3c);
    float4 _SampleTexture2D_788f1328664a4466b2c14f1239388def_RGBA_0 = SAMPLE_TEXTURE2D(_Property_dc87be653d124345b18016b175cf92cc_Out_0.tex, _Property_dc87be653d124345b18016b175cf92cc_Out_0.samplerstate, IN.uv0.xy);
    float _SampleTexture2D_788f1328664a4466b2c14f1239388def_R_4 = _SampleTexture2D_788f1328664a4466b2c14f1239388def_RGBA_0.r;
    float _SampleTexture2D_788f1328664a4466b2c14f1239388def_G_5 = _SampleTexture2D_788f1328664a4466b2c14f1239388def_RGBA_0.g;
    float _SampleTexture2D_788f1328664a4466b2c14f1239388def_B_6 = _SampleTexture2D_788f1328664a4466b2c14f1239388def_RGBA_0.b;
    float _SampleTexture2D_788f1328664a4466b2c14f1239388def_A_7 = _SampleTexture2D_788f1328664a4466b2c14f1239388def_RGBA_0.a;
    float4 _Multiply_f65bbf6422c944ffb65b5a454044ff97_Out_2;
    Unity_Multiply_float(_SampleTexture2D_788f1328664a4466b2c14f1239388def_RGBA_0, (_SampleTexture2D_788f1328664a4466b2c14f1239388def_A_7.xxxx), _Multiply_f65bbf6422c944ffb65b5a454044ff97_Out_2);
    float4 _Add_a53bbeea75be437ab4a0c20a880c049b_Out_2;
    Unity_Add_float4(_Multiply_08bec010d46e401da1694722db7aea32_Out_2, _Multiply_f65bbf6422c944ffb65b5a454044ff97_Out_2, _Add_a53bbeea75be437ab4a0c20a880c049b_Out_2);
    float4 _Multiply_10e85da68f1d474dbbdfee599635d649_Out_2;
    Unity_Multiply_float(_Multiply_8c6db7d42d7b43a493803e230569274a_Out_2, (_SampleTexture2D_4f39e30c146a41c6b34594e6d8188856_A_7.xxxx), _Multiply_10e85da68f1d474dbbdfee599635d649_Out_2);
    float4 _Add_f235db20fc6d4193802744971aee1743_Out_2;
    Unity_Add_float4(_Multiply_10e85da68f1d474dbbdfee599635d649_Out_2, (_SampleTexture2D_788f1328664a4466b2c14f1239388def_A_7.xxxx), _Add_f235db20fc6d4193802744971aee1743_Out_2);
    surface.BaseColor = (_Add_a53bbeea75be437ab4a0c20a880c049b_Out_2.xyz);
    surface.Alpha = (_Add_f235db20fc6d4193802744971aee1743_Out_2).x;
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

    #include "Library/PackageCache/com.unity.shadergraph@12.1.10/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Library/PackageCache/com.unity.shadergraph@12.1.10/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Library/PackageCache/com.unity.shadergraph@12.1.10/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/SpriteUnlitPass.hlsl"

    ENDHLSL
}
    }
        FallBack "Hidden/Shader Graph/FallbackError"
}