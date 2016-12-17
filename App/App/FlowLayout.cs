//*************************************************************
//  File: FlowLayout.cs
//  Date created: 12/17/2016
//  Date edited: 12/17/2016
//  Author: Nathan Martindale
//  Copyright © 2016 Digital Warrior Labs
//  Description: Shamelessly (and frustratedly) reproduced from http://stackoverflow.com/questions/2961777/android-linearlayout-horizontal-with-wrapping-children
//*************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace App
{

	public class LayoutParams : ViewGroup.LayoutParams 
	{
		// member variables
		private int m_iHorizontalSpacing;
		private int m_iVerticalSpacing;

		// construction
		public LayoutParams(int iHorizontalSpacing, int iVerticalSpacing) : base(0, 0)
		{
			this.HorizontalSpacing = iHorizontalSpacing;
			this.VerticalSpacing = iVerticalSpacing;
		}
		public LayoutParams(int iHorizontalSpacing, int iVerticalSpacing, ViewGroup.LayoutParams p) : base(p)
		{
			this.HorizontalSpacing = iHorizontalSpacing;
			this.VerticalSpacing = iVerticalSpacing;
		}

		// properties
		public int HorizontalSpacing { get { return m_iHorizontalSpacing; } set { m_iHorizontalSpacing = value; } }
		public int VerticalSpacing { get { return m_iVerticalSpacing; } set { m_iVerticalSpacing = value; } }
	}

	public class FlowLayout : ViewGroup
	{
		// member variables
		private int m_iLineHeight;
	
		// construction
		public FlowLayout(Context pContext) : base(pContext) { }
		public FlowLayout(Context pContext, Android.Util.IAttributeSet pAttrs) : base(pContext, pAttrs) { }

		// truth be told, I have only a vague idea what most of this is doing, and I'm too tired to care...dear future me, please figure out why this works (if it works)
		protected override void OnMeasure(int iWidthMeasureSpec, int iHeightMeasureSpec)
		{
			int iWidth = MeasureSpec.GetSize(iWidthMeasureSpec) - PaddingLeft - PaddingRight; 
			int iHeight = MeasureSpec.GetSize(iHeightMeasureSpec) - PaddingTop - PaddingBottom;

			int iCount = ChildCount;
			int iLineHeight = 0;

			int iXPos = PaddingLeft;
			int iYPos = PaddingTop;

			int iChildHeightMeasureSpec;
			if (MeasureSpec.GetMode(iHeightMeasureSpec) == MeasureSpecMode.AtMost) { iChildHeightMeasureSpec = MeasureSpec.MakeMeasureSpec(iHeight, MeasureSpecMode.AtMost); }
			else { iChildHeightMeasureSpec = MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified); }

			for (int i = 0; i < iCount; i++)
			{
				View pChild = GetChildAt(i);
				if (pChild.Visibility != ViewStates.Gone)
				{
					App.LayoutParams pParams = (App.LayoutParams)pChild.LayoutParameters;
					pChild.Measure(MeasureSpec.MakeMeasureSpec(iWidth, MeasureSpecMode.AtMost), iChildHeightMeasureSpec);
					int iChildWidth = pChild.MeasuredWidth;
					iLineHeight = Math.Max(m_iLineHeight, pChild.MeasuredHeight + pParams.VerticalSpacing);

					if (iXPos + iChildWidth > iWidth)
					{
						iXPos = PaddingLeft;
						iYPos += m_iLineHeight;
					}

					iXPos += iChildWidth + pParams.HorizontalSpacing;
				}
			}
			m_iLineHeight = iLineHeight;

			if (MeasureSpec.GetMode(iHeightMeasureSpec) == MeasureSpecMode.Unspecified) { iHeight = iYPos + iLineHeight; }
			else if (MeasureSpec.GetMode(iHeightMeasureSpec) == MeasureSpecMode.AtMost)
			{
				if (iYPos + iLineHeight < iHeight) { iHeight = iYPos + iLineHeight; }
			}
			SetMeasuredDimension(iWidth, iHeight);
		}

		protected override ViewGroup.LayoutParams GenerateDefaultLayoutParams() { return new App.LayoutParams(1, 1); }

		protected override ViewGroup.LayoutParams GenerateLayoutParams(ViewGroup.LayoutParams p)
		{
			return new App.LayoutParams(1, 1, p);
		}

		protected override bool CheckLayoutParams(LayoutParams p)
		{
			if (p is App.LayoutParams) { return true; }
			return false;
		}

		protected override void OnLayout(bool changed, int l, int t, int r, int b)
		{
			int iCount = ChildCount;
			int iWidth = r - l;

			int iXPos = PaddingLeft;
			int iYPos = PaddingTop;

			for (int i = 0; i < iCount; i++)
			{
				View pChild = GetChildAt(i);
				if (pChild.Visibility != ViewStates.Gone)
				{
					int iChildWidth = pChild.MeasuredWidth;
					int iChildHeight = pChild.MeasuredHeight;
					App.LayoutParams pParams = (App.LayoutParams)pChild.LayoutParameters;
					if (iXPos + iChildWidth > iWidth)
					{
						iXPos = PaddingLeft;
						iYPos += m_iLineHeight;
					}
					pChild.Layout(iXPos, iYPos, iXPos + iChildWidth, iYPos + iChildHeight);
					iXPos += iChildWidth + pParams.HorizontalSpacing;
				}
			}
		}
	}
}