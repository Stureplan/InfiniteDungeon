// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/FlatColored"
{
	Properties
	{
		_MainColor ("Main Color", Color) = (1, 1, 1, 1)
		_Cube("Cubemap", CUBE) = "" {}
		_Reflection("Reflection Cutoff", Range(0.0, 1.1)) = 0.7
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout" }

		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
				fixed3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				fixed3 normal : NORMAL;
				fixed3 reflectedDir : TEXCOORD0;
			};

			fixed4 _MainColor;
			samplerCUBE _Cube;
			fixed _Reflection;

			v2f vert (appdata v)
			{
				v2f o;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = v.normal;


				o.color = lerp(_MainColor, fixed4(v.color.rgb, 1.0), v.color.a);
				//o.color.a = 1.0;
				fixed a = v.uv.x;		// Alpha is in the 0-1 range
				o.color.rgb *= 1 - a;	// 0 = Fully Light, 1 = Fully Dark



				float4x4 modelMatrix = unity_ObjectToWorld;
				float4x4 modelMatrixInverse = unity_WorldToObject;
				fixed3 normalDir = normalize(mul(float4(v.normal, 0.0), modelMatrixInverse).xyz);
				fixed3 viewDir = mul(modelMatrix, v.vertex).xyz - _WorldSpaceCameraPos;
				fixed3 reflectedDir = reflect(viewDir, normalize(normalDir));
				o.reflectedDir = reflectedDir;

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = i.color;
				
				
				//fixed4 cube;
				//cube.rgb= texCUBE(_Cube, i.normal).rgb;
				//cube.a = 1.0;
				
				float4 cube = texCUBE(_Cube, i.reflectedDir);
				cube.a = 1.0;

				fixed amt = (cube.r + cube.g + cube.b) / 3;
				fixed mult = step(_Reflection, amt);
				cube *= mult;

				return col + cube;
			}
			ENDCG
		}
	}
}
