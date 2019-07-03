Shader "ToucanSystems/Charts"
{
	Properties
	{
		[PerRendererData] _MainTex("Data Texture", 2D) = "white" {}
		[HideInInspector] _SecTex("UnderLine Texture", 2D) = "white" {}
		_Alpha("Alpha", Range(0, 1)) = 1
		[HideInInspector] _ShiftFactor("Shift Factor", Range(0, 1)) = 0
		[HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest[unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha

		Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp] 
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        ColorMask [_ColorMask]

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _SecTex;
			float _Alpha;
			float _ShiftFactor;

			struct vIn
			{
				float4 vertex : POSITION;
				float2 uv_MainTex : TEXCOORD0;
				float4 color : COLOR;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv_MainTex  : TEXCOORD0;
				float4 color : COLOR;
			};

			struct fOut
			{
				fixed4 color : SV_Target;
			};

			v2f vert(vIn i)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(i.vertex);
				o.uv_MainTex = i.uv_MainTex;
				o.color = i.color;
				return o;
			}

			fOut frag(v2f i)
			{
				fOut result;

				float4 targetColor = tex2D(_SecTex, i.uv_MainTex);
				float4 testColor = tex2D(_MainTex, float2(i.uv_MainTex.x + _ShiftFactor , 0.5));
				float testValue = testColor.r + testColor.g / 255;

				if (i.uv_MainTex.y > testValue)
					targetColor = float4(0, 0, 0, 0);
				
				result.color = targetColor * i.color;
				result.color.a *= _Alpha;
				return result;
			}

			ENDCG
		}
	}
}