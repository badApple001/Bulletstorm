﻿Shader "UI/GuideMask"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		_Blur("边缘虚化的范围", Range(1,1000)) = 100
		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255		
		_ColorMask("Color Mask", Float) = 15
		//中心
		_Origin("Circle",Vector) = (0,0,0,0)
		//裁剪方式 0圆形 1圆形
		_MaskType("Type",Float) = 0	
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

		Stencil
	{
		Ref[_Stencil]
		Comp[_StencilComp]
		Pass[_StencilOp]
		ReadMask[_StencilReadMask]
		WriteMask[_StencilWriteMask]
	}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest[unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask[_ColorMask]

		Pass
	{
		Name "Default"
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 2.0

#include "UnityCG.cginc"
#include "UnityUI.cginc"



		struct appdata_t
	{
		float4 vertex : POSITION;
		float4 color : COLOR;
		float2 texcoord : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct v2f
	{
		float4 vertex : SV_POSITION;
		fixed4 color : COLOR;
		float2 texcoord : TEXCOORD0;
		float4 worldPosition : TEXCOORD1;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	fixed4 _Color;
	fixed4 _TextureSampleAdd;
	float4 _ClipRect;
	float4 _Origin;
	float _MaskType;	
	float _Blur;
	v2f vert(appdata_t IN)
	{
		v2f OUT;
		UNITY_SETUP_INSTANCE_ID(IN);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
		OUT.worldPosition = IN.vertex;
		OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
		OUT.texcoord = IN.texcoord;
		OUT.color = IN.color * _Color;
		return OUT;
	}

	sampler2D _MainTex;

	fixed4 frag(v2f IN) : SV_Target
	{
		half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;

		if (_MaskType == 0) {
			//if (distance(IN.worldPosition.xy, _Origin.xy) <= _Origin.z)
			//{
			//	color.a = 0;
			//}			
			  float dis = distance(IN.worldPosition.xy, _Origin.xy);
			  //过滤掉距离小于（半径-过渡范围）的片元
			  clip(dis - (_Origin.z - _Blur));
			  //优化if条件判断，如果距离小于半径则执行下一步，等于if(dis < _Radius)
			  fixed tmp = step(dis, _Origin.z);
			  //计算过渡范围内的alpha值
			  color.a *= (1 - tmp) + tmp * (dis - (_Origin.z - _Blur)) / _Blur;
		}
		else if (_MaskType == 1) {
			//UnityGet2DClipping这个函数实现了判断2D空间中的一点是否在一个矩形区域中
			if (UnityGet2DClipping(IN.worldPosition.xy, _Origin))
			{
				color.a = 0;

			}

		}
		else if (_MaskType == 2)
		{
			if (UnityGet2DClipping(IN.worldPosition.xy, _Origin))
			{
				color.a = 0;
				#ifdef UNITY_UI_CLIP_RECT
               color.a *= UnityGet2DClipping(IN.worldPosition.xy, _Origin);
                #endif
 

			}               
 

		}

		return color;
	}
		ENDCG
	}
	}
}