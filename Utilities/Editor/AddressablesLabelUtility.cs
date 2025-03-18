using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace InfiniteCanvas.Utilities.Editor
{
	public class AddressableLabelUtility : EditorWindow
	{
		private const string    _UXML_GUID = "42d78cd0158e437aba548d96476ee196";
		private const string    _USS_GUID  = "d37cebef4bf047e9bb40425a9b3cb25e";
		private       Button    _applyButton;
		private       string    _customFilter;
		private       Label     _previewCountLabel;
		private       ListView  _previewList;
		private       Button    _refreshButton;
		private       TextField _searchPreviewField;
		private       string    _targetGroup = "Default Local Group";
		private       string    _targetLabel = "CharacterData";
		private       Type      _targetType;

		private VisualElement _typeSelectionContainer;

	#region Event Functions

		public void CreateGUI()
		{
			var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath(_UXML_GUID));
			var root = visualTree.CloneTree();
			rootVisualElement.Add(root);

			var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath(_USS_GUID));
			rootVisualElement.styleSheets.Add(styleSheet);

			_typeSelectionContainer = root.Q<VisualElement>("type-selection");
			var typeButton = root.Q<Button>("select-type-button");
			_applyButton = root.Q<Button>("apply-button");
			var targetLabelField = root.Q<TextField>("target-label");
			var targetGroupField = root.Q<TextField>("target-group");
			var customFilterField = root.Q<TextField>("custom-filter");

			targetLabelField.value = _targetLabel;
			targetGroupField.value = _targetGroup;
			customFilterField.value = _customFilter;

			_searchPreviewField = new TextField("Active Search Filter")
			{
				isReadOnly = true, value = "Select a type to see search filter", style = { marginTop = 5, unityFontStyleAndWeight = FontStyle.Italic },
			};

			_searchPreviewField.Q<Label>().style.minWidth = 120;
			_searchPreviewField.Q<TextField>().style.backgroundColor = new Color(0.15f, 0.15f, 0.15f);
			_searchPreviewField.Q<TextField>().style.color = new Color(0.7f,            0.7f,  0.7f);

			rootVisualElement.Add(_searchPreviewField);

			// Update preview when values change
			targetLabelField.RegisterValueChangedCallback(_ => UpdateSearchPreview());
			targetGroupField.RegisterValueChangedCallback(_ => UpdateSearchPreview());
			customFilterField.RegisterValueChangedCallback(_ => UpdateSearchPreview());

			typeButton.clicked += ShowTypePicker;
			_applyButton.clicked += () => LabelAssetsOfType(_targetType,
			                                                _targetLabel,
			                                                _targetGroup,
			                                                _customFilter);

			targetLabelField.RegisterValueChangedCallback(e => _targetLabel = e.newValue);
			targetGroupField.RegisterValueChangedCallback(e => _targetGroup = e.newValue);
			customFilterField.RegisterValueChangedCallback(e => _customFilter = e.newValue);

			UpdateTypeDisplay();

			DisplayPreview();
			targetLabelField.RegisterValueChangedCallback(_ => UpdatePreview());
			targetGroupField.RegisterValueChangedCallback(_ => UpdatePreview());
			customFilterField.RegisterValueChangedCallback(_ => UpdatePreview());
		}

	#endregion

		[MenuItem("Window/Asset Management/Addressables/Label By Type"), MenuItem("Tools/Label Addressables By Type")]
		public static void ShowWindow()
		{
			var window = GetWindow<AddressableLabelUtility>();
			window.titleContent = new GUIContent("Addressables Labeling");
			window.minSize = new Vector2(350, 250);
		}

		private void UpdateSearchPreview()
		{
			try
			{
				var typeString = _targetType != null ? _targetType.Name : "None";
				var filterPreview = $"t:{typeString} {_customFilter}";
				_searchPreviewField.value = filterPreview.Trim();
			}
			catch (Exception e)
			{
				Debug.LogError($"Search preview update failed: {e.Message}");
			}
		}

		private void DisplayPreview()
		{
			var previewContainer = new VisualElement { style = { marginTop = 10, borderTopWidth = 1, paddingTop = 10 } };

			_previewCountLabel = new Label { style = { fontSize = 11 } };
			previewContainer.Add(_previewCountLabel);

			_previewList = new ListView
			{
				virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
				showBorder = true,
				showAlternatingRowBackgrounds = AlternatingRowBackground.All,
				style = { height = 150, marginTop = 5 },
			};

			_previewList.makeItem = () =>
			                        {
				                        var container = new VisualElement
				                        {
					                        style =
					                        {
						                        flexDirection = FlexDirection.Row, justifyContent = Justify.FlexStart, paddingLeft = 5, paddingRight = 5,
					                        },
				                        };

				                        container.Add(new Label
				                        {
					                        style =
					                        {
						                        unityTextAlign = TextAnchor.MiddleLeft,
						                        fontSize = 12,
						                        whiteSpace = WhiteSpace.NoWrap,
						                        flexBasis = new Length(30, LengthUnit.Percent),
						                        flexGrow = 1,
						                        flexShrink = 1,
					                        },
				                        });

				                        container.Add(new Label
				                        {
					                        style =
					                        {
						                        unityTextAlign = TextAnchor.MiddleLeft,
						                        fontSize = 10,
						                        color = new Color(0.7f, 0.7f, 0.7f),
						                        flexBasis = new Length(70, LengthUnit.Percent),
						                        flexGrow = 1,
						                        flexShrink = 1,
						                        whiteSpace = WhiteSpace.NoWrap,
						                        overflow = Overflow.Hidden,
						                        textOverflow = TextOverflow.Ellipsis,
					                        },
				                        });

				                        return container;
			                        };

			_previewList.bindItem = (element, index) =>
			                        {
				                        var container = element;
				                        var labels = container.Children().Cast<Label>().ToList();
				                        var guid = (string)_previewList.itemsSource[index];
				                        var path = AssetDatabase.GUIDToAssetPath(guid);

				                        labels[0].text = Path.GetFileNameWithoutExtension(path);
				                        labels[1].text = path;

				                        container.tooltip = path;
			                        };
			_previewList.selectionType = SelectionType.Single;
			_previewList.itemsChosen += OnPreviewItemChosen;
			_previewList.AddToClassList("unity-list-view__with-border");

			_refreshButton = new Button(UpdatePreview) { text = "Refresh Preview" };

			previewContainer.Add(_previewList);
			previewContainer.Add(_refreshButton);
			rootVisualElement.Add(previewContainer);
		}

		private void UpdatePreview()
		{
			if (_targetType == null) return;

			try
			{
				var filter = $"t:{_targetType.Name} {_customFilter}";
				var guids = AssetDatabase.FindAssets(filter, new[] { "Assets" });

				_previewList.itemsSource = guids;
				_previewCountLabel.text = $"Matching Assets: {guids.Length}";
				_previewList.Rebuild();
			}
			catch (Exception e)
			{
				Debug.LogError($"Preview update failed: {e.Message}");
			}
		}

		private void OnPreviewItemChosen(IEnumerable<object> items)
		{
			if (items.FirstOrDefault() is string guid)
			{
				var path = AssetDatabase.GUIDToAssetPath(guid);
				var asset = AssetDatabase.LoadAssetAtPath<Object>(path);

				if (asset != null)
				{
					Selection.activeObject = asset;
					EditorGUIUtility.PingObject(asset);
				}
			}
		}

		private void ShowTypePicker()
			=> TypePickerWindow.ShowWindow(selectedType =>
			                               {
				                               _targetType = selectedType;
				                               UpdateTypeDisplay();
				                               UpdatePreview();
				                               UpdateSearchPreview();
			                               },
			                               typeof(Object));


		private void UpdateTypeDisplay()
		{
			_typeSelectionContainer.Clear();
			_typeSelectionContainer.Add(new Label(_targetType?.FullName ?? "No type selected") { name = "type-label" });
			_applyButton.SetEnabled(_targetType != null);
		}

		private static void LabelAssetsOfType(Type   type,
		                                      string label,
		                                      string groupName,
		                                      string customFilter)
		{
			var settings = AddressableAssetSettingsDefaultObject.Settings;
			if (settings == null)
			{
				Debug.LogError("Addressable Settings not found");
				return;
			}

			if (!settings.GetLabels().Contains(label))
				settings.AddLabel(label);

			var group = settings.groups.Find(g => g.Name == groupName);
			if (group == null)
			{
				Debug.LogError($"Group {groupName} not found");
				return;
			}

			var filter = $"t:{type.Name} {customFilter}";
			var guids = AssetDatabase.FindAssets(filter, new[] { "Assets" });

			var processed = 0;
			var skipped = 0;

			foreach (var guid in guids)
			{
				var path = AssetDatabase.GUIDToAssetPath(guid);
				var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
				if (asset == null) continue;

				var entry = settings.FindAssetEntry(guid) ?? settings.CreateOrMoveEntry(guid, group);
				if (!entry.labels.Contains(label))
				{
					entry.labels.Add(label);
					processed++;
				}
				else
				{
					skipped++;
				}
			}

			Debug.Log($"Labeled {processed} {type.Name} assets. {skipped} already had that label.");
			AssetDatabase.SaveAssets();
		}
	}

	public class TypePickerWindow : EditorWindow
	{
		private List<Type>              _filteredTypes = new();
		private IVisualElementScheduler _scheduler;
		private TextField               _searchField;
		private string                  _searchPattern = "";
		private bool                    _selectionInProgress;
		private ListView                _typeList;

		public static void ShowWindow(Action<Type> callback, Type baseType)
		{
			var window = GetWindow<TypePickerWindow>();
			window.titleContent = new GUIContent("Type Selector");
			window.minSize = new Vector2(300, 400);
			window.Initialize(callback, baseType);
		}

		private void Initialize(Action<Type> callback, Type baseType)
		{
			var root = new VisualElement();
			rootVisualElement.Add(root);

			_scheduler = rootVisualElement.schedule;

			_searchField = new TextField { value = "Filter types..." };
			_searchField.RegisterValueChangedCallback(e =>
			                                          {
				                                          _searchPattern = e.newValue;
				                                          RefreshList(baseType);
			                                          });
			root.Add(_searchField);

			_typeList = new ListView
			{
				virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
				selectionType = SelectionType.Single,
				makeItem = () => new Label(),
				bindItem = (e, i) => ((Label)e).text = _filteredTypes[i].FullName,
				fixedItemHeight = 22,
			};

			_typeList.selectedIndicesChanged += _ =>
			                                    {
				                                    if (_selectionInProgress || _typeList.selectedIndex < 0)
					                                    return;

				                                    _selectionInProgress = true;
				                                    _scheduler.Execute(() =>
				                                                       {
					                                                       callback(_filteredTypes
						                                                                [_typeList.selectedIndex]);
					                                                       Close();
					                                                       _selectionInProgress = false;
				                                                       })
				                                              .StartingIn(50);
			                                    };

			root.Add(_typeList);
			RefreshList(baseType);
		}

		private void RefreshList(Type baseType)
		{
			_filteredTypes = GetFilteredTypes(baseType)
			                .OrderBy(t => t.Namespace)
			                .ThenBy(t => t.Name)
			                .ToList();

			_typeList.itemsSource = _filteredTypes;
			_typeList.Rebuild();
		}

		private IEnumerable<Type> GetFilteredTypes(Type baseType)
		{
			var regex = new Regex(_searchPattern, RegexOptions.IgnoreCase);
			return AppDomain.CurrentDomain.GetAssemblies()
			                .Where(a => !a.IsDynamic)
			                .SelectMany(a => a.GetExportedTypes())
			                .Where(t => t.IsClass
			                         && !t.IsAbstract
			                         && baseType.IsAssignableFrom(t)
			                         && regex.IsMatch(t.FullName))
			                .Distinct();
		}
	}
}