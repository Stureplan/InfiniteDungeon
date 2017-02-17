Shader "InfiniteDungeon/VertexLitColorShaded_Vegetation"
{
	Properties
	{
		_WindY("Wind Y Speed", Range(0, 5)) = 1.0
		_Shakiness("Shake Amount", Range(1, 2)) = 1.0

	}

	SubShader
	{
		Tags{ "LightMode" = "ForwardBase" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				fixed3 normal : NORMAL;
				fixed2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
			};

			fixed _WindY;
			fixed _Shakiness;

			fixed4 GLOBAL_LIGHT_POS_0 = fixed4(0, 500, 0, 0);
			fixed4 GLOBAL_LIGHT_POS_1 = fixed4(0, 500, 0, 0);
			fixed4 GLOBAL_LIGHT_POS_2 = fixed4(0, 500, 0, 0);
			fixed4 GLOBAL_LIGHT_POS_3 = fixed4(0, 500, 0, 0);

			fixed4 GLOBAL_LIGHT_COL_0 = fixed4(1, 1, 1, 0);
			fixed4 GLOBAL_LIGHT_COL_1 = fixed4(1, 1, 1, 0);
			fixed4 GLOBAL_LIGHT_COL_2 = fixed4(1, 1, 1, 0);
			fixed4 GLOBAL_LIGHT_COL_3 = fixed4(1, 1, 1, 0);

			fixed4 GLOBAL_PLAYER_POS = fixed4(0, 0, 0, 0);


			fixed3 lights(fixed3 pos, fixed3 normal)
			{
				// Vertex to light.
				fixed3 v2l0 = (GLOBAL_LIGHT_POS_0.xyz - pos);
				fixed3 v2l1 = (GLOBAL_LIGHT_POS_1.xyz - pos);
				fixed3 v2l2 = (GLOBAL_LIGHT_POS_2.xyz - pos);
				fixed3 v2l3 = (GLOBAL_LIGHT_POS_3.xyz - pos);

				// Distance.
				fixed d0 = length(v2l0);
				fixed d1 = length(v2l1);
				fixed d2 = length(v2l2);
				fixed d3 = length(v2l3);

				// Direction.
				v2l0 = normalize(v2l0);
				v2l1 = normalize(v2l1);
				v2l2 = normalize(v2l2);
				v2l3 = normalize(v2l3);

				// Diffuse variable
				fixed diff0 = max(dot(normal, v2l0), 0.1);
				fixed diff1 = max(dot(normal, v2l1), 0.1);
				fixed diff2 = max(dot(normal, v2l2), 0.1);
				fixed diff3 = max(dot(normal, v2l3), 0.1);

				diff0 = diff0 * (1.0 / (1.0 + (0.25 * d0 * d0)));
				diff1 = diff1 * (1.0 / (1.0 + (0.25 * d1 * d1)));
				diff2 = diff2 * (1.0 / (1.0 + (0.25 * d2 * d2)));
				diff3 = diff3 * (1.0 / (1.0 + (0.25 * d3 * d3)));

				fixed3 col0 = GLOBAL_LIGHT_COL_0.rgb * diff0;
				fixed3 col1 = GLOBAL_LIGHT_COL_1.rgb * diff1;
				fixed3 col2 = GLOBAL_LIGHT_COL_2.rgb * diff2;
				fixed3 col3 = GLOBAL_LIGHT_COL_3.rgb * diff3;

				return (col0 + col1 + col2 + col3);
			}


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

				// v.vertex = Object Space (Base is (0, 0, 0))
				fixed localDistance = distance(v.vertex.xyz, fixed3(0, 0, 0));
				
				
				
				fixed s = sin(_Time.y * _WindY + v.vertex.x);
				fixed c = cos(_Time.y * _WindY + v.vertex.z);

				fixed sy = sin(_Time.y + v.vertex.y);
				fixed cy = cos(_Time.y + v.vertex.y);


				v.vertex.y += (rand(v.vertex.xyz) * _Shakiness) * s * c * localDistance * 0.2;


				// Vertex in WORLD space.
				fixed3 pos = mul(unity_ObjectToWorld, v.vertex);

				fixed3 dir = normalize(pos - GLOBAL_PLAYER_POS);
				fixed d = length(dir);



				o.vertex = UnityObjectToClipPos(v.vertex);
				fixed3 normal = v.normal;

				// Apply lighting.
				o.color.rgb = v.color.rgb + fixed4(lights(pos, normal), 1.0);
				o.color.a = v.color.a;

				// Hard edges.
				half nDot = dot(normal, fixed3(1, 1, 0));
				nDot = clamp(nDot, 0.8, 1);
				o.color.rgb *= nDot;

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = i.color;
				col.a = 1.0;

				
				return col;
			}
			ENDCG
		}
	}
}
