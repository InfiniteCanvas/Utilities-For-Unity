using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InfiniteCanvas.Utilities
{
	// simple nested dictionary using 2 lists
	[Serializable]
	public class SerializableNestedDictionary<TOuterKey, TInnerKey, TValue> : ISerializationCallbackReceiver
	{
		// for editor
		[SerializeField] private List<OuterKeyValuePair>                              _serializedData = new();
		[NonSerialized]  private Dictionary<TOuterKey, Dictionary<TInnerKey, TValue>> _dictionary     = new();

		public Dictionary<TInnerKey, TValue> this[TOuterKey outerKey]
		{
			get
			{
				if (_dictionary.TryGetValue(outerKey, out var innerDict)) return innerDict;

				innerDict = new Dictionary<TInnerKey, TValue>();
				_dictionary[outerKey] = innerDict;

				return innerDict;
			}
			set => _dictionary[outerKey] = value;
		}

	#region ISerializationCallbackReceiver

		public void OnBeforeSerialize()
		{
			_serializedData.Clear();

			foreach (var outerPair in _dictionary)
			{
				var innerList = outerPair.Value.Select(innerPair => new InnerKeyValuePair { InnerKey = innerPair.Key, Value = innerPair.Value }).ToList();
				_serializedData.Add(new OuterKeyValuePair { OuterKey = outerPair.Key, InnerDictionary = innerList });
			}
		}

		public void OnAfterDeserialize()
		{
			_dictionary = new Dictionary<TOuterKey, Dictionary<TInnerKey, TValue>>();

			foreach (var outerPair in _serializedData)
			{
				var innerDict = new Dictionary<TInnerKey, TValue>();

				foreach (var innerPair in outerPair.InnerDictionary) innerDict[innerPair.InnerKey] = innerPair.Value;

				_dictionary[outerPair.OuterKey] = innerDict;
			}
		}

	#endregion

		public bool TryGetValue(TOuterKey outerKey, TInnerKey innerKey, out TValue value)
		{
			value = default;

			if (!_dictionary.TryGetValue(outerKey, out var innerDict))
				return false;

			return innerDict.TryGetValue(innerKey, out value);
		}

		public bool RemoveInnerValue(TOuterKey outerKey, TInnerKey innerKey, out TValue value)
		{
			if (_dictionary.TryGetValue(outerKey, out var innerDict))
			{
				return innerDict.Remove(innerKey, out value);
			}

			value = default;
			return false;
		}

		public bool RemoveInnerDictionary(TOuterKey outerKey, out Dictionary<TInnerKey, TValue> innerDict) => _dictionary.Remove(outerKey, out innerDict);

		public bool TryGetInnerDictionary(TOuterKey outerKey, out Dictionary<TInnerKey, TValue> innerDict) => _dictionary.TryGetValue(outerKey, out innerDict);

		public void AddOrUpdate(TOuterKey outerKey, TInnerKey innerKey, TValue value)
		{
			if (!_dictionary.TryGetValue(outerKey, out var innerDict))
			{
				innerDict = new Dictionary<TInnerKey, TValue>();
				_dictionary[outerKey] = innerDict;
			}

			innerDict[innerKey] = value;
		}

		public bool ContainsKey(TOuterKey outerKey) => _dictionary.ContainsKey(outerKey);

		public void Clear() => _dictionary.Clear();

		public Dictionary<TOuterKey, Dictionary<TInnerKey, TValue>> GetOuterDictionary() => _dictionary;

		[Serializable]
		private struct OuterKeyValuePair
		{
			public TOuterKey               OuterKey;
			public List<InnerKeyValuePair> InnerDictionary;
		}

		[Serializable]
		private struct InnerKeyValuePair
		{
			public TInnerKey InnerKey;
			public TValue    Value;
		}
	}
}