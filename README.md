# ![logo](https://raw.githubusercontent.com/MiltoxBeyond/MZUtils/main/MZUtils.BaseViewModel/lavagear.jpg) MZUtils.BaseViewModel
Base Maui ViewModel to Help with Property Relationships



## Purpose
This library was created to eliminate some of the boilerplating that is needed to have a useful viewmodel in Xamarin (or WPF). 

## Basic Usage

Create a viewmodel that extends `BaseViewModel` to have access to the built in features of the library. 

```csharp
public class ExampleViewModel : MZUtils.BaseViewModel {
	
	//Basic Examples
	public string Title { 
		get => GetValue<string>(); 
		set => SetValue(value); 
	}

	public int Count {
		get => GetValue<int>();
		set => SetValue(value);
	}
}
```

## Extended Usage (Relationships)

```csharp

using MZUtils.BaseViewModel.Attributes;

//...

public class RelationshipViewModel : MZUtils.BaseViewModel {
	//Basic Examples
	//This value affects IsValid
	public string Title { 
		get => GetValue<string>(); 
		set => SetValue(value); 
	}

	//This Value Affects both Message and IsValid
	[Affects(nameof(Message))]
	public int Count {
		get => GetValue<int>();
		set => SetValue(value);
	}

	[AffectedBy(nameof(Title), nameof(Count))]
	public bool IsValid => Count > 0 && !string.IsNullOrEmpty(Title);

	//This one could also be AffectedBy Count.
	public string Message => $"The total count is: {Count}";
}
```
