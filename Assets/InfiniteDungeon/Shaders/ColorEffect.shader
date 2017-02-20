Shader "InfiniteDungeon/ColorEffect"
{
	Properties
	{
		_Scale("Scale", Range(0, 5)) = 1.0
		_Speed("Speed", Range(0, 5)) = 1.0
		_Frequency("Frequency", Range(0, 10)) = 1.0

		_Tint("Tint Color", Color) = (1, 1, 1, 1)
		_TintAmount("Tint Amount", Range(0, 1)) = 1.0

		_RandVector("Light Vertex", Vector) = (1, 1, 1, 1)
	}

	SubShader
	{
		Tags
		{ 
		"Queue" = "AlphaTest"
		"RenderType" = "Transparent" 
		"LightMode" = "ForwardBase" 
		}

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
				float4 normal : NORMAL;
				fixed4 color : COLOR;
			};

			struct v2f
			{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
			};

			fixed _Scale;
			fixed _Speed;
			fixed _Frequency;

			fixed _Shakiness;
			fixed4 _Tint;
			fixed  _TintAmount;

			fixed4 _RandVector;

			float TriangleWave(float x) 
			{
				return (frac(x + 0.5) * 2.0 - 1.0);
			}
			float rand(float3 co)
			{
				return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 45.5432))) * 8.5453);
			}

			v2f vert (appdata v)
			{
				v2f o;
				fixed3 pos;

				pos.x = 0;
				//pos.y = _YSpeed + rand(v.normal.xyz + _SinTime.x/50) * 0.05;
				fixed offset = (v.vertex.x * v.vertex.x) + (v.vertex.z * v.vertex.z);

				pos.y = _Scale * sin(_Time.w * _Speed + offset * _Frequency);

				pos.z = 0;

				v.vertex.xyz += pos;

				o.vertex = UnityObjectToClipPos(v.vertex);
				
				// Apply colors.
				o.color = lerp(v.color, _Tint, _TintAmount);

				//fixed s = abs(sin(_Time.y)) * 0.8;
				//o.color.rgb -= s;

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return i.color;
			}
			ENDCG
		}
	}
}
