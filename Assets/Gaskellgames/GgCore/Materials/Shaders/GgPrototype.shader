// Made with Amplify Shader Editor v1.9.7.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Gaskellgames/Basic/GgPrototype"
{
	Properties
	{
		[NoScaleOffset]_Albedo("Albedo", 2D) = "white" {}
		[NoScaleOffset][Normal]_Normal("Normal", 2D) = "white" {}
		[Toggle(_USEEMISSION_ON)] _UseEmission("Use Emission", Float) = 0
		[NoScaleOffset]_Emission("Emission", 2D) = "white" {}
		_TilingX("Tiling X", Float) = 1
		_TilingY("Tiling Y", Float) = 1
		_Vibrance("Vibrance", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma shader_feature_local _USEEMISSION_ON
		#define ASE_VERSION 19701
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float3 worldNormal;
			INTERNAL_DATA
			float2 uv_texcoord;
		};

		uniform sampler2D _Normal;
		uniform float _TilingX;
		uniform float _TilingY;
		uniform sampler2D _Albedo;
		uniform float _Vibrance;
		uniform sampler2D _Emission;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_vertexNormal = mul( unity_WorldToObject, float4( ase_worldNormal, 0 ) );
			ase_vertexNormal = normalize( ase_vertexNormal );
			float3 normalizeResult2_g3 = normalize( ase_vertexNormal );
			float3 break3_g3 = normalizeResult2_g3;
			float3 ase_parentObjectScale = (1.0/float3( length( unity_WorldToObject[ 0 ].xyz ), length( unity_WorldToObject[ 1 ].xyz ), length( unity_WorldToObject[ 2 ].xyz ) ));
			float3 break3_g4 = ase_parentObjectScale;
			float2 appendResult6_g4 = (float2(break3_g4.z , break3_g4.y));
			float2 appendResult4_g4 = (float2(break3_g4.x , break3_g4.z));
			float2 appendResult5_g4 = (float2(break3_g4.x , break3_g4.y));
			float2 appendResult62 = (float2(_TilingX , _TilingY));
			float2 uv_TexCoord10_g3 = i.uv_texcoord * ( ( ( break3_g3.x * appendResult6_g4 ) + ( break3_g3.y * appendResult4_g4 ) + ( break3_g3.z * appendResult5_g4 ) ) * appendResult62 );
			float2 temp_output_61_0 = uv_TexCoord10_g3;
			o.Normal = tex2D( _Normal, temp_output_61_0 ).rgb;
			float4 tex2DNode55 = tex2D( _Albedo, temp_output_61_0 );
			float grayscale63 = Luminance(tex2DNode55.rgb);
			float4 temp_cast_2 = (grayscale63).xxxx;
			float temp_output_74_0 = ( 1.0 - _Vibrance );
			float4 lerpResult67 = lerp( tex2DNode55 , temp_cast_2 , temp_output_74_0);
			o.Albedo = lerpResult67.rgb;
			#ifdef _USEEMISSION_ON
				float4 staticSwitch34 = tex2D( _Emission, temp_output_61_0 );
			#else
				float4 staticSwitch34 = float4( 0,0,0,0 );
			#endif
			float grayscale73 = Luminance(staticSwitch34.rgb);
			float4 temp_cast_5 = (grayscale73).xxxx;
			float4 lerpResult72 = lerp( staticSwitch34 , temp_cast_5 , temp_output_74_0);
			o.Emission = lerpResult72.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19701
Node;AmplifyShaderEditor.RangedFloatNode;38;-672,192;Inherit;False;Property;_TilingY;Tiling Y;5;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;37;-672,96;Inherit;False;Property;_TilingX;Tiling X;4;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;62;-480,128;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;61;-288,128;Inherit;False;ScaleIndependentUV;-1;;3;9d14e07de66f8e94daabfa937eb0eb55;0;1;16;FLOAT2;8,8;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;32;0,320;Inherit;True;Property;_Emission;Emission;3;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.StaticSwitch;34;320,320;Inherit;False;Property;_UseEmission;Use Emission;2;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;55;0,-128;Inherit;True;Property;_Albedo;Albedo;0;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;a8f03b0db0084564c859e7de6829d974;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode;66;352,160;Inherit;False;Property;_Vibrance;Vibrance;6;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCGrayscale;73;608,416;Inherit;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCGrayscale;63;608,-32;Inherit;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;74;640,160;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;31;0,96;Inherit;True;Property;_Normal;Normal;1;2;[NoScaleOffset];[Normal];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.LerpOp;72;832,320;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;67;832,-128;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1024,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Gaskellgames/Basic/GgPrototype;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;False;False;Cylindrical;False;True;Absolute;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;62;0;37;0
WireConnection;62;1;38;0
WireConnection;61;16;62;0
WireConnection;32;1;61;0
WireConnection;34;0;32;0
WireConnection;55;1;61;0
WireConnection;73;0;34;0
WireConnection;63;0;55;0
WireConnection;74;0;66;0
WireConnection;31;1;61;0
WireConnection;72;0;34;0
WireConnection;72;1;73;0
WireConnection;72;2;74;0
WireConnection;67;0;55;0
WireConnection;67;1;63;0
WireConnection;67;2;74;0
WireConnection;0;0;67;0
WireConnection;0;1;31;0
WireConnection;0;2;72;0
ASEEND*/
//CHKSM=826CC5EDA0F4431BF735A0F3315084132246CA85