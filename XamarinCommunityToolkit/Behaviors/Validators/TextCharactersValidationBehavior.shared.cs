﻿using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Xamarin.CommunityToolkit.Behaviors
{
	public class TextCharactersValidationBehavior : TextValidationBehavior
	{
		List<Predicate<char>> characterPredicates;

		public static readonly BindableProperty CharacterTypeProperty =
			BindableProperty.Create(nameof(CharacterType), typeof(CharacterType), typeof(TextCharactersValidationBehavior), CharacterType.Any, propertyChanged: OnCharacterTypePropertyChanged);

		public static readonly BindableProperty MinimumCharacterCountProperty =
			BindableProperty.Create(nameof(MinimumCharacterCount), typeof(int), typeof(TextCharactersValidationBehavior), 0, propertyChanged: OnValidationPropertyChanged);

		public static readonly BindableProperty MaximumCharacterCountProperty =
			BindableProperty.Create(nameof(MaximumCharacterCount), typeof(int), typeof(TextCharactersValidationBehavior), int.MaxValue, propertyChanged: OnValidationPropertyChanged);

		public TextCharactersValidationBehavior()
			=> OnCharacterTypePropertyChanged();

		public CharacterType CharacterType
		{
			get => (CharacterType)GetValue(CharacterTypeProperty);
			set => SetValue(CharacterTypeProperty, value);
		}

		public int MinimumCharacterCount
		{
			get => (int)GetValue(MinimumCharacterCountProperty);
			set => SetValue(MinimumCharacterCountProperty, value);
		}

		public int MaximumCharacterCount
		{
			get => (int)GetValue(MaximumCharacterCountProperty);
			set => SetValue(MaximumCharacterCountProperty, value);
		}

		protected override bool Validate(object value)
			=> base.Validate(value) && Validate(value?.ToString());

		static void OnCharacterTypePropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{
			((TextCharactersValidationBehavior)bindable).OnCharacterTypePropertyChanged();
			OnValidationPropertyChanged(bindable, oldValue, newValue);
		}

		void OnCharacterTypePropertyChanged()
			=> characterPredicates = GetCharacterPredicates().ToList();

		IEnumerable<Predicate<char>> GetCharacterPredicates()
		{
			if (CharacterType.HasFlag(CharacterType.LowercaseLetter))
				yield return char.IsLower;

			if (CharacterType.HasFlag(CharacterType.UppercaseLetter))
				yield return char.IsUpper;

			if (CharacterType.HasFlag(CharacterType.Digit))
				yield return char.IsDigit;

			if (CharacterType.HasFlag(CharacterType.WhiteSpace))
				yield return char.IsWhiteSpace;

			if (CharacterType.HasFlag(CharacterType.Symbol))
				yield return c => !char.IsLetterOrDigit(c);
		}

		bool Validate(string value)
		{
			var count = value?.ToCharArray().Count(character => characterPredicates.Any(predicate => predicate.Invoke(character))) ?? 0;
			return count >= MinimumCharacterCount
				&& count <= MaximumCharacterCount;
		}
	}
}