Shader "Witchcraft VFX/Dissolve" {
    Properties {
        _Brightness ("Brightness", Float ) = 1
        _Intensity ("Intensity", Float ) = 1
        _Pan_Speed ("Pan_Speed", Float ) = 1
        [MaterialToggle] _Gradient_Or_Solid_Color ("Gradient_Or_Solid_Color", Float ) = 1
        _Gradient_Color ("Gradient_Color", 2D) = "white" {}
        _Solid_Color ("Solid_Color", Color) = (0.1764706,0.5229208,1,1)
        _Texture ("Texture", 2D) = "white" {}
        _Gradient_Texture_Decay ("Gradient_Texture_Decay", 2D) = "white" {}
        _Decay ("Decay", Range(0.05, 0.95)) = 0.3
        [MaterialToggle] _Fresnel ("Fresnel", Float ) = 1
        [MaterialToggle] _Make_Same_As_Fresnel ("Make_Same_As_Fresnel", Float ) = 0
        _Fresnel_Exponent ("Fresnel_Exponent", Float ) = 1
        [MaterialToggle] _Edge_Detection_Fake ("Edge_Detection_Fake", Float ) = 0.2901961
        _Gradient_Edge_Fake ("Gradient_Edge_Fake", 2D) = "white" {}
        [MaterialToggle] _Soft_Texture ("Soft_Texture", Float ) = 2
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "ForwardBase"
           
            Blend One One
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _Texture; uniform float4 _Texture_ST;
            uniform float _Fresnel_Exponent;
            uniform sampler2D _Gradient_Texture_Decay; uniform float4 _Gradient_Texture_Decay_ST;
            uniform sampler2D _Gradient_Edge_Fake; uniform float4 _Gradient_Edge_Fake_ST;
            uniform sampler2D _Gradient_Color; uniform float4 _Gradient_Color_ST;
            uniform fixed _Edge_Detection_Fake;
            uniform fixed _Fresnel;
            uniform fixed _Gradient_Or_Solid_Color;
            uniform float4 _Solid_Color;
            uniform fixed _Make_Same_As_Fresnel;
            uniform fixed _Soft_Texture;
            uniform float _Decay;
            uniform float _Pan_Speed;
            uniform float _Intensity;
            uniform float _Brightness;
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
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(unity_ObjectToWorld, float4(v.normal,0)).xyz;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);

                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                
                float nSign = sign( dot( viewDirection, i.normalDir ) );
                i.normalDir *= nSign;
                normalDirection *= nSign;
                float4 node_2434 = _Time + _TimeEditor;
                float2 node_923 = (i.uv0+(node_2434.g*_Pan_Speed)*float2(0,0.1));
                float4 _Texture_var = tex2D(_Texture,TRANSFORM_TEX(node_923, _Texture));
                float node_1772 = 0.0;
                float _Fresnel_var = lerp( node_1772, pow(1.0-max(0,dot(normalDirection, viewDirection)),_Fresnel_Exponent), _Fresnel );
                float2 node_1319 = float2(lerp( (_Texture_var.r+_Fresnel_var), _Texture_var.r, _Soft_Texture ),((i.uv0.g*0.0)+_Decay));
                float4 _Gradient_Texture_Decay_var = tex2D(_Gradient_Texture_Decay,TRANSFORM_TEX(node_1319, _Gradient_Texture_Decay));
                float4 _Gradient_Edge_Fake_var = tex2D(_Gradient_Edge_Fake,TRANSFORM_TEX(i.uv0, _Gradient_Edge_Fake));
                float _Edge_Detection_Fake_var = lerp( node_1772, _Gradient_Edge_Fake_var.r, _Edge_Detection_Fake );
                float node_1665 = clamp(((lerp( _Gradient_Texture_Decay_var.r, (_Gradient_Texture_Decay_var.r*(_Fresnel_var+_Edge_Detection_Fake_var)), _Make_Same_As_Fresnel )+_Edge_Detection_Fake_var)*_Intensity),0.05,0.95);
                float2 node_1654 = float2(node_1665,i.uv0.g);
                float4 _Gradient_Color_var = tex2D(_Gradient_Color,TRANSFORM_TEX(node_1654, _Gradient_Color));
                float3 emissive = (lerp( (_Solid_Color.rgb*node_1665), _Gradient_Color_var.rgb, _Gradient_Or_Solid_Color )*_Brightness);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
}
