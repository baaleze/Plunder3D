// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class TemplateAdditionalParentHelper : TemplateModuleParent
	{
		protected string m_helpBoxMessage = string.Empty;
		private const float ShaderKeywordButtonLayoutWidth = 15;
		private ParentNode m_currentOwner;

		[SerializeField]
		protected List<string> m_additionalItems = new List<string>();

		[SerializeField]
		protected List<string> m_outsideItems = new List<string>();

		public TemplateAdditionalParentHelper( string moduleName ) : base( moduleName ) { }
		public void MarkAsValid() { m_validData = true; }

		public void Draw( ParentNode owner )
		{
			m_currentOwner = owner;
			NodeUtils.DrawNestedPropertyGroup( ref m_foldoutValue, m_moduleName, DrawMainBody, DrawButtons );
		}

		public void CopyFrom( TemplateAdditionalParentHelper other )
		{
			m_additionalItems.Clear();
			m_outsideItems.Clear();
			int otherAdditionalItemsCount = other.ItemsList.Count;
			for( int i = 0; i < otherAdditionalItemsCount; i++ )
			{
				m_additionalItems.Add( other.ItemsList[ i ] );
			}

			int otherOusideItemsCount = other.OutsideList.Count;
			for( int i = 0; i < otherOusideItemsCount; i++ )
			{
				m_outsideItems.Add( other.OutsideList[ i ] );
			}
		}

		void DrawButtons()
		{
			EditorGUILayout.Separator();

			// Add keyword
			if( GUILayout.Button( string.Empty, UIUtils.PlusStyle, GUILayout.Width( ShaderKeywordButtonLayoutWidth ) ) )
			{
				m_additionalItems.Add( string.Empty );
				EditorGUI.FocusTextInControl( null );
				m_isDirty = true;
			}

			//Remove keyword
			if( GUILayout.Button( string.Empty, UIUtils.MinusStyle, GUILayout.Width( ShaderKeywordButtonLayoutWidth ) ) )
			{
				if( m_additionalItems.Count > 0 )
				{
					m_additionalItems.RemoveAt( m_additionalItems.Count - 1 );
					EditorGUI.FocusTextInControl( null );
				}
				m_isDirty = true;
			}
		}

		void DrawMainBody()
		{
			EditorGUILayout.Separator();
			int itemCount = m_additionalItems.Count;
			int markedToDelete = -1;
			for( int i = 0; i < itemCount; i++ )
			{
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUI.BeginChangeCheck();
					m_additionalItems[ i ] = EditorGUILayout.TextField( m_additionalItems[ i ] );
					if( EditorGUI.EndChangeCheck() )
					{
						m_additionalItems[ i ] = UIUtils.RemoveShaderInvalidCharacters( m_additionalItems[ i ] );
						m_isDirty = true;
					}

					// Add new port
					if( m_currentOwner.GUILayoutButton( string.Empty, UIUtils.PlusStyle, GUILayout.Width( ShaderKeywordButtonLayoutWidth ) ) )
					{
						m_additionalItems.Insert( i + 1, string.Empty );
						EditorGUI.FocusTextInControl( null );
						m_isDirty = true;
					}

					//Remove port
					if( m_currentOwner.GUILayoutButton( string.Empty, UIUtils.MinusStyle, GUILayout.Width( ShaderKeywordButtonLayoutWidth ) ) )
					{
						markedToDelete = i;
						m_isDirty = true;
					}
				}
				EditorGUILayout.EndHorizontal();
			}

			if( markedToDelete > -1 )
			{
				if( m_additionalItems.Count > markedToDelete )
				{
					m_additionalItems.RemoveAt( markedToDelete );
					EditorGUI.FocusTextInControl( null );
				}
			}
			EditorGUILayout.Separator();
			EditorGUILayout.HelpBox( m_helpBoxMessage, MessageType.Info );
		}

		public override void ReadFromString( ref uint index, ref string[] nodeParams )
		{
			try
			{
				int count = Convert.ToInt32( nodeParams[ index++ ] );
				for( int i = 0; i < count; i++ )
				{
					m_additionalItems.Add( nodeParams[ index++ ] );
				}
			}
			catch( Exception e )
			{
				Debug.LogException( e );
			}
		}

		public override void WriteToString( ref string nodeInfo )
		{
			IOUtils.AddFieldValueToString( ref nodeInfo, m_additionalItems.Count );
			for( int i = 0; i < m_additionalItems.Count; i++ )
			{
				IOUtils.AddFieldValueToString( ref nodeInfo, m_additionalItems[ i ] );
			}
		}

		public virtual void AddToDataCollector( ref MasterNodeDataCollector dataCollector ) { }

		public override void Destroy()
		{
			m_additionalItems.Clear();
			m_additionalItems = null;
			m_currentOwner = null;
		}

		public List<string> ItemsList { get { return m_additionalItems; } set { m_additionalItems = value; } }
		public List<string> OutsideList { get { return m_outsideItems; } set { m_outsideItems = value; } }
	}
}
