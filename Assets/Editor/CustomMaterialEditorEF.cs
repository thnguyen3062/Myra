// using UnityEngine;
// using UnityEditor;

// [ExecuteInEditMode]
// public class CustomMaterialEditorEF : MaterialEditor
// {
//     public override void OnInspectorGUI()
//     {
//         if (!isVisible)
//             return;

//         Material material = target as Material;

//         MaterialProperty[] properties = GetMaterialProperties(targets);

//         string[] keys = material.shaderKeywords;

//         bool effectsLayer1Enabled = keys.Contains("EFFECTS_LAYER_1_ON");
//         bool effectsLayer2Enabled = keys.Contains("EFFECTS_LAYER_2_ON");
//         bool effectsLayer3Enabled = keys.Contains("EFFECTS_LAYER_3_ON");
//         bool effectsLayer4Enabled = keys.Contains("EFFECTS_LAYER_4_ON");

//         EditorGUI.BeginChangeCheck();
//         EditorGUIUtility.labelWidth = 200;

//     //3 top main textures: MainTex - the image, Distortion Texture - the pattern, and Distortion Mask - where different effects applies.
//         for (int i = 0; i < 3; i++)
//             TexturePropertySingleLine(new GUIContent(properties[i].displayName), properties[i]); 

//         EditorGUIUtility.labelWidth = 140;

//     //The speed of distortion, horizontally and vertically.        
//         Vector4 _distortionSpd = properties[3].vectorValue;
//         EditorGUILayout.BeginHorizontal();
//         {
//             EditorGUILayout.LabelField("Distortion Spd X Y");
//             GUILayout.Space(-56);
//             _distortionSpd.x = EditorGUILayout.FloatField(_distortionSpd.x);
//             _distortionSpd.y = EditorGUILayout.FloatField(_distortionSpd.y);
//         }
//         EditorGUILayout.EndHorizontal();
//         properties[3].vectorValue = _distortionSpd;

//     //Does the effect involve whole image shaking?
//         BoolProperty((properties[4]), properties[4].displayName);  
//     //Shake durations.
//         Vector4 _ShakeThings = properties[5].vectorValue;
//         EditorGUILayout.BeginHorizontal();
//         {
//             EditorGUILayout.LabelField("Shake Total Alone");
//             GUILayout.Space(-56);
//             _ShakeThings.x = EditorGUILayout.FloatField(_ShakeThings.x);
//             _ShakeThings.y = EditorGUILayout.FloatField(_ShakeThings.y);
//         }
//         EditorGUILayout.EndHorizontal();
//         properties[5].vectorValue = _ShakeThings;
//     //Shake speed and magnitude.
//         EditorGUILayout.BeginHorizontal();
//         {
//             EditorGUILayout.LabelField("Shake Spd Mag");
//             GUILayout.Space(-56);
//             _ShakeThings.z = EditorGUILayout.FloatField(_ShakeThings.z);
//             _ShakeThings.w = EditorGUILayout.FloatField(_ShakeThings.w);
//         }
//         EditorGUILayout.EndHorizontal();
//         properties[5].vectorValue = _ShakeThings;

//     // //Does the effect involve ripple effect?
//     //     BoolProperty((properties[6]), properties[6].displayName);
//     // //Size of the ripple.
//     //     FloatProperty((properties[7]), properties[7].displayName);

//     //Things related to the ripple. Magnitude and Duration.
//     //     Vector4 _RippleThings = properties[8].vectorValue;
//     //     EditorGUILayout.BeginHorizontal();
//     //     {
//     //         EditorGUILayout.LabelField("Ripple Mag Dur");
//     //         GUILayout.Space(-56);
//     //         _RippleThings.x = EditorGUILayout.FloatField(_RippleThings.x);
//     //         _RippleThings.y = EditorGUILayout.FloatField(_RippleThings.y);
//     //     }
//     //     EditorGUILayout.EndHorizontal();
//     //     properties[8].vectorValue = _RippleThings;
        
//     // //The center of the ripple;
//     //     EditorGUILayout.BeginHorizontal();
//     //     {
//     //         EditorGUILayout.LabelField("Ripple Center X Y");
//     //         GUILayout.Space(-56);
//     //         _RippleThings.z = EditorGUILayout.FloatField(_RippleThings.z);
//     //         _RippleThings.w = EditorGUILayout.FloatField(_RippleThings.w);
//     //     }
//     //     EditorGUILayout.EndHorizontal();
//     //     properties[8].vectorValue = _RippleThings;

// //Tiling
//         Vector4 _tilingOffset = properties[6].vectorValue;
//         EditorGUILayout.BeginHorizontal();
//         {
//             EditorGUILayout.LabelField("Tiling");
//             GUILayout.Space(-56);
//             _tilingOffset.x = EditorGUILayout.FloatField(_tilingOffset.x);
//             _tilingOffset.y = EditorGUILayout.FloatField(_tilingOffset.y);
//         }
//         EditorGUILayout.EndHorizontal();
//         properties[6].vectorValue = _tilingOffset;

// //Offset
//         EditorGUILayout.BeginHorizontal();
//         {
//             EditorGUILayout.LabelField("Offset");
//             GUILayout.Space(-56);
//             _tilingOffset.z = EditorGUILayout.FloatField(_tilingOffset.z);
//             _tilingOffset.w = EditorGUILayout.FloatField(_tilingOffset.w);
//         }
//         EditorGUILayout.EndHorizontal();
//         properties[6].vectorValue = _tilingOffset;

// //Grayscale for tired.
//         BoolProperty((properties[7]), properties[7].displayName);


// //Color on card texture.
//         TexturePropertySingleLine(new GUIContent(properties[8].displayName), properties[8]); 

// //ColTiling
//         Vector4 _ColTilingOffset = properties[9].vectorValue;
//         EditorGUILayout.BeginHorizontal();
//         {
//             EditorGUILayout.LabelField("ColTiling");
//             GUILayout.Space(-56);
//             _ColTilingOffset.x = EditorGUILayout.FloatField(_ColTilingOffset.x);
//             _ColTilingOffset.y = EditorGUILayout.FloatField(_ColTilingOffset.y);
//         }
//         EditorGUILayout.EndHorizontal();
//         properties[9].vectorValue = _ColTilingOffset;

// //ColOffset
//         EditorGUILayout.BeginHorizontal();
//         {
//             EditorGUILayout.LabelField("ColOffset");
//             GUILayout.Space(-56);
//             _ColTilingOffset.z = EditorGUILayout.FloatField(_ColTilingOffset.z);
//             _ColTilingOffset.w = EditorGUILayout.FloatField(_ColTilingOffset.w);
//         }
//         EditorGUILayout.EndHorizontal();
//         properties[9].vectorValue = _ColTilingOffset;


// //Separated effects starts here.
//         EditorGUILayout.Separator();

//         effectsLayer1Enabled = EditorGUILayout.Toggle("Effect 1", effectsLayer1Enabled);
//         if (effectsLayer1Enabled)
//             DrawEffectsLayer(properties, 1);

//         effectsLayer2Enabled = EditorGUILayout.Toggle("Effect 2", effectsLayer2Enabled);
//         if (effectsLayer2Enabled)
//             DrawEffectsLayer(properties, 2);

//         effectsLayer3Enabled = EditorGUILayout.Toggle("Effect 3", effectsLayer3Enabled);
//         if (effectsLayer3Enabled)
//             DrawEffectsLayer(properties, 3);

//         effectsLayer4Enabled = EditorGUILayout.Toggle("Effect 4", effectsLayer4Enabled);
//         if (effectsLayer4Enabled)
//             DrawEffectsLayer(properties, 4);

//         if (EditorGUI.EndChangeCheck())
//         {
//             string[] newKeys = new string[] {
//                 effectsLayer1Enabled ? "EFFECTS_LAYER_1_ON" : "EFFECTS_LAYER_1_OFF",
//                 effectsLayer2Enabled ? "EFFECTS_LAYER_2_ON" : "EFFECTS_LAYER_2_OFF",
//                 effectsLayer3Enabled ? "EFFECTS_LAYER_3_ON" : "EFFECTS_LAYER_3_OFF",
//                 effectsLayer4Enabled ? "EFFECTS_LAYER_4_ON" : "EFFECTS_LAYER_4_OFF",
//             };

//             material.shaderKeywords = newKeys;
//             EditorUtility.SetDirty(material);
//         }
//     }

//     void DrawEffectsLayer(MaterialProperty[] properties, int layer)
//     {
//         EditorGUIUtility.labelWidth = 130;
//         GUIStyle style = EditorStyles.helpBox;
//         style.margin = new RectOffset(20, 0, 0, 0);  

//         EditorGUILayout.BeginVertical(style);
//         {
//             EditorGUIUtility.labelWidth = 200;
//         //Main Image Texture.
//             TexturePropertySingleLine(new GUIContent("Effect Main Texture"), properties.GetByName(EffectName(layer, "Tex")));
//         //Main Motion Texture.
//             TexturePropertySingleLine(new GUIContent("Motion & Breathe Texture"), properties.GetByName(EffectName(layer, "Motion")));
//             EditorGUIUtility.labelWidth = 130;
//         //Cut part - Alpha blend.
//             BoolProperty(properties.GetByName(EffectName(layer, "IsCutPart")), "Is Cut Part");
//         //Additive - Add blend.
//             BoolProperty(properties.GetByName(EffectName(layer, "IsAdditive")), "Is Additive");
//         //Color of effect.
//             ColorProperty(properties.GetByName(EffectName(layer, "Color")), "Tint Color");
//         //Color intensity of effect.
//             FloatProperty(properties.GetByName(EffectName(layer, "Intensity")), "Blend Intensity");

//         //Distortion and intensity.
//             EditorGUILayout.BeginHorizontal();
//             {
//                 EditorGUILayout.LabelField("Distort & Intensity");
//                 EditorGUIUtility.labelWidth = 130;
//                 GUILayout.Space(-56);
//                 BoolProperty(properties.GetByName(EffectName(layer, "Distorted")), "");
//                 GUILayout.Space(-120);
//                 FloatProperty(properties.GetByName(EffectName(layer, "DistortionIntensity")), "");
//                 GUILayout.MaxWidth (10);
//                 GUILayout.FlexibleSpace();
//             }
//             EditorGUILayout.EndHorizontal();

//         //Is blinking or not.
//             BoolProperty(properties.GetByName(EffectName(layer, "IsBlinking")), "Is Blinking");
//         //Blink speed, blink duration.
//             Vector4 blinkThings = properties.GetByName(EffectName(layer, "BlinkSpdDurOffsetAdd")).vectorValue;
//             EditorGUILayout.BeginHorizontal();
//             {
//                 EditorGUILayout.LabelField("Blink Spd Dur");
//                 GUILayout.Space(-56);
//                 blinkThings.x = EditorGUILayout.FloatField(blinkThings.x);
//                 blinkThings.y = EditorGUILayout.FloatField(blinkThings.y);
//             }
//             EditorGUILayout.EndHorizontal();
//             properties.GetByName(EffectName(layer, "BlinkSpdDurOffsetAdd")).vectorValue = blinkThings;
//         //How long blink shows (visible).
//             EditorGUILayout.BeginHorizontal();
//             {
//                 EditorGUILayout.LabelField("Blink Appear More");
//                 GUILayout.Space(-56);
//                 blinkThings.z = EditorGUILayout.FloatField(blinkThings.z);
//             }
//             EditorGUILayout.EndHorizontal();
//             properties.GetByName(EffectName(layer, "BlinkSpdDurOffsetAdd")).vectorValue = blinkThings;
//         //Does this blink effect have more disappear period?
//             BoolProperty(properties.GetByName(EffectName(layer, "IsBlinkOffset")), "Is Blink Offset");
//         //How long this blink disappear. If use this, set Blink Appear More = 0 to avoid complications.
//             EditorGUILayout.BeginHorizontal();
//             {
//                 EditorGUILayout.LabelField("Blink Disappear More");
//                 GUILayout.Space(-56);
//                 blinkThings.w = EditorGUILayout.FloatField(blinkThings.w);
//             }
//             EditorGUILayout.EndHorizontal();
//             properties.GetByName(EffectName(layer, "BlinkSpdDurOffsetAdd")).vectorValue = blinkThings;


//         //The speed this effect moves.
//             FloatProperty(properties.GetByName(EffectName(layer, "MotionSpeed")), "Motion Speed");
//         //Does it move back and forth.
//             BoolProperty(properties.GetByName(EffectName(layer, "BackAndForth")), "Motion Back&Forth");
        
//         //Speed and magnitude of the back and forth movements.
//             Vector4 backForthOsci = properties.GetByName(EffectName(layer, "BackForthOsciCycleMag")).vectorValue;
//             EditorGUILayout.BeginHorizontal();
//             {
//                 EditorGUILayout.LabelField("BackForth Cycle Mag");
//                 GUILayout.Space(-56);
//                 backForthOsci.x = EditorGUILayout.FloatField(backForthOsci.x);
//                 backForthOsci.y = EditorGUILayout.FloatField(backForthOsci.y);
//             }
//             EditorGUILayout.EndHorizontal();
//             properties.GetByName(EffectName(layer, "BackForthOsciCycleMag")).vectorValue = backForthOsci;

//         //Does this effect rotate around?
//             FloatProperty(properties.GetByName(EffectName(layer, "Rotation")), "Rotation Speed");

//         //Does this effect osscilate around?
//             BoolProperty(properties.GetByName(EffectName(layer, "Oscillate")), "Oscillate");

//         //Rotation and Osscilation speed and magnitude.
//             EditorGUILayout.BeginHorizontal();
//             {
//                 EditorGUILayout.LabelField("BackForth Cycle Mag");
//                 GUILayout.Space(-56);
//                 backForthOsci.z = EditorGUILayout.FloatField(backForthOsci.z);
//                 backForthOsci.w = EditorGUILayout.FloatField(backForthOsci.w);
//             }
//             EditorGUILayout.EndHorizontal();
//             properties.GetByName(EffectName(layer, "BackForthOsciCycleMag")).vectorValue = backForthOsci;

//         //Does this effect involve breathing?
//             Vector4 breathe = properties.GetByName(EffectName(layer, "BreatheXYFregMag")).vectorValue;
//         //Breathing speed left right.
//             BoolProperty(properties.GetByName(EffectName(layer, "BreatheX")), "BreatheX");    
//             EditorGUILayout.BeginHorizontal();
//             {
//                 EditorGUILayout.LabelField("BreatheXFregMag");
//                 GUILayout.Space(-56);
//                 breathe.x = EditorGUILayout.FloatField(breathe.x);
//                 breathe.y = EditorGUILayout.FloatField(breathe.y);
//             }
//             EditorGUILayout.EndHorizontal();

//         //Breathing speed up down.
//             BoolProperty(properties.GetByName(EffectName(layer, "BreatheY")), "BreatheY");
//             EditorGUILayout.BeginHorizontal();
//             {
//                 EditorGUILayout.LabelField("BreatheYFregMag");
//                 GUILayout.Space(-56);
//                 breathe.z = EditorGUILayout.FloatField(breathe.z);
//                 breathe.w = EditorGUILayout.FloatField(breathe.w);
//             }
//             EditorGUILayout.EndHorizontal();
//             properties.GetByName(EffectName(layer, "BreatheXYFregMag")).vectorValue = breathe;

//         //Position of the center of this effect.
//             Vector4 translation = properties.GetByName(EffectName(layer, "Translation")).vectorValue;
//             EditorGUILayout.BeginHorizontal();
//             {
//                 EditorGUILayout.LabelField("Translation X Y");
//                 GUILayout.Space(-56);
//                 translation.x = EditorGUILayout.FloatField(translation.x);
//                 translation.y = EditorGUILayout.FloatField(translation.y);
//             }
//             EditorGUILayout.EndHorizontal();
//             properties.GetByName(EffectName(layer, "Translation")).vectorValue = translation;

//         //Position of the pivot of this effect. Applies when it rotates and oscillates.
//             Vector4 pivotScale = properties.GetByName(EffectName(layer, "PivotScale")).vectorValue;
//             EditorGUILayout.BeginHorizontal();
//             {
//                 EditorGUILayout.LabelField("Pivot X Y");
//                 GUILayout.Space(-56);
//                 pivotScale.x = EditorGUILayout.FloatField(pivotScale.x);
//                 pivotScale.y = EditorGUILayout.FloatField(pivotScale.y);
//             }
//             EditorGUILayout.EndHorizontal();

//         //Scale X Y of this effect.
//             EditorGUILayout.BeginHorizontal();
//             {
//                 EditorGUILayout.LabelField("Scale X Y");
//                 GUILayout.Space(-56);
//                 pivotScale.z = EditorGUILayout.FloatField(pivotScale.z);
//                 pivotScale.w = EditorGUILayout.FloatField(pivotScale.w);
//             }
//             EditorGUILayout.EndHorizontal();
//             properties.GetByName(EffectName(layer, "PivotScale")).vectorValue = pivotScale;

//         BoolProperty(properties.GetByName(EffectName(layer, "Foreground")), "Foreground");
//         }
//         EditorGUILayout.EndVertical();
//     }

//     bool BoolProperty(MaterialProperty property, string name)
//     {
//         bool toggle = property.floatValue == 0 ? false : true;
//         toggle = EditorGUILayout.Toggle(name, toggle);
//         property.floatValue = toggle ? 1 : 0;

//         return toggle;
//     }

//     string EffectName(int layer, string property)
//     {
//         return string.Format("_EF{0}{1}", layer.ToString(), property);
//     }
// }